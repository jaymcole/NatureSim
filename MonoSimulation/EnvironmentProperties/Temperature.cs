using System;
using System.Collections.Generic;
using System.Text;

namespace MonoSimulator.EnvironmentProperties
{
    public class Temperature
    {
        public double value;

        public Temperature()
        {
            value = 0;
        }

        public Temperature(double tempValue)
        {
            value = tempValue;
        }

        public static Temperature operator +(Temperature e1, Temperature e2) => new Temperature(e1.value + e2.value);
        public static Temperature operator -(Temperature e1, Temperature e2) => new Temperature(e1.value - e2.value);
    }
}
