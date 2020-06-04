using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;

namespace Swarm_Of_Iron_namespace
{
    [BurstCompile]
    struct UnitMovingJob : IJobForEach<Translation, MoveToComponent>
    {
        [ReadOnly] public float deltatime;
        [ReadOnly] public int m_width, m_height;
        [ReadOnly] public NativeArray<float2> m_flowFields;

        public void Execute(ref Translation translation, ref MoveToComponent moveTo)
        {
            if (moveTo.move)
            {
                int2 coords = MiniMapHelpers.ConvertWorldCoord(translation.Value, m_width, m_height);

                float posX = translation.Value.x;
                float posZ = translation.Value.z;

                int floorX = coords[0];
                int floorZ = coords[1];

                //The 4 weights we'll interpolate, see http://en.wikipedia.org/wiki/File:Bilininterp.png for the coordinates
                float2 f00 = m_flowFields[(floorX + 1) + floorZ * m_width];
                float2 f01 = m_flowFields[floorX + (floorZ + 1) * m_width];
                float2 f10 = m_flowFields[(floorX - 1) + floorZ * m_width];
                float2 f11 = m_flowFields[floorX + (floorZ - 1) * m_width];

                //Do the x interpolations
                float xWeight = posX - math.floor(posX);

                float2 top = f00 * (1 - xWeight) + (f10 * (xWeight));
                float2 bottom = f01 * (1 - xWeight) + (f11 * (xWeight));

                //Do the y interpolation
                float zWeight = posZ - math.floor(posZ);

                //This is now the direction we want to be travelling in (needs to be normalized)
                float2 direction = math.normalize(top * (1 - zWeight) + (bottom * (zWeight)));

                //Multiply our direction by speed for our desired speed
                float2 desiredVelocity = direction * moveTo.moveSpeed;

                // Far from target position, Move to position
                //moveTo.lastMoveDir = math.float3(desiredVelocity, 0);
                translation.Value.x += desiredVelocity[0] * deltatime;
                translation.Value.z += desiredVelocity[1] * deltatime;
            }
            else
            {
                // Already there
                moveTo.move = false;
            }
        }
    }
}