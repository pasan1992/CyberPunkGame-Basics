using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : Weapon
{
    private GameObject m_tempGrenede;

    public override WEAPONTYPE getWeaponType()
    {
        return WEAPONTYPE.grenede;
    }

    public void pullGrenedePin()
    {
        m_tempGrenede = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.Grenade);
        m_tempGrenede.SetActive(true);
        m_tempGrenede.GetComponent<BasicTimerExplosion>().resetAll();
        Rigidbody rb = m_tempGrenede.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        m_tempGrenede.transform.parent = this.transform;
        m_tempGrenede.transform.localPosition =Vector3.zero;
        m_tempGrenede.GetComponent<BasicTimerExplosion>().startCountDown();
    }

    public void ThrowGrenede()
    {
        m_tempGrenede.transform.parent = null;
        Rigidbody rb = m_tempGrenede.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.velocity = Vector3.zero;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        m_tempGrenede.transform.position = this.transform.position;
        Vector3 throwVeclocity = calculateThrowVelocity(m_target.transform.position - this.transform.position - Vector3.up*1.5f);
        m_tempGrenede.GetComponent<Rigidbody>().AddForce(throwVeclocity,ForceMode.VelocityChange);
    }


    public Vector3 calculateThrowVelocity(Vector3 relativePosition)
    {
        if(relativePosition.magnitude > 12)
        {
            relativePosition = relativePosition.normalized*12;
        }

        float throwTime = 1f;
        float X_velocity = relativePosition.x/throwTime;
        float Z_velocity = relativePosition.z/throwTime;
        float Y_velocity = (2*relativePosition.y + Physics.gravity.magnitude*throwTime*throwTime)/(2*throwTime);
        return new Vector3(X_velocity,Y_velocity,Z_velocity);
    }
}
