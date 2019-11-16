using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [System.Serializable]

    
    public class InteractableProperties
    {

        public enum InteractableType {FastInteraction,PickupInteraction,TimedInteraction,ContinousInteraction}
        public InteractableType Type = InteractableType.PickupInteraction;
        public bool interactionEnabled = false;
        public string itemName = "";
        public float interactionTime;
        public int interactionID;
        public Vector3 offset = Vector3.zero;
        public Vector3 rotation = Vector3.zero;
    }

   [SerializeField]
    public InteractableProperties properties;

    private bool interacting;
    
    public Outline m_outLine;
    public virtual void Awake()
    {
        m_outLine = this.GetComponent<Outline>();
        setOutLineState(false);
    }

    public virtual void OnPickUpAction()
    {
        properties.interactionEnabled = false;
        this.gameObject.SetActive(false);
        setOutLineState(false);
    }

    public virtual void OnEquipAction()
    {
        properties.interactionEnabled = false;
        setOutLineState(false);
    }

    public virtual void setOutLineState(bool state)
    {
        if(m_outLine)
        {
            m_outLine.enabled = state;
        }
    }

    public virtual void interact()
    {
        interacting = true;
    }

    public virtual bool isInteracting()
    {
        return interacting;
    }

    public void stopInteraction()
    {
        interacting = false;
    }
}
