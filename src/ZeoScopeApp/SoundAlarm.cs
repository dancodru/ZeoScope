//Copyright 2011 dancodru

//Licensed under the Apache License, Version 2.0 (the "License");
//You may not use this file except in compliance with the License.
//You may obtain a copy of the License at

//   http://www.apache.org/licenses/LICENSE-2.0

//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

namespace ZeoScope
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading;

    using Microsoft.DirectX.AudioVideoPlayback;

    internal struct AlarmState
    {
        public int Count { get; set; }

        public ZeoSleepStage SleepStage { get; set; }
    }

    internal class SoundAlarm : IDisposable
    {
        public static readonly int[] Volumes = { -4000, -2500, -1200, -600, 0 };  // 0 - (-10000) range

        public static int MaxVolume { get; private set; } // max volume: 0
        public static int MinVolume { get; private set; } // min volume: -10000

        private static string regexCue = @"((?<cue>(\d{1,3}[ARLDSU];?)+)(?:\s+OR\s+)?)+";
        private static string regexState = @"((?<state>\d{1,3}[ARLDSU]);?)+";
        private static string parseState = @"(\d{1,3})([ARLDSU])";

        private Stopwatch stopWatch;
        private Audio audio;
        private int fadeInSeconds;
        private int fadeOutSeconds;
        private int durationSeconds;
        private string fromTime;
        private string toTime;
        private string snoozeTime;
        
        private List<ZeoSleepStage> sleepStages;

        private List<List<AlarmState>> alarmCues;

        public SoundAlarm(string mp3FileName, string fadeInMinutes, string fadeOutMinutes, 
            string durationMinutes, string fromTime, string toTime, string snoozeTime, string alarmCue)
        {
            this.fadeInSeconds = (int)TimeSpan.Parse("00:" + fadeInMinutes).TotalSeconds; // 00 hours
            this.fadeOutSeconds = (int)TimeSpan.Parse("00:" + fadeOutMinutes).TotalSeconds; // 00 hours
            this.durationSeconds = (int)TimeSpan.Parse("00:" + durationMinutes).TotalSeconds; // 00 hours
            this.fromTime = fromTime;
            this.toTime = toTime;
            this.snoozeTime = snoozeTime;

            if (string.IsNullOrEmpty(this.fromTime) == true)
            {
                this.AlarmEnabled = true;
            }

            this.AlarmStarted = false;
            this.stopWatch = new Stopwatch();
            this.audio = new Audio(mp3FileName, false);
            this.audio.Volume = SoundAlarm.MinVolume;
            this.audio.Ending += new EventHandler(this.Audio_Ending);

            this.sleepStages = new List<ZeoSleepStage>();

            this.ParseAlarmCue(alarmCue);
        }

        public bool AlarmStarted { get; set; }

        public bool AlarmEnabled { get; private set; }

        public static void SetVolumes(int v)
        {
            SoundAlarm.MaxVolume = SoundAlarm.Volumes[v - 1];
            SoundAlarm.MinVolume = SoundAlarm.MaxVolume - 3500;
        }

        public static bool ValidateAlarmCue(string alarmCue)
        {
            return Regex.Match(alarmCue, regexCue).Value == alarmCue;
        }

        public ZeoMessage ProcessZeoMessage(ZeoMessage zeoMessage)
        {
            if (string.IsNullOrEmpty(this.fromTime) == false && DateTime.Now.ToString("HH:mm") == this.fromTime)
            {
                this.AlarmEnabled = true;
            }

            if (string.IsNullOrEmpty(this.toTime) == false && DateTime.Now.ToString("HH:mm") == this.toTime)
            {
                this.AlarmEnabled = false;
                
                // This will stop the alarm
                this.AlarmStarted = false;
            }

            if (this.AlarmStarted == true && this.audio.Playing == false)
            {
                // Called from SettingsForm
                // TODO: Improve the design here
                this.StartAlarm();
                return null; ;
            }

            if (this.AlarmStarted == true || this.audio.Playing == true)
            {
                this.SetAudioVolume();
                zeoMessage.SoundAlarmVolume = this.audio.Volume;
                return zeoMessage;
            }

            if (this.AlarmEnabled == false)
            {
                return null;
            }

            if (zeoMessage.SleepStage != null)
            {
                this.sleepStages.Add(zeoMessage.SleepStage.Value);
            }

            int count = this.sleepStages.Count - 1;
            bool startAlarm = true;

            foreach (List<AlarmState> alarmCue in this.alarmCues)
            {
                int i = count;
                startAlarm = true;
                foreach (AlarmState state in alarmCue)
                {
                    int end = i - state.Count;
                    if (end < -1 || startAlarm == false)
                    {
                        startAlarm = false;
                        break;
                    }

                    for (; i >= 0 && i > end; i--)
                    {
                        if (state.SleepStage == ZeoSleepStage.Sleep &&
                            (this.sleepStages[i] != ZeoSleepStage.REM && this.sleepStages[i] != ZeoSleepStage.Light && this.sleepStages[i] != ZeoSleepStage.Deep))
                        {
                            startAlarm = false;
                            break;
                        }
                        else if (state.SleepStage != ZeoSleepStage.Sleep && this.sleepStages[i] != state.SleepStage)
                        {
                            startAlarm = false;
                            break;
                        }
                    }
                }

                if (startAlarm == true)
                {
                    break;
                }
            }

            if (startAlarm == true)
            {
                this.StartAlarm();
            }

            return null;
        }

        public void Dispose()
        {
            if (this.audio != null)
            {
                this.audio.Dispose();
                this.audio = null;
            }
        }

        private void ParseAlarmCue(string alarmCue)
        {
            this.alarmCues = new List<List<AlarmState>>();
            foreach (Capture cueCapture in Regex.Match(alarmCue, regexCue).Groups["cue"].Captures)
            {
                List<AlarmState> cue = new List<AlarmState>();
                foreach (Capture stateCapture in Regex.Match(cueCapture.Value, regexState).Groups["state"].Captures)
                {
                    AlarmState c = new AlarmState();
                    c.Count = Convert.ToInt32(Regex.Match(stateCapture.Value, SoundAlarm.parseState).Groups[1].Value);
                    switch (Regex.Match(stateCapture.Value, SoundAlarm.parseState).Groups[2].Value)
                    {
                        case "A":
                            c.SleepStage = ZeoSleepStage.Awake;
                            break;
                        case "R":
                            c.SleepStage = ZeoSleepStage.REM;
                            break;
                        case "L":
                            c.SleepStage = ZeoSleepStage.Light;
                            break;
                        case "D":
                            c.SleepStage = ZeoSleepStage.Deep;
                            break;
                        case "U":
                            c.SleepStage = ZeoSleepStage.Undefined0;
                            break;
                        case "S":
                            c.SleepStage = ZeoSleepStage.Sleep;
                            break;
                    }

                    cue.Add(c);
                }

                cue.Reverse();

                this.alarmCues.Add(cue);
            }
        }

        private void StartAlarm()
        {
            this.stopWatch.Reset();
            this.stopWatch.Start();
            this.audio.Play();

            this.AlarmStarted = true;
        }

        private void SetAudioVolume()
        {
            double seconds = this.stopWatch.Elapsed.TotalSeconds;

            if (seconds > this.durationSeconds || this.AlarmStarted == false)
            {
                this.sleepStages.Clear();
                this.audio.Stop();
                this.audio.Volume = SoundAlarm.MinVolume;
                this.AlarmStarted = false;

                // Snooze the alarm for snoozeTime minutes
                if (string.IsNullOrEmpty(this.snoozeTime) == false && TimeSpan.Parse(this.snoozeTime).TotalMinutes != 0)
                {
                    this.fromTime = DateTime.Now.Add(TimeSpan.Parse(this.snoozeTime)).ToString("HH:mm");
                    this.AlarmEnabled = false;
                }

                return;
            }

            if (seconds <= this.fadeInSeconds)
            {
                this.audio.Volume = (int)(((SoundAlarm.MaxVolume - SoundAlarm.MinVolume) * Math.Sqrt(seconds / this.fadeInSeconds)) + SoundAlarm.MinVolume);
            }
            else
            {
                seconds = this.durationSeconds - seconds;
                if (seconds > this.fadeOutSeconds)
                {
                    this.audio.Volume = SoundAlarm.MaxVolume;
                }
                else
                {
                    this.audio.Volume = (int)(((SoundAlarm.MaxVolume - SoundAlarm.MinVolume) * Math.Sqrt(seconds / this.fadeOutSeconds)) + SoundAlarm.MinVolume);
                }
            }
        }

        private void Audio_Ending(object sender, EventArgs e)
        {
            this.audio.CurrentPosition = 0;
        }
    }
}
