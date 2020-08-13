using System;
using System.Collections.Generic;
using System.Text;

namespace MonoSimulator.EnvironmentProperties
{
    public class Moisture
    {
        public double value;

        public Moisture()
        {
            value = 0;
        }

        public Moisture(double moistureValue)
        {
            value = moistureValue;
        }

        public static Moisture operator +(Moisture e1, Moisture e2) => new Moisture(e1.value + e2.value);
        public static Moisture operator -(Moisture e1, Moisture e2) => new Moisture(e1.value - e2.value);
    }
}
