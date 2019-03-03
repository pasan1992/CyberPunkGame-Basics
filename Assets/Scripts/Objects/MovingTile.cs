using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTile : MonoBehaviour 
{	
	private float m_distanceTravelled = 0;
	private float m_speed;
	private float m_maxDistance;
    private Vector3 m_movingDirection;

	public bool MoveTile() 
	{
		this.transform.Translate(m_movingDirection* m_speed*Time.deltaTime);
		m_distanceTravelled +=m_speed*Time.deltaTime;

		if(m_distanceTravelled > m_maxDistance)
		{
			return false;
		}
		return true;
	}

	public void setParameters(float speed,float maxDistance, Vector3 movingDirection)
	{
		this.m_speed = speed;
		this.m_maxDistance = maxDistance;
        this.m_movingDirection = movingDirection;
    }

    public void setDistanceTravelled(float distanceTravelled)
    {
        m_distanceTravelled = distanceTravelled;
    }
}
