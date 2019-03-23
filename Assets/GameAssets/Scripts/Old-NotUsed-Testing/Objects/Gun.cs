using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    // Use this for initialization
    public GameObject m_muzzleFlash;
    public GameObject m_bulletTrace;
    public float m_ShootingTime;
    public float m_recoilTime;
    public delegate void GunEventDekegate();
    private GunEventDekegate notifyRecoilStart;

    private bool m_isShooting  = false;
    private float m_currentShootingTime =0;


    private float m_currentRecoilTime = 0;
    private bool m_isRecoil = false;
    

    public void Start()
    {
        m_muzzleFlash.SetActive(false);
        m_bulletTrace.SetActive(false);
    }

    public void UpdateGun()
    {
        if(m_isShooting && m_currentShootingTime <m_ShootingTime)
        {
            m_currentShootingTime +=Time.deltaTime;
            m_muzzleFlash.SetActive(true);
            m_bulletTrace.SetActive(true);
        }
        else if(m_isShooting)
        {
            notifyRecoilStart();
            m_isRecoil = true;
            m_isShooting = false;
            m_currentRecoilTime = 0;
            
            m_bulletTrace.SetActive(false);
        }
        else if(m_isRecoil && m_currentRecoilTime < m_recoilTime)
        {
            m_currentRecoilTime += Time.deltaTime;
        }
        else
        {
            m_currentShootingTime = 0;
            m_currentRecoilTime = 0;
            m_isRecoil = false;
            m_muzzleFlash.SetActive(false);
        }
    }

    public bool isShoting()
    {
        return m_isShooting || m_isRecoil;
    }

    public void pulTrigger()
    {
        m_currentShootingTime = 0;
        m_isShooting = true;
    }

    public void setNotifyRecoilStart(GunEventDekegate recoilActionFunction)
    {
        notifyRecoilStart = recoilActionFunction;
    }
}
