using UnityEngine;

namespace Puyopuyo.Domain {
    public interface IClockBattery {
        bool CanProvideEnergy { get; }
        void Insert();
        void Remove();
    }

    public class ClockBattery : IClockBattery {
        public bool CanProvideEnergy { get; private set; }

        public ClockBattery()
        {
            Insert();
        }

        public void Insert()
        {
            CanProvideEnergy = true;
        }

        public void Remove()
        {
            CanProvideEnergy = false;
        }
    }
}