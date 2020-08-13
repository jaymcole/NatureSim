using MonoSimulator.Map;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoSimulator.Generators
{
    public interface ITerrainGenerator
    {
        void GenerateHeight(Tile[,] tiles);
    }
}
