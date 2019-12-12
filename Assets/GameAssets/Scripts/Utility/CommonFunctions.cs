using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonFunctions
{
    public static bool isAllies(ICyberAgent agent1, ICyberAgent agent2)
    {
        return agent1.getFaction().Equals(agent2.getFaction());
    }
}
