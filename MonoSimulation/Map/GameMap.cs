using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSimulation;
using MonoSimulation.Enums;
using MonoSimulation.Generators;
using MonoSimulation.Globals;
using MonoSimulation.Map;
using MonoSimulator.Generators;

namespace MonoSimulator.Map
{
    public class GameMap
    {


        private int _rows, _columns;
        public Tile[,] _tiles;
        private Weather[,] _weather;

        private VertexPositionColor[] _groundVerticesTop;
        private VertexPositionColor[] _groundVertices;
        private VertexPositionColor[] _groundLines;

        private VertexPositionColor[] _cloudVertices;

        private int[] _groundIndices;
        private int[] _cloudIndices;


        float tileAmplitude = 0.1f;
        int primitives = 0;
        int cloudPrimitives = 0;
        //float edgeLength = 1f;
        //float miniMapscale = 0.1f;

        public static Texture2D LoadPicture(string Filename)
        {
            FileStream setStream = File.Open(Filename, FileMode.Open);
            Texture2D NewTexture = Texture2D.FromStream(Game1.graphics.GraphicsDevice, setStream);
            setStream.Dispose();
            return NewTexture;
        }

        public GameMap()
        {


            _rows = GlobalVariables.LandImage.Width;
            _columns = GlobalVariables.LandImage.Height;
            
            ModelGenerator.CreateHexagonsHeightMap2(Game1.graphics, GlobalVariables.LandImage, tileAmplitude, GlobalVariables.TileRadius, out primitives, out _groundVerticesTop, out _groundVertices, out _groundIndices);
            ModelGenerator.CreateClouds(Game1.graphics, GlobalVariables.LandImage, GlobalVariables.TileRadius, out cloudPrimitives, out _cloudVertices, out _cloudIndices);

            _tiles = new Tile[_rows, _columns];
            _weather = new Weather[_rows, _columns];

            WaterTable = new WaterTable(900, 900);
            WaterTable.WaterX = -100;
            WaterTable.WaterZ = -100;

            List<Vector3> startPoints = new List<Vector3>();
            for (int i = 0; i < _groundVertices.Length/12; i++)
            {
                startPoints.Add(_groundVertices[(i * 12)].Position);
            }


            for (int row2 = 0; row2 < _rows; row2++)
            {
                for (int col2 = 0; col2 < _columns; col2++)
                {
                    int startIndex = GetTileStartIndex(row2, col2);
                    _tiles[row2, col2] = new Tile(
                        row2,
                        col2,
                        _groundVertices[startIndex].Position.X,
                        _groundVertices[startIndex].Position.Z,
                        _groundVertices[startIndex].Position.Y);
                    
                    _weather[row2, col2] = new Weather() { row = row2, col = col2, map = this };
                }
            }

            //for (int i = 0, row = 0; row < _rows; row++)
            //{
            //    for (int col = 0; col < _columns; col++, i++)
            //    {
            //        _tiles[row, col].elevation = _groundVertices[GetTileStartIndex(row,col)].Position.Y;
            //    }
            //}

            //_groundLines = new VertexPositionColor[_groundVertices.Length*2];
            //for (int i = 0; i < _groundVertices.Length; i++)
            //{
            //    _groundLines[i * 2] = new VertexPositionColor();
            //    _groundLines[i * 2].Position        = new Vector3(_groundVertices[i].Position.X, _groundVertices[i].Position.Y, _groundVertices[i].Position.Z);
            //    _groundLines[i * 2].Color           = Color.Red;
            //    _groundLines[(i * 2) + 1] = new VertexPositionColor();
            //    _groundLines[(i * 2) + 1].Position  = new Vector3(_groundVertices[i].Position.X, _groundVertices[i].Position.Y + 1, _groundVertices[i].Position.Z);
            //    _groundLines[(i * 2) + 1].Color     = Color.Red;
            //}

            //for (int i = 0; i < _groundVertices.Length; i+=12)
            //{





            //    _groundLines[i * 2] = new VertexPositionColor();
            //    _groundLines[i * 2].Position = new Vector3(_groundVertices[i].Position.X, _groundVertices[i].Position.Y, _groundVertices[i].Position.Z);
            //    _groundLines[i * 2].Color = Color.Red;
            //    _groundLines[(i * 2) + 1] = new VertexPositionColor();
            //    _groundLines[(i * 2) + 1].Position = new Vector3(_groundVertices[i].Position.X, _groundVertices[i].Position.Y + 10, _groundVertices[i].Position.Z);
            //    _groundLines[(i * 2) + 1].Color = Color.Red;
            //}


            _groundLines = new VertexPositionColor[(_groundVertices.Length/6)];
            for (int i = 0, row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _columns; col++, i++)
                {
                    int tempRow = row;
                    int tempCol = col+1;
                    if (tempCol >= _columns)
                    {
                        tempCol = 0;
                        tempRow++;
                    }
                    Tile tile1 = GetGroundTile(row, col);
                    Tile tile2 = GetGroundTile(tempRow, tempCol);

                    if (tile2 != null)
                    {
                        _groundLines[i * 2] = new VertexPositionColor();
                        _groundLines[i * 2].Position = new Vector3(tile1.position.X, tile1.elevation + 2, tile1.position.Y);
                        _groundLines[i * 2].Color = Color.Green;
                        _groundLines[(i * 2) + 1] = new VertexPositionColor();
                        _groundLines[(i * 2) + 1].Position = new Vector3(tile2.position.X, tile2.elevation + 2, tile2.position.Y);
                        _groundLines[(i * 2) + 1].Color = Color.Green;
                    }
                    else
                    {
                        _groundLines[i * 2] = new VertexPositionColor();
                        _groundLines[i * 2].Position = new Vector3(tile1.position.X, tile1.elevation + 2 , tile1.position.Y);
                        _groundLines[i * 2].Color = Color.Green;
                        _groundLines[(i * 2) + 1] = new VertexPositionColor();
                        _groundLines[(i * 2) + 1].Position = new Vector3(tile1.position.X, tile1.elevation + 100, tile1.position.Y);
                        _groundLines[(i * 2) + 1].Color = Color.Blue;
                    }

                }
            }
        }
    


        #region Updating
        private TimeSpan timeSinceLastWeatherUpdate = new TimeSpan();
        public void Update(GameTime delta)
        {
            timeSinceLastWeatherUpdate += delta.ElapsedGameTime;
            for (int i = 0, row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _columns; col++, i++)
                {
                    if (timeSinceLastWeatherUpdate.TotalSeconds > 1)
                    {
                        _weather[row, col].Update(delta);
                    }
                    _tiles[row, col].Update(1);

                    UpdateGroundTile(row, col, i);
                    UpdateCloudTile(row, col, i);


                }
            }

            if (timeSinceLastWeatherUpdate.TotalSeconds > 1)
            {
                timeSinceLastWeatherUpdate = new TimeSpan();
            }

            WaterTable.Update(delta);
            WaterTable.Update(delta);
        }

        

        //private void UpdateGroundLines(int row, int col, int i)
        //{
        //    float elevation = (float)_tiles[row, col].elevation;
        //    _groundVertices[i].Position.Y = elevation;

        //    float yOffset = 30;
        //    float h = (edgeLength * _tiles[row, col].);

        //    _groundLines[i * 2] = new VertexPositionColor();
        //    _groundLines[i * 2].Position = new Vector3(_groundVertices[i].Position.X * miniMapscale, yOffset, _groundVertices[i].Position.Z * miniMapscale);

        //    _groundLines[(i * 2) + 1] = new VertexPositionColor();
        //    _groundLines[(i * 2) + 1].Position = new Vector3(_groundVertices[i].Position.X * miniMapscale, yOffset + Math.Abs(h), _groundVertices[i].Position.Z * miniMapscale);

        //    Color colorBot = Color.Blue;
        //    Color colorTop = Color.Blue;
        //    if (h > 0)
        //    {
        //        colorBot = Color.Green;
        //        colorTop = new Color(0f, (float)Math.Abs(h) / 100f, 0f, 1f);
        //    }
        //    else
        //    {
        //        colorTop = Color.Red;
        //        colorBot = new Color((float)Math.Abs(h) / 100f, 0f, 0f, 1f);
        //    }
        //    _groundLines[i * 2].Color = colorTop;
        //    _groundLines[(i * 2) + 1].Color = colorBot;
        //}

        private void UpdateCloudTile(int row, int col, int i)
        {
            int vertsPerColumn = 6;
            for (int vertIndexStart = i * vertsPerColumn; vertIndexStart < (i * vertsPerColumn) + vertsPerColumn; vertIndexStart++)
            {
                Color cloudColor = Color.Lerp(Color.Black, Color.White, (float)_weather[row, col].moisture.value * 0.1f);
                //Color cloudColor = Color.Lerp(Color.White, Color.Black, (float)_weather[row, col].moisture.value * 0.1f);
                //cloudColor = Color.Black;
                _cloudVertices[vertIndexStart].Color = new Color(cloudColor.R, cloudColor.G, cloudColor.B, (float)_weather[row, col].moisture.value * 1f);

            }
        }

        private void UpdateGroundTile(int row, int col, int i)
        {
            int vertsPerColumn = 12;
            for (int vertIndexStart = i * vertsPerColumn; vertIndexStart < (i * vertsPerColumn) + vertsPerColumn; vertIndexStart++)
            {
                //_groundVertices[vertIndexStart].Color = new Color(row, col, 0, 255);//_tiles[row, col].TopColor;
                _groundVertices[vertIndexStart].Color = _tiles[row, col].TopColor;

            }
        }
        #endregion

        #region Rendering

        private SpriteFont myFont;


        public void debugRender(GraphicsDeviceManager graphics, SpriteBatch batch)
        {
            //graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, _groundLines, 0, _rows*_columns);
            WaterTable.RenderDebug(graphics);
        }

        public void debugCoords (GraphicsDeviceManager graphics, SpriteBatch batch)
        {
            if (myFont == null)
                myFont = GlobalVariables.Content.Load<SpriteFont>("Fonts/Font");

            List<Vector3> centers = new List<Vector3>();
            Random random = new Random(123456);
            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _columns; col++)
                {
                    var position3d = GetTileCenter(_tiles[row, col]);
                    centers.Add(position3d);
                    Vector3 pos = position3d;
                    position3d.Y += (float)random.NextDouble();
                    var viewport = graphics.GraphicsDevice.Viewport;
                    var position2d = viewport.Project(position3d, Game1.cam.Projection, Game1.cam.View, Matrix.Identity);

                    //var text = $"({row}, {col})";
                    int activeCount = 0;
                    int sleepingCount = 0;
                    foreach (GameObject go in _tiles[row, col].ObjectsOnTile)
                    {
                        if (go.IsSleeping)
                            sleepingCount++;
                        else
                            activeCount++;
                    }

                    var text = $"({_tiles[row, col].ObjectCount}, {activeCount}/{sleepingCount})";

                    var measure = myFont.MeasureString(text);

                    var centeredPosition = new Vector2(position2d.X - measure.X / 2, position2d.Y - measure.Y / 2);
                    batch.DrawString(myFont, text, centeredPosition, Color.Blue);
                }
            }

            //for (int i = 0; i < _groundVertices.Length; i+=6)
            //{
            //    for (int j = 0; j < 6; j++,  i++)
            //    {
            //        var position3d = _groundVertices[i+j].Position;
            //        centers.Add(position3d);
            //        var viewport = graphics.GraphicsDevice.Viewport;
            //        var position2d = viewport.Project(position3d, Game1.cam.Projection, Game1.cam.View, Matrix.Identity);

            //        var text = $"({position3d})";

            //        var measure = myFont.MeasureString(text);

            //        var centeredPosition = new Vector2(position2d.X - measure.X / 2, position2d.Y - measure.Y / 2);
            //        batch.DrawString(myFont, text, centeredPosition, Color.Black);
            //    }
            //}
        }

        public void RenderGroundLevel(GraphicsDeviceManager graphics, SpriteBatch batch)
        {
            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            graphics.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _groundVertices, 0, _groundVertices.Length, _groundIndices, 0, primitives);
            graphics.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _groundVerticesTop, 0, _groundVerticesTop.Length, _groundIndices, 0, primitives);
        }

        public void RenderClouds(GraphicsDeviceManager graphics, SpriteBatch batch)
        {
            graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            graphics.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _cloudVertices, 0, _cloudVertices.Length, _cloudIndices, 0, cloudPrimitives);
        }

        public void RenderWaterLevel(GraphicsDeviceManager graphics, SpriteBatch batch)
        {
            WaterTable.Render(graphics);
        }
        #endregion

        #region Getters/Setters

        public Vector3 GetTileCenter(Tile tile)
        {
            Vector3 pos = _groundVertices[GetTileStartIndex(tile.row, tile.col)].Position;
            //pos.X = tile.position.X - 2;
            //pos.Z = tile.position.Y;
            //pos.Y = tile.elevation;
            pos.X -= 2;
            return pos;
        }

        public int GetTileStartIndex (int row, int col)
        {
            int verticesPerColumn = _groundVertices.Length / (_rows * _columns);
            int startVertex = row * _columns * verticesPerColumn;
            startVertex += col * verticesPerColumn;
            return startVertex;

            //int verticesPerColumn = _groundVertices.Length / (_rows * _columns);
            //int startVertex = row * _columns * verticesPerColumn;
            //startVertex += col * verticesPerColumn;
            //return startVertex;
        }

        public Tile GetGroundTile(int row, int col)
        {
            while (row < 0)
                row += _rows;
            row %= _rows;

            while (col < 0)
                col += _columns;
            col %= _columns;

            if (InBounds(row, col))
                return _tiles[row, col];
            return null;
        }

        public Weather GetWeatherTile(int row, int col)
        {
            if (InBounds(row, col))
                return _weather[row, col];
            return new Weather();
        }

        private WaterTable _waterTable;
        public WaterTable WaterTable
        {
            get { return _waterTable; }
            set { _waterTable = value; }
        }

        public List<Tile> GetAdjacent(Tile tile)
        {
            List<Tile> adjacent = new List<Tile>();
            Vector2 temp;
            int row = tile.row, col = tile.col;
            foreach (Direction d in DirectionExtension.Values())
            {
                row = tile.row;
                col = tile.col;
                temp = DirectionExtension.ForwardCoordinates(d, row, col);
                adjacent.Add(GetGroundTile((int)temp.X, (int)temp.Y));
            }
            return adjacent;
        }


        #endregion
        public bool InBounds(int row, int column)
        {
            if (row < 0 || row >= _rows)
                return false;
            if (column < 0 || column >= _columns)
                return false;
            return true;
        }
    }
}
