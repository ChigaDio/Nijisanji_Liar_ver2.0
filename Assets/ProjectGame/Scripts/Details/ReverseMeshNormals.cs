using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class ReverseMeshCollider : MonoBehaviour
{
    void Start()
    {
        FlipMesh();
    }



    [ContextMenu("Flip Normals And Triangles")]
    void FlipMesh()
    {
        MeshCollider mc = GetComponent<MeshCollider>();
        mc.convex = false;
        Mesh mesh = mc.sharedMesh;

        if (mesh == null)
        {
            Debug.LogWarning("MeshColliderにsharedMeshが設定されていません");
            return;
        }

        Mesh newMesh = Instantiate(mesh);

        // 法線反転
        Vector3[] normals = newMesh.normals;
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = -normals[i];
        }
        newMesh.normals = normals;

        // 三角形の巻き順反転
        for (int s = 0; s < newMesh.subMeshCount; s++)
        {
            int[] triangles = newMesh.GetTriangles(s);
            for (int i = 0; i < triangles.Length; i += 3)
            {
                int temp = triangles[i + 1];
                triangles[i + 1] = triangles[i + 2];
                triangles[i + 2] = temp;
            }
            newMesh.SetTriangles(triangles, s);
        }

        // MeshColliderに反映
        mc.sharedMesh = null;
        mc.sharedMesh = newMesh;

        // 見た目も同期させたい場合はMeshFilterがあれば更新(なくてもOK)
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf != null)
        {
            mf.sharedMesh = newMesh;
        }
    }
        
    
}