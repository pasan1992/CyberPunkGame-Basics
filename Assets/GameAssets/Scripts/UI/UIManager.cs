using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    public FloatingInfoText lootText;
    public Transform playerTransfrom;

    private Interactable m_currentInteractable;
    // Update is called once per frame
    void Update()
    {
        updateLootText();
    }

    private void updateLootText()
    {
        if(m_currentInteractable)
        {
            m_currentInteractable.setOutLineState(false);
        }

        m_currentInteractable = AgentItemFinder.findNearItem(playerTransfrom.position);

        if(m_currentInteractable)
        {
            lootText.target = m_currentInteractable.gameObject.transform;
            m_currentInteractable.setOutLineState(true);
            lootText.setText(m_currentInteractable.properties.itemName);
        }
        else
        {
            lootText.target = null;
            lootText.transform.position = Vector3.zero;
        }
    }
}
