using System;
using System.Collections.Generic;
using System.Globalization;
using Syroot.NintenTools.Byaml;

namespace Syroot.NintenTools.Maths
{
    /// <summary>
    /// Represents a three-dimensional vector which uses float values.
    /// </summary>
    public struct Vector3F : IEquatable<Vector3F>, IEquatableByRef<Vector3F>, INearlyEquatable<Vector3F>,
        INearlyEquatableByRef<Vector3F>, IComparable, IByamlSerializable
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        /// <summary>
        /// A <see cref="Vector3F"/> with the X, Y and Z components being 0f.
        /// </summary>
        public static readonly Vector3F Zero = new Vector3F();

        /// <summary>
        /// A <see cref="Vector3F"/> with the X, Y and Z components being 1f.
        /// </summary>
        public static readonly Vector3F One = new Vector3F(1f, 1f, 1f);

        /// <summary>
        /// Gets the amount of value types required to represent this structure.
        /// </summary>
        internal const int ValueCount = 3;

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

        /// <summary>
        /// The Z float component.
        /// </summary>
        public float Z;

        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3F"/> struct with the given values for the X, Y and Z
        /// components.
        /// </summary>
        /// <param name="x">The value of the X component.</param>
        /// <param name="y">The value of the Y component.</param>
        /// <param name="z">The value of the Z component.</param>
        public Vector3F(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3"/> struct from the given BYAML dictionary node.
        /// </summary>
        /// <param name="node">The BYAML dictionary node to load the data from.</param>
        public Vector3F(IDictionary<string, dynamic> node)
        {
            X = 0;
            Y = 0;
            Z = 0;
            DeserializeByaml(node);
        }

        // ---- OPERATORS ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Returns the given <see cref="Vector3F"/>.
        /// </summary>
        /// <param name="a">The <see cref="Vector3F"/>.</param>
        /// <returns>The result.</returns>
        public static Vector3F operator +(Vector3F a)
        {
            return a;
        }

        /// <summary>
        /// Adds the first <see cref="Vector3F"/> to the second one.
        /// </summary>
        /// <param name="a">The first <see cref="Vector3F"/>.</param>
        /// <param name="b">The second <see cref="Vector3F"/>.</param>
        /// <returns>The addition result.</returns>
        public static Vector3F operator +(Vector3F a, Vector3F b)
        {
            return new Vector3F(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        /// <summary>
        /// Negates the given <see cref="Vector3F"/>.
        /// </summary>
        /// <param name="a">The <see cref="Vector3F"/> to negate.</param>
        /// <returns>The negated result.</returns>
        public static Vector3F operator -(Vector3F a)
        {
            return new Vector3F(-a.X, -a.Y, -a.Z);
        }

        /// <summary>
        /// Subtracts the first <see cref="Vector3F"/> from the second one.
        /// </summary>
        /// <param name="a">The first <see cref="Vector3F"/>.</param>
        /// <param name="b">The second <see cref="Vector3F"/>.</param>
        /// <returns>The subtraction result.</returns>
        public static Vector3F operator -(Vector3F a, Vector3F b)
        {
            return new Vector3F(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        /// <summary>
        /// Multiplicates the given <see cref="Vector3F"/> by the scalar.
        /// </summary>
        /// <param name="a">The <see cref="Vector3F"/>.</param>
        /// <param name="s">The scalar.</param>
        /// <returns>The multiplication result.</returns>
        public static Vector3F operator *(Vector3F a, float s)
        {
            return new Vector3F(a.X * s, a.Y * s, a.Z * s);
        }

        /// <summary>
        /// Multiplicates the first <see cref="Vector3F"/> by the second one.
        /// </summary>
        /// <param name="a">The first <see cref="Vector3F"/>.</param>
        /// <param name="b">The second <see cref="Vector3F"/>.</param>
        /// <returns>The multiplication result.</returns>
        public static Vector3F operator *(Vector3F a, Vector3F b)
        {
            return new Vector3F(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        /// <summary>
        /// Divides the given <see cref="Vector3F"/> through the scalar.
        /// </summary>
        /// <param name="a">The <see cref="Vector3F"/>.</param>
        /// <param name="s">The scalar.</param>
        /// <returns>The division result.</returns>
        public static Vector3F operator /(Vector3F a, float s)
        {
            return new Vector3F(a.X / s, a.Y / s, a.Z / s);
        }

        /// <summary>
        /// Divides the first <see cref="Vector3F"/> through the second one.
        /// </summary>
        /// <param name="a">The first <see cref="Vector3F"/>.</param>
        /// <param name="b">The second <see cref="Vector3F"/>.</param>
        /// <returns>The division result.</returns>
        public static Vector3F operator /(Vector3F a, Vector3F b)
        {
            return new Vector3F(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }

        /// <summary>
        /// Gets a value indicating whether the components of the first specified <see cref="Vector3F"/> are the same as
        /// the components of the second specified <see cref="Vector3F"/>.
        /// </summary>
        /// <param name="a">The first <see cref="Vector3F"/> to compare.</param>
        /// <param name="b">The second <see cref="Vector3F"/> to compare.</param>
        /// <returns>true, if the components of both <see cref="Vector3F"/> are the same.</returns>
        public static bool operator ==(Vector3F a, Vector3F b)
        {
            return a.Equals(ref b);
        }

        /// <summary>
        /// Gets a value indicating whether the components of the first specified <see cref="Vector3F"/> are not the
        /// same as the components of the second specified <see cref="Vector3F"/>.
        /// </summary>
        /// <param name="a">The first <see cref="Vector3F"/> to compare.</param>
        /// <param name="b">The second <see cref="Vector3F"/> to compare.</param>
        /// <returns>true, if the components of both <see cref="Vector3F"/> are not the same.</returns>
        public static bool operator !=(Vector3F a, Vector3F b)
        {
            return !a.Equals(ref b);
        }

        /// <summary>
        /// Gets a value indicating whether the components of the first specified <see cref="Vector3F"/> are bigger than
        /// the components of the second specified <see cref="Vector3F"/>.
        /// </summary>
        /// <param name="a">The first <see cref="Vector3F"/> to compare.</param>
        /// <param name="b">The second <see cref="Vector3F"/> to compare.</param>
        /// <returns>true, if the components of the first <see cref="Vector3F"/> are bigger than the second.</returns>
        public static bool operator >(Vector3F a, Vector3F b)
        {
            return a.X > b.X && a.Y > b.X && a.Z > b.Z;
        }

        /// <summary>
        /// Gets a value indicating whether the components of the first specified <see cref="Vector3F"/> are smaller
        /// than the components of the second specified <see cref="Vector3F"/>.
        /// </summary>
        /// <param name="a">The first <see cref="Vector3F"/> to compare.</param>
        /// <param name="b">The second <see cref="Vector3F"/> to compare.</param>
        /// <returns>true, if the components of the first <see cref="Vector3F"/> are smaller than the second.</returns>
        public static bool operator <(Vector3F a, Vector3F b)
        {
            return a.X < b.X && a.Y < b.X && a.Z < b.Z;
        }

        /// <summary>
        /// Implicit conversion from <see cref="Vector3"/>.
        /// </summary>
        /// <param name="size">The <see cref="Vector3"/> to convert from.</param>
        /// <returns>The retrieved <see cref="Vector3F"/>.</returns>
        public static implicit operator Vector3F(Vector3 size)
        {
            return new Vector3F(size.X, size.Y, size.Z);
        }

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <summary>
        /// Gets a value indicating whether the components of this <see cref="Vector3F"/> are the same as the components
        /// of the second specified <see cref="Vector3F"/>.
        /// </summary>
        /// <param name="obj">The object to compare, if it is a <see cref="Vector3F"/>.</param>
        /// <returns>true, if the components of both <see cref="Vector3F"/> are the same.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Vector3F))
            {
                return false;
            }
            Vector3F vector3F = (Vector3F)obj;
            return Equals(ref vector3F);
        }

        /// <summary>
        /// Gets a hash code as an indication for object equality.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 263;
                hash *= 383 + X.GetHashCode();
                hash *= 383 + Y.GetHashCode();
                hash *= 383 + Z.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// Gets a string describing the components of this <see cref="Vector3F"/>.
        /// </summary>
        /// <returns>A string describing this <see cref="Vector3F"/>.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{{X={0},Y={1},Z={2}}}", X, Y, Z);
        }

        /// <summary>
        /// Indicates whether the current <see cref="Vector3F"/> is equal to another <see cref="Vector3F"/>.
        /// </summary>
        /// <param name="other">A <see cref="Vector3F"/> to compare with this <see cref="Vector3F"/>.</param>
        /// <returns>true if the current <see cref="Vector3F"/> is equal to the other parameter; otherwise, false.
        /// </returns>
        public bool Equals(Vector3F other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Indicates whether the current <see cref="Vector3F"/> is equal to another <see cref="Vector3F"/>.
        /// Structures are passed by reference to avoid stack structure copying.
        /// </summary>
        /// <param name="other">A <see cref="Vector3F"/> to compare with this structure.</param>
        /// <returns><c>true</c> if the current <see cref="Vector3F"/> is equal to the other parameter; otherwise,
        /// <c>false</c>.</returns>
        public bool Equals(ref Vector3F other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        /// <summary>
        /// Indicates whether the current <see cref="Vector3F"/> is nearly equal to another <see cref="Vector3F"/>.
        /// </summary>
        /// <param name="other">A <see cref="Vector3F"/> to compare with this <see cref="Vector3F"/>.</param>
        /// <returns>true if the current <see cref="Vector3F"/> is nearly equal to the other parameter; otherwise,
        /// false.</returns>
        public bool NearlyEquals(Vector3F other)
        {
            return NearlyEquals(ref other);
        }

        /// <summary>
        /// Indicates whether the current <see cref="Vector3F"/> is nearly equal to another <see cref="Vector3F"/>.
        /// Structures are passed by reference to avoid stack structure copying.
        /// </summary>
        /// <param name="other">A <see cref="Vector3F"/> to compare with this <see cref="Vector3F"/>.</param>
        /// <returns><c>true</c> if the current <see cref="Vector3F"/> is nearly equal to the other parameter;
        /// otherwise, <c>false</c>.</returns>
        public bool NearlyEquals(ref Vector3F other)
        {
            return X.NearlyEquals(other.X) && Y.NearlyEquals(other.Y) && Z.NearlyEquals(other.Z);
        }

        /// <summary>
        /// Compares the current instance with another <see cref="Vector3F"/> and returns an integer that indicates
        /// whether the current instance precedes, follows, or occurs in the same position in the sort order as the
        /// other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>A value that indicates the relative order of the <see cref="Vector3F"/> instances being compared.
        /// </returns>
        public int CompareTo(object obj)
        {
            Vector3F? other = obj as Vector3F?;
            if (other.HasValue)
            {
                return this > other ? 1 : this < other ? -1 : 0;
            }
            else
            {
                throw new ArgumentException($"{nameof(obj)} must be a {nameof(Vector3F)}.", nameof(obj));
            }
        }

        /// <summary>
        /// Reads the data from the given dynamic BYAML node into the instance.
        /// </summary>
        /// <param name="node">The dynamic BYAML node to deserialize.</param>
        public dynamic DeserializeByaml(dynamic node)
        {
            X = node["X"];
            Y = node["Y"];
            Z = node["Z"];
            return this;
        }

        /// <summary>
        /// Creates a dynamic BYAML node from the instance's data.
        /// </summary>
        /// <returns>The dynamic BYAML node.</returns>
        public dynamic SerializeByaml()
        {
            return new Dictionary<string, dynamic>
            {
                ["X"] = X,
                ["Y"] = Y,
                ["Z"] = Z
            };
        }
    }
}
