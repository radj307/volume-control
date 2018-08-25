using System.Diagnostics;
using System.Timers;

namespace Toastify.Common
{
    public class PausableTimer : Timer
    {
        private Stopwatch stopwatch;

        #region Public Properties

        public double? IntervalRemaining { get; private set; }

        public bool Paused { get; private set; }

        public double OriginalInterval { get; private set; }

        public new bool Enabled
        {
            get { return base.Enabled; }
            set
            {
                base.Enabled = value;
                this.ResetStopWatch(base.Enabled, true);
            }
        }

        public new double Interval
        {
            get { return base.Interval; }
            set
            {
                base.Interval = value;
                if (!this.Paused)
                    this.OriginalInterval = base.Interval;
            }
        }

        #endregion

        public PausableTimer()
        {
        }

        public PausableTimer(double interval) : base(interval)
        {
            this.Interval = interval;
        }

        public void Pause()
        {
            if (this.Enabled)
            {
                this.IntervalRemaining = this.Interval - this.stopwatch.ElapsedMilliseconds;
                this.Paused = true;
                this.ResetStopWatch(false, false);
                base.Stop();
            }
        }

        public void Resume()
        {
            if (this.Paused)
            {
                this.Interval = this.IntervalRemaining ?? this.OriginalInterval;
                this.ResetStopWatch(true, false);
                base.Start();
            }
        }

        public new void Start()
        {
            this.ResetStopWatch(true, true);
            base.Start();
        }

        public new void Stop()
        {
            this.ResetStopWatch(false, true);
            base.Stop();
        }

        private void ResetStopWatch(bool startNew, bool resetPause)
        {
            if (this.stopwatch != null)
            {
                this.stopwatch.Stop();
                this.stopwatch = null;
            }

            if (resetPause)
            {
                if (this.Paused)
                    this.Interval = this.OriginalInterval;

                this.Paused = false;
                this.IntervalRemaining = null;
            }

            if (startNew)
                this.stopwatch = Stopwatch.StartNew();
        }
    }
}