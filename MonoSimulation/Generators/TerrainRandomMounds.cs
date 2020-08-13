using MonoSimulator.Generators;
using MonoSimulator.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoSimulation.Generators
{
    class TerrainRandomMounds : ITerrainGenerator
    {
        public void GenerateHeight(Tile[,] tiles)
        {


            Random random = new Random();
            List<Seed> seeds = new List<Seed>();

            for (int i = 0; i < random.Next(5,20); i++)
            {
                seeds.Add(new Seed()
                {
                    row = random.Next(0, tiles.GetLength(0)),
                    col = random.Next(0, tiles.GetLength(1)),
                    strength = random.Next(0, 10),
                    deviation = (float)random.NextDouble()
                });
            };

            foreach(Seed seed in seeds)
            {
                for (int row = 0; row < tiles.GetLength(0); row++)
                {
                    for (int col = 0; col < tiles.GetLength(1); col++)
                    {
                        tiles[row, col].elevation += (float)(Math.Sqrt(Distance(col, row, seed.col, seed.row)) * (seed.strength * seed.deviation));
                        //tiles[row, col].elevation.elevation += (float)( * random.NextDouble() * random.Next(-1, 1));
                    }
                }
            }
   
        
        
        
        }

        private static float Distance(float x1, float y1, float x2, float y2)
        {
            float x = x2 - x1;
            x *= x;
            float y = y2 - y1;
            y *= y;

            return (float)Math.Sqrt(x + y);
        }

    }
}
