using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [System.Serializable]

    
    public class InteractableProperties
    {

        public enum InteractableType {Switch,Pickup,TimedInteraction}
        public InteractableType Type = InteractableType.Pickup;
        public bool interactionEnabled = false;
        public string itemName = "";

        public bool isTimed = false;
        public float interactionTime;
        public int interactionID;
    }

   [SerializeField]
    public InteractableProperties properties;

    public virtual void OnPickUpAction()
    {
        properties.interactionEnabled = false;
        this.gameObject.SetActive(false);
    }

    public virtual void OnEquipAction()
    {
        properties.interactionEnabled = false;
    }
}
