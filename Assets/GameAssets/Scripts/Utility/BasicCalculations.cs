using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCalculations
{
    public static Vector3 calculateThrowVelocity(Vector3 relativePosition)
    {
        // if(relativePosition.magnitude > 12)
        // {
        //     relativePosition = relativePosition.normalized*12;
        // }

        float throwTime = 1f;
        float X_velocity = relativePosition.x/throwTime;
        float Z_velocity = relativePosition.z/throwTime;
        float Y_velocity = (2*relativePosition.y + Physics.gravity.magnitude*throwTime*throwTime)/(2*throwTime);
        return new Vector3(X_velocity,Y_velocity,Z_velocity);
    }
}
