using Microsoft.Xna.Framework;
using MonoSimulation.Generators;
using MonoSimulator.Map;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoSimulator.Generators
{
    class TerrainCenterPuddle : ITerrainGenerator
    {

        public void GenerateHeight(Tile[,] tiles)
        {
            int rowTarget = tiles.GetLength(0) / 2;
            int colTarget = tiles.GetLength(1) / 2;

            for (int row = 0; row < tiles.GetLength(0); row++)
            {
                for (int col = 0; col < tiles.GetLength(1); col++)
                {
                    tiles[row, col].elevation = Math.Max(Math.Abs(row - rowTarget), Math.Abs(col - colTarget));      
                }
            }


        }
    }
}
