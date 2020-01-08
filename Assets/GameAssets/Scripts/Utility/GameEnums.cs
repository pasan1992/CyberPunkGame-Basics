using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnums 
{
    public enum CharacterStage {Combat,Iteraction}

    public enum DroneState {Flying,Landing,Landed,TakeOff,Disabled,Recovering}

    public enum Interaction {LOOK_AROUND = 1,SIT = 2 ,LAND = 3}

    #region Behavior
    public enum MovmentBehaviorType {FREE, NEAR_POINT,FIXED_POSITION}
    public enum MovmentBehaviorStage {MOVING_TO_POINT,AT_POINT,CALULATING_NEXT_POINT}     
    public enum Cover_based_combat_stages {IN_COVER,SHOOT}
    #endregion

}
