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
        [ReadOnly] public int m_width, m_height, MAX_VALUE;
        [ReadOnly] public NativeArray<int> dijkstraGridBase;

        public struct Neighbour
        {
            public int2 position;
            public int distance;
        }

        public void Execute(ref Translation translation, ref MoveToComponent moveTo)
        {
            if (moveTo.move)
            {
                int2 coords = MiniMapHelpers.ConvertWorldCoord(translation.Value, m_width, m_height);
                int2 target = MiniMapHelpers.ConvertWorldCoord(moveTo.position, m_width, m_height);

                if (target[0] != coords[0] || target[1] != coords[1])
                {

                    NativeArray<int> dijkstraGrid = new NativeArray<int>(dijkstraGridBase, Allocator.Temp);
                    NativeArray<int2> neighbours = new NativeArray<int2>(4, Allocator.Temp);

                    /* STEP 2 - Explore all node to construct Dijkstra Grid */

                    //flood fill out from the end point
                    Neighbour pathEnd = new Neighbour { position = target, distance = 0 };
                    dijkstraGrid[target[0] + (target[1] * m_width)] = 0;

                    int toVisitIndex = 0;
                    NativeArray<Neighbour> toVisit = new NativeArray<Neighbour>(m_width * m_height, Allocator.Temp);
                    toVisit[toVisitIndex++] = pathEnd;

                    //for each node we need to visit, starting with the pathEnd
                    for (var i = 0; i < toVisit.Length; i++)
                    {
                        FlowFieldSystem.straightNeighboursOf(toVisit[i].position, m_width, neighbours);

                        //for each neighbour of this node (only straight line neighbours, not diagonals)
                        for (var j = 0; j < neighbours.Length; j++)
                        {
                            int2 n = neighbours[j];

                            //We will only ever visit every node once as we are always visiting nodes in the most efficient order
                            var dist = toVisit[i].distance + 1;
                            if (dijkstraGrid[n[0] + (n[1] * m_width)] == -1 || dijkstraGrid[n[0] + (n[1] * m_width)] > dist)
                            {
                                dijkstraGrid[n[0] + (n[1] * m_width)] = dist;
                                toVisit[toVisitIndex++] = new Neighbour { position = n, distance = dist };
                            }
                        }
                    }
                    toVisit.Dispose();

                    /* STEP 2.5 - DEBUG DIJKSTRA */

                    // JobHandle four = Entities
                    //     .WithAll<MiniMapComponent>()
                    //     .ForEach((DynamicBuffer<RenderTexture> buffer) =>
                    //     {
                    //         NativeArray<RenderTexture> colorArray = new NativeArray<RenderTexture>(m_width * m_height, Allocator.Temp);
                    //         Color color;

                    //         for (var x = 0; x < m_width; x++)
                    //         {
                    //             for (var y = 0; y < m_width; y++)
                    //             {
                    //                 float val = dijkstraGrid[x + y * m_width];

                    //                 if (dijkstraGrid[x + y * m_width] < 0) color = new Color(0.0f, 0.0f, 1.0f, 1.0f);
                    //                 else {
                    //                   var tmp = val / 50;
                    //                   if (MAX_VALUE <= val) color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
                    //                   else color = new Color(tmp, 1.0f - tmp, 0.0f, 1.0f);
                    //                 }
                    //                 colorArray[x + y * m_width] = new RenderTexture { Value = color };
                    //             }
                    //         }

                    //         buffer.CopyFrom(colorArray);
                    //         colorArray.Dispose();
                    // }).Schedule(Two);
                    // four.Complete();

                    /* STEP 3 - With Dijkstra Grid construct FlowField (array of dir vector) */

                    NativeArray<float2> flowField = new NativeArray<float2>(m_width * m_height, Allocator.Temp);
                    neighbours = new NativeArray<int2>(8, Allocator.Temp);

                    int marg = 5;
                    int padd = 4;
                    int baseX = (int)math.floor(coords[0] / marg) * marg;
                    int baseY = (int)math.floor(coords[1] / marg) * marg;
                    for (var x = baseX - padd; x < baseX + marg + padd; x++) {
                        for (var y = baseY - padd; y < baseY + marg + padd; y++) {
                            int index = x + y * m_width;
                            flowField[index] = new float2(0, 0);

                            //Obstacles have no flow value
                            if (dijkstraGrid[index] != MAX_VALUE)
                            {
                                int2 pos = new int2(x, y);
                                FlowFieldSystem.allNeighboursOf(pos, baseX - padd, baseY - padd, marg + (padd * 2), neighbours);

                                //Go through all neighbours and find the one with the lowest distance
                                int2 min = new int2(0, 0);
                                float minDist = MAX_VALUE;
                                for (var j = 0; j < neighbours.Length; j++)
                                {
                                    int2 n = neighbours[j];
                                    float dist = dijkstraGrid[n[0] + (n[1] * m_width)] - dijkstraGrid[index];

                                    if (dist < minDist)
                                    {
                                        min = n;
                                        minDist = dist;
                                    }
                                }

                                //If we found a valid neighbour, point in its direction
                                if (minDist < MAX_VALUE)
                                {
                                    flowField[index] = math.normalize(min - pos);
                                }
                            }
                        }
                    }
                    neighbours.Dispose();

                    flowField[target[0] + (target[1] * m_width)] = new float2(0.0f, 0.0f);

                    dijkstraGrid.Dispose();

                    /* STEP 3.5 - DEBUG FLOWFIELD */

                    // for (var i = 0; i < m_width; i++) {
                    //     for (var j = 0; j < m_height; j++) {
                    //         float x = ((i * 500) / 100) - 250;
                    //         float z = (((j) * 500) / 100) - 250;

                    //         float2 dir = flowField[i + j * m_width];
                    //         Vector3 direction = new Vector3(dir[0], 0, dir[1]);
                    //         Debug.DrawRay(new Vector3(x, 5, z), direction, Color.green);
                    //         Debug.DrawRay(new Vector3(x + dir[0], 5, z + dir[1]), direction, Color.red);
                    //     }
                    // }

                    /* STEP 4 -  */

                    //Work out the force to apply to us based on the flow field grid squares we are on.
                    //we apply bilinear interpolation on the 4 grid squares nearest to us to work out our force.
                    // http://en.wikipedia.org/wiki/Bilinear_interpolation#Nonlinear

                    float posX = translation.Value.x;
                    float posZ = translation.Value.z;

                    int floorX = coords[0];
                    int floorZ = coords[1];

                    //The 4 weights we'll interpolate, see http://en.wikipedia.org/wiki/File:Bilininterp.png for the coordinates
                    // float2 f00 = flowField[floorX + floorZ * m_width];
                    // float2 f01 = flowField[floorX + (floorZ + 1) * m_width];
                    // float2 f10 = flowField[(floorX + 1) + floorZ * m_width];
                    // float2 f11 = flowField[(floorX + 1) + (floorZ + 1) * m_width];

                    float2 f00 = flowField[(floorX + 1) + floorZ * m_width];
                    float2 f01 = flowField[floorX + (floorZ + 1) * m_width];
                    float2 f10 = flowField[(floorX - 1) + floorZ * m_width];
                    float2 f11 = flowField[floorX + (floorZ - 1) * m_width];

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

                    flowField.Dispose();
                }
                else
                {
                    // Already there
                    moveTo.move = false;
                }
            }
        }
    }
}