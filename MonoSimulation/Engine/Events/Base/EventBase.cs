using MonoSimulator.GameObjects.BaseControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoSimulation.Engine.Events
{
    public class EventBase : IGameEvent, IComparable
    {
        public INotifier EventCreater { get; private set; }
        public bool IsStale { get; set; }

        public EventBase (INotifier self)
        {
            EventCreater = self;
        }

        public virtual int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
