using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FlipNormals : MonoBehaviour
{
    [ContextMenu("Flip")]
    public void Flip()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.triangles = mesh.triangles.Reverse().ToArray();
    }

}
