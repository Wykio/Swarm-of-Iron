using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Physics;

namespace Swarm_Of_Iron_namespace {
    public class RaycastHelper {
        public struct RaycastJob : IJobParallelFor {

            [ReadOnly] public CollisionWorld m_collisionWorld;
            [ReadOnly] public NativeArray<RaycastInput> m_raycastInputs;

            public NativeArray<RaycastHit> m_results;

            public void Execute (int index) {
                m_collisionWorld.CastRay(m_raycastInputs[index], out var hit);
                m_results[index] = hit;
            }
        }

        public static JobHandle ScheduleRaycast(CollisionWorld _collisionWorld, NativeArray<RaycastInput> _raycastInputs, NativeArray<RaycastHit> _raycastHits) {
            var raycastJob = new RaycastJob() {
                m_collisionWorld = _collisionWorld,
                m_raycastInputs = _raycastInputs,
                m_results = _raycastHits
            };

            JobHandle jobHandle = raycastJob.Schedule(_raycastInputs.Length, 1);
            return jobHandle;
        }

        public static void SingleRaycast(CollisionWorld _collisionWorld, RaycastInput _raycastInput, ref RaycastHit _raycastHit) {
            var raycastInputs = new NativeArray<RaycastInput>(1, Allocator.TempJob);
            var raycastHits = new NativeArray<RaycastHit>(1, Allocator.TempJob);

            raycastInputs[0] = _raycastInput;

            JobHandle jobHandle = ScheduleRaycast(_collisionWorld, raycastInputs, raycastHits);
            jobHandle.Complete();

            _raycastHit = raycastHits[0];

            raycastInputs.Dispose();
            raycastHits.Dispose(); 
        }
    }
}
