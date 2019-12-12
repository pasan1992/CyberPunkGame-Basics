using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WaypointRutine))]
public class RoutineEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        WaypointRutine myTarget = (WaypointRutine)target;
        if(GUILayout.Button("Build Waypoint"))
        {
            myTarget.createWaypoint();
        }

        if(GUILayout.Button("Remove Waypoint"))
        {
            myTarget.removeWaypoint();
        }
    }

     void OnSceneGUI()
     {
        WaypointRutine myTarget = (WaypointRutine)target;

        BasicWaypoint previousPoint = null;
        
        foreach (BasicWaypoint point in myTarget.m_wayPoints)
        {
            if( previousPoint != null)
            {
                Handles.DrawLine(previousPoint.transform.position, point.transform.position);
                previousPoint = point;
            }
            else
            {
                previousPoint = point;
            }
        }
     }
}
