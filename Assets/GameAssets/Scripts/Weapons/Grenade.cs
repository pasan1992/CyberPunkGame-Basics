using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : Weapon
{
    public GameObject tempGrenede;
    public override WEAPONTYPE getWeaponType()
    {
        return WEAPONTYPE.grenede;
    }

    public void pullGrenedePin()
    {
        tempGrenede.SetActive(true);
        tempGrenede.GetComponent<BasicTimerExplosion>().resetAll();
        Rigidbody rb = tempGrenede.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        tempGrenede.transform.parent = this.transform;
        tempGrenede.transform.localPosition =Vector3.zero;
        tempGrenede.GetComponent<BasicTimerExplosion>().startCountDown();
    }

    public void ThrowGrenede()
    {
        tempGrenede.transform.parent = null;
        Rigidbody rb = tempGrenede.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.velocity = Vector3.zero;
        // tempGrenede.GetComponent<Rigidbody>().isKinematic = false;
        // tempGrenede.GetComponent<Rigidbody>().velocity = Vector3.zero;
        tempGrenede.transform.position = this.transform.position;
        Vector3 throwVeclocity = calculateThrowVelocity(m_target.transform.position - this.transform.position - Vector3.up*1.5f);
        tempGrenede.GetComponent<Rigidbody>().AddForce(throwVeclocity,ForceMode.VelocityChange);
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
