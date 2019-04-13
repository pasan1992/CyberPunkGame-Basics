using UnityEngine;

public interface IPoints 
{
    Vector3 getPosition();
    void stPointOccupentsName(string name);
    bool isOccupied();
}
