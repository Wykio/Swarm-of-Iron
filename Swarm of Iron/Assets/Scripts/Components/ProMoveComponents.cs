using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace SOI
{
    public struct ProMoveComponents : IComponentData
    {
        public float3 startPosition;
        public float3 endPosition;
        //Don't know if it usefull to creat this file.
    }
}