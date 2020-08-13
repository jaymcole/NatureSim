using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoSimulation.Enums;
using MonoSimulator;
using MonoSimulator.Map;
using System;

namespace MonoSimulation.GameObjects.Behaviors
{
    class PlayerRunner : IBehavior
    {
        private GameObject parent;

        public PlayerRunner(GameObject parent)
        {
            this.parent = parent;
            TimeSinceLastMove = new TimeSpan();
            nextStep = new Vector2();
        }

        private TimeSpan TimeSinceLastMove;
        public void Update(GameTime gameTime)
        {
            TimeSinceLastMove += gameTime.ElapsedGameTime;
            if (TimeSinceLastMove.TotalMilliseconds > 150)
            {
                MoveControlled();
                TimeSinceLastMove = new TimeSpan();
            }
        }

        private Vector2 nextStep;
        private void MoveControlled()
        {
            GameMap map = Game1.map;
            
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad8))
                nextStep = DirectionExtension.ForwardCoordinates(parent.FacingDirection, parent.Tile.row, parent.Tile.col);
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad5))
                nextStep = DirectionExtension.ReverseCoordinates(parent.FacingDirection, parent.Tile.row, parent.Tile.col);
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad4))
                parent.FacingDirection = DirectionExtension.RotateCCW(parent.FacingDirection);
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad6))
                parent.FacingDirection = DirectionExtension.RotateCW(parent.FacingDirection);


            if (Keyboard.GetState().IsKeyDown(Keys.NumPad7))
                nextStep.X += 1;
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad9))
                nextStep.Y += 1;
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad1))
                nextStep.X -= 1;
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad3))
                nextStep.Y -= 1;



            Tile tile = map.GetGroundTile((int)nextStep.X, (int)nextStep.Y);
            if (tile != null && parent.Tile != tile)
            {

                

                parent.Tile = tile;

                foreach (Tile t in parent.Tile.AdjacentTiles)
                {
                    t.TopColor = Color.Red;
                }

            }

            if (Keyboard.GetState().IsKeyDown(Keys.NumPad0) && parent.Tile != null)
                foreach (Tile t in parent.Tile.AdjacentTiles)
                {
                    t.RemoveType(typeof(PlantObject), true);
                    t.TopColor = Color.Blue;
                }



        }
    }
}
