using MonoSimulation.Engine.Events.Base;
using MonoSimulation.Interfaces.BaseControls;
using MonoSimulator.GameObjects.BaseControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoSimulation.Engine.Events
{
    public class OnTime : EventBase
    {
        public IEventReason EventReason { get; private set; }
        public ISubscriber EventTarget { get; private set; }
        public TimeSpan FireTime { get; private set; }


        public OnTime(INotifier self, ISubscriber target, IEventReason eventReason, double millisecondsFromNow) : base(self)
        {
            EventReason = eventReason;
            FireTime = TimeSpan.FromMilliseconds(Game1.PreviousGameTime.TotalMilliseconds + millisecondsFromNow);
            EventTarget = target;
        }

        public override int CompareTo(object obj)
        {
            if (obj.GetType().IsEquivalentTo(typeof(OnTime)))
            {
                OnTime nextTime = obj as OnTime;
                if (FireTime.TotalMilliseconds > nextTime.FireTime.TotalMilliseconds)
                {
                    return 1;
                } else if (FireTime.TotalMilliseconds < nextTime.FireTime.TotalMilliseconds)
                {
                    return -1;
                }
                return 0;
            }
                //return FireTime.CompareTo((obj as OnTime).FireTime);
            else
                return base.CompareTo(obj);
        }
    }
}
