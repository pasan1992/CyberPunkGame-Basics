using UnityEngine;
using RootMotion.FinalIK;
using humanoid;

public class ProjectileBasic : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = 0;
    public float DistanceTravelled = 0;
    public AnimationCurve laserBeamTrailCurve;
    public AnimationCurve microLaserBearmTrailCurve;


    private string m_shooterName ="test";
    private bool m_hit = false;
    private AgentController.AgentFaction m_fireFrom;
    private TrailRenderer m_trail;
    private bool m_enabled = true;
    private Transform m_targetTransfrom;
    private bool m_followTarget = false;
    private bool m_targetReached = false;

    #region updates

    private void Awake()
    {
        m_trail = this.GetComponent<TrailRenderer>();
    }

    void FixedUpdate()
    {
        if(m_enabled)
        {
            //this.transform.Translate(Vector3.forward * speed);

            //if(Vector3.Distance(this.transform.position,m_targetPosition)> 0.5f && !m_targetReached)
            //{
            //    this.transform.position = Vector3.MoveTowards(this.transform.position, m_targetPosition, speed);
            //}
            //else
            //}

            if(m_followTarget && !m_targetReached)
            {
                this.transform.position = Vector3.MoveTowards(this.transform.position, m_targetTransfrom.position, speed);

                if(Vector3.Distance(this.transform.position,m_targetTransfrom.position) <0.3f)
                {
                    m_targetReached = true;
                }
            }
            else
            {
                this.transform.Translate(Vector3.forward * speed);
            }



            DistanceTravelled += Time.deltaTime * speed;

            if (DistanceTravelled > 0.5f)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
    #endregion

    #region event handlers

    private void OnTriggerEnter(Collider other)
    {
        string tag = other.tag;
        //Debug.Log(other.name);
        switch (tag)
        {
            case "Enemy":
            case "Player":
            case "Head":
            case "Chest":
                hitOnEnemy(other);
                break;
            case "Wall":
            case "Cover":
                hitOnWall(other);
                break;
        }

    }

    private void hitOnEnemy(Collider other)
    {
        AgentController agentController = other.transform.GetComponentInParent<AgentController>();
        //Debug.Log(other.name);
        if (agentController != null && !m_hit)
        {
            ICyberAgent cyberAgent = agentController.getICyberAgent();
            if (cyberAgent !=null && !m_fireFrom.Equals(cyberAgent.getFaction()))
            {
                m_hit = true;
                cyberAgent.reactOnHit(other, (this.transform.forward) * 3f, other.transform.position);
                cyberAgent.damageAgent(1);
            
                speed = 0;
                //Destroy(this.gameObject);
                this.gameObject.SetActive(false);

                //if(particleObject)
                //{
                //    //GameObject hitParticle = GameObject.Instantiate(particleObject);
                //    //hitParticle.transform.position = this.transform.position;
                //    //hitParticle.transform.LookAt(Vector3.up);
                //}

                //.getBulletHitBasicParticle();
                GameObject basicHitParticle = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.HitBasicParticle);
                basicHitParticle.SetActive(true);
                basicHitParticle.transform.position = this.transform.position;
                basicHitParticle.transform.LookAt(Vector3.up);

                
             
                if (!cyberAgent.IsFunctional())
                {
                    MovingAgent movingAgent = cyberAgent as MovingAgent;
                    if(movingAgent !=null)
                    {
                        Rigidbody rb = other.transform.GetComponent<Rigidbody>();

                        if (rb != null)
                        {
                            rb.isKinematic = false;
                            rb.AddForce((this.transform.forward) * 150, ForceMode.Impulse);
                        }

                        movingAgent.getChestTransfrom().GetComponent<Rigidbody>().AddForce((this.transform.forward) * 60 + Random.insideUnitSphere*10, ForceMode.Impulse);
                    }


                    //ExplosionEffect(this.transform.position);
                }
            }

        }
    }

    private void hitOnWall(Collider wall)
    {
        if(DistanceTravelled > 0.03)
        {
            speed = 0;

            //.getBulletHitBasicParticle();
            GameObject basicHitParticle = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.HitBasicParticle);
            basicHitParticle.SetActive(true);
            basicHitParticle.transform.position = this.transform.position;
            basicHitParticle.transform.LookAt(Vector3.up);

            // Destroy(this.gameObject);
            this.gameObject.SetActive(false);
        }

    }

    public void OnEnable()
    {
        resetProjectile();
    }

    public void OnDisable()
    {
        //CancelInvoke();
        m_enabled = false;
        speed = 0;
    }
    #endregion


    #region getters and setters

    public void setFollowTarget(bool enable)
    {
        m_followTarget = enable;
    }

    public void setTargetTransfrom(Transform targetTransfrom)
    {
        m_targetTransfrom = targetTransfrom;
    }

    public void setShooterName(string name)
    {
        this.m_shooterName = name;
    }

    public void setFiredFrom(AgentController.AgentFaction group)
    {
        m_fireFrom = group;
    }

    public AgentController.AgentFaction getFireFrom()
    {
        return m_fireFrom;
    }


    public void resetProjectile()
    {
        DistanceTravelled = 0;
        m_hit = false;
        m_trail.time = 0.1f;
        m_trail.minVertexDistance = 0.1f;
        m_trail.widthCurve = laserBeamTrailCurve;
        m_enabled = true;
        m_followTarget = false;
        m_targetReached = false;
    }

    public void resetToMicroBeam()
    {
        DistanceTravelled = 0;
        m_hit = false;
        m_trail.time = 0.05f;
        m_trail.minVertexDistance = 0.05f;
        m_trail.widthCurve = microLaserBearmTrailCurve;
    }

    #endregion

    #region Commands

    //public void ExplosionEffect(Vector3 position)
    //{
    //    //.getBasicDroneExplosion()
    //    BasicExplosion droneExplosion = ProjectilePool.getInstance().getEffect(ProjectilePool.POOL_OBJECT_TYPE.DroneExplosion).GetComponent<BasicExplosion>();
    //    droneExplosion.gameObject.SetActive(true);
    //    droneExplosion.gameObject.transform.position = position;
    //    droneExplosion.GetComponent<BasicExplosion>().exploade();
    //}

    #endregion
}
