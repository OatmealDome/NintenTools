namespace Syroot.NintenTools.Byaml
{
    /// <summary>
    /// Represents the type of which a dynamic BYAML node can be.
    /// </summary>
    internal enum ByamlNodeType : byte
    {
        /// <summary>
        /// The node represents a <see cref="string"/> (internally referenced by index).
        /// </summary>
        StringIndex = 0xA0,

        /// <summary>
        /// The node represents a <see cref="ByamlPath"/> (internally referenced by index).
        /// </summary>
        PathIndex = 0xA1,

        /// <summary>
        /// The node represents an array of dynamic child instances.
        /// </summary>
        Array = 0xC0,

        /// <summary>
        /// The node represents a dictionary of dynamic child instances referenced by a <see cref="string"/> key.
        /// </summary>
        Dictionary = 0xC1,

        /// <summary>
        /// The node represents an array of <see cref="string"/> instances.
        /// </summary>
        StringArray = 0xC2,

        /// <summary>
        /// The node represents an array of <see cref="ByamlPath"/> instances.
        /// </summary>
        PathArray = 0xC3,

        /// <summary>
        /// The node represents a <see cref="bool"/>.
        /// </summary>
        Boolean = 0xD0,

        /// <summary>
        /// The node represents an <see cref="int"/>.
        /// </summary>
        Integer = 0xD1,

        /// <summary>
        /// The node represents a <see cref="float"/>.
        /// </summary>
        Float = 0xD2,

        /// <summary>
        /// The node represents null.
        /// </summary>
        Null = 0xFF
    }
}
