using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTimerExplosion : BasicExplodingObject
{
    public float explosionCountDown;
    public float currentCownDown = 0;
    public bool countDownStarted = false;


    public void Update()
    {
        if(explosionCountDown < currentCownDown && countDownStarted)
        {
            explode();
            resetAll();
        }

        if(countDownStarted)
        {
            currentCownDown += (Time.deltaTime % 60);
        }
    }

    public void startCountDown()
    {
        countDownStarted = true;
    }

    public void resetAll()
    {
        currentCownDown = 0;
        countDownStarted = false;
    }

}
