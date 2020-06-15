using Unity.Entities;
using Unity.Mathematics;

namespace SOI
{
    public struct PathPosition : IBufferElementData {
        public static implicit operator int2(PathPosition e) { return e.position; }
        public static implicit operator PathPosition(int2 e) { return new PathPosition { position = e }; }
        public int2 position;
    }
}
