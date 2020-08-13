using MonoSimulator.GameObjects.BaseControls;
using MonoSimulator.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoSimulation.Engine.Events
{
    public class OnLeaveTile : EventBase
    {
        public Tile LeavingTile { get; private set; }

        public OnLeaveTile(INotifier self, Tile LeavingTile) : base(self)
        {
            this.LeavingTile = LeavingTile;
        }

    }
}
