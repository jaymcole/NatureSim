using Microsoft.Xna.Framework;
using MonoSimulation.Generators;
using MonoSimulation.Globals;
using MonoSimulation.Model;
using MonoSimulator;
using MonoSimulator.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoSimulation.GameObjects.Behaviors
{
    class PlantBehavior : IBehavior
    {

        public PlantBehavior(PlantObject parent)
        {
            TimeSinceLastReproduction = new TimeSpan();
            TimeSinceLastGrowth = new TimeSpan();
            Parent = parent;
        }

        private PlantObject Parent;
        private TimeSpan TimeSinceLastReproduction;
        private TimeSpan TimeSinceLastGrowth;

        public void Update(GameTime gameTime)
        {

            TimeSinceLastGrowth += gameTime.ElapsedGameTime;
            if (TimeSinceLastGrowth.TotalSeconds >= Parent.MaxLifeSpan / ((Parent.lifeStage*2)+1))
            {

                Parent.Grow();
                TimeSinceLastReproduction = new TimeSpan();
            }


            TimeSinceLastReproduction += gameTime.ElapsedGameTime;
            if (TimeSinceLastReproduction.TotalSeconds >= 10)
            {
                if (Parent.Tile.TypeCount(typeof(PlantObject)) > 2)
                {
                    foreach (Tile tile in Parent.AdjacentTiles)
                    {
                        if (GlobalVariables.random.NextDouble() >= .97)
                        {
                            CreateNewPlant(tile);
                        }
                    }
                }
                else
                {
                    if (GlobalVariables.random.NextDouble() >= .5)
                    {
                        CreateNewPlant(Parent.Tile);
                    }
                }
                TimeSinceLastGrowth = new TimeSpan();
            }
        }

        public void CreateNewPlant(Tile tile)
        {
            if (tile.TypeCount(typeof(PlantObject)) > 3)
                return;
            PlantObject ob2 = new PlantObject(tile,
                Parent.LifeStages,
                Parent.LifeScales,
                1
                );
            ob2.behavior = new PlantBehavior(ob2);
            Game1.objectManager.AddObject(ob2);
        }
    }
}
