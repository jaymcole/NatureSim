using Microsoft.Xna.Framework;
using MonoSimulator.EnvironmentProperties;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoSimulator.Map
{
    public class Weather
    {
        public int row, col;
        public GameMap map;
        public Moisture moisture;
        public Temperature temperature;
        public Light light;
        public bool IsRaining;

        public Weather ()
        {
            moisture = new Moisture() { value = Environment.random.Next(0, 15) };
            temperature = new Temperature() { value = Environment.random.Next(0, 150) };
            light = new Light() { value = Environment.random.Next(0, 150) };
        }

        public void Update(GameTime gameTime)
        {
            Weather right = map.GetWeatherTile(row, col + 1);
            moisture.value = right.moisture.value;
            temperature.value = (temperature.value + right.temperature.value) / 2;
            light.value = moisture.value;


            Tile below = map.GetGroundTile(row, col);

            if (moisture.value > 65 && IsRaining)
            {
                if (below != null)
                    below.Water += 5;
                moisture.value -= 5;

            } else if (moisture.value > 90)
            {
                IsRaining = true;
                if (below != null)
                    below.Water += 7;
                moisture.value -= 7;
            } else
            {
                IsRaining = false;
            }

            moisture.value = 100;
        }
    }
}
