using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace Swarm_Of_Iron_namespace
{
    public struct MoveToComponent : IComponentData
    {
        public bool move;
        public bool harvest;
        public float3 targetPosition;
        public float3 position;
        public float3 lastMoveDir;
        public float moveSpeed;
    }
}