using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnums 
{
    public enum CharacterStage {Combat,Iteraction}

    public enum DroneState {Flying,Landing,Landed,TakeOff,Disabled,Recovering}

    public enum Interaction {LOOK_AROUND = 1,SIT = 2 ,LAND = 3}
}
