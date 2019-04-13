using UnityEngine;
using RootMotion.FinalIK;

public class ProjectileBasic : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = 0;
    private float moveDirection;
    private Transform target;
    public float DistanceTravelled = 0;
    private string shooterName ="test";
    private bool hit = false;
    public GameObject particleObject;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(Vector3.forward * speed);
        DistanceTravelled += Time.deltaTime * speed;

        if (DistanceTravelled > 1)
        {
            Destroy(this.gameObject);
        }
    }

    public void setTarget(Transform target)
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        string tag = other.tag;
        switch (tag)
        {
            case "Enemy":         
                hitOnEnemy(other);
                break;
            case "Wall":
            case "Cover":
                hitOnWall(other);
                break;
            case "Player":
                hitOnEnemy(other);
                break;
        }

    }

    private void hitOnEnemy(Collider other)
    {
        ICyberAgent movingAgnet = other.transform.GetComponentInParent<ICyberAgent>();

        if (movingAgnet != null && !hit)
        {
            if (!shooterName.Equals(movingAgnet.getNamge()))
            {
                hit = true;
                movingAgnet.reactOnHit(other, (this.transform.forward) * 5f, other.transform.position);
                movingAgnet.damageAgent(1);
            
                speed = 0;
                Destroy(this.gameObject);

                if(particleObject)
                {
                    GameObject hitParticle = GameObject.Instantiate(particleObject);
                    hitParticle.transform.position = this.transform.position;
                }
             
                if (!movingAgnet.IsFunctional())
                {
                    movingAgnet.DestroyCharacter();
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
        speed = 0;
        if (particleObject)
        {
            GameObject hitParticle = GameObject.Instantiate(particleObject);
            hitParticle.transform.position = this.transform.position;
        }
        Destroy(this.gameObject);
    }

    public void setShooterName(string name)
    {
        this.shooterName = name;
    }
}
