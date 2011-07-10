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
        public static readonly int MinVolume = -5000; // min volume: -10000
        public static readonly int MaxVolume = 0; // max volume: 0

        private static string regexCue = @"((?<cue>(\d{1,3}[ARLDU];?)+)(?:\s+OR\s+)?)+";
        private static string regexState = @"((?<state>\d{1,3}[ARLDU]);?)+";
        private static string parseState = @"(\d{1,3})([ARLDU])";

        private Stopwatch stopWatch;
        private Audio audio;
        private int fadeInSeconds;
        private int fadeOutSeconds;
        private int durationSeconds;
        private int maxVolume;
        private string fromTime;
        private string toTime;
        
        private List<ZeoSleepStage> sleepStages;

        private List<List<AlarmState>> alarmCues;

        public SoundAlarm(string mp3FileName, string fadeInMinutes, string fadeOutMinutes, string durationMinutes, string fromTime, string toTime, string alarmCue)
        {
            this.fadeInSeconds = (int)TimeSpan.Parse("00:" + fadeInMinutes).TotalSeconds; // 00 hours
            this.fadeOutSeconds = (int)TimeSpan.Parse("00:" + fadeOutMinutes).TotalSeconds; // 00 hours
            this.durationSeconds = (int)TimeSpan.Parse("00:" + durationMinutes).TotalSeconds; // 00 hours
            this.maxVolume = SoundAlarm.MaxVolume;
            this.fromTime = fromTime;
            this.toTime = toTime;

            this.AlarmStarted = false;
            this.stopWatch = new Stopwatch();
            this.audio = new Audio(mp3FileName, false);
            this.audio.Volume = SoundAlarm.MinVolume;
            this.audio.Ending += new EventHandler(this.Audio_Ending);

            this.sleepStages = new List<ZeoSleepStage>();

            this.ParseAlarmCue(alarmCue);
        }

        public bool AlarmStarted { get; set; }

        public static bool ValidateAlarmCue(string alarmCue)
        {
            return Regex.Match(alarmCue, regexCue).Value == alarmCue;
        }

        public ZeoMessage ProcessZeoMessage(ZeoMessage zeoMessage)
        {
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
                        if (this.sleepStages[i] != state.SleepStage)
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
                return;
            }

            if (seconds <= this.fadeInSeconds)
            {
                this.audio.Volume = (int)(((this.maxVolume - SoundAlarm.MinVolume) * Math.Sqrt(seconds / this.fadeInSeconds)) + SoundAlarm.MinVolume);
            }
            else
            {
                seconds = this.durationSeconds - seconds;
                if (seconds > this.fadeOutSeconds)
                {
                    this.audio.Volume = this.maxVolume;
                }
                else
                {
                    this.audio.Volume = (int)(((this.maxVolume - SoundAlarm.MinVolume) * Math.Sqrt(seconds / this.fadeOutSeconds)) + SoundAlarm.MinVolume);
                }
            }
        }

        private void Audio_Ending(object sender, EventArgs e)
        {
            this.audio.CurrentPosition = 0;
        }
    }
}
