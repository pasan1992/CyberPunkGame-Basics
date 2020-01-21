using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateShower : MonoBehaviour
{
    // Start is called before the first frame update
    private static StateShower _instance;
    public Text debugText;
    public string selectedAgent;


    public static StateShower Instance 
    { 
        get
        {
            if(_instance == null)
                Debug.LogError("State shower not in scene");

            return _instance;
        }
        set => _instance = value; 
    }

    void Awake()
    {
        Instance = this;
    }

    public void setText(string text,ICyberAgent agent)
    {
        if(selectedAgent == agent.getTransfrom().name)
        {
            debugText.text = text;    
        }
    }
}
