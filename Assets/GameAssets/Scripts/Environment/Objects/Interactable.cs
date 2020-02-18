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
        public bool enablePositionRequirment = true;
        public bool placeObjectInHand = false;
        public Transform actualObject = null;
    }

    [System.Serializable]
    public class VisualProperites
    {
        public Vector3 nameTagOffset;
        public Vector3 holdingPositionOffset;
        public Vector3 holdingRotationOffset;
    }

   [SerializeField]
    public InteractableProperties properties;

    [SerializeField]
    public VisualProperites visualProperties;

    private bool interacting;
    
    private Outline m_outLine;

    // To Reset the actual object loaction after interaction
    private Vector3 m_relativePosition;
    private Vector3 m_relativeRotation;

    
    private GameEvents.BasicNotifactionEvent onInteractionStartCallback;
    private GameEvents.BasicNotifactionEvent onInteractionStopCallback;

    public GameEvents.BasicNotifactionEvent OnInteractionStopCallback { get => onInteractionStopCallback; 
    set
    {
        onInteractionStopCallback += value;
    }
    }

    public GameEvents.BasicNotifactionEvent OnInteractionStartCallback { get => onInteractionStartCallback; 
    set 
    {
        onInteractionStartCallback +=value;
    }
    }

    public virtual void Awake()
    {
        m_outLine = this.GetComponent<Outline>();
        setOutLineState(false);
        
        if(properties.actualObject != null)
        {
            m_relativePosition = properties.actualObject.localPosition;
            m_relativeRotation = properties.actualObject.localRotation.eulerAngles;
        }
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
            if(state)
            {
                m_outLine.OutlineWidth = 0.7F;
            }
            else
            {
                m_outLine.OutlineWidth = 0;
            }
            
        }
    }

    public virtual void interact()
    {
        interacting = true;

        if(onInteractionStartCallback!=null)
            onInteractionStartCallback();

        Debug.Log("interact");
    }

    public virtual bool isInteracting()
    {
        return interacting;
    }

    public void stopInteraction()
    {
        interacting = false;

        if(onInteractionStopCallback !=null)
            onInteractionStopCallback();

        resetObject();
    }

    public void resetObject()
    {
        if(properties.placeObjectInHand && properties.actualObject !=null)
        {
            properties.actualObject.parent  = this.transform;
            properties.actualObject.transform.localPosition = m_relativePosition;
            properties.actualObject.transform.localRotation = Quaternion.Euler(m_relativeRotation);
        }
    }
}
