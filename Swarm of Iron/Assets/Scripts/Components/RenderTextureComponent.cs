using Unity.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;

namespace SOI
{
    public struct RenderTexture : IBufferElementData
    {
        // These implicit conversions are optional, but can help reduce typing.
        public static implicit operator Color(RenderTexture e) { return e.Value; }
        public static implicit operator RenderTexture(Color e) { return new RenderTexture { Value = e }; }

        // Actual value each buffer element will store.
        public Color Value;

    }
}