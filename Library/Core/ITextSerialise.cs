namespace Pryo
{
    /// <summary>
    /// Can be serialised to/from strings.
    ///
    /// Default is to use the object's ToString() method, and throw on FromText()
    /// </summary>
    public interface ITextSerialise
    {
        /// <summary>
        /// Use ToString by default
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        string ToText(IRegistry reg = null);

        /// <summary>
        /// Throws NotImplemented by default;
        /// </summary>
        /// <returns>true if the conversion succeeded</returns>
        bool FromText(string s, IRegistry reg);

        bool FromText(IStringSlice s, IRegistry reg);
    }
}
