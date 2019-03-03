using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMovingAgent : MovingAgent
{
    //GameObject player;
    //private float ShootingTIme;

    //public void Start()
    //{
    //    m_animationSystem.toggleEquip();
    //}
    //public override Vector3 getTargetPoint()
    //{
    //    //Vector3 mouse = Input.mousePosition;
    //    //Ray castPoint = Camera.main.ScreenPointToRay(mouse);
    //    //RaycastHit hit;
    //    //if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, floorHitLayerMask))
    //    //{
    //    //    return hit.point;
    //    //}

    //    if(player == null)
    //    {
    //        player = GameObject.FindGameObjectWithTag("Player");
    //    }



    //    return new Vector3(player.transform.position.x,player.transform.position.y + 1.5f,player.transform.position.z);
    //}

    ///*
    // * Current Target direction from the player
    // */
    //public override  Vector3 getTargetDirection()
    //{
    //    return (getTargetPoint() - this.transform.position).normalized;
    //}

    ///*
    // * Get Target Position with y value as characters feet height.
    // */
    //public override Vector3 getTurnPoint()
    //{
    //    Vector3 position = m_target.transform.position;
    //    position.y = this.transform.position.y;
    //    return position;
    //}

    ///*
    // * Get Target Postion with y value as characters weapon level height. 
    // */
    //public override Vector3 getLookPoint()
    //{
    //    Vector3 position = m_target.transform.position;
    //    position.y = this.transform.position.y + 1.25f;
    //    return position;
    //}

    ///*
    // * Start Shooting.
    // */
    //public override void updateShooting()
    //{
    //    //if (Input.GetMouseButtonUp(0) && Input.GetMouseButton(1))
    //    //{
    //    //    if (m_aimIK.solver.IKPositionWeight > 0.99)
    //    //    {
    //    //        m_recoil.Fire(2);
    //    //    }
    //    //}
    //    ShootingTIme += Time.deltaTime;

    //    if(ShootingTIme >Random.Range(1,3))
    //    {
    //        ShootingTIme = 0;

    //        if(Random.value >0.5)
    //        {
    //            //m_equipmentSystem.FireCurrentWeapon();
    //        }
    //    }
       
    //}

    ///*
    // * Set Character State.
    // */
    //public override void setCharacterState()
    //{
    //    m_characterState = CharacterMainStates.Aimed;
    //}

    ///*
    // * Set Character movment.
    // */
    //public override Vector3 getMovmentInput()
    //{
    //    return Vector3.zero;
    //}

}
