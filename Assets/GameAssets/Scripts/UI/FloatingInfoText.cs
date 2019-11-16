using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingInfoText : MonoBehaviour
{

    public Transform target;
    public int BUTTON_OFFSET;

    private Text m_text;

    void Awake()
    {
        m_text = this.GetComponent<Text>();
    }

 
    void Update()
    {
        if(target)
        {
            transform.position =  Camera.main.WorldToScreenPoint(target.position) + Vector3.up * BUTTON_OFFSET;
        } 
    }

    public void setText(string name)
    {
        m_text.text = name;
    }
}
