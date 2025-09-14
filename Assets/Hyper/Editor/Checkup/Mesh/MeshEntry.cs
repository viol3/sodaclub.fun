using UnityEditor;
using UnityEngine;

namespace Hyper.Checkup
{
    public class MeshEntry
    {
        public Mesh Mesh { get; set; }
        public bool IsSelected { get; set; }
        public string Path => AssetDatabase.GetAssetPath(Mesh);
        public bool IsEmpty => Mesh == null;
    }
}
