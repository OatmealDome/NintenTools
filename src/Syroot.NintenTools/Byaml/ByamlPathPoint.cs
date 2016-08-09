using Syroot.NintenTools.IO;
using Syroot.NintenTools.Maths;

namespace Syroot.NintenTools.Byaml
{
    /// <summary>
    /// Represents a point in a <see cref="ByamlPath"/>.
    /// </summary>
    public class ByamlPathPoint
    {
        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="ByamlPathPoint"/> class.
        /// </summary>
        public ByamlPathPoint()
        {
            Normal = new Vector3F(0, 1, 0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByamlPathPoint"/> class, reading the data from the given
        /// <see cref="BinaryDataReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryDataReader"/> to read the data from.</param>
        internal ByamlPathPoint(BinaryDataReader reader)
            : this()
        {
            Position = reader.ReadVector3F();
            Normal = reader.ReadVector3F();
            Unknown0x18 = reader.ReadUInt32();
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        public Vector3F Position
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the normal.
        /// </summary>
        public Vector3F Normal
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an unknown value.
        /// </summary>
        public uint Unknown0x18
        {
            get;
            set;
        }
    }
}
