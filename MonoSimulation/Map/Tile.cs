using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSimulation;
using MonoSimulation.Engine.Events;
using MonoSimulation.GameObjects;
using MonoSimulation.Globals;
using MonoSimulation.Map;
using MonoSimulator.EnvironmentProperties;
using MonoSimulator.GameObjects.BaseControls;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MonoSimulator.Map
{
    public class Tile : NotifierBase, ISubscriber
    {
        public static Random random = new Random();

        public int row, col;
        public Vector2 position;
        public bool NeedsUpdating;
        public float waterCapacity;
        public float elevation;


        public List<GameObject> ObjectsOnTile;
        public void RemoveType(Type type, bool removeSubclasses)
        {
            for (int i = 0; i < ObjectsOnTile.Count; i++)
            {
                if (ObjectsOnTile[i].GetType().IsEquivalentTo(type) || (removeSubclasses && ObjectsOnTile[i].GetType().IsSubclassOf(type)))
                {
                    ObjectsOnTile[i].Kill();
                }
            }
        }
        public void EnterTile (GameObject go)
        {
            ObjectsOnTile.Add(go);
            Notify(new OnEnterTile(go, this));
        }

        public void LeaveTile (GameObject go)
        {
            ObjectsOnTile.Remove(go);
            Notify(new OnLeaveTile(go, this));
        }

        public int ObjectCount 
        {
            get
            {
                if (ObjectsOnTile != null)
                    return ObjectsOnTile.Count;
                return 0;
            }
        }
        


        public Tile(int row, int col, float x, float z, float elevation)
        {
            this.row = row;
            this.col = col;
            ObjectsOnTile = new List<GameObject>();
            this.elevation = elevation;
            position = new Vector2(x, z);
            Water = random.Next(5, 40);
            waterCapacity = 25;
        }

        public void Update(int steps)
        {
            UpdateWater();
            //TopColor = Color.Lerp(GlobalVariables.MoistDirtColor, Color.Tan, GroundWaterSaturation);
            TopColor = GlobalVariables.MoistDirtColor;
        }

        private void UpdateWater()
        {

            if (_water > waterCapacity)
            {
                float difference = _water - waterCapacity;
                _water = waterCapacity;

                if (Game1.map.WaterTable != null)
                    Game1.map.WaterTable.WaterLevel += difference;

            }

            float toWaterTable = Water * 0.2f;
            float toEvaporate = Water * 0.2f;

            Water -= toWaterTable + toEvaporate;
        }

        public float GroundWaterSaturation 
        {
            get
            {
                if (elevation >= Game1.map.WaterTable.WaterLevel)
                    return 1.0f;
                return Water/(waterCapacity + 0.0f);
            }
        }

        private List<Tile> _adjacentTiles;
        public List<Tile> AdjacentTiles
        {
            get
            {
                if (_adjacentTiles == null)
                {
                    _adjacentTiles = Game1.map.GetAdjacent(this);
                }
                return _adjacentTiles;
            }

            protected set
            {
                _adjacentTiles = value;
            }
        }

        public float _water;
        public float Water 
        {
            get 
            {
                return _water;
            }
            set
            {
                _water = value;
            }
        }

        private Color _topColor;
        public Color TopColor
        {
            get
            {
                if (_topColor == null)
                    return Color.Magenta;
                return _topColor;
            }
            set
            {
                _topColor = value;
            }
        }

        public int TypeCount(Type type)
        {
            int count = 0;
            foreach (GameObject go in ObjectsOnTile)
            {
                if (go.GetType() == type)
                    count++;
            }
            return count;
        }

        public bool OnNotify(IGameEvent gameEvent)
        {
            return false;
        }
    }
}
