using UnityEngine;

public class DroneDamageModule : DamageModule
{
    public DroneDamageModule(float health,OnDestoryDeligate onDestroyCallback):base(health, onDestroyCallback)
    {
    }

    //public void destroyDrone(Vector3 movmentDirection)
    //{
    //    m_droneRigitBody.isKinematic = false;
    //    m_droneRigitBody.useGravity = true;
    //    m_droneRigitBody.WakeUp();
    //    m_droneRigitBody.AddForce(movmentDirection, ForceMode.Impulse);
    //    m_droneRigitBody.AddTorque(Random.insideUnitSphere * 200, ForceMode.Impulse);
    //    m_droneRigitBody.transform.parent = null;
    //}
}
