using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myfirstscript : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody m_rigidBody;
    void Start()
    {
       m_rigidBody = this.GetComponent<Rigidbody>(); 
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            m_rigidBody.AddForce(new Vector3(0,1,0)*10,ForceMode.Impulse);
        }

        if(Input.GetKey(KeyCode.UpArrow))
        {
            Debug.Log("here");
            m_rigidBody.AddForce(Vector3.forward*10,ForceMode.Force);
        }
    }
}
