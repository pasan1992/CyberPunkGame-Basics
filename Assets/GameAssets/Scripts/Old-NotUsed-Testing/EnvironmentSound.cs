using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSound : MonoBehaviour
{
    // Start is called before the first frame update
    private static EnvironmentSound _environmentSound;
    private GameEvents.BasicSoundEvent m_positionCallback;

    public static EnvironmentSound Instance
    {
        get
        {
            if(_environmentSound == null)
            {
                Debug.LogError("No env sound");
            }
            return _environmentSound;
        }
    }

    void Awake()
    {
        _environmentSound = this;
    }

    public void broadcastSound(Vector3 position, AgentBasicData.AgentFaction faction)
    {
        if(m_positionCallback != null)
        {
            m_positionCallback(position,faction);
        }
    }

    public void listenToSound(GameEvents.BasicSoundEvent callback)
    {
        m_positionCallback += callback;
    }

    public void removeListen(GameEvents.BasicSoundEvent callback)
    {
        m_positionCallback -= callback;
    }
}
