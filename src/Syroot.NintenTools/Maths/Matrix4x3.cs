using System;
using System.Globalization;

namespace Syroot.NintenTools.Maths
{
    /// <summary>
    /// Represents a matrix with 3 columns and 4 rows in row-major notation.
    /// </summary>
    public struct Matrix4x3 : IEquatable<Matrix4x3>, IEquatableByRef<Matrix4x3>, INearlyEquatable<Matrix4x3>,
        INearlyEquatableByRef<Matrix4x3>
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        /// <summary>
        /// A <see cref="Matrix4x3"/> with all components being 0f.
        /// </summary>
        public static readonly Matrix4x3 Zero = new Matrix4x3();

        /// <summary>
        /// Gets the amount of value types required to represent this structure.
        /// </summary>
        internal const int ValueCount = 12;

        /// <summary>
        /// Gets the size of this structure.
        /// </summary>
        internal const int SizeInBytes = ValueCount * sizeof(float);

        // ---- MEMBERS ------------------------------------------------------------------------------------------------

        /// <summary>
        /// The value in the first row and the first column.
        /// </summary>
        public float M11;

        /// <summary>
        /// The value in the first row and the second column.
        /// </summary>
        public float M12;

        /// <summary>
        /// The value in the first row and the third column.
        /// </summary>
        public float M13;
        
        /// <summary>
        /// The value in the second row and the first column.
        /// </summary>
        public float M21;

        /// <summary>
        /// The value in the second row and the second column.
        /// </summary>
        public float M22;

        /// <summary>
        /// The value in the second row and the third column.
        /// </summary>
        public float M23;
        
        /// <summary>
        /// The value in the third row and the first column.
        /// </summary>
        public float M31;

        /// <summary>
        /// The value in the third row and the second column.
        /// </summary>
        public float M32;

        /// <summary>
        /// The value in the third row and the third column.
        /// </summary>
        public float M33;
        
        /// <summary>
        /// The value in the fourth row and the first column.
        /// </summary>
        public float M41;

        /// <summary>
        /// The value in the fourth row and the second column.
        /// </summary>
        public float M42;

        /// <summary>
        /// The value in the fourth row and the third column.
        /// </summary>
        public float M43;
        
        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix4x3"/> struct with the given values.
        /// </summary>
        /// <param name="m11">The value in the first row and the first column.</param>
        /// <param name="m12">The value in the first row and the second column.</param>
        /// <param name="m13">The value in the first row and the third column.</param>
        /// <param name="m21">The value in the second row and the first column.</param>
        /// <param name="m22">The value in the second row and the second column.</param>
        /// <param name="m23">The value in the second row and the third column.</param>
        /// <param name="m31">The value in the third row and the first column.</param>
        /// <param name="m32">The value in the third row and the second column.</param>
        /// <param name="m33">The value in the third row and the third column.</param>
        /// <param name="m41">The value in the fourth row and the first column.</param>
        /// <param name="m42">The value in the fourth row and the second column.</param>
        /// <param name="m43">The value in the fourth row and the third column.</param>
        public Matrix4x3(float m11, float m12, float m13, float m21, float m22, float m23,
            float m31, float m32, float m33, float m41, float m42, float m43)
        {
            M11 = m11; M12 = m12; M13 = m13;
            M21 = m21; M22 = m22; M23 = m23;
            M31 = m31; M32 = m32; M33 = m33;
            M41 = m41; M42 = m42; M43 = m43;
        }

        // ---- OPERATORS ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets a value indicating whether the components of Matrix4 first specified <see cref="Matrix4x3"/> are the
        /// same as the components of the second specified <see cref="Matrix2x3"/>.
        /// </summary>
        /// <param name="a">The first <see cref="Matrix4x3"/> to compare.</param>
        /// <param name="b">The second <see cref="Matrix4x3"/> to compare.</param>
        /// <returns>true, if the components of both <see cref="Matrix4x3"/> are the same.</returns>
        public static bool operator ==(Matrix4x3 a, Matrix4x3 b)
        {
            return a.Equals(ref b);
        }

        /// <summary>
        /// Gets a value indicating whether the components of the first specified <see cref="Matrix4x3"/> are not the
        /// same as the components of the second specified<see cref="Matrix4x3"/>.
        /// </summary>
        /// <param name="a">The first <see cref="Matrix4x3"/> to compare.</param>
        /// <param name="b">The second <see cref="Matrix4x3"/> to compare.</param>
        /// <returns>true, if the components of both <see cref="Matrix4x3"/> are not the same.</returns>
        public static bool operator !=(Matrix4x3 a, Matrix4x3 b)
        {
            return !a.Equals(ref b);
        }
        
        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------
        
        /// <summary>
        /// Gets a value indicating whether the components of this <see cref="Matrix4x3"/> are the same as the
        /// components of the second specified <see cref="Matrix4x3"/>.
        /// </summary>
        /// <param name="obj">The object to compare, if it is a <see cref="Matrix4x3"/>.</param>
        /// <returns>true, if the components of both <see cref="Matrix4x3"/> are the same.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Matrix4x3))
            {
                return false;
            }
            Matrix4x3 matrix4x3 = (Matrix4x3)obj;
            return Equals(ref matrix4x3);
        }

        /// <summary>
        /// Gets a hash code as an indication for object equality.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 409;
                hash *= 359 + M11.GetHashCode();
                hash *= 359 + M12.GetHashCode();
                hash *= 359 + M13.GetHashCode();
                hash *= 359 + M21.GetHashCode();
                hash *= 359 + M22.GetHashCode();
                hash *= 359 + M23.GetHashCode();
                hash *= 359 + M31.GetHashCode();
                hash *= 359 + M32.GetHashCode();
                hash *= 359 + M33.GetHashCode();
                hash *= 359 + M41.GetHashCode();
                hash *= 359 + M42.GetHashCode();
                hash *= 359 + M43.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// Gets a string describing the components of this <see cref="Matrix4x3"/>.
        /// </summary>
        /// <returns>A string describing this <see cref="Matrix4x3"/>.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "{{[M11={0},M12={1},M13={2}][M21={4},M22={5},M23={6}]"
                + "[M31={8},M32={9},M33={10}][M41={12},M42={13},M43={14}]}}",
                M11, M12, M13, M21, M22, M23, M31, M32, M33, M41, M42, M43);
        }

        /// <summary>
        /// Indicates whether the current <see cref="Matrix4x3"/> is equal to another <see cref="Matrix4x3"/>.
        /// </summary>
        /// <param name="other">A <see cref="Matrix4x3"/> to compare with this <see cref="Matrix4x3"/>.</param>
        /// <returns>true if the current <see cref="Matrix4x3"/> is equal to the other parameter; otherwise, false.
        /// </returns>
        public bool Equals(Matrix4x3 other)
        {
            return Equals(ref other);
        }
        
        /// <summary>
        /// Indicates whether the current <see cref="Matrix4x3"/> is equal to another <see cref="Matrix4x3"/>.
        /// Structures are passed by reference to avoid stack structure copying.
        /// </summary>
        /// <param name="other">A <see cref="Matrix4x3"/> to compare with this structure.</param>
        /// <returns><c>true</c> if the current <see cref="Matrix4x3"/> is equal to the other parameter; otherwise,
        /// <c>false</c>.</returns>
        public bool Equals(ref Matrix4x3 other)
        {
            return M11 == other.M11 && M12 == other.M12 && M13 == other.M13
                && M21 == other.M21 && M22 == other.M22 && M23 == other.M23
                && M31 == other.M31 && M32 == other.M32 && M33 == other.M33
                && M41 == other.M41 && M42 == other.M42 && M43 == other.M43;
        }

        /// <summary>
        /// Indicates whether the current <see cref="Matrix4x3"/> is nearly equal to another <see cref="Matrix4x3"/>.
        /// </summary>
        /// <param name="other">A <see cref="Matrix4x3"/> to compare with this <see cref="Matrix4x3"/>.</param>
        /// <returns>true if the current <see cref="Matrix4x3"/> is nearly equal to the other parameter; otherwise,
        /// false.</returns>
        public bool NearlyEquals(Matrix4x3 other)
        {
            return NearlyEquals(ref other);
        }
        
        /// <summary>
        /// Indicates whether the current <see cref="Matrix4x3"/> is nearly equal to another <see cref="Matrix4x3"/>.
        /// Structures are passed by reference to avoid stack structure copying.
        /// </summary>
        /// <param name="other">A <see cref="Matrix4x3"/> to compare with this <see cref="Matrix4x3"/>.</param>
        /// <returns><c>true</c> if the current <see cref="Matrix4x3"/> is nearly equal to the other parameter;
        /// otherwise, <c>false</c>.</returns>
        public bool NearlyEquals(ref Matrix4x3 other)
        {
            return M11.NearlyEquals(other.M11) && M12.NearlyEquals(other.M12) && M13.NearlyEquals(other.M13)
                && M21.NearlyEquals(other.M21) && M22.NearlyEquals(other.M22) && M23.NearlyEquals(other.M23)
                && M31.NearlyEquals(other.M31) && M32.NearlyEquals(other.M32) && M33.NearlyEquals(other.M33)
                && M41.NearlyEquals(other.M41) && M42.NearlyEquals(other.M42) && M43.NearlyEquals(other.M43);
        }
    }
}
