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

    private Vector3 aimedPlayerPositon;
    Vector3 newCameraPosition;

    public float speedMultiplayer;
    void Start()
    {
        offset = target.transform.position - this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(roataionEnabled)
        {
            // if (Input.GetMouseButton(2))
            // {
            //     Quaternion cameraTurnAngle = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * rotationSpeed, Vector3.up);
            //     offset = cameraTurnAngle * offset;
            // }

            //this.transform.position = Vector3.Lerp(this.transform.position, target.transform.position - offset, Time.deltaTime * 5);
            speedMultiplayer =Mathf.Lerp(speedMultiplayer, (target.getCurrentVelocity().normalized).magnitude,0.1f);
            this.transform.position = Vector3.Lerp(this.transform.position, calcualteCameraAimPositon(), Time.deltaTime * 4);
            //this.transform.LookAt(target.transform);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(target.transform.position - this.transform.position), Time.deltaTime * 1);
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
        if(target.isAimed())
        {
            newCameraPosition = target.transform.position + Vector3.ClampMagnitude((target.getTargetPosition() - target.transform.position)/2,7 ) - offset;
            aimedPlayerPositon = target.transform.position;
            return newCameraPosition;
        }
        else if(Vector3.Distance(aimedPlayerPositon,target.transform.position) <0.1f )
        {
            return  newCameraPosition;
        }
        else
        {
           return  target.transform.position - offset; 
        }
    }
}
