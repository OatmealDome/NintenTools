﻿namespace Syroot.NintenTools.Maths
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Represents a two-dimensional vector which uses float values.
    /// </summary>
    public struct Vector2F : IEquatable<Vector2F>, IEquatableByRef<Vector2F>, INearlyEquatable<Vector2F>,
        INearlyEquatableByRef<Vector2F>
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        /// <summary>
        /// A <see cref="Vector2F"/> with the X and Y components being 0f.
        /// </summary>
        public static readonly Vector2F Zero = new Vector2F();

        /// <summary>
        /// A <see cref="Vector2F"/> with the X and Y components being 1f.
        /// </summary>
        public static readonly Vector2F One = new Vector2F(1f, 1f);

        /// <summary>
        /// Gets the amount of value types required to represent this structure.
        /// </summary>
        internal const int ValueCount = 2;

        /// <summary>
        /// Gets the size of this structure.
        /// </summary>
        internal const int SizeInBytes = ValueCount * sizeof(float);

        // ---- MEMBERS ------------------------------------------------------------------------------------------------

        /// <summary>
        /// The X float component.
        /// </summary>
        public float X;

        /// <summary>
        /// The Y float component.
        /// </summary>
        public float Y;

        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector2F"/> struct with the given values for the X and Y
        /// components.
        /// </summary>
        /// <param name="x">The value of the X component.</param>
        /// <param name="y">The value of the Y component.</param>
        public Vector2F(float x, float y)
        {
            X = x;
            Y = y;
        }

        // ---- OPERATORS ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Returns the given <see cref="Vector2F"/>.
        /// </summary>
        /// <param name="a">The <see cref="Vector2F"/>.</param>
        /// <returns>The result.</returns>
        public static Vector2F operator +(Vector2F a)
        {
            return a;
        }

        /// <summary>
        /// Adds the first <see cref="Vector2F"/> to the second one.
        /// </summary>
        /// <param name="a">The first <see cref="Vector2F"/>.</param>
        /// <param name="b">The second <see cref="Vector2F"/>.</param>
        /// <returns>The addition result.</returns>
        public static Vector2F operator +(Vector2F a, Vector2F b)
        {
            return new Vector2F(a.X + b.X, a.Y + b.Y);
        }

        /// <summary>
        /// Negates the given <see cref="Vector2F"/>.
        /// </summary>
        /// <param name="a">The <see cref="Vector2F"/> to negate.</param>
        /// <returns>The negated result.</returns>
        public static Vector2F operator -(Vector2F a)
        {
            return new Vector2F(-a.X, -a.Y);
        }

        /// <summary>
        /// Subtracts the first <see cref="Vector2F"/> from the second one.
        /// </summary>
        /// <param name="a">The first <see cref="Vector2F"/>.</param>
        /// <param name="b">The second <see cref="Vector2F"/>.</param>
        /// <returns>The subtraction result.</returns>
        public static Vector2F operator -(Vector2F a, Vector2F b)
        {
            return new Vector2F(a.X - b.X, a.Y - b.Y);
        }

        /// <summary>
        /// Multiplicates the given <see cref="Vector2F"/> by the scalar.
        /// </summary>
        /// <param name="a">The <see cref="Vector2F"/>.</param>
        /// <param name="s">The scalar.</param>
        /// <returns>The multiplication result.</returns>
        public static Vector2F operator *(Vector2F a, float s)
        {
            return new Vector2F(a.X * s, a.Y * s);
        }

        /// <summary>
        /// Multiplicates the first <see cref="Vector2F"/> by the second one.
        /// </summary>
        /// <param name="a">The first <see cref="Vector2F"/>.</param>
        /// <param name="b">The second <see cref="Vector2F"/>.</param>
        /// <returns>The multiplication result.</returns>
        public static Vector2F operator *(Vector2F a, Vector2F b)
        {
            return new Vector2F(a.X * b.X, a.Y * b.Y);
        }

        /// <summary>
        /// Divides the given <see cref="Vector2F"/> through the scalar.
        /// </summary>
        /// <param name="a">The <see cref="Vector2F"/>.</param>
        /// <param name="s">The scalar.</param>
        /// <returns>The division result.</returns>
        public static Vector2F operator /(Vector2F a, float s)
        {
            return new Vector2F(a.X / s, a.Y / s);
        }

        /// <summary>
        /// Divides the first <see cref="Vector2F"/> through the second one.
        /// </summary>
        /// <param name="a">The first <see cref="Vector2F"/>.</param>
        /// <param name="b">The second <see cref="Vector2F"/>.</param>
        /// <returns>The division result.</returns>
        public static Vector2F operator /(Vector2F a, Vector2F b)
        {
            return new Vector2F(a.X / b.X, a.Y / b.Y);
        }

        /// <summary>
        /// Gets a value indicating whether the components of the first specified <see cref="Vector2F"/> are the same as
        /// the components of the second specified <see cref="Vector2F"/>.
        /// </summary>
        /// <param name="a">The first <see cref="Vector2F"/> to compare.</param>
        /// <param name="b">The second <see cref="Vector2F"/> to compare.</param>
        /// <returns>true, if the components of both <see cref="Vector2F"/> are the same.</returns>
        public static bool operator ==(Vector2F a, Vector2F b)
        {
            return a.Equals(ref b);
        }

        /// <summary>
        /// Gets a value indicating whether the components of the first specified <see cref="Vector2F"/> are not the
        /// same as the components of the second specified <see cref="Vector2F"/>.
        /// </summary>
        /// <param name="a">The first <see cref="Vector2F"/> to compare.</param>
        /// <param name="b">The second <see cref="Vector2F"/> to compare.</param>
        /// <returns>true, if the components of both <see cref="Vector2F"/> are not the same.</returns>
        public static bool operator !=(Vector2F a, Vector2F b)
        {
            return !a.Equals(ref b);
        }
        
        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <summary>
        /// Gets a value indicating whether the components of this <see cref="Vector2F"/> are the same as the components
        /// of the second specified <see cref="Vector2F"/>.
        /// </summary>
        /// <param name="obj">The object to compare, if it is a <see cref="Vector2F"/>.</param>
        /// <returns>true, if the components of both <see cref="Vector2F"/> are the same.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Vector2F))
            {
                return false;
            }
            Vector2F vector2F = (Vector2F)obj;
            return Equals(ref vector2F);
        }

        /// <summary>
        /// Gets a hash code as an indication for object equality.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 443;
                hash *= 397 + X.GetHashCode();
                hash *= 397 + Y.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// Gets a string describing the components of this <see cref="Vector2F"/>.
        /// </summary>
        /// <returns>A string describing this <see cref="Vector2F"/>.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{{X={0},Y={1}}}", X, Y);
        }

        /// <summary>
        /// Indicates whether the current <see cref="Vector2F"/> is equal to another <see cref="Vector2F"/>.
        /// </summary>
        /// <param name="other">A <see cref="Vector2F"/> to compare with this <see cref="Vector2F"/>.</param>
        /// <returns>true if the current <see cref="Vector2F"/> is equal to the other parameter; otherwise, false.
        /// </returns>
        public bool Equals(Vector2F other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Indicates whether the current <see cref="Vector2F"/> is equal to another <see cref="Vector2F"/>.
        /// Structures are passed by reference to avoid stack structure copying.
        /// </summary>
        /// <param name="other">A <see cref="Vector2F"/> to compare with this structure.</param>
        /// <returns><c>true</c> if the current <see cref="Vector2F"/> is equal to the other parameter; otherwise,
        /// <c>false</c>.</returns>
        public bool Equals(ref Vector2F other)
        {
            return X == other.X && Y == other.Y;
        }

        /// <summary>
        /// Indicates whether the current <see cref="Vector2F"/> is nearly equal to another <see cref="Vector2F"/>.
        /// </summary>
        /// <param name="other">A <see cref="Vector2F"/> to compare with this <see cref="Vector2F"/>.</param>
        /// <returns>true if the current <see cref="Vector2F"/> is nearly equal to the other parameter; otherwise,
        /// false.</returns>
        public bool NearlyEquals(Vector2F other)
        {
            return NearlyEquals(ref other);
        }

        /// <summary>
        /// Indicates whether the current <see cref="Vector2F"/> is nearly equal to another <see cref="Vector2F"/>.
        /// Structures are passed by reference to avoid stack structure copying.
        /// </summary>
        /// <param name="other">A <see cref="Vector2F"/> to compare with this <see cref="Vector2F"/>.</param>
        /// <returns><c>true</c> if the current <see cref="Vector2F"/> is nearly equal to the other parameter;
        /// otherwise, <c>false</c>.</returns>
        public bool NearlyEquals(ref Vector2F other)
        {
            return X.NearlyEquals(other.X) && Y.NearlyEquals(other.Y);
        }
    }
}
