namespace Puyopuyo.Domain {
    public interface IClockAlarm {
        bool IsRing { get; }
        void Start();
        void Stop();
    }

    public class ClockAlarm : IClockAlarm {
        public bool IsRing { get; private set; }

        public ClockAlarm()
        {
            IsRing = false;
        }

        public void Start()
        {
            IsRing = true;
        }

        public void Stop()
        {
            IsRing = false;
        }
    }
}