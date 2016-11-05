using System;
using System.Globalization;

namespace Syroot.NintenTools.Maths
{
    /// <summary>
    /// Represents a three-dimensional vector which uses integer values.
    /// </summary>
    public struct Vector3 : IEquatable<Vector3>, IEquatableByRef<Vector3>, IComparable
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        /// <summary>
        /// A <see cref="Vector3"/> with the X, Y and Z components being 0.
        /// </summary>
        public static readonly Vector3 Zero = new Vector3();

        /// <summary>
        /// A <see cref="Vector3"/> with the X, Y and Z components being 1.
        /// </summary>
        public static readonly Vector3 One = new Vector3(1, 1, 1);

        /// <summary>
        /// Gets the amount of value types required to represent this structure.
        /// </summary>
        internal const int ValueCount = 3;

        /// <summary>
        /// Gets the size of this structure.
        /// </summary>
        internal const int SizeInBytes = ValueCount * sizeof(int);

        // ---- MEMBERS ------------------------------------------------------------------------------------------------

        /// <summary>
        /// The X integer component.
        /// </summary>
        public int X;

        /// <summary>
        /// The Y integer component.
        /// </summary>
        public int Y;

        /// <summary>
        /// The Z integer component.
        /// </summary>
        public int Z;

        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3"/> structure with the given values for the X, Y and Z
        /// components.
        /// </summary>
        /// <param name="x">The value of the X component.</param>
        /// <param name="y">The value of the Y component.</param>
        /// <param name="z">The value of the Z component.</param>
        public Vector3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        // ---- OPERATORS ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Returns the given <see cref="Vector3"/>.
        /// </summary>
        /// <param name="a">The <see cref="Vector3"/>.</param>
        /// <returns>The result.</returns>
        public static Vector3 operator +(Vector3 a)
        {
            return a;
        }

        /// <summary>
        /// Adds the first <see cref="Vector3"/> to the second one.
        /// </summary>
        /// <param name="a">The first <see cref="Vector3"/>.</param>
        /// <param name="b">The second <see cref="Vector3"/>.</param>
        /// <returns>The addition result.</returns>
        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        /// <summary>
        /// Negates the given <see cref="Vector3"/>.
        /// </summary>
        /// <param name="a">The <see cref="Vector3"/> to negate.</param>
        /// <returns>The negated result.</returns>
        public static Vector3 operator -(Vector3 a)
        {
            return new Vector3(-a.X, -a.Y, -a.Z);
        }

        /// <summary>
        /// Subtracts the first <see cref="Vector3"/> from the second one.
        /// </summary>
        /// <param name="a">The first <see cref="Vector3"/>.</param>
        /// <param name="b">The second <see cref="Vector3"/>.</param>
        /// <returns>The subtraction result.</returns>
        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        /// <summary>
        /// Multiplicates the given <see cref="Vector3"/> by the scalar.
        /// </summary>
        /// <param name="a">The <see cref="Vector3"/>.</param>
        /// <param name="s">The scalar.</param>
        /// <returns>The multiplication result.</returns>
        public static Vector3 operator *(Vector3 a, float s)
        {
            return new Vector3((int)(a.X * s), (int)(a.Y * s), (int)(a.Z * s));
        }

        /// <summary>
        /// Multiplicates the first <see cref="Vector3"/> by the second one.
        /// </summary>
        /// <param name="a">The first <see cref="Vector3"/>.</param>
        /// <param name="b">The second <see cref="Vector3"/>.</param>
        /// <returns>The multiplication result.</returns>
        public static Vector3 operator *(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        /// <summary>
        /// Divides the given <see cref="Vector3"/> through the scalar.
        /// </summary>
        /// <param name="a">The <see cref="Vector3"/>.</param>
        /// <param name="s">The scalar.</param>
        /// <returns>The division result.</returns>
        public static Vector3 operator /(Vector3 a, float s)
        {
            return new Vector3((int)(a.X / s), (int)(a.Y / s), (int)(a.Z / s));
        }

        /// <summary>
        /// Divides the first <see cref="Vector3"/> through the second one.
        /// </summary>
        /// <param name="a">The first <see cref="Vector3"/>.</param>
        /// <param name="b">The second <see cref="Vector3"/>.</param>
        /// <returns>The division result.</returns>
        public static Vector3 operator /(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }

        /// <summary>
        /// Gets a value indicating whether the components of the first specified <see cref="Vector3"/> are the same as
        /// the components of the second specified <see cref="Vector3"/>.
        /// </summary>
        /// <param name="a">The first <see cref="Vector3"/> to compare.</param>
        /// <param name="b">The second <see cref="Vector3"/> to compare.</param>
        /// <returns>true, if the components of both <see cref="Vector3"/> are the same.</returns>
        public static bool operator ==(Vector3 a, Vector3 b)
        {
            return a.Equals(ref b);
        }

        /// <summary>
        /// Gets a value indicating whether the components of the first specified <see cref="Vector3"/> are not the same
        /// as the components of the second specified <see cref="Vector3"/>.
        /// </summary>
        /// <param name="a">The first <see cref="Vector3"/> to compare.</param>
        /// <param name="b">The second <see cref="Vector3"/> to compare.</param>
        /// <returns>true, if the components of both <see cref="Vector3"/> are not the same.</returns>
        public static bool operator !=(Vector3 a, Vector3 b)
        {
            return !a.Equals(ref b);
        }

        /// <summary>
        /// Gets a value indicating whether the components of the first specified <see cref="Vector3"/> are bigger than
        /// the components of the second specified <see cref="Vector3"/>.
        /// </summary>
        /// <param name="a">The first <see cref="Vector3"/> to compare.</param>
        /// <param name="b">The second <see cref="Vector3"/> to compare.</param>
        /// <returns>true, if the components of the first <see cref="Vector3"/> are bigger than the second.</returns>
        public static bool operator >(Vector3 a, Vector3 b)
        {
            return a.X > b.X && a.Y > b.X && a.Z > b.Z;
        }

        /// <summary>
        /// Gets a value indicating whether the components of the first specified <see cref="Vector3"/> are smaller
        /// than the components of the second specified <see cref="Vector3"/>.
        /// </summary>
        /// <param name="a">The first <see cref="Vector3"/> to compare.</param>
        /// <param name="b">The second <see cref="Vector3"/> to compare.</param>
        /// <returns>true, if the components of the first <see cref="Vector3"/> are smaller than the second.</returns>
        public static bool operator <(Vector3 a, Vector3 b)
        {
            return a.X < b.X && a.Y < b.X && a.Z < b.Z;
        }

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <summary>
        /// Gets a value indicating whether the components of this <see cref="Vector3"/> are the same as the components
        /// of the second specified <see cref="Vector3"/>.
        /// </summary>
        /// <param name="obj">The object to compare, if it is a <see cref="Vector3"/>.</param>
        /// <returns>true, if the components of both <see cref="Vector3"/> are the same.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Vector3))
            {
                return false;
            }
            Vector3 vector3 = (Vector3)obj;
            return Equals(ref vector3);
        }

        /// <summary>
        /// Gets a hash code as an indication for object equality.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 131;
                hash *= 521 + X;
                hash *= 521 + Y;
                hash *= 521 + Z;
                return hash;
            }
        }

        /// <summary>
        /// Gets a string describing the components of this <see cref="Vector3"/>.
        /// </summary>
        /// <returns>A string describing this <see cref="Vector3"/>.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{{X={0},Y={1},Z={2}}}", X, Y, Z);
        }

        /// <summary>
        /// Indicates whether the current <see cref="Vector3"/> is equal to another <see cref="Vector3"/>.
        /// </summary>
        /// <param name="other">A <see cref="Vector3"/> to compare with this <see cref="Vector3"/>.</param>
        /// <returns>true if the current <see cref="Vector3"/> is equal to the other parameter; otherwise, false.
        /// </returns>
        public bool Equals(Vector3 other)
        {
            return Equals(ref other);
        }
        
        /// <summary>
        /// Indicates whether the current <see cref="Vector3"/> is equal to another <see cref="Vector3"/>.
        /// Structures are passed by reference to avoid stack structure copying.
        /// </summary>
        /// <param name="other">A <see cref="Vector3"/> to compare with this structure.</param>
        /// <returns><c>true</c> if the current <see cref="Vector3"/> is equal to the other parameter; otherwise,
        /// <c>false</c>.</returns>
        public bool Equals(ref Vector3 other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        /// <summary>
        /// Compares the current instance with another <see cref="Vector3"/> and returns an integer that indicates
        /// whether the current instance precedes, follows, or occurs in the same position in the sort order as the
        /// other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>A value that indicates the relative order of the <see cref="Vector3"/> instances being compared.
        /// </returns>
        public int CompareTo(object obj)
        {
            Vector3? other = obj as Vector3?;
            if (other.HasValue)
            {
                return this > other ? 1 : this < other ? -1 : 0;
            }
            else
            {
                throw new ArgumentException($"{nameof(obj)} must be a {nameof(Vector3)}.", nameof(obj));
            }
        }
    }
}
