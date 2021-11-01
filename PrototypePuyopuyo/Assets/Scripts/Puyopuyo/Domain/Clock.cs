using UnityEngine;

namespace Puyopuyo.Domain {
    public interface IClock {
        IClockAlarm Alarm { get; }
        IClockBattery Battery { get; }
        bool CanTikTok { get; }
        void TikTok();
        void ReturnShippingState();
        void SetHandsToZero();
    }

    public class Clock : IClock {
        private float timeToGoRound;
        private float currentTime;

        public IClockAlarm Alarm { get; private set; }
        public IClockBattery Battery { get; private set; }
        public bool CanTikTok => Battery.CanProvideEnergy;

        public Clock(float timeToGoRound) {
            this.timeToGoRound = timeToGoRound;
            currentTime = 0f;
            Alarm = new ClockAlarm();
            Battery = new ClockBattery();
            Battery.Remove();
        }

        public void TikTok()
        {
            if (!CanTikTok) { return; }
            currentTime += Time.deltaTime;
            if (currentTime > timeToGoRound) {
                currentTime -= timeToGoRound;
                Alarm.Start();
            }
        }

        public void ReturnShippingState()
        {
            currentTime = 0f;
            Alarm.Stop();
            Battery.Remove();
        }

        public void SetHandsToZero()
        {
            currentTime = 0f;
            Alarm.Stop();
            Battery.Insert();
        }
    }
}