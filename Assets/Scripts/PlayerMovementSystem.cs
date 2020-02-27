using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
//using static Unity.Mathematics.math;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
[UpdateAfter(typeof(PlayerInputSystem))]    
public class PlayerMovementSystem : JobComponentSystem
{ 
    [BurstCompile]
    struct MouseRaycastJob : IJobForEach<PlayerInputsComponent>{
        
        public float3 rayStart;
        public float3 rayEnd;
        [NativeDisableParallelForRestriction]    
        [WriteOnly]    
        public NativeArray<float3> result;
        [ReadOnly]
        public PhysicsWorld physicsWorld;
        public void Execute([ReadOnly] ref PlayerInputsComponent inp)
        {
            var input = new RaycastInput
            {
                Start = rayStart,
                End = rayEnd,
                Filter = new CollisionFilter
                {
                    BelongsTo = ~0u,
                    CollidesWith = ~0u,
                    GroupIndex = 0
                }
            };
            Unity.Physics.RaycastHit hit;
            if(physicsWorld.CastRay(input, out hit))
            {
                result[0] = hit.Position;
            }
        }
    }


    [BurstCompile][RequireComponentTag(typeof(PlayerTagComponent))]
    struct PlayerMovementSystemJob : IJobForEach<PlayerInputsComponent, PhysicsVelocity, Rotation, PlayerMovementStats, Translation>
    {
        public float deltaTime;
        public float3 hitPosition;
        public void Execute([ReadOnly] ref PlayerInputsComponent inputs, ref PhysicsVelocity velocity, ref Rotation rotation, [ReadOnly] ref PlayerMovementStats stats, [ReadOnly] ref Translation translation)
        {
            velocity = new PhysicsVelocity{
                Linear = new float3(inputs.MovementInput.x, 0.0f, inputs.MovementInput.y) * stats.MovementSpeed,
                Angular = float3.zero
            };
            var quat = math.slerp(rotation.Value, quaternion.LookRotationSafe(inputs.Orientation, math.up()), 1f - math.exp(-stats.RotationSharpness * deltaTime));
            
            rotation.Value = quat; 
            
        }
    }
    [BurstCompile][RequireComponentTag(typeof(PlayerTagComponent))]
    struct RaycastMouseHitJob : IJobForEach<PlayerInputsComponent, Rotation, PlayerMovementStats, Translation>
    {
        [ReadOnly] public PhysicsWorld physicsWorld;
        public RaycastInput input;
        //[WriteOnly] public NativeArray<float3> result;
        public void Execute(ref PlayerInputsComponent inputs, ref Rotation rot, ref PlayerMovementStats movement, ref Translation translate)
        {
            if(physicsWorld.CastRay(input, out Unity.Physics.RaycastHit hit))
            {
                inputs.Orientation = math.normalize(new float3(hit.Position.x, translate.Value.y, hit.Position.z) - translate.Value);
                //result[0] = hit.Position;
            }
        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
       
        PhysicsWorld world = World.GetOrCreateSystem<BuildPhysicsWorld>().PhysicsWorld;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastInput input = new RaycastInput{
            Start = ray.origin,
            End = ray.GetPoint(1000),
            Filter = new CollisionFilter{
                BelongsTo = ~0u,
                CollidesWith = ~0u,
                GroupIndex = 0
            }
        };

        var rayJob = new RaycastMouseHitJob();
        rayJob.physicsWorld = world;
        rayJob.input = input;
        //NativeArray<float3> res = new NativeArray<float3>(1, Allocator.TempJob);
        //rayJob.result = res;
        var handle = rayJob.Schedule(this, World.GetOrCreateSystem<BuildPhysicsWorld>().FinalJobHandle);
        handle.Complete();
        var job = new PlayerMovementSystemJob();
        job.deltaTime = Time.DeltaTime;
        // if(res.Length > 0)
        //     job.hitPosition = res[0];
        // else
        // job.hitPosition = new float3(0, 0, 0);
        // Now that the job is set up, schedule it to be run. 
        var h = job.Schedule(this, handle);
        
        //res.Dispose();
        return h;
    }
}