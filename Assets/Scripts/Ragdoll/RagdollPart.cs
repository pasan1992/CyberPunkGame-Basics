using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollPart : MonoBehaviour {

	// Use this for initialization
	public enum TYPE {head,UpperLegL,UpperLegR,LowerLegR,LowerLegL,Pevlis,Chest,ShoulderR,ShoulderL,ArmR,ArmL,HandL,HandR,FootL,FootR}
	public TYPE m_type;
	private Rigidbody m_rigidBody;
	private Collider m_collider;
	public RagdollController controller {get;set;}
	public TYPE getType()
	{
		return m_type;
	}
	void Awake () 
	{
		m_rigidBody = this.GetComponent<Rigidbody>();
		m_collider = this.GetComponent<Collider>();
		this.gameObject.layer = 10;
		m_rigidBody.drag = 1;
	}
	
	public void setRagdollPartState(bool enabled)
	{
		if(enabled)
		{
			if(m_rigidBody ==null)
			{
				Debug.Log(this.name);
			}
			m_rigidBody.isKinematic = false;
			m_collider.isTrigger = false;
		}
		else
		{
			m_collider.isTrigger = true;
			m_rigidBody.isKinematic = true;
		}
	}

	public void addImpulse(Vector3 impulseVector)
	{
		m_rigidBody.AddForce(impulseVector,ForceMode.Impulse);
	}

}
