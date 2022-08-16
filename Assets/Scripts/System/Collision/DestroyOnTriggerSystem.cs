using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

/// <summary>
/// System that handle objects ontrigger events.
/// </summary>
public partial class DestroyOnTriggerSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem endECBSystem;        // Command buffer.
    private StepPhysicsWorld stepPhysicsWorld;                          

    protected override void OnCreate()
    {
        endECBSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    protected override void OnUpdate()
    {
        // Creates a new trigger job.
        var triggerJob = new TriggerJob
        {
            colliders = GetComponentDataFromEntity<DestroyOnTriggerTag>(true),
            ecb = endECBSystem.CreateCommandBuffer()
        };

        Dependency = triggerJob.Schedule(stepPhysicsWorld.Simulation, Dependency);
        endECBSystem.AddJobHandleForProducer(Dependency);
    }
}

[BurstCompile]
struct TriggerJob : ITriggerEventsJob
{
    [ReadOnly] public ComponentDataFromEntity<DestroyOnTriggerTag> colliders;       // All colliders on scene with DestroyOnTriggerTag.
    public EntityCommandBuffer ecb;

    public void Execute(TriggerEvent triggerEvent)
    {
        Entity entityA = triggerEvent.EntityA;
        Entity entityB = triggerEvent.EntityB;

        // If colliders has other object, removes both.
        if (colliders.HasComponent(entityB))
        {
            ecb.DestroyEntity(entityA);
            ecb.DestroyEntity(entityB);
        }
    }
}
