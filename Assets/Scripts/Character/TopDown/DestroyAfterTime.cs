using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float time;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(waitAndDestory());
    }

    IEnumerator waitAndDestory()
    {
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }
}
