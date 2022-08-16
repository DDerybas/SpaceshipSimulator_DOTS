using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

/// <summary>
/// A system that randomly spawns asteroids over time.
/// </summary>
public partial class AsteroidSpawnSystem : SystemBase
{
    EntityCommandBufferSystem ecbs => 
        World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

    double nextSpawnTime;                       // Asteroid spawn time.
    double period = .2f;                        // Time between spawns. 

    float asteroidMaxRotationSpeed = 15;        // Asteroid maximum rotation speed (Random(.., max)).

    protected override void OnUpdate()
    {
        if (Time.ElapsedTime < nextSpawnTime)
            return;

        nextSpawnTime += period;
        // Creates a command buffer.
        var commandBuffer = ecbs.CreateCommandBuffer();

        var rotationSpeed = asteroidMaxRotationSpeed;
        // Creates a random and sets its seed.
        Random random = new Random((uint)UnityEngine.Random.Range(uint.MinValue, uint.MaxValue));

        // Loops through all the asteroid data that placed in the entity container in the scene.
        Entities.WithAll<AsteriodData>().ForEach((in AsteriodData data) =>
        {
            // Chance to spawn an asteroid.
            float spawnChance = random.NextFloat(0, 100);
            if (spawnChance > 50)
                return;

            // Creates the asteroid entity from the asteroid data.
            Entity asteroidEntity = commandBuffer.Instantiate(data.entity);

            // Gets the random position around Vector.zero
            UnityEngine.Vector3 insideUnitCircleVector = UnityEngine.Random.insideUnitCircle.normalized;
            float3 randomPos = insideUnitCircleVector * random.NextFloat(2, 7);
            
            // Sets the position.
            Translation translation = new Translation { Value = randomPos };
            commandBuffer.SetComponent(asteroidEntity, translation);
            
            // Gets the move data from the asteroid prefab.
            MoveData newMoveData = GetComponent<MoveData>(data.entity);
            // Set the random rotation speed.
            newMoveData.rotationSpeed = random.NextFloat(-rotationSpeed, rotationSpeed);
            commandBuffer.SetComponent(asteroidEntity, newMoveData);
        }).Run();
    }
}
