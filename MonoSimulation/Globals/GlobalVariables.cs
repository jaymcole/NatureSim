using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MonoSimulation.Globals
{
    class GlobalVariables
    {

        public static Color MoistDirtColor = new Color(74, 49, 0,1);

        public static float CloudAltitude = 25f;

        public static float TileRadius = 2f;

        public static int MaxPlantsPerTile = 1;





        public static Random random = new Random();

        public static ContentManager Content;

        public static Effect WaterShader;
        public static Texture2D WaterImage;
        public static Texture2D LandImage;
        public static Texture2D SkyImage;
        public static Texture2D Man;

        private static string[] grass = new string[]
        {
            "Models/Foliage/ForestPack/Grass/Grass"
        };

        private static string[] flowers = new string[]
        {
            "Models/Foliage/ForestPack/Flowers/Flower1.1",
            "Models/Foliage/ForestPack/Flowers/Flower1.2",
            "Models/Foliage/ForestPack/Flowers/Flower1.3",
            "Models/Foliage/ForestPack/Flowers/Flower2.1",
            "Models/Foliage/ForestPack/Flowers/Flower2.2",
            "Models/Foliage/ForestPack/Flowers/Flower2.3",
            "Models/Foliage/ForestPack/Flowers/Flower3.1",
            "Models/Foliage/ForestPack/Flowers/Flower3.2",
            "Models/Foliage/ForestPack/Flowers/Flower3.3",
        };

        private static string[] trees = new string[]
        {
            "Models/Foliage/ForestPack/Trees/Tree1.1",
            "Models/Foliage/ForestPack/Trees/Tree1.2",
            "Models/Foliage/ForestPack/Trees/Tree1.3",
            "Models/Foliage/ForestPack/Trees/Tree2.1",
            "Models/Foliage/ForestPack/Trees/Tree2.2",
            "Models/Foliage/ForestPack/Trees/Tree2.3",
            "Models/Foliage/ForestPack/Trees/Tree3.1",
            "Models/Foliage/ForestPack/Trees/Tree3.2",
            "Models/Foliage/ForestPack/Trees/Tree3.3",
        };

        public static Microsoft.Xna.Framework.Graphics.Model GetTree()
        {
            return GlobalVariables.Content.Load<Microsoft.Xna.Framework.Graphics.Model>(trees[random.Next(0, trees.Length)]);
        }

        public static Microsoft.Xna.Framework.Graphics.Model GetFlower()
        {
            return GlobalVariables.Content.Load<Microsoft.Xna.Framework.Graphics.Model>(flowers[random.Next(0, flowers.Length)]);
        }

        public static void Initialize(ContentManager content)
        {
            Content = content;
            WaterShader = Content.Load<Effect>("Shaders/basic");
            WaterImage = Content.Load<Texture2D>("Images/Water/water_02");
            LandImage = Content.Load<Texture2D>("Images/Land/land_100x100_01");
            SkyImage = Content.Load<Texture2D>("Images/Clouds/clouds_seamless_01");
            Man = Content.Load<Texture2D>("Images/Misc/man_small");
        }
    }
}
