using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSimulation;
using MonoSimulation.Globals;
using MonoSimulation.Models;
using MonoSimulator.Generators;

namespace MonoSimulation.Generators
{
    public class ModelGenerator
    {
        private static Random random = new Random();


        public static void Grass(out VertexPositionColor[] vertices, out int[] indices)
        {
            
            vertices = new VertexPositionColor[] 
            {
                   
            };

            indices = new int[] 
            {
                
            };

        }

        public static JModel CreateQuad(float scale = 1f, float xScale = 1f, float yScale = 1f, float zScale = 1f)
        {
            JModel model = new JModel();

            float x = 0, y = 0, z = 0;
            float xOffset = 1, yOffset = 1;

            model.Vertices = new VertexPositionNormalTexture[4];
            model.Indices = new int[6];
            model.PrimitivesTriangles = 2;
            model.PrimitivesLines = 4;

            float[] pointOffsetsX = new float[6];
            float[] pointOffsetsZ = new float[6];
            float[] distances = new float[6];

            Vector2[] texCoords = new Vector2[] {
                new Vector2(1,1),
                new Vector2(0,1),
                new Vector2(1,0),
                new Vector2(0,0),
            };

            Vector3[] normal = new Vector3[] {
                new Vector3(1,0,0),
                new Vector3(1,0,0),
                new Vector3(1,0,0),
                new Vector3(1,0,0),
            };


            model.Vertices[0] = new VertexPositionNormalTexture(
                new Vector3(
                    x,
                    y,
                    z),
                normal[0],
                texCoords[0]
            );


            model.Vertices[1] = new VertexPositionNormalTexture(
                new Vector3(
                    x + xOffset,
                    y,
                    z),
                normal[1],
                texCoords[1]
            );

            model.Vertices[2] = new VertexPositionNormalTexture(
                new Vector3(
                    x,
                    y + yOffset,
                    z),
                normal[2],
                texCoords[2]
            );


            model.Vertices[3] = new VertexPositionNormalTexture(
                new Vector3(
                    x + xOffset,
                    y + yOffset,
                    z),
                normal[3],
                texCoords[3]
            );

            model.Indices[0] = 0;
            model.Indices[1] = 3;
            model.Indices[2] = 2;
            model.Indices[3] = 0;
            model.Indices[4] = 3;
            model.Indices[5] = 1;

            for (int i = 0; i < model.Vertices.Length; i++)
            {
                model.Vertices[i].Position.Y *= yScale * scale;
                model.Vertices[i].Position.X *= xScale * scale;
                model.Vertices[i].Position.Z *= zScale * scale;
                model.Vertices[i].Position.X -= xScale * 0.5f;
            }
            return model;
        }

        public static void CreateClouds(GraphicsDeviceManager graphics, Texture2D image, float tileRadius, out int primitives, out VertexPositionColor[] vertices, out int[] triangles)
        {
            int numberOfVertices = 6;
            int numberOfIndices = 12;
            int numberOfPrimitives = 3;

            int xSize = (int)image.Width;
            int zSize = (int)image.Height;

            vertices = new VertexPositionColor[xSize * zSize * numberOfVertices];
            triangles = new int[xSize * zSize * numberOfIndices];
            primitives = xSize * zSize * numberOfPrimitives;

            xSize--; zSize--;

            Color[] colors = new Color[image.Width * image.Height];
            image.GetData<Color>(colors);

            float[] pointOffsetsX = new float[6];
            float[] pointOffsetsZ = new float[6];
            float[] distances = new float[6];

            float temp = tileRadius * 0.9f;
            pointOffsetsX = new float[]
            {
                temp,
                temp / 2.0f,
                -temp / 2.0f,
                -temp,
                -temp / 2.0f,
                temp / 2.0f,
            };
            pointOffsetsZ = new float[]
            {
                0,
                (float)(Math.Sqrt(3) * temp) / 2.0f,
                (float)(Math.Sqrt(3) * temp) / 2.0f,
                0,
                (float)(Math.Sqrt(3) * temp) / -2.0f,
                (float)(Math.Sqrt(3) * temp) / -2.0f,
            };

            for (int i = 0; i < pointOffsetsX.Length; i++)
            {
                distances[i] = (float)(Math.Sqrt((pointOffsetsX[i] * pointOffsetsX[i]) + (pointOffsetsZ[i] * pointOffsetsZ[i])));
            }

            Color cloudColor = Color.White;

            for (int i = 0, z = 0; z <= zSize; z++)
            {
                for (int x = 0; x <= xSize; x++, i += numberOfVertices)
                {
                    float apothem = (float)((tileRadius * Math.Sqrt(3)) / 2.0f);
                    float xCenter = (float)(tileRadius + (tileRadius * 0.5f)) * x;
                    float zCenter = (float)(2 * apothem) * z;

                    if (x % 2 == 0)
                        zCenter += apothem;

                    float height = GlobalVariables.CloudAltitude;

                    vertices[i] = new VertexPositionColor(
                        new Vector3(
                            xCenter + pointOffsetsX[0],
                            height,
                            zCenter + pointOffsetsZ[0]),
                        cloudColor
                    );


                    vertices[i + 1] = new VertexPositionColor(
                        new Vector3(
                            xCenter + pointOffsetsX[1],
                            height,
                            zCenter + pointOffsetsZ[1]),
                        cloudColor
                    );

                    vertices[i + 2] = new VertexPositionColor(
                        new Vector3(
                            xCenter + pointOffsetsX[2],
                           height,
                            zCenter + pointOffsetsZ[2]),
                        cloudColor
                    );


                    vertices[i + 3] = new VertexPositionColor(
                        new Vector3(
                            xCenter + pointOffsetsX[3],
                            height,
                            zCenter + pointOffsetsZ[3]),
                        cloudColor
                    );

                    vertices[i + 4] = new VertexPositionColor(
                        new Vector3(
                            xCenter + pointOffsetsX[4],
                            height,
                            zCenter + pointOffsetsZ[4]),
                        cloudColor
                    );

                    vertices[i + 5] = new VertexPositionColor(
                        new Vector3(
                            xCenter + pointOffsetsX[5],
                            height,
                            zCenter + pointOffsetsZ[5]),
                        cloudColor
                    );
                }
            }


            for (int ti = 0, hexStartVert = 0; ti < xSize * zSize * numberOfIndices; ti += numberOfIndices, hexStartVert += numberOfVertices)
            {
                triangles[ti] = hexStartVert + 5;
                triangles[ti + 1] = hexStartVert + 0;
                triangles[ti + 2] = hexStartVert + 4;

                triangles[ti + 3] = hexStartVert + 0;
                triangles[ti + 4] = hexStartVert + 3;
                triangles[ti + 5] = hexStartVert + 4;

                triangles[ti + 6] = hexStartVert + 0;
                triangles[ti + 7] = hexStartVert + 1;
                triangles[ti + 8] = hexStartVert + 3;

                triangles[ti + 9] = hexStartVert + 1;
                triangles[ti + 10] = hexStartVert + 2;
                triangles[ti + 11] = hexStartVert + 3;
            }
        }

        public static int GetTileStartIndex(int row, int col, int _columns, int verticesPerColumn)
        {
            int startVertex = row * _columns * verticesPerColumn;
            startVertex += col * verticesPerColumn;
            return startVertex;
        }

        /// <summary>
        /// Creates a 2D array of hexagonal columns. The height of each column is determined by image.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="image"></param>
        /// <param name="tileAmplitude"></param>
        /// <param name="tileRadius"></param>
        /// <param name="primitives"></param>
        /// <param name="vertices"></param>
        /// <param name="triangles"></param>
        public static void CreateHexagonsHeightMap(GraphicsDeviceManager graphics, Texture2D image, float tileAmplitude, float tileRadius, out int primitives, out VertexPositionNormalTexture[] vertices, out int[] triangles)
        {
            int xSize = (int)image.Width;
            int zSize = (int)image.Height;

            vertices = new VertexPositionNormalTexture[xSize * zSize * 120];
            triangles = new int[xSize * zSize * 60];
            primitives = xSize * zSize * 20;

            Color[] colors = new Color[image.Width * image.Height];
            image.GetData<Color>(colors);

            float[] pointOffsetsX = new float[6];
            float[] pointOffsetsZ = new float[6];
            float[] distances = new float[6];

            //for (int i = 0; i < pointOffsetsX.Length; i++)
            //{
            //    pointOffsetsX[i] = (float)(Math.Cos(i * 60 * 0.0174533) * radius);
            //    pointOffsetsZ[i] = (float)(Math.Sin(i * 60 * 0.0174533) * radius);
            //    distances[i] = (float)(Math.Sqrt(Math.Exp(pointOffsetsX[i]) + Math.Exp(pointOffsetsZ[i])));
            //}

            pointOffsetsX = new float[]
            {
                tileRadius,
                tileRadius / 2.0f,
                -tileRadius / 2.0f,
                -tileRadius,
                -tileRadius / 2.0f,
                tileRadius / 2.0f,
            };
            pointOffsetsZ = new float[]
            {
                0,
                (float)(Math.Sqrt(3) * tileRadius) / 2.0f,
                (float)(Math.Sqrt(3) * tileRadius) / 2.0f,
                0,
                (float)(Math.Sqrt(3) * tileRadius) / -2.0f,
                (float)(Math.Sqrt(3) * tileRadius) / -2.0f,
            };

            for (int i = 0; i < pointOffsetsX.Length; i++)
            {
                distances[i] = (float)(Math.Sqrt((pointOffsetsX[i] * pointOffsetsX[i]) + (pointOffsetsZ[i] * pointOffsetsZ[i])));
            }

            //pointOffsetsX = new float[] 
            //{ 
            //    -1,1,2,1,-1,-2
            //};
            //pointOffsetsZ = new float[]
            //{
            //    1,1,0,-1,-1,0
            //};

            for (int i = 0, y = 0; y <= zSize; y++)
            {
                for (int x = 0; x <= xSize; x++, i += 12)
                {
                    Vector2 textCoords = new Vector2((x + 0.0f) / image.Width, (y + 0.0f) / image.Height);
                    Vector3 normal = new Vector3(random.Next(0, 10), random.Next(0, 10), random.Next(0, 10));
                    normal = Vector3.Normalize(normal);

                    float w = (float)(Math.Sqrt(3) * tileRadius);
                    float h = tileRadius * 2;


                    float apothem = (float)((tileRadius * Math.Sqrt(3)) / 2.0f);
                    float xCenter = (float)(tileRadius + (tileRadius * 0.5f)) * x;
                    float zCenter = (float)(2 * apothem) * y;

                    if (x % 2 == 0)
                        zCenter += apothem;

                    float height = 0.1f * (colors[i / 12].R + colors[i / 12].G + colors[i / 12].B);

                    vertices[i] = new VertexPositionNormalTexture(
                        new Vector3(
                            xCenter + pointOffsetsX[0],
                            height,
                            zCenter + pointOffsetsZ[0]),
                        normal,
                        textCoords
                    );


                    vertices[i + 1] = new VertexPositionNormalTexture(
                        new Vector3(
                            xCenter + pointOffsetsX[1],
                            height,
                            zCenter + pointOffsetsZ[1]),
                        normal,
                        textCoords
                    );

                    vertices[i + 2] = new VertexPositionNormalTexture(
                        new Vector3(
                            xCenter + pointOffsetsX[2],
                           height,
                            zCenter + pointOffsetsZ[2]),
                        normal,
                        textCoords
                    );


                    vertices[i + 3] = new VertexPositionNormalTexture(
                        new Vector3(
                            xCenter + pointOffsetsX[3],
                            height,
                            zCenter + pointOffsetsZ[3]),
                        normal,
                        textCoords
                    );

                    vertices[i + 4] = new VertexPositionNormalTexture(
                        new Vector3(
                            xCenter + pointOffsetsX[4],
                            height,
                            zCenter + pointOffsetsZ[4]),
                        normal,
                        textCoords
                    );

                    vertices[i + 5] = new VertexPositionNormalTexture(
                        new Vector3(
                            xCenter + pointOffsetsX[5],
                            height,
                            zCenter + pointOffsetsZ[5]),
                        normal,
                        textCoords
                    );

                    //---------------------

                    vertices[i + 6] = new VertexPositionNormalTexture(
                        new Vector3(
                            xCenter + pointOffsetsX[0],
                            0,
                            zCenter + pointOffsetsZ[0]),
                        normal,
                        textCoords
                    );


                    vertices[i + 7] = new VertexPositionNormalTexture(
                        new Vector3(
                            xCenter + pointOffsetsX[1],
                            0,
                            zCenter + pointOffsetsZ[1]),
                        normal,
                        textCoords
                    );

                    vertices[i + 8] = new VertexPositionNormalTexture(
                        new Vector3(
                            xCenter + pointOffsetsX[2],
                            0,
                            zCenter + pointOffsetsZ[2]),
                        normal,
                        textCoords
                    );


                    vertices[i + 9] = new VertexPositionNormalTexture(
                        new Vector3(
                            xCenter + pointOffsetsX[3],
                            0,
                            zCenter + pointOffsetsZ[3]),
                        normal,
                        textCoords
                    );

                    vertices[i + 10] = new VertexPositionNormalTexture(
                        new Vector3(
                            xCenter + pointOffsetsX[4],
                            0,
                            zCenter + pointOffsetsZ[4]),
                        normal,
                        textCoords
                    );

                    vertices[i + 11] = new VertexPositionNormalTexture(
                        new Vector3(
                            xCenter + pointOffsetsX[5],
                            0,
                            zCenter + pointOffsetsZ[5]),
                        normal,
                        textCoords
                    );


                }
            }


            for (int ti = 0, hexStartVert = 0; ti < xSize * zSize * 60; ti += 60, hexStartVert += 12)
            {
                triangles[ti] = hexStartVert;
                triangles[ti + 1] = hexStartVert + 1;
                triangles[ti + 2] = hexStartVert + 7;
                triangles[ti + 3] = hexStartVert + 0;
                triangles[ti + 4] = hexStartVert + 7;
                triangles[ti + 5] = hexStartVert + 6;

                triangles[ti + 6] = hexStartVert + 1;
                triangles[ti + 7] = hexStartVert + 2;
                triangles[ti + 8] = hexStartVert + 8;
                triangles[ti + 9] = hexStartVert + 1;
                triangles[ti + 10] = hexStartVert + 8;
                triangles[ti + 11] = hexStartVert + 7;

                triangles[ti + 12] = hexStartVert + 3;
                triangles[ti + 13] = hexStartVert + 2;
                triangles[ti + 14] = hexStartVert + 8;
                triangles[ti + 15] = hexStartVert + 3;
                triangles[ti + 16] = hexStartVert + 8;
                triangles[ti + 17] = hexStartVert + 9;

                triangles[ti + 18] = hexStartVert + 4;
                triangles[ti + 19] = hexStartVert + 3;
                triangles[ti + 20] = hexStartVert + 9;
                triangles[ti + 21] = hexStartVert + 4;
                triangles[ti + 22] = hexStartVert + 9;
                triangles[ti + 23] = hexStartVert + 10;

                triangles[ti + 24] = hexStartVert + 5;
                triangles[ti + 25] = hexStartVert + 4;
                triangles[ti + 26] = hexStartVert + 10;
                triangles[ti + 27] = hexStartVert + 5;
                triangles[ti + 28] = hexStartVert + 10;
                triangles[ti + 29] = hexStartVert + 11;

                triangles[ti + 30] = hexStartVert + 5;
                triangles[ti + 31] = hexStartVert + 0;
                triangles[ti + 32] = hexStartVert + 6;
                triangles[ti + 33] = hexStartVert + 5;
                triangles[ti + 34] = hexStartVert + 6;
                triangles[ti + 35] = hexStartVert + 11;

                triangles[ti + 36] = hexStartVert + 5;
                triangles[ti + 37] = hexStartVert + 0;
                triangles[ti + 38] = hexStartVert + 4;

                triangles[ti + 39] = hexStartVert + 0;
                triangles[ti + 40] = hexStartVert + 3;
                triangles[ti + 41] = hexStartVert + 4;

                triangles[ti + 42] = hexStartVert + 0;
                triangles[ti + 43] = hexStartVert + 1;
                triangles[ti + 44] = hexStartVert + 3;

                triangles[ti + 45] = hexStartVert + 1;
                triangles[ti + 46] = hexStartVert + 2;
                triangles[ti + 47] = hexStartVert + 3;

                triangles[ti + 48] = hexStartVert + 11;
                triangles[ti + 49] = hexStartVert + 6;
                triangles[ti + 50] = hexStartVert + 10;


                triangles[ti + 51] = hexStartVert + 6;
                triangles[ti + 52] = hexStartVert + 9;
                triangles[ti + 53] = hexStartVert + 10;

                triangles[ti + 54] = hexStartVert + 6;
                triangles[ti + 55] = hexStartVert + 7;
                triangles[ti + 56] = hexStartVert + 9;

                triangles[ti + 57] = hexStartVert + 7;
                triangles[ti + 58] = hexStartVert + 8;
                triangles[ti + 59] = hexStartVert + 9;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="image"></param>
        /// <param name="tileAmplitude"></param>
        /// <param name="tileRadius"></param>
        /// <param name="primitives"></param>
        /// <param name="verticesBottom"></param>
        /// <param name="verticesTop"></param>
        /// <param name="triangles"></param>
        public static void CreateHexagonsHeightMap2(GraphicsDeviceManager graphics, Texture2D image, float tileAmplitude, float tileRadius, out int primitives, out VertexPositionColor[] verticesBottom, out VertexPositionColor[] verticesTop, out int[] triangles)
        {
            //int xSize = (int)image.Width-1;
            //int zSize = (int)image.Height-1;

            //float topColumnHeight = 1;
            //verticesBottom = new VertexPositionColor[(xSize+1) * (zSize+1) * 12];
            //verticesTop = new VertexPositionColor[(xSize + 1) * (zSize + 1) * 12];
            //triangles = new int[xSize * zSize * 60];
            //primitives = xSize * zSize * 20;

            //Color[] colors = new Color[image.Width * image.Height];
            //image.GetData<Color>(colors);

            int xSize = (int)image.Height;
            int zSize = (int)image.Width;


            float topColumnHeight = 5;
            verticesBottom = new VertexPositionColor[(xSize) * (zSize) * 12];
            verticesTop = new VertexPositionColor[(xSize) * (zSize) * 12];
            triangles = new int[xSize * zSize * 60];
            primitives = xSize * zSize * 20;

            Color[] colors = new Color[image.Width * image.Height];
            image.GetData<Color>(colors);

            float[] pointOffsetsX = new float[]{
                tileRadius,
                tileRadius / 2.0f,
                -tileRadius / 2.0f,
                -tileRadius,
                -tileRadius / 2.0f,
                tileRadius / 2.0f,
            };

            float[] pointOffsetsZ = new float[]
                {
                0,
                (float)(Math.Sqrt(3) * tileRadius) / 2.0f,
                (float)(Math.Sqrt(3) * tileRadius) / 2.0f,
                0,
                (float)(Math.Sqrt(3) * tileRadius) / -2.0f,
                (float)(Math.Sqrt(3) * tileRadius) / -2.0f,
            };

            float[] distances = new float[6];


            for (int i = 0; i < pointOffsetsX.Length; i++)
            {
                distances[i] = (float)(Math.Sqrt((pointOffsetsX[i] * pointOffsetsX[i]) + (pointOffsetsZ[i] * pointOffsetsZ[i])));
            }


            Color topColumnsTopColor = new Color(random.Next(0, 256), random.Next(0, 256), random.Next(0, 256), 1);
            Color topColumnsBottomColor = Color.Tan;
            Color bottomColumnTopColor = Color.Tan;
            Color bottomColumnsBottomColor = Color.Black;

            #region column vertices
            for (int i = 0, row = 0; row < zSize; row++)
            {
                for (int col = 0; col < xSize; col++, i += 12)
                {
                    i = GetTileStartIndex(row, col, xSize, 12);

                    topColumnsTopColor = new Color(random.Next(0, 256), random.Next(0, 256), random.Next(0, 256), 1);

                    //Vector2 textCoords = new Vector2((col + 0.0f) / image.Width, (row + 0.0f) / image.Height);
                    //Vector3 normal = new Vector3(random.Next(0, 10), random.Next(0, 10), random.Next(0, 10));
                    //normal = Vector3.Normalize(normal);
                    //float w = (float)(Math.Sqrt(3) * tileRadius);
                    //float h = tileRadius * 2;


                    float apothem = (float)((tileRadius * Math.Sqrt(3)) / 2.0f);
                    float xCenter = (float)(tileRadius + (tileRadius * 0.5f)) * col;
                    float zCenter = (float)(2 * apothem) * row;

                    if (col % 2 == 0)
                        zCenter += apothem;

                    int colorIndex = (col * zSize) + row;

                    float height = ((colors[colorIndex].R + colors[colorIndex].G + colors[colorIndex].B));
                    float bottomHeight = height - topColumnHeight;
                    height *= tileAmplitude;
                    bottomHeight *= tileAmplitude;

                    for (int vert = 0; vert < 6; vert++)
                    {

                        verticesTop[i + vert] = new VertexPositionColor(
                        new Vector3(
                            xCenter + pointOffsetsX[vert],
                            height,
                            zCenter + pointOffsetsZ[vert]),
                        topColumnsTopColor
                        );

                        verticesTop[i + 6 + vert] = new VertexPositionColor(
                        new Vector3(
                            xCenter + pointOffsetsX[vert],
                            bottomHeight,
                            zCenter + pointOffsetsZ[vert]),
                        topColumnsBottomColor
                        );
                        //--------------------------------------------------
                        verticesBottom[i + vert] = new VertexPositionColor(
                        new Vector3(
                            xCenter + pointOffsetsX[vert],
                            bottomHeight,
                            zCenter + pointOffsetsZ[vert]),
                        bottomColumnTopColor
                        );
                        verticesBottom[i + vert + 6] = new VertexPositionColor(
                        new Vector3(
                            xCenter + pointOffsetsX[vert],
                            0,
                            zCenter + pointOffsetsZ[vert]),
                        bottomColumnsBottomColor
                        );
                    }

                    #region Top Columns
                    //verticesTop[i] = new VertexPositionColor(
                    //    new Vector3(
                    //        xCenter + pointOffsetsX[0],
                    //        height,
                    //        zCenter + pointOffsetsZ[0]),
                    //    topColumnsTopColor
                    //);


                    //verticesTop[i + 1] = new VertexPositionColor(
                    //    new Vector3(
                    //        xCenter + pointOffsetsX[1],
                    //        height,
                    //        zCenter + pointOffsetsZ[1]),
                    //    topColumnsTopColor
                    //);

                    //verticesTop[i + 2] = new VertexPositionColor(
                    //    new Vector3(
                    //        xCenter + pointOffsetsX[2],
                    //       height,
                    //        zCenter + pointOffsetsZ[2]),
                    //    topColumnsTopColor
                    //);


                    //verticesTop[i + 3] = new VertexPositionColor(
                    //    new Vector3(
                    //        xCenter + pointOffsetsX[3],
                    //        height,
                    //        zCenter + pointOffsetsZ[3]),
                    //    topColumnsTopColor
                    //);

                    //verticesTop[i + 4] = new VertexPositionColor(
                    //    new Vector3(
                    //        xCenter + pointOffsetsX[4],
                    //        height,
                    //        zCenter + pointOffsetsZ[4]),
                    //    topColumnsTopColor
                    //);

                    //verticesTop[i + 5] = new VertexPositionColor(
                    //    new Vector3(
                    //        xCenter + pointOffsetsX[5],
                    //        height,
                    //        zCenter + pointOffsetsZ[5]),
                    //    topColumnsTopColor
                    //);


                    ////---------------------


                    //verticesTop[i + 6] = new VertexPositionColor(
                    //    new Vector3(
                    //        xCenter + pointOffsetsX[0],
                    //        bottomHeight,
                    //        zCenter + pointOffsetsZ[0]),
                    //    topColumnsBottomColor
                    //);


                    //verticesTop[i + 7] = new VertexPositionColor(
                    //    new Vector3(
                    //        xCenter + pointOffsetsX[1],
                    //        bottomHeight,
                    //        zCenter + pointOffsetsZ[1]),
                    //    topColumnsBottomColor
                    //);

                    //verticesTop[i + 8] = new VertexPositionColor(
                    //    new Vector3(
                    //        xCenter + pointOffsetsX[2],
                    //        bottomHeight,
                    //        zCenter + pointOffsetsZ[2]),
                    //    topColumnsBottomColor
                    //);


                    //verticesTop[i + 9] = new VertexPositionColor(
                    //    new Vector3(
                    //        xCenter + pointOffsetsX[3],
                    //        bottomHeight,
                    //        zCenter + pointOffsetsZ[3]),
                    //    topColumnsBottomColor
                    //);

                    //verticesTop[i + 10] = new VertexPositionColor(
                    //    new Vector3(
                    //        xCenter + pointOffsetsX[4],
                    //        bottomHeight,
                    //        zCenter + pointOffsetsZ[4]),
                    //    topColumnsBottomColor
                    //);

                    //verticesTop[i + 11] = new VertexPositionColor(
                    //    new Vector3(
                    //        xCenter + pointOffsetsX[5],
                    //        bottomHeight,
                    //        zCenter + pointOffsetsZ[5]),
                    //    topColumnsBottomColor
                    //);
                    #endregion

                    #region Bottom Columns
                    //verticesBottom[i] = new VertexPositionColor(
                    //    new Vector3(
                    //        xCenter + pointOffsetsX[0],
                    //        bottomHeight,
                    //        zCenter + pointOffsetsZ[0]),
                    //    bottomColumnTopColor
                    //);


                    //verticesBottom[i + 1] = new VertexPositionColor(
                    //    new Vector3(
                    //        xCenter + pointOffsetsX[1],
                    //        bottomHeight,
                    //        zCenter + pointOffsetsZ[1]),
                    //    bottomColumnTopColor
                    //);

                    //verticesBottom[i + 2] = new VertexPositionColor(
                    //    new Vector3(
                    //        xCenter + pointOffsetsX[2],
                    //       bottomHeight,
                    //        zCenter + pointOffsetsZ[2]),
                    //    bottomColumnTopColor
                    //);


                    //verticesBottom[i + 3] = new VertexPositionColor(
                    //    new Vector3(
                    //        xCenter + pointOffsetsX[3],
                    //        bottomHeight,
                    //        zCenter + pointOffsetsZ[3]),
                    //    bottomColumnTopColor
                    //);

                    //verticesBottom[i + 4] = new VertexPositionColor(
                    //    new Vector3(
                    //        xCenter + pointOffsetsX[4],
                    //        bottomHeight,
                    //        zCenter + pointOffsetsZ[4]),
                    //    bottomColumnTopColor
                    //);

                    //verticesBottom[i + 5] = new VertexPositionColor(
                    //    new Vector3(
                    //        xCenter + pointOffsetsX[5],
                    //        bottomHeight,
                    //        zCenter + pointOffsetsZ[5]),
                    //    bottomColumnTopColor
                    //);
                    ////---------------------

                    //verticesBottom[i + 6] = new VertexPositionColor(
                    //    new Vector3(
                    //        xCenter + pointOffsetsX[0],
                    //        0,
                    //        zCenter + pointOffsetsZ[0]),
                    //    bottomColumnsBottomColor
                    //);


                    //verticesBottom[i + 7] = new VertexPositionColor(
                    //    new Vector3(
                    //        xCenter + pointOffsetsX[1],
                    //        0,
                    //        zCenter + pointOffsetsZ[1]),
                    //    bottomColumnsBottomColor
                    //);

                    //verticesBottom[i + 8] = new VertexPositionColor(
                    //    new Vector3(
                    //        xCenter + pointOffsetsX[2],
                    //        0,
                    //        zCenter + pointOffsetsZ[2]),
                    //    bottomColumnsBottomColor
                    //);


                    //verticesBottom[i + 9] = new VertexPositionColor(
                    //    new Vector3(
                    //        xCenter + pointOffsetsX[3],
                    //        0,
                    //        zCenter + pointOffsetsZ[3]),
                    //    bottomColumnsBottomColor
                    //);

                    //verticesBottom[i + 10] = new VertexPositionColor(
                    //    new Vector3(
                    //        xCenter + pointOffsetsX[4],
                    //        0,
                    //        zCenter + pointOffsetsZ[4]),
                    //    bottomColumnsBottomColor
                    //);

                    //verticesBottom[i + 11] = new VertexPositionColor(
                    //    new Vector3(
                    //        xCenter + pointOffsetsX[5],
                    //        0,
                    //        zCenter + pointOffsetsZ[5]),
                    //    bottomColumnsBottomColor
                    //);
                    #endregion
                }
            }
            #endregion

            #region bottom column indices
            for (int ti = 0, hexStartVert = 0; ti < xSize * zSize * 60; ti += 60, hexStartVert += 12)
            {
                triangles[ti] = hexStartVert;
                triangles[ti + 1] = hexStartVert + 1;
                triangles[ti + 2] = hexStartVert + 7;
                triangles[ti + 3] = hexStartVert + 0;
                triangles[ti + 4] = hexStartVert + 7;
                triangles[ti + 5] = hexStartVert + 6;

                triangles[ti + 6] = hexStartVert + 1;
                triangles[ti + 7] = hexStartVert + 2;
                triangles[ti + 8] = hexStartVert + 8;
                triangles[ti + 9] = hexStartVert + 1;
                triangles[ti + 10] = hexStartVert + 8;
                triangles[ti + 11] = hexStartVert + 7;

                triangles[ti + 12] = hexStartVert + 3;
                triangles[ti + 13] = hexStartVert + 2;
                triangles[ti + 14] = hexStartVert + 8;
                triangles[ti + 15] = hexStartVert + 3;
                triangles[ti + 16] = hexStartVert + 8;
                triangles[ti + 17] = hexStartVert + 9;

                triangles[ti + 18] = hexStartVert + 4;
                triangles[ti + 19] = hexStartVert + 3;
                triangles[ti + 20] = hexStartVert + 9;
                triangles[ti + 21] = hexStartVert + 4;
                triangles[ti + 22] = hexStartVert + 9;
                triangles[ti + 23] = hexStartVert + 10;

                triangles[ti + 24] = hexStartVert + 5;
                triangles[ti + 25] = hexStartVert + 4;
                triangles[ti + 26] = hexStartVert + 10;
                triangles[ti + 27] = hexStartVert + 5;
                triangles[ti + 28] = hexStartVert + 10;
                triangles[ti + 29] = hexStartVert + 11;

                triangles[ti + 30] = hexStartVert + 5;
                triangles[ti + 31] = hexStartVert + 0;
                triangles[ti + 32] = hexStartVert + 6;
                triangles[ti + 33] = hexStartVert + 5;
                triangles[ti + 34] = hexStartVert + 6;
                triangles[ti + 35] = hexStartVert + 11;

                triangles[ti + 36] = hexStartVert + 5;
                triangles[ti + 37] = hexStartVert + 0;
                triangles[ti + 38] = hexStartVert + 4;

                triangles[ti + 39] = hexStartVert + 0;
                triangles[ti + 40] = hexStartVert + 3;
                triangles[ti + 41] = hexStartVert + 4;

                triangles[ti + 42] = hexStartVert + 0;
                triangles[ti + 43] = hexStartVert + 1;
                triangles[ti + 44] = hexStartVert + 3;

                triangles[ti + 45] = hexStartVert + 1;
                triangles[ti + 46] = hexStartVert + 2;
                triangles[ti + 47] = hexStartVert + 3;

                triangles[ti + 48] = hexStartVert + 11;
                triangles[ti + 49] = hexStartVert + 6;
                triangles[ti + 50] = hexStartVert + 10;


                triangles[ti + 51] = hexStartVert + 6;
                triangles[ti + 52] = hexStartVert + 9;
                triangles[ti + 53] = hexStartVert + 10;

                triangles[ti + 54] = hexStartVert + 6;
                triangles[ti + 55] = hexStartVert + 7;
                triangles[ti + 56] = hexStartVert + 9;

                triangles[ti + 57] = hexStartVert + 7;
                triangles[ti + 58] = hexStartVert + 8;
                triangles[ti + 59] = hexStartVert + 9;
            }
            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="image"></param>
        /// <param name="normalMap"></param>
        /// <param name="edgeLength"></param>
        /// <param name="primitives"></param>
        /// <param name="vertices"></param>
        /// <param name="triangles"></param>
        public static void CreateHeightMap(GraphicsDeviceManager graphics, Texture2D image, Texture2D normalMap, float edgeLength, out int primitives, out VertexPositionNormalTexture[] vertices, out int[] triangles)
        {
            int xSize = (int)image.Width;
            int ySize = (int)image.Height;

            float min = float.MaxValue, max = float.MinValue;


            Color[] colors = new Color[image.Width * image.Height];
            image.GetData<Color>(colors);
            Random random = new Random();
            vertices = new VertexPositionNormalTexture[(xSize + 1) * (ySize + 1)];

            for (int i = 0, y = 0; y <= ySize; y++)
            {
                for (int x = 0; x <= xSize; x++, i++)
                {
                    Vector2 textCoords = new Vector2((x + 0.0f) / image.Width, (y + 0.0f) / image.Height);
                    Vector3 normal = new Vector3(random.Next(0, 10), random.Next(0, 10), random.Next(0, 10));
                    normal = Vector3.Normalize(normal);
                    vertices[i] = new VertexPositionNormalTexture(
                        new Vector3(x * edgeLength, 0.1f * (colors[i].R + colors[i].G + colors[i].B), y * edgeLength),
                        normal,
                        textCoords);

                    if (vertices[i].Position.Y > max)
                        max = vertices[i].Position.Y;
                    if (vertices[i].Position.Y < min)
                        min = vertices[i].Position.Y;
                }
            }
            max -= min;
            float lowTier = (max) * 0.33f;
            float midTier = (max) * 0.66f;
            normalMap = new Texture2D(graphics.GraphicsDevice, image.Width, image.Height);
            Color[] normalMapColors = new Color[image.Width * image.Height];
            for (int i = 0, y = 0; y <= ySize; y++)
            {
                for (int x = 0; x <= xSize; x++, i++)
                {
                    vertices[i].Position.Y -= min;
                    normalMapColors[i] = new Color((float)(random.NextDouble()), (float)(random.NextDouble()), (float)(random.NextDouble()), 1f);
                }
            }
            normalMap.SetData(normalMapColors);

            primitives = 0;
            triangles = new int[xSize * ySize * 6];
            for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
            {
                for (int x = 0; x < xSize; x++, ti += 6, vi++)
                {
                    triangles[ti] = (int)vi;
                    triangles[ti + 3] = triangles[ti + 2] = (int)(vi + 1);
                    triangles[ti + 4] = triangles[ti + 1] = (int)(vi + xSize + 1);
                    triangles[ti + 5] = (int)(vi + xSize + 2);
                    primitives += 2;
                }
            }
        }

    }
}
