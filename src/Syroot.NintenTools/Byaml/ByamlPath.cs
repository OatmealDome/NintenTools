using System;
using System.Collections.Generic;
using System.Linq;
using Syroot.NintenTools.IO;

namespace Syroot.NintenTools.Byaml
{
    /// <summary>
    /// Represents a path which is basically a list of <see cref="ByamlPathPoint"/> instances.
    /// </summary>
    public class ByamlPath : List<ByamlPathPoint>, IEquatable<ByamlPath>, ICloneable
    {
        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(ByamlPath other)
        {
            return this.SequenceEqual(other);
        }

        public object Clone()
        {
            ByamlPath path = new ByamlPath();
            foreach (ByamlPathPoint pathPoint in this)
            {
                path.Add((ByamlPathPoint)pathPoint.Clone());
            }

            return path;
        }
    }
}
