using MonoSimulator.GameObjects.BaseControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoSimulation.Engine.Events
{
    public class OnSleep : EventBase
    {
        public OnSleep (INotifier self) : base(self)
        {

        } 

    }
}
