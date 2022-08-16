using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

/// <summary>
/// A system that rotates the objects with Asteroid tag.
/// </summary>
public partial class RotationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities.WithAll<AsteroidTag>().ForEach((ref Rotation rotation, in MoveData moveData) => {
            rotation.Value = math.mul(rotation.Value, quaternion.RotateZ(math.radians(moveData.rotationSpeed * deltaTime)));
        }).ScheduleParallel();
    }
}
