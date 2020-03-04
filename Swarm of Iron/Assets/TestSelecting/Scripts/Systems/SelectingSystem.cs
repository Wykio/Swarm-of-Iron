using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class SelectingSystem : ComponentSystem {

    protected override void OnUpdate() {
        // Action is play when entity is selecting
        Entities
            .WithAll<PlayerUnitSelect>()
            .ForEach((ref Translation translation, ref AABB aabb) => {
                // When update position
                translation.Value.y += 2.0f * Time.DeltaTime;

                // you should update Hitbox
                aabb = new AABB {
                    max = translation.Value + 0.5f,
                    min = translation.Value - 0.5f,
                };
            });
    }
}
