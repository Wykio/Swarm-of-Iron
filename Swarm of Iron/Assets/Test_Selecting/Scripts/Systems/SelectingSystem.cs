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

public class SelectingRenderer : ComponentSystem {

    protected override void OnUpdate() {
        Entities
            .WithAll<PlayerUnitSelect>()
            .ForEach((ref Translation translation) => {
                float3 position = translation.Value - new float3(0.0f, 1.0f, 0.0f);
                Graphics.DrawMesh(
                    Spawn.instance.unitSelectedMesh,
                    position,
                    Quaternion.identity,
                    Spawn.instance.unitSelectedMaterial,
                    0
                );
            });
    }
}