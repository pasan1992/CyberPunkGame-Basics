using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioSource))]
//[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public abstract class RangedWeapon : Weapon
{
    
    public delegate void WeaponFireDeligaet(float weight);


    [Header("Fire Effects")]
    public ParticleSystem gunMuzzle;
    public ParticleSystem gunFireLight;

    [Header("Functional Parameters")]
    // This is for aimIK
    public GameObject targetPointTransfrom;
    public float fireRate;
    public float weaponRecoil = 2;
    public bool m_enableLine = true;
    public int m_magazineSize = 0; 

    protected LayerMask hitLayerMask;
    
    protected LineRenderer m_line;
    protected Rigidbody m_rigidbody;
    protected BoxCollider m_collider;
    protected WeaponFireDeligaet m_onWeaponFire;
    protected Vector3 m_gunFireingPoint;
    protected AudioSource m_audioScource;
    protected SoundManager m_soundManager;
    protected ProjectilePool m_projectilePool;
    protected bool triggerPulled = false;
    protected bool m_realoding = false;
    protected int m_ammoCount = 0;
    
    public void Awake()
    {
        m_line = this.GetComponent<LineRenderer>();
        m_rigidbody = this.GetComponent<Rigidbody>();
        m_collider = this.GetComponent<BoxCollider>();
        m_audioScource = this.GetComponent<AudioSource>();
        m_soundManager = GameObject.FindObjectOfType<SoundManager>();
        m_projectilePool = GameObject.FindObjectOfType<ProjectilePool>();
        hitLayerMask = LayerMask.NameToLayer("Enemy");
    }

    #region updates
    public override void updateWeapon()
    {
        // Update line from gun to target.
        if(m_line != null && m_enableLine)
        {
            if (m_isAimed)
            {
                Vector3 direction = m_target.transform.position - targetPointTransfrom.transform.position;
                m_line.SetPosition(0, targetPointTransfrom.transform.position);


                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                // Raycast to find a ragdoll collider
                RaycastHit hit = new RaycastHit();

                if (Physics.Raycast(targetPointTransfrom.transform.position, direction.normalized, out hit, 1000, hitLayerMask))
                {
                    m_line.SetPosition(1, hit.point);
                }
                else
                {
                    // m_line.SetPosition(1, targetPoint.transform.position + direction * 50);
                    m_line.SetPosition(1, m_target.transform.position);
                }

                Debug.DrawRay(targetPointTransfrom.transform.position, direction.normalized, Color.red);
            }
            else
            {
                m_line.SetPosition(0, Vector3.zero);
                m_line.SetPosition(1, Vector3.zero);
            }
        }

        if(targetPointTransfrom)
        {
            m_gunFireingPoint = targetPointTransfrom.transform.position - targetPointTransfrom.transform.forward * 0.1f;
        }
    }
    #endregion 

    #region getters and setters

    public void setReloading(bool isReloading)
    {
        m_realoding = isReloading;

        if(!m_realoding)
        {
            nonFunctionalProperties.magazineObjProp.SetActive(true);
        }
    }

    public bool isReloading()
    {
        return m_realoding;
    }

    // public override void setGunTarget(GameObject target)
    // {
    //     this.m_target = target;
    // }

    // public override void setOwnerFaction(AgentBasicData.AgentFaction owner)
    // {
    //     m_ownersFaction = owner;
    // }

    public void SetGunTargetLineStatus(bool status)
    {
        m_enableLine = status;
    }

    public void addOnWeaponFireEvent(WeaponFireDeligaet onfire)
    {
        m_onWeaponFire = null;
        m_onWeaponFire = onfire;
    }

    // public override void setAimed(bool aimed)
    // {
    //     m_isAimed = aimed;
    // }

    // public override void setWeaponSafty(bool enabled)
    // {
    //     weaponSafty = enabled;
    // }

    public int getAmmoCount()
    {
        return m_ammoCount;
    }

    public void setAmmoCount(int count)
    {
        m_ammoCount = count;
    }

    public bool isWeaponEmpty()
    {
        return m_ammoCount == 0;
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
        if(getAmmoCount() > 0)
        {
            m_ammoCount--;
            // GameObject Tempprojectile = GameObject.Instantiate(projectile, m_gunFireingPoint, this.transform.rotation);
            GameObject Tempprojectile = m_projectilePool.getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.BasicProjectile);
            Tempprojectile.transform.position = m_gunFireingPoint;
            Tempprojectile.transform.rotation = this.transform.rotation;


            Tempprojectile.transform.forward = (m_target.transform.position - m_gunFireingPoint).normalized;

            Tempprojectile.SetActive(true);
            ProjectileBasic projetcileBasic = Tempprojectile.GetComponent<ProjectileBasic>();
            projetcileBasic.speed = 1f;
            projetcileBasic.setFiredFrom(m_ownersFaction);
            projetcileBasic.setTargetTransfrom(m_target.transform);

            // if (!playerWeapon)
            // {
            //     projetcileBasic.setFollowTarget(true);
            // }


            if (this.isActiveAndEnabled)
            {
                StartCoroutine(waitAndRecoil());
            }
        }
        else
        {
            m_audioScource.PlayOneShot(m_soundManager.getEmptyGunSound());
        }
    }

    public override void dropWeapon()
    {
        this.transform.parent = null;
        m_rigidbody.isKinematic = false;
        m_rigidbody.useGravity = true;
        m_collider.isTrigger = false;
        if(m_line)
        {
            m_line.enabled = false;
        }

    }

    public virtual void reloadWeapon()
    {
        setReloading(true);
        GameObject obj = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.RifleAmmo);
        obj.transform.position = this.transform.position + nonFunctionalProperties.magazinePositionOffset;
        obj.transform.parent = null;
        obj.SetActive(true);
        obj.transform.rotation = Quaternion.Euler(Random.insideUnitSphere*90);
        nonFunctionalProperties.magazineObjProp.SetActive(false);
    }

    public override void resetWeapon()
    {
        m_rigidbody.isKinematic = true;
        m_rigidbody.useGravity = false;
        m_collider.isTrigger = true;
    }

    public virtual void pullTrigger()
    {
        if(!weaponSafty && !m_realoding)
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

    public override void onWeaponEquip()
    {
        setAimed(true);
        m_rigidbody.isKinematic = true;
    }

    #endregion
}
