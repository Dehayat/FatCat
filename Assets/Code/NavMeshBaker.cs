using NavMeshPlus.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMeshBaker : MonoBehaviour
{
    public NavMeshSurface[] sufaces;

    private void Update()
    {
        if (Time.frameCount % 300 == 0)
        {
            foreach (NavMeshSurface surface in sufaces)
            {
                surface.UpdateNavMesh(surface.navMeshData);
            }
        }
    }
}
