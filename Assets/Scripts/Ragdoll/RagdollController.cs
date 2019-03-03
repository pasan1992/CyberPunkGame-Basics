using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour 
{

	private RagdollPart[] m_ragdollParts;
	private Animator m_animator;
	public string RagdollPartTag;
	void Awake () 
	{
		m_ragdollParts = this.GetComponentsInChildren<RagdollPart>();
		m_animator = this.GetComponent<Animator>();
	}


	void Start()
	{
		setRagdollState(false);
	}

	public void setRagdollState(bool enabled)
	{
		foreach (RagdollPart ragdollPart in m_ragdollParts)
		{
			ragdollPart.setRagdollPartState(enabled);
			ragdollPart.controller = this;
			if(RagdollPartTag !="" && RagdollPartTag !=null)
			{
				ragdollPart.transform.tag = RagdollPartTag;
			}

		}

		m_animator.enabled =! enabled;
	}

	public Transform getRandomRagdollPart()
	{
		int randomValue = Random.Range(0,m_ragdollParts.Length);

		return m_ragdollParts[randomValue].transform;
	}

    public void AddImpulseToRagdoll(RagdollPart.TYPE part,Vector3 force)
    {
        foreach (RagdollPart ragdollPart in m_ragdollParts)
        {
            
            if(ragdollPart.m_type.Equals(part))
            {
                Debug.Log("Impulse");
                ragdollPart.addImpulse(force);
            }
        }
    }
}
