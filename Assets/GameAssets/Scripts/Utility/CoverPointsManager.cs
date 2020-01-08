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
