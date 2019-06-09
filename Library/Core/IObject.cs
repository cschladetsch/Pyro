namespace Pyro
{
    /// <summary>
    /// Common for all objects in the system.
    /// </summary>
    public interface IObject
    {
        /// <summary>
        /// The unique ident of this object in its Registry.
        /// </summary>
        Id Id { get; }

        /// <summary>
        /// The thing that made and registered this object.
        /// </summary>
        IRegistry Registry { get; }

        /// <summary>
        /// The type of this thing.
        /// </summary>
        IClassBase Class { get; }
    }
}
