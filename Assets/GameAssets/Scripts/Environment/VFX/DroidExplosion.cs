using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroidExplosion : BasicExplosion
{
    public Rigidbody head;
    public List<Rigidbody> hands;
    public List<Rigidbody> legs;
    public Rigidbody body;

    public enum ExplosionPart{Body,OneLeg,OneHand,Hands,Legs,Head,All};

    private List<Rigidbody> explodingList = new List<Rigidbody>();

    public void explodePart(ExplosionPart part)
    {
        switch (part)
        {
            case ExplosionPart.Body:
                explodingList.Add(body);
            break;
            case ExplosionPart.Hands:
                explodingList = hands;
            break;
            case ExplosionPart.Head:
                explodingList.Add(head);
            break;
            case ExplosionPart.Legs:
                explodingList = legs;
            break;
            case ExplosionPart.OneHand:
                explodingList.Add(hands[0]);
            break;
            case ExplosionPart.OneLeg:
                explodingList.Add(legs[0]);
            break;
            case ExplosionPart.All:
                exploade();
            break;
        }
        explodePart(explodingList);
        explodingList.Clear();
    }

    private void explodePart(List<Rigidbody> explodingParts)
    {
        foreach (Rigidbody rb in explodingParts)
        {
            rb.gameObject.SetActive(true);
            rb.gameObject.transform.parent = null;
            rb.transform.transform.position = this.transform.position;
            //rb.WakeUp();
            rb.AddForce(Random.insideUnitSphere *(Random.value * 5+3), ForceMode.Impulse);
            rb.AddForce(Vector3.up * 3, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * (Random.value * 5 + 5), ForceMode.Impulse);
        }

        getExplosionParticleEffect();

        Invoke("resetAll", 3f);
    }

    protected void resetAllParts()
    {
        body.gameObject.SetActive(false);
        head.gameObject.SetActive(false);
    }
}
