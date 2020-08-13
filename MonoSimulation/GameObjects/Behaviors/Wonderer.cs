using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoSimulation.Enums;
using MonoSimulation.Globals;
using MonoSimulator;
using MonoSimulator.Map;
using System;

namespace MonoSimulation.GameObjects.Behaviors
{
    public class Wonderer : IBehavior
    {

        private GameObject parent;

        public Wonderer (GameObject parent)
        {
            this.parent = parent;
            TimeSinceLastMove = new TimeSpan();
        }

        private TimeSpan TimeSinceLastMove;
        public void Update(GameTime gameTime)
        {
            TimeSinceLastMove += gameTime.ElapsedGameTime;
            if (TimeSinceLastMove.TotalMilliseconds > 50)
            {
                MoveRandomAdjacent();
                TimeSinceLastMove = new TimeSpan();
            }
        }

        private Vector2 nextStep;

        private void MoveRandomAdjacent()
        {
            GameMap map = Game1.map;

            if (GlobalVariables.random.NextDouble() > 0.7)
            {
                if (GlobalVariables.random.NextDouble() > 0.5)
                    parent.FacingDirection = DirectionExtension.RotateCW(parent.FacingDirection);
                else
                    parent.FacingDirection = DirectionExtension.RotateCCW(parent.FacingDirection);
            }

            if (GlobalVariables.random.NextDouble() > 0.35)
                nextStep = DirectionExtension.ForwardCoordinates(parent.FacingDirection, parent.Tile.row, parent.Tile.col);

            Tile tile = map.GetGroundTile((int)nextStep.X, (int)nextStep.Y);
            tile.RemoveType(typeof(PlantObject), true);
            if (parent.Tile != null && parent.Tile != tile && tile.ObjectCount == 0)
            {
                parent.Tile = tile;
            }



        }
    }
}
