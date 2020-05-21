/*using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Assertions;

namespace Swarm_Of_Iron_namespace
{
    public class FlowFieldSystem : JobComponentSystem
    {
        const int width = 500;
        const int height = 500;

        const float MAX_VALUE = 15.0f;

        struct Neighbour {
            public int2 position;
            public float distance;
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            float deltatime = Time.DeltaTime;

            NativeArray<float> dijkstraGrid = new NativeArray<float>(width * height, Allocator.TempJob);

            Entities.WithAll<CityHallComponent>().ForEach((ref Translation translation) =>
            {
                int3 pos = math.int3(translation.Value);
                dijkstraGrid[pos[0] + pos[2] * width] = MAX_VALUE;
            });

            //flood fill out from the end point
            Neighbour pathEnd = new Neighbour { position = new int2(0, 0), distance = 0 };
            dijkstraGrid[pathEnd.position[0] + pathEnd.position[1] * width] = 0.0f;

            int toVisitIndex = 0;
            NativeArray<Neighbour> toVisit = new NativeArray<Neighbour>(width * height, Allocator.TempJob);
            toVisit[toVisitIndex++] = pathEnd;

            Job.WithCode(() =>
            {
                //for each node we need to visit, starting with the pathEnd
                for (i = 0; i < toVisit.Length; i++) {
                    Neighbour neighbours = neighboursOf(toVisit[i]);

                    //for each neighbour of this node (only straight line neighbours, not diagonals)
                    for (var j = 0; j < neighbours.Length; j++) {
                        Neighbour n = neighbours[j];

                        //We will only ever visit every node once as we are always visiting nodes in the most efficient order
                        if (dijkstraGrid[n.position[0] + n.position[1] * width] == null) {
                            n.distance = toVisit[i].distance + 1;
                            dijkstraGrid[n.position[0] + n.position[1] * width] = n.distance;
                            IsTrue(toVisitIndex < toVisit.Length);
                            toVisit[toVisitIndex++] = n;
                        }
                    }
                }
            }).Schedule();

            NativeArray<float2> flowField = new NativeArray<float2>(width * height, Allocator.TempJob);

            Job.WithCode(() =>
            {
                for (var x = 0; x < width; x++) {
                    for (var y = 0; y < height; y++) {

                        //Obstacles have no flow value
                        if (dijkstraGrid[x][y] == MAX_VALUE) {
                            continue;
                        }

                        int2 pos = new int2(x, y);
                        NativeArray<Neighbour> neighbours = allNeighboursOf(pos);

                        //Go through all neighbours and find the one with the lowest distance
                        bool found = false;
                        float2 min;
                        float minDist = 0;
                        for (var i = 0; i < neighbours.Length; i++) {
                            float2 n = neighbours[i].position;
                            float dist = dijkstraGrid[n.position[0] + (n.position[1] * width)] - dijkstraGrid[pos[0] + (pos[2] * width)];

                            if (dist < minDist) {
                                found = true;
                                min = n;
                                minDist = dist;
                            }
                        }

                        //If we found a valid neighbour, point in its direction
                        if (found) {
                            flowField[x + (y * width)] = math.normalize(min - pos);
                        }
                    }
                }
            }).Schedule();

            return Entities.ForEach((ref Translation translation, ref MoveToComponent moveTo) => {
                if (moveTo.move) {
                    float reachedPositionDistance = 1.0f;

                    if (math.distance(translation.Value, moveTo.position) > reachedPositionDistance) {
                        //Work out the force to apply to us based on the flow field grid squares we are on.
                        //we apply bilinear interpolation on the 4 grid squares nearest to us to work out our force.
                        // http://en.wikipedia.org/wiki/Bilinear_interpolation#Nonlinear

                        float posX = translation.Value[0];
                        float posZ = translation.Value[2];

                        int3 floor = math.int3(translation.Value);
                        int floorX = floor[0];
                        int floorZ = floor[2];

                        //The 4 weights we'll interpolate, see http://en.wikipedia.org/wiki/File:Bilininterp.png for the coordinates
                        float2 f00 = flowField[floorX][floorZ];
                        float2 f01 = flowField[floorX][floorZ + 1];
                        float2 f10 = flowField[floorX + 1][floorZ];
                        float2 f11 = flowField[floorX + 1][floorZ + 1];

                        //Do the x interpolations
                        float xWeight = posX - floorX;

                        float2 top = f00 * (1 - xWeight) + (f10 * (xWeight));
                        float2 bottom = f01 * (1 - xWeight) + (f11 * (xWeight));

                        //Do the y interpolation
                        float zWeight = posZ - floorZ;

                        //This is now the direction we want to be travelling in (needs to be normalized)
                        float2 direction = math.normalize(top * (1 - zWeight) + (bottom * (zWeight)));


                        //If we are centered on a grid square with no vector this will happen
                        //if (isNaN(direction.length())) {
                        //    return new float2(0, 0);
                        //}

                        //Multiply our direction by speed for our desired speed
                        float2 desiredVelocity = direction * moveTo.moveSpeed;

                        // Far from target position, Move to position
                        moveTo.lastMoveDir = math.float3(desiredVelocity, 0);
                        translation.Value[0] += desiredVelocity[0] * moveTo.moveSpeed * deltatime;
                        translation.Value.z += desiredVelocity[1] * moveTo.moveSpeed * deltatime;
                    } else {
                        // Already there
                        moveTo.move = false;
                    }
                }
            }).Schedule(inputDeps);
        }
    }
}*/