using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

namespace Swarm_Of_Iron_namespace
{
    public class SoldierAnimationSystem : JobComponentSystem
    {
        // This is the right way to write a JobSystem
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            float deltaTime = Time.DeltaTime;

            return Entities.ForEach((ref Translation translation, ref Soldier moveSpeedComponent) =>
            {
                translation.Value.y += moveSpeedComponent.animationSpeed * deltaTime;
                if (translation.Value.y > 1.4f)
                {
                    moveSpeedComponent.animationSpeed = -math.abs(moveSpeedComponent.animationSpeed);
                }
                if (translation.Value.y < 1.0f)
                {
                    moveSpeedComponent.animationSpeed = +math.abs(moveSpeedComponent.animationSpeed);
                }
            }).Schedule(inputDeps);
        }
    }
}
