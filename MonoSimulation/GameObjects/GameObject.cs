using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSimulation;
using MonoSimulation.Engine.Events;
using MonoSimulation.Enums;
using MonoSimulation.GameObjects.Behaviors;
using MonoSimulation.Globals;
using MonoSimulation.Models;
using MonoSimulator.EnvironmentProperties;
using MonoSimulator.GameObjects.BaseControls;
using MonoSimulator.Map;
using System;
using System.Collections.Generic;
using System.Management.Instrumentation;
using System.Text;

namespace MonoSimulator
{
    public abstract class GameObject : NotifierBase, ISubscriber
    {
        public GameMap map;
        public IBehavior behavior;
        public Direction FacingDirection;
        public TimeSpan timeAlive;
        public JModel model;
        public Model model3D;
        public Matrix TransformMatrix;


        public GameObject (Tile tile)
        {
            FacingDirection = Direction.North;
            map = Game1.map;
            this.Tile = tile;
            timeAlive = new TimeSpan();
        }


        public virtual void Update(GameTime time)
        {
            timeAlive += time.ElapsedGameTime;
            behavior.Update(time);
            //TODO replace model with array for various stages (ex. plantObject)
            if (model != null)
                model.worldPosition = Matrix.CreateScale(1f) * Matrix.CreateRotationY(DirectionExtension.GetAngleDegrees(FacingDirection) * 0.0174533f)  * Matrix.CreateTranslation(map.GetTileCenter(Tile));
            TransformMatrix = Matrix.CreateScale(.0003f) * Matrix.CreateRotationY(DirectionExtension.GetAngleDegrees(FacingDirection) * 0.0174533f) * Matrix.CreateTranslation(map.GetTileCenter(Tile));                
        }

        public virtual void Render(Effect effect, GraphicsDeviceManager graphics, SpriteBatch batch)
        {
            BasicEffect be = (BasicEffect)effect;
            be.World = model.worldPosition;
            be.Texture = model.texture;
            foreach (EffectPass pass in be.CurrentTechnique.Passes)
            {
                pass.Apply();
                if (model != null)
                    model.Render(effect, graphics, batch);
                if (model3D != null)
                {
                    model3D.Draw(TransformMatrix, Game1.cam.View, Game1.cam.Projection);
                }
            }
        }

        private Tile _tile;
        public Tile Tile 
        {
            get { return _tile; }
            set 
            { 
                if (_tile != null)
                {
                    _tile.LeaveTile(this);
                }
                
                if (value != null)
                {
                    value.EnterTile(this);
                }
                _tile = value;
            }
        }


        private bool _removeMe;
        public bool RemoveMe {
            get
            {
                return _removeMe;
            }

            protected set
            {
                //If removeMe == true, then KILL
                if (value)
                {
                    this.Tile.LeaveTile(this);
                    Notify(new OnDeath(this));
                }
                _removeMe = value;
            }

        }

        public void Kill()
        {
            RemoveMe = true;
        }

        public bool IsSleeping { get; protected set; }
        public void Wake()
        {
            if (IsSleeping)
                Notify(new OnWake(this));
            IsSleeping = false;
        }

        public void Sleep()
        {
            if (!IsSleeping)
                Notify(new OnSleep(this));
            IsSleeping = true;
        }

        public abstract bool OnNotify(IGameEvent gameEvent);
    }
}
