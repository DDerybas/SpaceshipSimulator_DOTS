using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

/// <summary>
/// A system that rotates the Player and MoveDirectionObjTag objects to the MoveData.rotationAngle.
/// </summary>
public partial class DirectionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities.WithAny<MoveDirectionObjTag, PlayerTag>().ForEach((ref Rotation rotation, in MoveData moveData) =>
        {
            rotation.Value = math.slerp(rotation.Value, quaternion.EulerXYZ(0, 0, moveData.rotationAngle), deltaTime * moveData.rotationSpeed);
        }).ScheduleParallel();
    }
}
