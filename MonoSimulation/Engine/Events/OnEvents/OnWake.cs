using MonoSimulator.GameObjects.BaseControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoSimulation.Engine.Events
{
    public class OnWake : EventBase
    {
        public OnWake(INotifier self) : base(self)
        {

        }

    }
}
