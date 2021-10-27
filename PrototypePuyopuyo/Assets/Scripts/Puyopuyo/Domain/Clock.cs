using UnityEngine;

namespace Puyopuyo.Domain {
    public interface IClock {
        float TimeToGoRound { get; }
        bool IsRing { get; }
        bool CanTikTok { get; }
        void TikTok();
        void StopTime();
        void StartTime();
        void ResetAll();
        void StopRing();
    }

    public class Clock : IClock {
        public float TimeToGoRound { get; private set; }
        public float currentTime;
        public bool IsRing { get; private set; }
        public bool CanTikTok { get; private set; }

        public Clock(float timeToGoRound) {
            TimeToGoRound = timeToGoRound;
            ResetAll();
        }

        public void TikTok()
        {
            if (!CanTikTok) { return; }
            currentTime += Time.deltaTime;
            if (currentTime > TimeToGoRound) {
                currentTime -= TimeToGoRound;
                IsRing = true;
            }
        }

        public void StopTime()
        {
            CanTikTok = false;
        }

        public void StartTime()
        {
            CanTikTok = true;
        }

        public void StopRing()
        {
            IsRing = false;
        }

        public void ResetAll()
        {
            ResetTime();
            StopRing();
        }

        private void ResetTime()
        {
            currentTime = 0f;
        }
    }
}