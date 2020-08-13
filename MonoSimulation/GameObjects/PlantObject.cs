using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSimulation.Engine.Events;
using MonoSimulation.Engine.Events.EventReason;
using MonoSimulation.Enums;
using MonoSimulation.GameObjects.Behaviors;
using MonoSimulation.Generators;
using MonoSimulation.Globals;
using MonoSimulation.Models;
using MonoSimulator;
using MonoSimulator.GameObjects.BaseControls;
using MonoSimulator.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace MonoSimulation.GameObjects
{
    public class PlantObject : GameObject
    {

        public Texture2D[] LifeStages;
        public Vector3[] LifeScales;
        public float fullScale;

        public JModel[] quads;
        public int lifeStage = -1;
        protected int scaleIndex = -1;
        protected Vector3 offset;
        public int MaxLifeSpan = 1000;

        public List<Tile> AdjacentTiles; 
        private float yRotationOffset;


        public PlantObject(Tile tile, Texture2D[] texStages, Vector3[] texScales, int quadCount) : base(tile)
        {
            quadCount = 1;
            if (quadCount <= 0)
                throw new Exception();

            quads = new JModel[quadCount];
            LifeStages = texStages;
            LifeScales = texScales;
            offset = new Vector3((float)GlobalVariables.random.NextDouble(), 0, (float)GlobalVariables.random.NextDouble());
            fullScale = (float)GlobalVariables.random.NextDouble();

            quads = new JModel[quadCount];
            for(int i = 0; i < quads.Length; i++)
            {
                quads[i] = ModelGenerator.CreateQuad(1, LifeScales[0].X, LifeScales[0].Y, LifeScales[0].Z);
            }
            Grow();
            SecondsToReproduce = GlobalVariables.random.Next(1, 5);
            yRotationOffset = (float)GlobalVariables.random.NextDouble();

            SubscribeToAdjacent();
            EvaluateAdjacentTiles();

            //Game1.MasterTimeScheduler.Subscribe(new OnTime(this, this, new ReasonDie(), 60000), this);
            Game1.MasterTimeScheduler.Subscribe(new OnTime(this, this, new ReasonGrow(), 5000), this);
        }

        private void EvaluateAdjacentTiles()
        {
            AdjacentTiles = new List<Tile>();
            foreach (Tile t in Tile.AdjacentTiles)
                if (IsViableTile(t))
                    AdjacentTiles.Add(t);
        }

        private void SubscribeToAdjacent()
        {
            foreach (Tile t in Tile.AdjacentTiles)
            {
                t.Subscribe(new OnEnterTile(null, null), this);
                t.Subscribe(new OnLeaveTile(null, null), this);
            }
        }

        private TimeSpan TimeSinceLastReproduction;
        private int SecondsToReproduce;
        public override void Update(GameTime time)
        {
            if (AdjacentTiles.Count <= 0)
            {
                Sleep();
                return;
            }


            TimeSinceLastReproduction += time.ElapsedGameTime;
            if (TimeSinceLastReproduction.TotalSeconds >= SecondsToReproduce)
            {
                Tile t = AdjacentTiles[GlobalVariables.random.Next(0, AdjacentTiles.Count)];
                if (IsViableTile(t))
                    CreateNewPlant(t);
                else
                    AdjacentTiles.Remove(t);
                TimeSinceLastReproduction = new TimeSpan();
            }

            //if (timeAlive.TotalSeconds > MaxLifeSpan)
            //    RemoveMe = true;
        }

        public void CreateNewPlant(Tile tile)
        {
            PlantObject newPlant = new PlantObject(tile,
                LifeStages,
                LifeScales,
                GlobalVariables.random.Next(1, 5)
                );
            //newPlant.model3D = GlobalVariables.Content.Load<Microsoft.Xna.Framework.Graphics.Model>("Models/Foliage/ForestPack/Grass/Grass");
            newPlant.model3D = GlobalVariables.GetFlower();
            newPlant.behavior = new PlantBehavior(newPlant);
            Game1.objectManager.AddObject(newPlant);
        }

        public void Grow()
        {
            lifeStage = Math.Min(lifeStage + 1, LifeStages.Length-1);
            scaleIndex = Math.Min(lifeStage, LifeScales.Length-1);

            for (int i = 0; i < quads.Length; i++)
            {
                quads[i].worldPosition = Matrix.CreateScale(LifeScales[scaleIndex]) * Matrix.CreateRotationY(((360.0f / quads.Length) * i * 0.0174533f) + yRotationOffset) * Matrix.CreateTranslation(map.GetTileCenter(Tile) + offset);
                quads[i].texture = LifeStages[lifeStage];
            }
            TransformMatrix = Matrix.CreateScale(0.1f) * Matrix.CreateRotationY(DirectionExtension.GetAngleDegrees(FacingDirection) * 0.0174533f) * Matrix.CreateTranslation(map.GetTileCenter(Tile));
        }

        public override void Render(Effect effect, GraphicsDeviceManager graphics, SpriteBatch batch)
        {
            BasicEffect be = (BasicEffect)effect;
            


            for(int i = 0; i < quads.Length; i++)
            {
                be.World = quads[i].worldPosition;
                be.Texture = quads[i].texture;
                foreach (EffectPass pass in be.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    //quads[i].Render(effect, graphics, batch);

                }
            }

            foreach (ModelMesh mesh in  model3D.Root.Meshes)
            {
                foreach (Effect effects in mesh.Effects)
                {
                    foreach (EffectPass pass in effects.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        model3D.Draw(TransformMatrix, Game1.cam.View, Game1.cam.Projection);
                    }
                }

                
            }
        }

        private bool IsViableTile(Tile tile)
        {
            if (tile.TypeCount(typeof(PlantObject)) >= GlobalVariables.MaxPlantsPerTile)
                return false;
            return true;
        }

        public override bool OnNotify(IGameEvent gameEvent)
        {
            if (RemoveMe)
                return true;
            //risky
            var eventBase = gameEvent as EventBase;
            if (!eventBase.EventCreater.GetType().IsEquivalentTo(typeof(PlantObject)))
                return false;

            if (gameEvent.GetType().IsEquivalentTo(typeof(OnLeaveTile)))
            {
                var leave = gameEvent as OnLeaveTile;
                if (IsViableTile(leave.LeavingTile) && !AdjacentTiles.Contains(leave.LeavingTile))
                {
                    //AdjacentTiles.Add(leave.LeavingTile);
                    EvaluateAdjacentTiles();
                    if (AdjacentTiles.Count > 0)
                        Wake();
                }
                return false;
            } 
            else if (gameEvent.GetType().IsEquivalentTo(typeof(OnEnterTile)))
            {
                var enter = gameEvent as OnEnterTile;
                if (!IsViableTile(enter.EnteringTile))
                    AdjacentTiles.Remove(enter.EnteringTile);
                return false;
            }
            else if (gameEvent.GetType().IsEquivalentTo(typeof(OnTime)))
            {
                OnTime timeEvent = gameEvent as OnTime;
                if (timeEvent.EventReason.GetType().IsEquivalentTo(typeof(ReasonDie)))
                {
                    Kill();
                } 
                else if (timeEvent.EventReason.GetType().IsEquivalentTo(typeof(ReasonGrow)))
                {
                    Grow();
                    if (lifeStage <= LifeStages.Length)
                        Game1.MasterTimeScheduler.Subscribe(new OnTime(this, this, new ReasonGrow(), 10000), this);
                }
                return true;
            }
            else if (gameEvent.GetType().IsEquivalentTo(typeof(OnDeath)))
            {

                return false;
            }
            return true;
        }
    }
}
