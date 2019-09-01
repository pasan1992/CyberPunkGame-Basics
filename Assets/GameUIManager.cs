using UnityEngine;
using UnityEngine.UI;
using humanoid;

public class GameUIManager : MonoBehaviour
{
    // Start is called before the first frame update
    public enum UIType { PC,Mobile}

    public Text primaryBulletCount;
    public Text secondaryBulletCount;
    public Image crouchButton;


    public UIType m_uiType;
    private AgentController m_controller;
    private MovingAgent m_movingAgent;
    private bool crouched = false;
    

    void Start()
    {
        switch (m_uiType)
        {
            case UIType.PC:
                m_controller = FindObjectOfType<PlayerController>();
                break;
            case UIType.Mobile:
                m_controller = FindObjectOfType<PlayerControllerMobile>();
                break;
            default:
                break;
        }
        m_movingAgent = m_controller.GetComponent<MovingAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        primaryBulletCount.text = m_movingAgent.getPrimaryWeaponAmmoCount().ToString();
        secondaryBulletCount.text = m_movingAgent.getSecondaryWeaponAmmoCount().ToString();

        if(SimpleInput.GetButtonDown("Crouch"))
        {
            crouched = !crouched;

            if(crouched)
            {
                crouchButton.color = Color.cyan;
            }
            else
            {
                crouchButton.color = Color.white;
            }
        }
    }
}
