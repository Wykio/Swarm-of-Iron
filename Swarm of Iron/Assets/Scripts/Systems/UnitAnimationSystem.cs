using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

namespace Swarm_Of_Iron_namespace
{
    public class UnitAnimationSystem : JobComponentSystem
    {
        // This is the right way to write a JobSystem
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            float deltaTime = Time.DeltaTime;

            return Entities.ForEach((ref Translation translation, ref UnitComponent moveSpeedComponent) =>
            {
                translation.Value.y += moveSpeedComponent.animationSpeed * deltaTime;
                if (translation.Value.y > 1.5f)
                {
                    moveSpeedComponent.animationSpeed = -math.abs(moveSpeedComponent.animationSpeed);
                }
                if (translation.Value.y < 1.1f)
                {
                    moveSpeedComponent.animationSpeed = +math.abs(moveSpeedComponent.animationSpeed);
                }
            }).Schedule(inputDeps);
        }
    }
}
