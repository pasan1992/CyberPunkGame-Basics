using UnityEngine;

public interface IPoints
    
{
    Vector3 getPosition();
    bool isOccupied();
    void setOccupent(ICyberAgent agent);
}
