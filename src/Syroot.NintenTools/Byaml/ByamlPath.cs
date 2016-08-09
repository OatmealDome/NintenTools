using System.Collections.Generic;
using Syroot.NintenTools.IO;

namespace Syroot.NintenTools.Byaml
{
    /// <summary>
    /// Represents a path which is basically a list of <see cref="ByamlPathPoint"/> instances.
    /// </summary>
    public class ByamlPath : List<ByamlPathPoint>
    {
        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="ByamlPath"/> class, reading the data from the given
        /// <see cref="BinaryDataReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryDataReader"/> to read the data from.</param>
        /// <param name="pointCount">The number of <see cref="ByamlPathPoint"/> instances this path has.</param>
        internal ByamlPath(BinaryDataReader reader, int pointCount)
        {
            for (int i = 0; i < pointCount; i++)
            {
                Add(new ByamlPathPoint(reader));
            }
        }
    }
}
