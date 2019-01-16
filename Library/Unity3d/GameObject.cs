using System;
using System.Collections.Generic;

namespace Pyro.Unity3d
{
    public interface IHasFileId
    {
        int FileId { get; }
    }

    public class Element
    {
    }

    public class Component 
        : Element
        , IHasFileId
    {
        public int FileId { get; set; }
    }

    public class Transform : Component
    {
        public GameObject GameObject;
        public Transform Parent;
        public List<Transform> Children = new List<Transform>();
        public Vector3 Position;
        public Vector3 LocalScale;
        public Quaternion Rotation;
    }

    public class GameObject
        : IHasFileId
    {
        public Transform Transform;
        public Guid Guid => _guid;
        public int FileId => _fileId;
        public List<Component> Components = new List<Component>();

        private Guid _guid;
        private int _fileId;
    }
}
