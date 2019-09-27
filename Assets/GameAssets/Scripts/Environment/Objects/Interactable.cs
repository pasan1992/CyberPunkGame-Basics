using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [System.Serializable]
    public class InteractableProperties
    {
        public enum InteractableType {PrimaryWeapon,SecondaryWeapon,Switch,Pickup}
        public InteractableType Type = InteractableType.PrimaryWeapon;
        public bool interactionEnabled = false;
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
