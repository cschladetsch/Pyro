namespace Pyro.Unity3d
{
    /// <summary>
    /// A Component of a GameObject
    /// </summary>
    public class Component 
        : Element
        , IHasFileId
    {
        public int FileId { get; set; }
    }
}