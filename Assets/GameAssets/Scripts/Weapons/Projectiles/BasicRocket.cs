using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRocket : MonoBehaviour
{
    private Vector3 m_targetLocation;
    private DamagableObject m_followingDamagableObject;

    public float Speed = 10;

    public float explosionTimeout = 20;

    public float rocketScale = 1;

    private BasicExplodingObject m_explodingObject;

    public void Awake()
    {
        m_explodingObject = this.GetComponent<BasicExplodingObject>();
        
        var All_transfroms = this.GetComponentsInChildren<Transform>();

        foreach (Transform tf in All_transfroms)
        {
            tf.localScale = tf.localScale * rocketScale;   
        }
    }

    private void OnEnable()
    {
        Invoke("selfDestoryOnTimeOut",explosionTimeout);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    
    public void fireRocket(Vector3 position)
    {
        m_followingDamagableObject = null;
        m_targetLocation = position;
        fireRocket();

    }

    private void fireRocket()
    {
        Vector3 relativePosition = m_targetLocation - this.transform.position;
    }

    [ContextMenu("Fire")]
    public void fireRocket(DamagableObject followTarget)
    {
        m_followingDamagableObject = followTarget;
        fireRocket();
    }

    public void Update()
    {
        // Enable follow target position
        if(m_followingDamagableObject != null && !m_followingDamagableObject.isDestroyed())
        {
            m_targetLocation = m_followingDamagableObject.getTransfrom().position;
        }


        Vector3 moveStep = Vector3.MoveTowards(this.transform.position,m_targetLocation,Time.deltaTime*Speed);

        this.transform.position = moveStep;
        this.transform.LookAt(m_targetLocation);

        checkExplodeCondition();
    }

    private void selfDestoryOnTimeOut()
    {
        m_explodingObject.explode(ProjectilePool.POOL_OBJECT_TYPE.RocketExplosionParticle);
    }

    private void checkExplodeCondition()
    {
        if(Vector3.Distance(this.transform.position,m_targetLocation)<0.2f)
        {
            m_explodingObject.explode(ProjectilePool.POOL_OBJECT_TYPE.RocketExplosionParticle);
        }
    }
}
