namespace Syroot.NintenTools.Byaml
{
    /// <summary>
    /// Represents an instance which can read its data from a dynamic BYAML node and be serialized back into one.
    /// </summary>
    public interface IByamlSerializable
    {
        // ---- METHODS ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Reads the data from the given dynamic BYAML node into the instance.
        /// </summary>
        /// <param name="node">The dynamic BYAML node to deserialize.</param>
        dynamic DeserializeByaml(dynamic node);

        /// <summary>
        /// Creates a dynamic BYAML node from the instance's data.
        /// </summary>
        /// <returns>The dynamic BYAML node.</returns>
        dynamic SerializeByaml();
    }
}
