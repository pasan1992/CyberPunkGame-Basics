using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingInfoText : MonoBehaviour
{

    private Transform target;

    private Text m_text;

    private Vector3 m_offset;

    void Awake()
    {
        m_text = this.GetComponent<Text>();
    }

 
    void Update()
    {
        if(target)
        {
            transform.position =  Camera.main.WorldToScreenPoint(target.position) + m_offset;
        } 
    }

    public void setText(string name)
    {
        m_text.text = name;
    }

    public void setInteratableObject(Interactable interactable)
    {
        m_text.text = interactable.properties.itemName;
        m_offset = interactable.visualProperties.nameTagOffset;  
        target = interactable.transform; 
    }

    public void resetText()
    {
        target = null;
        this.transform.position = Vector3.zero;
    }
}
