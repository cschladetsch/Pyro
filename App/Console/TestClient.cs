using System.Collections.Generic;

class TestClient {
    private readonly Dictionary<int, object> _objects = new Dictionary<int, object>();

    public void AddRemote(string fullName, int id) {
        if (_objects.ContainsKey(id)) {
            //Error("Duplicate Id");
            return;
        }

        _objects[id] = new object();

        System.Console.WriteLine($"AddRemote: {fullName} {id}");
    }

    // TODO: make UnityEngine.Vector3 and .Quaternion known types.
    //public void UpdateTransform(int id, Vector3 pos, Quaternion rot)
    public void UpdateTransform(int id, float x, float y, float z) {
        if (!_objects.TryGetValue(id, out var obj)) {
            //Error
            return;
        }

        //obj.transform.position = new Vector3(x,y,z);// TODO: make UnityEngine.Vector3 a known type
        System.Console.WriteLine($"Update: {id}: {x} {y} {z}");
    }

    public void Disconnect(int id) {
        if (!_objects.ContainsKey(id)) {
            //Error
            return;
        }

        _objects.Remove(id);
        System.Console.WriteLine($"Disconnect: {id}");
    }
}

