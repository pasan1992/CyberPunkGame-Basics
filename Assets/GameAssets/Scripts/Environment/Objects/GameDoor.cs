using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDoor : MonoBehaviour
{
    public bool lockedForPlayer = false;
    Animator m_animator;
    private bool opened =false;

    // Start is called before the first frame update
    void Start()
    {
        m_animator = this.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!opened)
        {
            if (other.tag == "Enemy")
            {
                m_animator.SetTrigger("open");
                opened = true;
                Invoke("closeDoor", 2);
                //Debug.Log("Open");
            }
            else if (other.tag == "Player" && !lockedForPlayer)
            {
                m_animator.SetTrigger("open");
                opened = true;
                Invoke("closeDoor", 2);

            }
        }
    }

    private void closeDoor()
    {
        opened = false;
    }

    public void activateForPlayer()
    {
        opened = true;
        m_animator.SetBool("Opened", true);
    }
}
