using System;
using System.Collections.Generic;

namespace Pyro.Unity3d
{
    /// <summary>
    /// Generic Unity3d GameObject from Unity3d version 2019.1+
    /// </summary>
    public class GameObject
        : IHasFileId
    {
        public Transform Transform;
        public Guid Guid => _guid;
        public int FileId => _fileId;
        public List<Component> Components = new List<Component>();
        public Dictionary<string, object> Properties = new Dictionary<string, object>();

        private Guid _guid;
        private int _fileId;
    }
}

