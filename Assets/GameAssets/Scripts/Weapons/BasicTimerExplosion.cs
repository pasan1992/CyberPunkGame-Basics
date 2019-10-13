using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTimerExplosion : BasicExplodingObject
{
     [SerializeField] 
    private float m_explosionCountDown;
    private float m_currentCownDown = 0;
    private bool m_countDownStarted = false;

    public float ExplosionCountDown { get => m_explosionCountDown; set => m_explosionCountDown = value; }
    public bool CountDownStarted { get => m_countDownStarted; set => m_countDownStarted = value; }
    public float CurrentCownDown { get => m_currentCownDown; set => m_currentCownDown = value; }

    public void Update()
    {
        if(ExplosionCountDown < CurrentCownDown && CountDownStarted)
        {
            resetAll();
            explode();
        }

        if(CountDownStarted)
        {
            CurrentCownDown += (Time.deltaTime % 60);
        }
    }

    public void startCountDown()
    {
        CountDownStarted = true;
    }

    public void resetAll()
    {
        CurrentCownDown = 0;
        CountDownStarted = false;
    }

}
