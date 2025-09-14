using UnityEditor;
using UnityEngine;
using UnityMeshSimplifier;

namespace Hyper.EditorMeshSimplifier
{
    public static class MeshUtility
    {
        public static Mesh GetMeshFromGameObject(this GameObject go)
        {
            Mesh mesh = null;

            MeshFilter meshFilter = go.GetComponent<MeshFilter>();
            SkinnedMeshRenderer skinnedMeshRenderer = go.GetComponent<SkinnedMeshRenderer>();

            if (meshFilter != null)
            {
                mesh = meshFilter.sharedMesh;
            }
            else if (skinnedMeshRenderer != null)
            {
                mesh = skinnedMeshRenderer.sharedMesh;
            }
            else
            {
                foreach (Transform child in go.transform)
                {
                    mesh = GetMeshFromGameObject(child.gameObject);
                    if (mesh != null)
                    {
                        break;
                    }
                }
            }
            return mesh;
        }
        public static void ExportMesh(Mesh mesh, string assetPath)
        {
            if (mesh != null)
            {
                // Save the mesh as an .asset file
                string assetFilePath = assetPath;

                string relativePath = assetFilePath.Substring(assetFilePath.IndexOf("Assets"));

                AssetDatabase.CreateAsset(mesh, relativePath);
                Debug.Log(relativePath);

                AssetDatabase.Refresh();

                Debug.Log("Mesh asset created and saved at: " + relativePath);
            }
            else
            {
                Debug.LogWarning("No mesh to export.");
            }
        }

        public static void ReplaceMeshRecursive(GameObject go, Mesh newMesh)
        {
            MeshFilter meshFilter = go.GetComponent<MeshFilter>();
            SkinnedMeshRenderer skinnedMeshRenderer = go.GetComponent<SkinnedMeshRenderer>();

            if (meshFilter != null)
            {
                meshFilter.sharedMesh = newMesh;
            }
            else if (skinnedMeshRenderer != null)
            {
                skinnedMeshRenderer.sharedMesh = newMesh;
            }

            foreach (Transform child in go.transform)
            {
                ReplaceMeshRecursive(child.gameObject, newMesh);
            }
        }
        public static Mesh SimplifyMesh(float targetQuality, Mesh targetMesh, MeshSimplifier meshSimplifier = null)
        {
            if (meshSimplifier == null) meshSimplifier = new MeshSimplifier();
            meshSimplifier.Initialize(targetMesh);
            meshSimplifier.SimplifyMesh(targetQuality);
            Mesh simplifiedMesh = meshSimplifier.ToMesh();
            return simplifiedMesh;
        }
        public static Mesh DuplicateMesh(this Mesh originalMesh)
        {
            Mesh newMesh = new Mesh
            {
                vertices = originalMesh.vertices,
                normals = originalMesh.normals,
                tangents = originalMesh.tangents,
                uv = originalMesh.uv,
                uv2 = originalMesh.uv2,
                uv3 = originalMesh.uv3,
                uv4 = originalMesh.uv4,
                colors = originalMesh.colors,
                colors32 = originalMesh.colors32,
                subMeshCount = originalMesh.subMeshCount
            };

            for (int submeshIndex = 0; submeshIndex < originalMesh.subMeshCount; submeshIndex++)
            {
                newMesh.SetTriangles(originalMesh.GetTriangles(submeshIndex), submeshIndex);
            }

            newMesh.bindposes = originalMesh.bindposes;
            newMesh.boneWeights = originalMesh.boneWeights;

            return newMesh;
        }
        public static void CopyParametersFrom(this Mesh targetMesh, Mesh sourceMesh)
        {
            targetMesh.Clear();  // Clear existing data from the target mesh

            targetMesh.vertices = sourceMesh.vertices;
            targetMesh.normals = sourceMesh.normals;
            targetMesh.tangents = sourceMesh.tangents;
            targetMesh.uv = sourceMesh.uv;
            targetMesh.uv2 = sourceMesh.uv2;
            targetMesh.uv3 = sourceMesh.uv3;
            targetMesh.uv4 = sourceMesh.uv4;
            targetMesh.colors = sourceMesh.colors;
            targetMesh.colors32 = sourceMesh.colors32;
            targetMesh.subMeshCount = sourceMesh.subMeshCount;

            for (int submeshIndex = 0; submeshIndex < sourceMesh.subMeshCount; submeshIndex++)
            {
                targetMesh.SetTriangles(sourceMesh.GetTriangles(submeshIndex), submeshIndex);
            }

            targetMesh.bindposes = sourceMesh.bindposes;
            targetMesh.boneWeights = sourceMesh.boneWeights;
        }

    }
}
