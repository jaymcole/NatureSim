using MonoSimulator.GameObjects.BaseControls;
using MonoSimulator.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoSimulation.Engine.Events
{
    public class OnEnterTile : EventBase
    {
        public Tile EnteringTile { get; private set; }

        public OnEnterTile (INotifier self, Tile EnteringTile) : base(self)
        {
            this.EnteringTile = EnteringTile;
        }

    }
}
