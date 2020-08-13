using Microsoft.Xna.Framework;
using MonoSimulation.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MonoSimulation.Enums
{
    public enum Direction
    {
        [AngleDeg(0f), OddOffset(1,0), EvenOffset(1,0)] North,
        [AngleDeg(120f), OddOffset(0,-1), EvenOffset(1, -1)] Northeast,
        [AngleDeg(240f), OddOffset(-1,-1), EvenOffset(0,-1)] Southeast,

        [AngleDeg(180f), OddOffset(-1,0), EvenOffset(-1,0)] South,
        [AngleDeg(300f), OddOffset(-1,1), EvenOffset(0,1)] Southwest,
        [AngleDeg(60f), OddOffset(0,1), EvenOffset(1, 1)] Northwest
    }

    #region Custom Attributes
    public class AngleDeg : Attribute
    {
        public float AngleAttribute;
        public AngleDeg(float angle) { AngleAttribute = angle; }
    }

    public class EvenOffset : Attribute
    {
        public Vector2 pos;
        public EvenOffset(int x, int z) { pos = new Vector2(x, z); }
    }

    public class OddOffset : Attribute
    {
        public Vector2 pos;
        public OddOffset(int x, int z) { pos = new Vector2(x, z); }
    }
    #endregion

    public static class DirectionExtension
    {

        private static Direction[] directions = Enum.GetValues(typeof(Direction)).Cast<Direction>().ToArray();

        public static Direction[] Values ()
        {
            return directions;
        }

        private static int DirectionIndex(Direction dir)
        {
            for (int i = 0; i < directions.Length; i++)
                if (directions[i] == dir)
                    return i;
            throw new InvalidDirectionException();
        }

        public static Direction RotateCW (Direction input)
        {
            return directions[(DirectionIndex(input) + 1) % directions.Length];
            throw new InvalidDirectionException();
        }

        public static Direction RotateCCW(Direction input)
        {
            int i = DirectionIndex(input);
            if (i-1 < 0)
                return directions[(i-1) + directions.Length];
            return directions[i - 1];
            throw new InvalidDirectionException();
        }

        public static Direction Opposite(Direction input)
        {
            try 
            {
                return directions[(DirectionIndex(input) + (directions.Length / 2)) % directions.Length]; 
            }
            catch (Exception e)
            {
                Game1.messages.AddLogMessage(e.Message);
                throw new InvalidDirectionException();
            }
        }

        public static Vector2 ForwardCoordinates(Direction dir, int row, int col)
        {
            Vector2 coordinates = new Vector2(row, col);
            if (col % 2 == 0)
                coordinates += GetEvenOffset(dir);
            else
                coordinates += GetOddOffset(dir);
            return coordinates;
        }

        public static Vector2 ReverseCoordinates(Direction dir, int row, int col)
        {
            return ForwardCoordinates(Opposite(dir), row, col);
        }

        #region Attribute Getters 
        public static Vector2 GetOddOffset(Direction direction)
        {
            return GetAttributeOfType<OddOffset>(direction).pos;
        }

        public static Vector2 GetEvenOffset(Direction direction)
        {
            return GetAttributeOfType<EvenOffset>(direction).pos;
        }

        public static float GetAngleDegrees(Direction direction)
        {
            return GetAttributeOfType<AngleDeg>(direction).AngleAttribute;
        }

        /// <summary>
        /// Gets an attribute on an enum field value
        /// </summary>
        /// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
        /// <param name="enumVal">The enum value</param>
        /// <returns>The attribute of type T that exists on the enum value</returns>
        /// <example><![CDATA[string desc = myEnumVariable.GetAttributeOfType<DescriptionAttribute>().Description;]]></example>
        private static T GetAttributeOfType<T>(this Direction enumVal) where T : System.Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return (attributes.Length > 0) ? (T)attributes[0] : null;
        }
        #endregion

    }
}
