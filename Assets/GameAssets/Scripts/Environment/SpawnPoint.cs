using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(transform.position + new Vector3(0, 0.3f, 0), new Vector3(0.4f, 0.4f, 0.4f));
    }
}
