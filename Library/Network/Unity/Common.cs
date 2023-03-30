namespace Pyro.Network.Unity {
    /// <summary>
    ///     Common to all Shared resources in a Unity3d app.
    /// </summary>
    public class Common
        : FlowBehaviour {
        /// <summary>
        ///     The fullname of the object in the runtime scene hierarchy. Names are separated by '/'.
        /// </summary>
        public string FullName;

        /// <summary>
        ///     The unique network id of this object.
        /// </summary>
        public int NetworkId;

        /// <summary>
        ///     How often to update if there are detected changes in range.
        /// </summary>
        public int UpdateMillis = 100;

        /// <summary>
        ///     Which clients are watching this object's changes.
        /// </summary>
        public ResourceGroup ResourceGroup;
    }
}