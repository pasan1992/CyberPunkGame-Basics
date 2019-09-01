using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedPlayerController : AgentController
{
    #region Initialize
    void Start()
    {
        
    }
    #endregion
    
    #region Update
    void Update()
    {
        
    }
    #endregion

    #region Commands
    public override void resetCharacher()
    {
        throw new System.NotImplementedException();
    }
    #endregion


    #region Getters and Setters
    public override void setMovableAgent(ICyberAgent agent)
    {
        throw new System.NotImplementedException();
    }

    public override void setPosition(Vector3 postion)
    {
        throw new System.NotImplementedException();
    }

    public override ICyberAgent getICyberAgent()
    {
        throw new System.NotImplementedException();
    }

    public override float getSkill()
    {
        throw new System.NotImplementedException();
    }

    #endregion

    #region Events callbacks
    public override void onAgentDisable()
    {
        throw new System.NotImplementedException();
    }

    public override void onAgentEnable()
    {
        throw new System.NotImplementedException();
    }

    #endregion

    #region Utility
    #endregion
}
