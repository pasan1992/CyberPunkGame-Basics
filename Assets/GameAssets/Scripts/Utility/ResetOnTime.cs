using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetOnTime : MonoBehaviour
{
    // Start is called before the first frame update
    public float resetTime = 3;

    public void resetOnTime(float time)
    {
        resetTime = time;
        Invoke("Reset",resetTime);
    }

    private void Reset()
    {
        this.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        resetOnTime(resetTime);
    }

}
