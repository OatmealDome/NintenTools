namespace Syroot.NintenTools.Byaml
{
    /// <summary>
    /// Represents the type of which a <see cref="ByamlNode"/> can be.
    /// </summary>
    public enum ByamlNodeType : byte
    {
        /// <summary>
        /// The node represents a <see cref="String"/> (internally referenced by index).
        /// </summary>
        StringIndex = 0xA0,

        /// <summary>
        /// The node represents a <see cref="ByamlPath"/> (internally referenced by index).
        /// </summary>
        PathIndex = 0xA1,

        /// <summary>
        /// The node represents an array of child <see cref="ByamlNode"/> instances.
        /// </summary>
        Array = 0xC0,

        /// <summary>
        /// The node represents a dictionary of child <see cref="ByamlNode"/> instances.
        /// </summary>
        Dictionary = 0xC1,

        /// <summary>
        /// The node represents an array of <see cref="System.String"/> instances.
        /// </summary>
        StringArray = 0xC2,

        /// <summary>
        /// The node represents an array of <see cref="ByamlPath"/> instances.
        /// </summary>
        PathArray = 0xC3,

        /// <summary>
        /// The node represents a <see cref="System.Boolean"/>.
        /// </summary>
        Boolean = 0xD0,

        /// <summary>
        /// The node represents an <see cref="System.Int32"/>.
        /// </summary>
        Integer = 0xD1,

        /// <summary>
        /// The node represents a <see cref="System.Single"/>.
        /// </summary>
        Float = 0xD2
    }
}
