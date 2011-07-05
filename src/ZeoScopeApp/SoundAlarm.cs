namespace ZeoScope
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using Microsoft.DirectX.AudioVideoPlayback;

    internal class SoundAlarm : IDisposable
    {
        private Stopwatch stopWatch;
        private Audio audio;
        private Timer timer;
        private int minVolume = -5000; // min volume: -10000
        private int fadeInSeconds;
        private int fadeOutSeconds;
        private int durationSeconds;
        private int maxVolume;

        public SoundAlarm(string mp3FileName, int fadeInSeconds, int fadeOutSeconds, int durationSeconds, int maxVolume)
        {
            this.fadeInSeconds = fadeInSeconds;
            this.fadeOutSeconds = fadeOutSeconds;
            this.durationSeconds = durationSeconds;
            this.maxVolume = maxVolume;

            this.stopWatch = new Stopwatch();
            this.audio = new Audio(mp3FileName, false);
            this.audio.Volume = minVolume;
            this.audio.Ending += new EventHandler(Audio_Ending);
        }

        public void StartAlarm()
        {
            this.stopWatch.Reset();
            this.stopWatch.Start();
            this.audio.Play();

            if (this.timer != null)
            {
                this.timer.Dispose();
            }

            this.timer = new Timer(new TimerCallback(this.TimerAudioContol), null, 0, 20);
        }

        public void Dispose()
        {
            if (this.timer != null)
            {
                this.timer.Dispose();
                this.timer = null;
            }

            if (this.audio != null)
            {
                this.audio.Dispose();
                this.audio = null;
            }
        }

        private void TimerAudioContol(object obj)
        {
            double seconds = this.stopWatch.Elapsed.TotalSeconds;

            if (seconds > this.durationSeconds)
            {
                this.audio.Stop();
                this.timer.Dispose();
                this.timer = null;
                return;
            }

            if (seconds <= fadeInSeconds)
            {
                this.audio.Volume = (int)((this.maxVolume - this.minVolume) * Math.Sqrt(seconds / this.fadeInSeconds) + this.minVolume);
                return;
            }

            seconds = durationSeconds - seconds;
            if (seconds <= fadeOutSeconds)
            {
                this.audio.Volume = (int)((this.maxVolume - this.minVolume) * Math.Sqrt(seconds / this.fadeOutSeconds) + this.minVolume);
            }
        }

        private void Audio_Ending(object sender, EventArgs e)
        {
            this.audio.CurrentPosition = 0;
        }
    }
}
