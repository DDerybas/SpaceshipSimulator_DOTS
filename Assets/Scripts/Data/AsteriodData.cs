using Unity.Entities;

/// <summary>
/// Asteroid data that contains the asteroid entity (asteroid prefab).
/// </summary>
[GenerateAuthoringComponent]
public struct AsteriodData : IComponentData
{
    public Entity entity;
}
