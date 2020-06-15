using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace SOI
{
    public struct MoveToComponent : IComponentData
    {
        public bool harvest;
        public float3 startPosition;
        public float3 endPosition;
    }
}