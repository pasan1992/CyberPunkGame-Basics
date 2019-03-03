using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
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

    private bool isAimed = false;
    private LineRenderer m_line;
    private Rigidbody m_rigidbody;
    private BoxCollider m_collider;
    private bool enableLine;
    private WeaponFireDeligaet onWeaponFire;
    private string ownerName;
    private GameObject target;
    private Vector3 gunFireingPoint;


    protected bool triggerPulled = false;


    public void Awake()
    {
        //m_line = this.GetComponent<LineRenderer>();
        m_rigidbody = this.GetComponent<Rigidbody>();
        m_collider = this.GetComponent<BoxCollider>();
    }

    #region updates
    public virtual void updateWeapon()
    {
        if (isAimed && enableLine)
        {
            Vector3 direction = target.transform.position - targetPoint.transform.position;
            //m_line.SetPosition(0, targetPoint.transform.position);


            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // Raycast to find a ragdoll collider
            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(targetPoint.transform.position, direction.normalized, out hit, 1000, hitLayerMask))
            {
                //m_line.SetPosition(1, hit.point);
            }
            else
            {
               // m_line.SetPosition(1, targetPoint.transform.position + direction * 50);
            }

            Debug.DrawRay(targetPoint.transform.position, direction.normalized, Color.red);

 
        }

        gunFireingPoint = targetPoint.transform.position - targetPoint.transform.forward * 0.1f;

    }
    #endregion 

    #region getters and setters

    public void setGunTarget(GameObject target)
    {
        this.target = target;
    }

    public void setOwner(string owner)
    {
        ownerName = owner;
    }

    public void SetGunTargetLineStatus(bool status)
    {
        enableLine = status;
    }

    public void addOnWeaponFireEvent(WeaponFireDeligaet onfire)
    {
        onWeaponFire = null;
        onWeaponFire = onfire;
    }

    public void setAimed(bool aimed)
    {
        isAimed = aimed;
        if (enableLine & m_line != null)
        {
            //m_line.enabled = aimed;
        }

    }

    public virtual WEAPONTYPE getWeaponType()
    {
        return WEAPONTYPE.primary;
    }

    #endregion

    #region commands

    protected IEnumerator waitAndRecoil()
    {
        yield return new WaitForSeconds(0.1f);
        onWeaponFire(weaponRecoil);

        if(gunMuzzle !=null)
        {
            gunMuzzle.Play();
            gunFireLight.Play();
        }

    }

    protected virtual void fireWeapon()
    {
        GameObject Tempprojectile = GameObject.Instantiate(projectile, gunFireingPoint, this.transform.rotation);
        Tempprojectile.transform.forward = (target.transform.position - targetPoint.transform.position).normalized;
        Tempprojectile.GetComponent<ProjectileBasic>().speed = 1f;
        Tempprojectile.GetComponent<ProjectileBasic>().setShooterName(ownerName);
        StartCoroutine(waitAndRecoil());
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
        triggerPulled = true;
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
