using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// A system that destroys an entity if it is off screen.
/// </summary>
public partial class DestroyOutsideSystem : SystemBase
{
    private EntityCommandBufferSystem ecbs => 
        World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

    private AABB worldBounds;                       // Game world bounds.

    protected override void OnStartRunning()
    {
        base.OnStartRunning();

        var camera = Camera.main;
        // Sets the bounds to the screen size.
        worldBounds = new AABB
        {
            Extents = new float3
            {
                x = camera.orthographicSize * Screen.width / Screen.height,
                y = camera.orthographicSize,
                z = 0
            }
        };
    }

    protected override void OnUpdate()
    {
        var commandBuffer = ecbs.CreateCommandBuffer().AsParallelWriter();
        var bounds = worldBounds;

        Entities.ForEach((in Entity entity, in int entityInQueryIndex, in Translation translation) =>
        {
            // Destroys the entity if outside bounds.
            if (!bounds.Contains(translation.Value))
                commandBuffer.DestroyEntity(entityInQueryIndex, entity);

        }).ScheduleParallel();
        CompleteDependency();
    }
}
