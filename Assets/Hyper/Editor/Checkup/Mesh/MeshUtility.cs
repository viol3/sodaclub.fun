using System.Collections.Generic;
using UnityEngine;

namespace Hyper.Checkup
{
    public static class MeshUtility
    {
        public static void GetMeshesInScene(ref List<MeshEntry> meshEntries)
        {
            meshEntries.Clear();
            MeshFilter[] allRenderers = GameObject.FindObjectsOfType<MeshFilter>();
            foreach (var renderer in allRenderers)
            {
                Mesh mesh = renderer.sharedMesh;
                if (mesh != null)
                {
                    AddMeshToList(ref meshEntries, mesh);
                }
            }
            SkinnedMeshRenderer[] allSkinnedRenderers = GameObject.FindObjectsOfType<SkinnedMeshRenderer>();
            foreach (var renderer in allSkinnedRenderers)
            {
                Mesh mesh = renderer.sharedMesh;
                if (mesh != null)
                {
                    AddMeshToList(ref meshEntries, mesh);
                }
            }
        }

        private static void AddMeshToList(ref List<MeshEntry> meshEntries, Mesh mesh)
        {
            if(meshEntries.Exists(x => x.Mesh == mesh))
            {
                return;
            }
            meshEntries.Add(new MeshEntry { Mesh = mesh, IsSelected = false });
        }
    }
}
