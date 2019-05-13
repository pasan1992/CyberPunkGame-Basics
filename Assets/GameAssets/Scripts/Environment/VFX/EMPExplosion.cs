using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMPExplosion : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        this.transform.localScale += Vector3.one * Time.deltaTime*20;
    }
}
