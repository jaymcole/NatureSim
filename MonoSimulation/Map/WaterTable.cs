using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSimulation.Globals;
using System;

namespace MonoSimulation.Map
{
    public class WaterTable
    {
        private VertexPositionTexture[] _waterVertices;
        private int[] _waterIndices;
        private float[] _waterVertOffsetY;
        private float waterPhase = 0;

        public static Effect WaterShader;

        public static Texture2D WaterImage;
        public static Texture2D LandImage;
        public static Texture2D SkyImage;


        public WaterTable(float width, float length)
        {
            WaterWidth = width;
            WaterLength = length;
            WaterLevel = Game1.GlobalWaterLevel;
            _waterVertOffsetY = new float[] {
                0,
                0,
                0,
                0,
            };
            LoadContent();
            GenerateModel();
        }

        private void LoadContent() 
        {
            WaterShader = GlobalVariables.WaterShader;
            WaterShader.Parameters["SampleType+normalTexture"].SetValue(GlobalVariables.WaterImage);
            WaterShader.Parameters["SampleType+reflectionTexture"].SetValue(GlobalVariables.SkyImage);
            WaterShader.Parameters["SampleType+refractionTexture"].SetValue(GlobalVariables.LandImage);
            WaterShader.Parameters["normalMapTiling"].SetValue(new Vector2(.1f, .1f));
            WaterShader.Parameters["reflectRefractScale"].SetValue(0.8f);
            WaterShader.Parameters["refractionTint"].SetValue(new Vector4(0.580f, 0.952f, 0.976f, 0.1f));
            WaterShader.Parameters["lightDirection"].SetValue(Game1.cam2.LookAtDirection);
            WaterShader.Parameters["specularShininess"].SetValue(0.5f);
        }

        private void GenerateModel()
        {
            _waterVertices = new VertexPositionTexture[]
            {
                new VertexPositionTexture(new Vector3(WaterX, WaterLevel, WaterZ), new Vector2(0,0)),
                new VertexPositionTexture(new Vector3(WaterX + WaterWidth, WaterLevel, WaterZ), new Vector2(1,0)),
                new VertexPositionTexture(new Vector3(WaterX, WaterLevel, WaterZ + WaterLength), new Vector2(0,1)),
                new VertexPositionTexture(new Vector3(WaterX + WaterWidth, WaterLevel, WaterZ + WaterLength), new Vector2(1,1)),
            };

            _waterIndices = new int[]
            {
                0, 1, 3,
                0, 3, 2
            };
        }

        public void Update(GameTime delta)
        {


            for (int i = 0; i < _waterVertices.Length; i++)
            {
                _waterVertOffsetY[i] += (float)GlobalVariables.random.NextDouble() * 0.1f;
                _waterVertOffsetY[i] %= 360;
                WaterLevel = Game1.GlobalWaterLevel + (float)(Math.Sin(_waterVertOffsetY[i] * 0.0174533) * 10);
                _waterVertices[i].Position.Y = WaterLevel ;
            }

            waterPhase += 0.001f;// (float)Game1.random.NextDouble() * 0.001f;
            if (waterPhase > 1)
                waterPhase -= 1;

            WaterShader.Parameters["worldMatrix"].SetValue(Matrix.Identity);
            WaterShader.Parameters["viewMatrix"].SetValue(Game1.cam.View);
            WaterShader.Parameters["projectionMatrix"].SetValue(Game1.cam.Projection);
            WaterShader.Parameters["reflectionMatrix"].SetValue(Game1.cam.View);
            WaterShader.Parameters["cameraPosition"].SetValue(Game1.cam.Position);
            WaterShader.Parameters["waterTranslation"].SetValue(waterPhase);
        }

        public void Render(GraphicsDeviceManager graphics)
        {
            foreach (EffectPass pass in WaterShader.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
                graphics.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _waterVertices, 0, _waterVertices.Length, _waterIndices, 0, 2);
            }
        }

        public void RenderDebug(GraphicsDeviceManager graphics)
        {
            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.FillMode = FillMode.WireFrame;
            rasterizerState.CullMode = CullMode.None;
            graphics.GraphicsDevice.RasterizerState = rasterizerState;
            graphics.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _waterVertices, 0, _waterVertices.Length, _waterIndices, 0, 2);
        }

        #region Getters/Setters
        private float _waterLevel;
        public float WaterLevel 
        {
            get { return _waterLevel; }
            set { _waterLevel = value; }
        }

        private float _waterX;
        public float WaterX
        {
            get { return _waterX; }
            set 
            {
                float difference = _waterX - value;
                _waterX = value;
                if (difference != 0 && _waterVertices != null)
                {
                    for (int i = 0; i < _waterVertices.Length; i++)
                    {
                        _waterVertices[i].Position.X -= difference;
                    }
                }
            }
        }

        private float _waterZ;
        public float WaterZ
        {
            get { return _waterZ; }
            set { 
                float difference = _waterZ - value;
                _waterZ = value; 
                if (difference != 0 && _waterVertices != null)
                {
                    for (int i = 0; i < _waterVertices.Length; i++)
                    {
                        _waterVertices[i].Position.Z -= difference;
                    }
                }
            }
        }

        private float _waterWidth;
        public float WaterWidth
        {
            get { return _waterWidth; }
            set { _waterWidth = value; }
        }

        private float _waterLength;
        public float WaterLength
        {
            get { return _waterLength; }
            set {
                if (value != _waterLength)
                {
                    _waterLength = value; 
                }
            }
        }
        #endregion
    }
}
