using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class Weapon : MonoBehaviour
{
    public enum WEAPONTYPE { primary,secondary};
    public delegate void WeaponFireDeligaet(float weight);

    public ParticleSystem gunMuzzle;
    public ParticleSystem gunFireLight;


    public GameObject targetPoint;

    public GameObject projectile;
    public LayerMask hitLayerMask;
    public float fireRate;
    public float weaponRecoil = 2;

    private bool m_isAimed = false;
    private LineRenderer m_line;
    private Rigidbody m_rigidbody;
    private BoxCollider m_collider;
    public bool m_enableLine = true;
    private WeaponFireDeligaet m_onWeaponFire;
    private AgentController.AgentFaction m_ownersFaction;
    private GameObject m_target;
    private Vector3 m_gunFireingPoint;

    protected AudioSource m_audioScource;
    protected SoundManager m_soundManager;
    protected ProjectilePool m_projectilePool;


    protected bool triggerPulled = false;
    protected bool weaponSafty = false;


    public void Awake()
    {
        m_line = this.GetComponent<LineRenderer>();
        m_rigidbody = this.GetComponent<Rigidbody>();
        m_collider = this.GetComponent<BoxCollider>();
        m_audioScource = this.GetComponent<AudioSource>();
        m_soundManager = GameObject.FindObjectOfType<SoundManager>();
        m_projectilePool = GameObject.FindObjectOfType<ProjectilePool>();
    }

    #region updates
    public virtual void updateWeapon()
    {
        if(m_line != null && m_enableLine)
        {
            if (m_isAimed)
            {
                Vector3 direction = m_target.transform.position - targetPoint.transform.position;
                m_line.SetPosition(0, targetPoint.transform.position);


                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                // Raycast to find a ragdoll collider
                RaycastHit hit = new RaycastHit();

                if (Physics.Raycast(targetPoint.transform.position, direction.normalized, out hit, 1000, hitLayerMask))
                {
                    m_line.SetPosition(1, hit.point);
                }
                else
                {
                    // m_line.SetPosition(1, targetPoint.transform.position + direction * 50);
                    m_line.SetPosition(1, m_target.transform.position);
                }

                Debug.DrawRay(targetPoint.transform.position, direction.normalized, Color.red);
            }
            else
            {
                m_line.SetPosition(0, Vector3.zero);
                m_line.SetPosition(1, Vector3.zero);
            }
        }


        m_gunFireingPoint = targetPoint.transform.position - targetPoint.transform.forward * 0.1f;

    }
    #endregion 

    #region getters and setters

    public void setGunTarget(GameObject target)
    {
        this.m_target = target;
    }

    public void setOwnerFaction(AgentController.AgentFaction owner)
    {
        m_ownersFaction = owner;
    }

    public void SetGunTargetLineStatus(bool status)
    {
        m_enableLine = status;
    }

    public void addOnWeaponFireEvent(WeaponFireDeligaet onfire)
    {
        m_onWeaponFire = null;
        m_onWeaponFire = onfire;
    }

    public void setAimed(bool aimed)
    {
        m_isAimed = aimed;
        if (m_enableLine & m_line != null)
        {
            //m_line.enabled = aimed;
        }

    }

    public virtual WEAPONTYPE getWeaponType()
    {
        return WEAPONTYPE.primary;
    }

    public void setWeaponSafty(bool enabled)
    {
        weaponSafty = enabled;
    }

    #endregion

    #region commands

    protected IEnumerator waitAndRecoil()
    {
        yield return new WaitForSeconds(0.1f);
        if(this.enabled)
        {
            m_onWeaponFire(weaponRecoil);

            if (gunMuzzle != null)
            {
                gunMuzzle.Play();
                gunFireLight.Play();
                //m_audioScource.PlayOneShot(m_soundManager.getLaserFireAudioClip());
                playWeaponFireSound();
            }
        }


    }

    protected abstract void playWeaponFireSound();

    protected virtual void fireWeapon()
    {
        // GameObject Tempprojectile = GameObject.Instantiate(projectile, m_gunFireingPoint, this.transform.rotation);
        GameObject Tempprojectile = m_projectilePool.getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.BasicProjectile);
        Tempprojectile.transform.position = m_gunFireingPoint;
        Tempprojectile.transform.rotation = this.transform.rotation;
        

        Tempprojectile.transform.forward = (m_target.transform.position - m_gunFireingPoint).normalized;

        Tempprojectile.SetActive(true);
        Tempprojectile.GetComponent<ProjectileBasic>().speed = 1f;
        Tempprojectile.GetComponent<ProjectileBasic>().setFiredFrom(m_ownersFaction);
        
        if (this.isActiveAndEnabled)
        {
            StartCoroutine(waitAndRecoil());
        }

    }

    //public void continouseFire()
    //{
    //    burstFireInterval += Time.deltaTime;

    //    if (burstFireInterval > (1 / fireRate))
    //    {
    //        burstFireInterval = 0;
    //        fireWeapon();
    //        StartCoroutine(waitAndRecoil());
    //    }
    //}

    public virtual void dropWeapon()
    {
        this.transform.parent = null;
        m_rigidbody.isKinematic = false;
        m_rigidbody.useGravity = true;
        m_collider.isTrigger = false;
    }

    public virtual void pullTrigger()
    {
        if(!weaponSafty)
        {
            triggerPulled = true;
        }

    }

    public virtual void releaseTrigger()
    {
        triggerPulled = false;
    }

    #endregion

    #region eventHandlers

    public void onWeaponEquip()
    {
        setAimed(true);
    }

    #endregion
}
