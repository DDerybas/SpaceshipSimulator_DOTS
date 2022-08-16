using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// Move data that contains an entity's rotation and movement speed and direction, angle of the movement.
/// </summary>
[GenerateAuthoringComponent]
public struct MoveData : IComponentData 
{
    public float3 movement;         // Movement direction.
    public float rotationAngle;

    public float movementSpeed;
    public float rotationSpeed;
}
