using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

/// <summary>
/// A system that moves the entity to the movement direction.
/// </summary>
public partial class MovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities.ForEach((ref Translation pos, in MoveData moveData) =>
        {
            float3 normalizedDir = math.normalizesafe(moveData.movement);
            pos.Value += normalizedDir * moveData.movementSpeed * deltaTime;
        }).ScheduleParallel();
        CompleteDependency();
    }
}
