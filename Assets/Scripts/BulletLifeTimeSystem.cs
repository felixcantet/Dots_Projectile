using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class BulletLifeTimeSystem : JobComponentSystem
{
    // This declares a new kind of job, which is a unit of work to do.
    // The job is declared as an IJobForEach<Translation, Rotation>,
    // meaning it will process all entities in the world that have both
    // Translation and Rotation components. Change it to process the component
    // types you want.
    //
    // The job is also tagged with the BurstCompile attribute, which means
    // that the Burst compiler will optimize it for the best performance.
    [BurstCompile][RequireComponentTag(typeof(BulletTagComponent))]
    struct BulletLifeTimeSystemJob : IJobForEachWithEntity<BulletLifeTimeComponent>
    {
        [NativeDisableParallelForRestriction]
        public EntityCommandBuffer.Concurrent ecb;

        public float deltaTime;
        public void Execute(Entity e, int entityInQueryIndex, ref BulletLifeTimeComponent bulletLifeTime)
        {
            bulletLifeTime.currentLifeTime += deltaTime;
            
            if(bulletLifeTime.currentLifeTime > bulletLifeTime.LifeTime)
            {
                ecb.DestroyEntity(entityInQueryIndex, e);
            }
        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        var job = new BulletLifeTimeSystemJob();
        job.ecb = commandBufferSystem.CreateCommandBuffer().ToConcurrent();
        job.deltaTime = Time.DeltaTime;
        var handle =  job.Schedule(this, inputDependencies);
        commandBufferSystem.AddJobHandleForProducer(handle);
        return handle;
    }
}