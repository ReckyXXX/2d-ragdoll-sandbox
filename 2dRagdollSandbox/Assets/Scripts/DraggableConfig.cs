using UnityEngine;

[CreateAssetMenu(menuName = "Game/DraggableConfig")]
public class DraggableConfig : ScriptableObject
{
    public float dragForce;
    public float dragDistanceThreshold;
    public float maxVelocity;
}
