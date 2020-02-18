using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverPointsManager : MonoBehaviour
{
    private static CoverPoint[] m_allCoverPoints;
    private static int COVER_POINT_MIN_DISTANCE = 15;

    void Awake()
    {
        m_allCoverPoints = GameObject.FindObjectsOfType<CoverPoint>();
    }

    public static SortedDictionary<float, CoverPoint> getDistanceMap(CoverPoint point)
    {
        var cpMap = new SortedDictionary<float, CoverPoint>();
        foreach (CoverPoint cp in m_allCoverPoints)
        {
            if(cp != point)
            {

                float distance = Vector3.Distance(cp.transform.position,point.transform.position);
                cpMap.Add(distance,cp);
            }
        }
        return cpMap;
    }

    public static CoverPoint getNearCoverObject(ICyberAgent m_selfAgent,ICyberAgent opponent,float fireRangeDistance,bool playitSafe)
    {
    //    float minimumDistanceToIdealCoverPoint = 999;
    //     float minimumDistanceToSafeCoverPoint = 999;
    //     float maximumDistanceToRiskyCoverPoint = 0;
    //     float distanceToClosestCoverPoint = 99999;

    //     CoverPoint tempIDealCoverPoint = null;
    //     CoverPoint tempSafeCoverPoint = null;
    //     CoverPoint tempRiskyCoverPoint = null;


    //     foreach (CoverPoint point in m_allCoverPoints)
    //     {
    //         if (!point.isOccupied())
    //         {
    //             // If Opponent avialalbe
    //             if(opponent != null)
    //             {
    //                 point.setTargetToCover(opponent);

    //                 // Find Combat CoverPoint
    //                 if(point.isSafeFromTarget())
    //                 {

    //                     // Find the safe cover point.
    //                 if(minimumDistanceToSafeCoverPoint > point.distanceTo(opponent.getCurrentPosition()))
    //                 {
    //                         minimumDistanceToSafeCoverPoint = point.distanceTo(opponent.getCurrentPosition());
    //                         tempSafeCoverPoint = point;
    //                 }

    //                 // Find the ideal closest cover point.
    //                 if(point.canFireToTarget(fireRangeDistance))
    //                     {
    //                         if (minimumDistanceToIdealCoverPoint > point.distanceTo(m_selfAgent.getCurrentPosition()))
    //                         {
    //                             minimumDistanceToIdealCoverPoint = point.distanceTo(m_selfAgent.getCurrentPosition());
    //                             tempIDealCoverPoint = point;
    //                         }
    //                     }

    //                 }
                

    //                 // Opponent avialable and playit safe is there
    //                 if(playitSafe)
    //                 {
    //                     // Find the safe cover point.
    //                     float distanceFromRiskyPoint = point.distanceTo(opponent.getCurrentPosition());
    //                     if (maximumDistanceToRiskyCoverPoint < distanceFromRiskyPoint && distanceFromRiskyPoint < COVER_POINT_MIN_DISTANCE)
    //                     {
    //                         maximumDistanceToRiskyCoverPoint = point.distanceTo(opponent.getCurrentPosition());
    //                         tempRiskyCoverPoint = point;
    //                     }
    //                 }
    //                 else
    //                 {
    //                     float distanceFromPoint = point.distanceTo(opponent.getCurrentPosition());
    //                     if(distanceToClosestCoverPoint > distanceFromPoint && point.canFireToTarget(fireRangeDistance) && distanceFromPoint > 5)
    //                     {
    //                         distanceToClosestCoverPoint = distanceFromPoint;
    //                         tempRiskyCoverPoint = point;
    //                     }
    //                 }
    //             }
    //             // Opponent is null
    //             else
    //             {

    //             }
                
    //         }
    //     }

    //     if(tempIDealCoverPoint !=null)
    //     {
    //         //tempIDealCoverPoint.stPointOccupentsName(selfAgent.getName());
    //         tempIDealCoverPoint.setOccupent(m_selfAgent);
    //         return tempIDealCoverPoint;
    //     }
    //     else if(tempSafeCoverPoint != null && Vector2.Distance(opponent.getCurrentPosition(), tempSafeCoverPoint.transform.position) <= COVER_POINT_MIN_DISTANCE)
    //     {
    //         //tempSafeCoverPoint.stPointOccupentsName(selfAgent.getName());
    //         tempSafeCoverPoint.setOccupent(m_selfAgent);
    //         return tempSafeCoverPoint;
    //     }
    //     else if(tempRiskyCoverPoint !=null)
    //     {
    //         return tempRiskyCoverPoint;
    //     }

    //     return null;
        return getNearCoverObject(m_selfAgent,opponent,fireRangeDistance,playitSafe,Vector3.zero,0);
    }

    public static CoverPoint getNextCoverPoint(CoverPoint currentCoverPoint,ICyberAgent target,Vector3 selfPosition)
    {
        if(currentCoverPoint == null)
        {
            float closestDistance = float.MaxValue;
            CoverPoint selectedClosestCoverPoint = null;

            foreach(CoverPoint cp in m_allCoverPoints)
            {
                float distance = Vector3.Distance(selfPosition,cp.transform.position);

                if(distance < closestDistance)
                {
                    closestDistance = distance;
                    selectedClosestCoverPoint = cp;
                }
            }
            return selectedClosestCoverPoint;
        }


        int i = 0;
        CoverPoint selectedSafeCoverPoint = null;
        CoverPoint selectedUnsafeCoverPoint = null;
        float minimumSafeDistance = float.MaxValue;
        float minimumUnsafeDistance = float.MaxValue;

        foreach (KeyValuePair<float, CoverPoint> pair in currentCoverPoint.Cp_map)
        {
            i++;
            if(i > 15 && selectedUnsafeCoverPoint !=null)
            {
                break;
            }

            if(target != null)
            {
                pair.Value.setTargetToCover(target);

                if(!pair.Value.isOccupied() && pair.Value.isSafeFromTarget())
                {
                    float distance = Vector3.Distance(pair.Value.transform.position,target.getCurrentPosition());
                    if(pair.Value.isSafeFromTarget())
                    {
                        if(distance < minimumSafeDistance)
                        {
                            selectedSafeCoverPoint = pair.Value;
                            minimumSafeDistance = distance;
                        }
                    }

                    if(distance < minimumUnsafeDistance)
                    {
                        selectedUnsafeCoverPoint = pair.Value;
                        minimumUnsafeDistance = distance;
                    }                
                }
            }
            else
            {
                if(!pair.Value.isOccupied())
                {
                    return null;
                }
            }

        }

        if(selectedSafeCoverPoint !=null)
        {
            Debug.Log(selectedUnsafeCoverPoint);
            return selectedUnsafeCoverPoint;
        }

        return selectedSafeCoverPoint;
    }

    public static CoverPoint getNearCoverObject(ICyberAgent m_selfAgent,ICyberAgent opponent,float fireRangeDistance,bool playitSafe,Vector3 centeredPoint,float maxDistance)
    {
        float minimumDistanceToIdealCoverPoint = 999;
        float minimumDistanceToSafeCoverPoint = 999;
        float maximumDistanceToRiskyCoverPoint = 0;
        float distanceToClosestCoverPoint = 99999;

        CoverPoint tempIDealCoverPoint = null;
        CoverPoint tempSafeCoverPoint = null;
        CoverPoint tempRiskyCoverPoint = null;


        foreach (CoverPoint point in m_allCoverPoints)
        {
            if (!point.isOccupied() &&
              (centeredPoint.Equals(Vector3.zero) || Vector3.Distance(centeredPoint,point.getPosition()) < maxDistance))
            {
                // If Opponent avialalbe
                if(opponent != null)
                {
                    point.setTargetToCover(opponent);

                    // Find Combat CoverPoint
                    if(point.isSafeFromTarget())
                    {

                            // Find the safe cover point.
                        if(minimumDistanceToSafeCoverPoint > point.distanceTo(opponent.getCurrentPosition())) 
                        {
                                minimumDistanceToSafeCoverPoint = point.distanceTo(opponent.getCurrentPosition());
                                tempSafeCoverPoint = point;
                        }

                        // Find the ideal closest cover point.
                        if(point.canFireToTarget(fireRangeDistance) &&   
                          minimumDistanceToIdealCoverPoint > point.distanceTo(m_selfAgent.getCurrentPosition()) )
                          {
                            minimumDistanceToIdealCoverPoint = point.distanceTo(m_selfAgent.getCurrentPosition());
                            tempIDealCoverPoint = point;
                          }

                    }
                

                    // Opponent avialable and playit safe is there
                    if(playitSafe)
                    {
                        // Find the safe cover point.
                        float distanceFromRiskyPoint = point.distanceTo(opponent.getCurrentPosition());

                        if (maximumDistanceToRiskyCoverPoint < distanceFromRiskyPoint && 
                        distanceFromRiskyPoint < COVER_POINT_MIN_DISTANCE)
                        {
                            maximumDistanceToRiskyCoverPoint = point.distanceTo(opponent.getCurrentPosition());
                            tempRiskyCoverPoint = point;
                        }
                    }
                    else
                    {
                        float distanceFromPoint = point.distanceTo(opponent.getCurrentPosition());
                        if(distanceToClosestCoverPoint > distanceFromPoint && 
                        point.canFireToTarget(fireRangeDistance) && distanceFromPoint > 5)
                        {
                            distanceToClosestCoverPoint = distanceFromPoint;
                            tempRiskyCoverPoint = point;
                        }
                    }
                }
                // Opponent is null
                else
                {
                    float distanceFromPoint = point.distanceTo(m_selfAgent.getCurrentPosition());
                    if(maximumDistanceToRiskyCoverPoint < distanceFromPoint && distanceFromPoint < 8)
                    {
                        maximumDistanceToRiskyCoverPoint = distanceFromPoint;
                        tempRiskyCoverPoint = point;
                    }
                }
                
            }
        }

        if(tempIDealCoverPoint !=null)
        {
            //tempIDealCoverPoint.stPointOccupentsName(selfAgent.getName());
            tempIDealCoverPoint.setOccupent(m_selfAgent);
            return tempIDealCoverPoint;
        }
        else if(tempSafeCoverPoint != null && Vector2.Distance(opponent.getCurrentPosition(), tempSafeCoverPoint.transform.position) <= COVER_POINT_MIN_DISTANCE)
        {
            //tempSafeCoverPoint.stPointOccupentsName(selfAgent.getName());
            tempSafeCoverPoint.setOccupent(m_selfAgent);
            return tempSafeCoverPoint;
        }
        else if(tempRiskyCoverPoint !=null)
        {
            return tempRiskyCoverPoint;
        }

        return null;
    }
}
