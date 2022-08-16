using Unity.Entities;
using Unity.Transforms;
using UnityEngine.InputSystem;

/// <summary>
/// System that spawns player bullets.
/// </summary>
public partial class SpawnBulletSystem : SystemBase
{
    private Entity player;                                  // The Player entity.
    private InputActions inputActions;                      // Unity input system input actions.

    private bool isShooting;                                // Is shoot button pressed?

    private double nextSpawnTime;                           // Bullet spawn time.
    private double period = .05f;                           // Period between bullet spawn.

    private BulletData bulletData;                          // Bullet data component.

    private EntityCommandBufferSystem ecbs => 
        World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

    protected override void OnCreate()
    {
        base.OnCreate();

        // Subscribing to fire input event.
        inputActions = new InputActions();
        inputActions.Player.Fire.Enable();
        inputActions.Player.Fire.performed += Fire_performed;
        inputActions.Player.Fire.canceled += Fire_performed;
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();

        player = GetSingletonEntity<PlayerTag>();
        bulletData = GetSingleton<BulletData>();
    }

    // On fire button pressed/released.
    private void Fire_performed(InputAction.CallbackContext obj) => isShooting = obj.ReadValueAsButton();

    protected override void OnUpdate()
    {
        // Shooting cooldown timer.
        if (Time.ElapsedTime < nextSpawnTime || !isShooting)
            return;

        // Gets the bullet prefab from bullet data.
        var bulletPrefab = bulletData.entity;
        int entityInQueryIndex = 0;
        // Creates the command buffer.
        var commandBuffer = ecbs.CreateCommandBuffer().AsParallelWriter();
        var playerRotation = GetComponent<Rotation>(player);

        // Looping through all the shoot points (Parallel).
        Entities.WithAll<ShootPointData>().ForEach(
        (in LocalToWorld localToWorld) =>
        {
            // Instantiates the bullet entity.
            Entity bulletEntity = commandBuffer.Instantiate(entityInQueryIndex, bulletPrefab);

            // Sets the bullet position to the player shoot point position.
            commandBuffer.SetComponent(entityInQueryIndex, bulletEntity, new Translation { Value = localToWorld.Position });
            // Gets the MoveData for the bullet.
            var moveData = GetComponent<MoveData>(bulletPrefab);
            // Sets the bullet move direction to the shoot point up vector.
            moveData.movement = localToWorld.Up;
            // Sets the MoveData.
            commandBuffer.SetComponent(entityInQueryIndex, bulletEntity, moveData);
            // Applies the bullet position and bullet rotation to the player rotation and shoot point position.
            commandBuffer.SetComponent(entityInQueryIndex, bulletEntity, playerRotation);
        }).ScheduleParallel();
        // Ensures all jobs running on this EntityQuery complete.
        CompleteDependency();

        // Calculates next spawn time.
        nextSpawnTime = Time.ElapsedTime + period;
    }
}
