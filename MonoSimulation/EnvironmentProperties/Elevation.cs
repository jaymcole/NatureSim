using System;
using System.Collections.Generic;
using System.Text;

namespace MonoSimulator.EnvironmentProperties
{
    public class Elevation
    {

        public static int SEA_LEVEL = 0;


        public double elevation;

        public Elevation()
        {
            elevation = 0;
        }

        public Elevation(double elev)
        {
            elevation = elev;
        }

        public static Elevation operator +(Elevation e1, Elevation e2) => new Elevation(e1.elevation + e2.elevation);
        public static Elevation operator -(Elevation e1, Elevation e2) => new Elevation(e1.elevation - e2.elevation);



    }
}
