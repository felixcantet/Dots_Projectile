using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;
using Unity.Physics;

[UpdateAfter(typeof(PlayerMovementSystem))]
public class WeaponShootSystem : JobComponentSystem
{
    [BurstCompile]
    struct WeaponShootSystemJob : IJobForEach<WeaponDataComponent, Rotation>
    {
        public float deltaTime;
        [NativeDisableParallelForRestriction]
        public EntityCommandBuffer entityCommandBuffer;

        [ReadOnly][NativeDisableParallelForRestriction]
        public ComponentDataFromEntity<LocalToWorld> translations;
        public void Execute(ref WeaponDataComponent weapon, [ReadOnly] ref Rotation rot)
        {
            weapon.ShootTimer += deltaTime;

            if(weapon.ShootTimer > weapon.ShootRate)
            {
                weapon.ShootTimer -= weapon.ShootRate;
                var e = entityCommandBuffer.Instantiate(weapon.BulletPrefab);
                var localToWorld = translations[weapon.shootPosition];
                entityCommandBuffer.SetComponent<Translation>(e, new Translation
                {
                    Value = localToWorld.Position
                });
                entityCommandBuffer.SetComponent<PhysicsVelocity>(e, new PhysicsVelocity
                {
                    Linear = localToWorld.Forward * 100
                });
                entityCommandBuffer.SetComponent<Rotation>(e, new Rotation{
                    Value = localToWorld.Rotation
                });
            }   
        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new WeaponShootSystemJob();
        job.deltaTime = Time.DeltaTime;
        var ecb = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer();
        job.entityCommandBuffer = ecb;
        var trans = GetComponentDataFromEntity<LocalToWorld>();
        job.translations = trans;
        var handle = job.Schedule(this, inputDependencies);
        World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>().AddJobHandleForProducer(handle);
        
        return handle;
    }
}