using System;
using System.Collections.Generic;
using System.Text;

namespace MonoSimulator.EnvironmentProperties
{
    public class Light
    {
        public double value;

        public Light()
        {
            value = 0;
        }

        public Light(double lightValue)
        {
            value = lightValue;
        }

        public static Light operator +(Light e1, Light e2) => new Light(e1.value + e2.value);
        public static Light operator -(Light e1, Light e2) => new Light(e1.value - e2.value);
    }
}
