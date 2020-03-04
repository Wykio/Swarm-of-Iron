using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

namespace Swarm_Of_Iron_namespace
{
    public class UnitControlSystem : ComponentSystem
    {
        private float3 startPosition;

        protected override void OnUpdate()
        {
            if (Input.GetMouseButtonDown(0)) {
                // Mouse Pressed
                //startPosition = 
            }
            if (Input.GetMouseButtonUp(0)) {
                // Mouse Released
            }

        }
    }
}