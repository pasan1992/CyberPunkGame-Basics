using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayCam : MonoBehaviour
{
    // Start is called before the first frame update

    public HumanoidMovingAgent target;
    public float rotationSpeed = 1;
    public bool roataionEnabled = true;
    private Vector3 offset;

    private Vector3 m_cameraAimOffset;

    private Vector3 aimedPlayerPositon;
    Vector3 newCameraPosition;

    public bool maintainAimedOffset = false;

    //public float speedMultiplayer;
    void Start()
    {
        offset = target.transform.position - this.transform.position;
        m_cameraAimOffset = Vector3.zero;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(roataionEnabled)
        {
            //this.transform.position = Vector3.Lerp(this.transform.position, target.transform.position - offset, Time.deltaTime * 5);
            //speedMultiplayer =Mathf.Lerp(speedMultiplayer, (target.getCurrentVelocity().normalized).magnitude,0.1f);
            this.transform.position = Vector3.Lerp(this.transform.position, calcualteCameraAimPositon(), Time.deltaTime * UtilityConstance.CAMERA_VIEW_FOLLOW_RATE);
            //this.transform.LookAt(target.transform);

           // transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(target.transform.position - this.transform.position), Time.deltaTime * 1);
        }
        else
        {
            Vector3 newPostion = target.transform.position - offset;
            //newPostion = new Vector3(newPostion.x, this.transform.position.y, target.transform.position.z);
            this.transform.position = newPostion;
        }
    }

    private Vector3 calcualteCameraAimPositon()
    {
        if(target.isAimed() 
        // To Avoid unplesent behavior of the camera when dodging + aimed 
        || (HumanoidMovingAgent.CharacterMainStates.Dodge.Equals(target.getCharacterMainStates()) && target.hasWeaponInHand() && Input.GetMouseButton(1) ) )
        {
            // Smooth the motion of the camera aim offset
            m_cameraAimOffset = Vector3.Lerp(m_cameraAimOffset,Vector3.ClampMagnitude((target.getTargetPosition() - target.transform.position)/2,UtilityConstance.CAMERA_AIM_OFFSET_MAX_DISTANCE ),UtilityConstance.CAMERA_AIM_OFFSET_CHANGE_RATE);
            newCameraPosition = target.transform.position + Vector3.ClampMagnitude((target.getTargetPosition() - target.transform.position)/2,UtilityConstance.CAMERA_AIM_OFFSET_MAX_DISTANCE ) - offset;
            aimedPlayerPositon = target.transform.position;
            return newCameraPosition;
        }
        else if(maintainAimedOffset && (Vector3.Distance(aimedPlayerPositon,target.transform.position) <0.1f || (target.hasWeaponInHand() && Vector3.Angle(m_cameraAimOffset,target.getMovmentDirection()) < 120 )))
        {
            return target.transform.position + m_cameraAimOffset - offset;
        }
        else
        {
            // To smoothly return the camera to its original position
           m_cameraAimOffset = Vector3.Lerp(m_cameraAimOffset,Vector3.zero,UtilityConstance.CAMERA_AIM_OFFSET_CHANGE_RATE);
           return  target.transform.position - offset + m_cameraAimOffset; 
        }
    }
}
