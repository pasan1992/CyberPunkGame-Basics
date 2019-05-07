using UnityEngine;
using RootMotion.FinalIK;

public class ProjectileBasic : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = 0;
    public float DistanceTravelled = 0;
    public AnimationCurve laserBeamTrailCurve;
    public AnimationCurve microLaserBearmTrailCurve;


    private string shooterName ="test";
    private bool hit = false;
    public GameObject particleObject;
    private AgentController.AgentFaction m_fireFrom;
    private TrailRenderer trail;


    #region updates

    private void Awake()
    {
        trail = this.GetComponent<TrailRenderer>();
    }

    void FixedUpdate()
    {
        this.transform.Translate(Vector3.forward * speed);
        DistanceTravelled += Time.deltaTime * speed;

        if (DistanceTravelled > 1)
        {
            this.gameObject.SetActive(false);
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
        if (agentController != null && !hit)
        {
            ICyberAgent movingAgnet = agentController.getICyberAgent();
            if (movingAgnet !=null && !m_fireFrom.Equals(movingAgnet.getFaction()))
            {
                hit = true;
                movingAgnet.reactOnHit(other, (this.transform.forward) * 5f, other.transform.position);
                movingAgnet.damageAgent(1);
            
                speed = 0;
                //Destroy(this.gameObject);
                this.gameObject.SetActive(false);

                if(particleObject)
                {
                    GameObject hitParticle = GameObject.Instantiate(particleObject);
                    hitParticle.transform.position = this.transform.position;
                }
             
                if (!movingAgnet.IsFunctional())
                {
                    Rigidbody rb = other.transform.GetComponent<Rigidbody>();

                    if (rb != null)
                    {
                        rb.isKinematic = false;
                        rb.AddForce((this.transform.forward) * 200, ForceMode.Impulse);
                    }
                }
            }

        }
    }

    private void hitOnWall(Collider wall)
    {
        if(DistanceTravelled > 0.03)
        {
            speed = 0;
            if (particleObject)
            {
                GameObject hitParticle = GameObject.Instantiate(particleObject);
                hitParticle.transform.position = this.transform.position;
            }
            // Destroy(this.gameObject);
            this.gameObject.SetActive(false);
        }

    }

    public void OnEnable()
    {
        //Invoke("hideBullet", 2.0f);
        resetProjectile();
    }

    public void OnDisable()
    {
        //CancelInvoke();
    }
    #endregion


    #region getters and setters
    public void setShooterName(string name)
    {
        this.shooterName = name;
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
        hit = false;
        trail.time = 0.1f;
        trail.minVertexDistance = 0.1f;
        trail.widthCurve = laserBeamTrailCurve;
    }

    public void resetToMicroBeam()
    {
        DistanceTravelled = 0;
        hit = false;
        trail.time = 0.02f;
        trail.minVertexDistance = 0.02f;
        trail.widthCurve = microLaserBearmTrailCurve;
    }

    #endregion
}
