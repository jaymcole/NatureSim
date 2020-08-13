using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSimulation;
using MonoSimulation.Engine.Events;
using MonoSimulation.Enums;
using MonoSimulation.GameObjects.Behaviors;
using MonoSimulation.Globals;
using MonoSimulation.Model;
using MonoSimulator.EnvironmentProperties;
using MonoSimulator.GameObjects.BaseControls;
using MonoSimulator.Map;
using System;
using System.Collections.Generic;
using System.Management.Instrumentation;
using System.Text;

namespace MonoSimulator
{
    public class AnimalObject : GameObject
    {

        public AnimalObject(Tile tile) : base(tile)
        {
            FacingDirection = Direction.North;
            map = Game1.map;
            this.Tile = tile;
            timeAlive = new TimeSpan();
        }


        //public override void Update(GameTime time)
        //{
        //    timeAlive += time.ElapsedGameTime;
        //    behavior.Update(time);
        //    //TODO replace model with array for various stages (ex. plantObject)
        //    if (model != null)
        //        model.worldPosition = Matrix.CreateRotationY(DirectionExtension.GetAngleDegrees(FacingDirection) * 0.0174533f) * Matrix.CreateTranslation(map.GetTileCenter(Tile));
        //}

        //public override void Render(Effect effect, GraphicsDeviceManager graphics, SpriteBatch batch)
        //{
        //    BasicEffect be = (BasicEffect)effect;
        //    be.World = model.worldPosition;
        //    be.Texture = model.texture;

        //    foreach (EffectPass pass in be.CurrentTechnique.Passes)
        //    {
        //        pass.Apply();
        //        model.Render(effect, graphics, batch);
        //    }
        //}


        public override bool OnNotify(IGameEvent gameEvent)
        {
            return true;
        }
    }
}
