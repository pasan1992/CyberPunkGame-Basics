using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountedWeapon : MonoBehaviour
{

    #region Initialize
    public Transform m_mountedWeaponTransfrom;
    public Transform m_weaponFireringPosition;
    public ParticleSystem m_fireParticle;
    private GameObject m_target;

    void Awake()
    {
        m_target = new GameObject();
        m_target.name = this.name + "_target";
    }
    #endregion
    

    #region Update



    void updateWeapon()
    {
       var targetRotation =  Quaternion.FromToRotation(this.transform.forward,m_target.transform.position - this.transform.position);
       m_mountedWeaponTransfrom.rotation = Quaternion.Lerp(m_mountedWeaponTransfrom.rotation,targetRotation,0.2f);
    }

    void Update()
    {
        updateWeapon();
    }


    #endregion

    #region Commands

    [ContextMenu("Fire")]
    void fireWeapon()
    {
        GameObject Tempprojectile = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.BasicProjectile);
        Tempprojectile.transform.position = m_weaponFireringPosition.transform.position;
        Tempprojectile.transform.rotation = m_weaponFireringPosition.transform.rotation;
        Tempprojectile.SetActive(true);
        Tempprojectile.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);


        Tempprojectile.transform.forward = (m_target.transform.position - this.transform.position).normalized;
        BasicProjectile tempProjectile = Tempprojectile.GetComponent<BasicProjectile>();
        tempProjectile.speed = 1f;

        m_fireParticle.Play();
        //tempProjectile.setFiredFrom(m_agentData.m_agentFaction);
        //tempProjectile.resetToMicroBeam();
    }

    #endregion


    #region Getters and Setters
    void setWeaponTargetLocaion(Vector3 targetPositon)
    {

    }
    #endregion

    #region Events
    #endregion

    #region Utility
    #endregion
}
