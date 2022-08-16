using Unity.Entities;

/// <summary>
/// Bullet data thet contains bullet entity (bullet prefab).
/// </summary>
[GenerateAuthoringComponent]
public struct BulletData : IComponentData 
{
    public Entity entity;
}
