using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayCam : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject target;
    public float rotationSpeed = 1;
    private Vector3 offset;


    void Start()
    {
        offset = target.transform.position - this.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if(Input.GetMouseButton(2))
        {
            Quaternion cameraTurnAngle = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * rotationSpeed, Vector3.up);
            offset = cameraTurnAngle * offset;
        }

       // Vector3 newPostion = target.transform.position - offset;
       // newPostion = new Vector3(newPostion.x, this.transform.position.y, target.transform.position.z);
        this.transform.position = Vector3.Lerp(this.transform.position, target.transform.position - offset, Time.deltaTime*6);
        this.transform.LookAt(target.transform);
    }
}
