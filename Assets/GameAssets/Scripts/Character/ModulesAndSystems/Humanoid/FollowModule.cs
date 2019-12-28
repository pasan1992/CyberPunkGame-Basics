using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowModule : BasicMovmentStage
{
    private enum MainModes {InCombat,Relaxed}

    private enum MovmendModes {Following,LookingforCover,MovingToCover,InCover}
    private enum CombatModes {Covered,Open}
    private MainModes m_mainMode;
    private MovmendModes m_movmentMode;
    private CombatModes m_combatMode;

    public FollowModule(ICyberAgent selfAgent,NavMeshAgent agent):base(selfAgent,agent)
    {

    }
    public override void setTargets(ICyberAgent target)
    {
    }

    protected override void stepUpdate()
    {

    }
    private void updateMovmentMode()
    {
        switch (m_mainMode)
        {
            case MainModes.InCombat:

                switch (m_movmentMode)
                {
                    case MovmendModes.Following:
                    break;
                    case MovmendModes.InCover:
                    break;
                    case MovmendModes.LookingforCover:
                    break;
                    case MovmendModes.MovingToCover:
                    break;
                }

            break;
            
            case MainModes.Relaxed:

            break;
        }
    }
    private void updaateCombatMode()
    {
    }
}
