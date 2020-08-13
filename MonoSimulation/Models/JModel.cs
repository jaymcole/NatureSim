using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoSimulation.Models
{
    public class JModel
    {
        public Matrix worldPosition;
        public int PrimitivesTriangles;
        public int PrimitivesLines;
        public VertexPositionNormalTexture[] Vertices;
        public Texture2D texture;
        public int[] Indices;

        public void UpdatePosition()
        {

        }

        public void Render(Effect effect, GraphicsDeviceManager graphics, SpriteBatch batch)
        {
            graphics.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, Vertices, 0, Vertices.Length, Indices, 0, PrimitivesTriangles);
        }

        public void RenderTriangles(Effect effect, GraphicsDeviceManager graphics, SpriteBatch batch)
        {
            graphics.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.LineList, Vertices, 0, Vertices.Length, Indices, 0, PrimitivesLines);
        }

    }
}
