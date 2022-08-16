using Unity.Entities;

/// <summary>
/// Shoot point data that contains the shoot point entity.
/// </summary>
[GenerateAuthoringComponent]
public struct ShootPointData : IComponentData
{
    public Entity entity;
}
