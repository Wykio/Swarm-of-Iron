using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Physics;

namespace Swarm_Of_Iron_namespace {
    public class DijkstraHelper {

        public struct Neighbour {
            public int2 position;
            public int distance;
        }

        public static JobHandle Construct(NativeArray<int> dijkstraGrid, [ReadOnly] EntityQuery m_rock, [ReadOnly] JobHandle deps,
          [ReadOnly] int width, [ReadOnly] int height, [ReadOnly] int MAX_VALUE) {
            for (var i = 0; i < dijkstraGrid.Length; i++) {
                dijkstraGrid[i] = -1;
            }
            
            var job_initDijkstraGrid = new InitDijkstraGridJob() {
                m_maxValue = MAX_VALUE,
                m_width = width,
                m_height = height,
                m_dijkstraGrid = dijkstraGrid
            };
            return job_initDijkstraGrid.Schedule(m_rock, deps);
        }

        public static void Explore(NativeArray<int> dijkstraGrid, [ReadOnly] int2 target, [ReadOnly] int width, [ReadOnly] int height) {
            Neighbour pathEnd = new Neighbour { position = target, distance = 0 };
            dijkstraGrid[target[0] + (target[1] * width)] = 0;

            int toVisitIndex = 0;
            NativeArray<Neighbour> toVisit = new NativeArray<Neighbour>(width * height, Allocator.Temp);
            toVisit[toVisitIndex++] = pathEnd;

            //for each node we need to visit, starting with the pathEnd
            for (var i = 0; i < toVisit.Length; i++)
            {   
                NativeArray<int2> neighbours = new NativeArray<int2>(4, Allocator.Temp);
                straightNeighboursOf(toVisit[i].position, width, neighbours);

                //for each neighbour of this node (only straight line neighbours, not diagonals)
                for (var j = 0; j < neighbours.Length; j++)
                {
                    int2 n = neighbours[j];

                    //We will only ever visit every node once as we are always visiting nodes in the most efficient order
                    var dist = toVisit[i].distance + 1;
                    if (dijkstraGrid[n[0] + (n[1] * width)] == -1 || dijkstraGrid[n[0] + (n[1] * width)] > dist)
                    {
                        dijkstraGrid[n[0] + (n[1] * width)] = dist;
                        toVisit[toVisitIndex++] = new Neighbour { position = n, distance = dist };
                    }
                }
                neighbours.Dispose();
            }
            toVisit.Dispose();
        }

        public static void straightNeighboursOf(int2 pos, int size, NativeArray<int2> res) {
            var index = 0;

            if (pos[0] > 0) res[index++] = new int2(pos[0] - 1, pos[1]);
            if (pos[1] > 0) res[index++] = new int2(pos[0], pos[1] - 1);

            if (pos[0] < size - 1) res[index++] = new int2(pos[0] + 1, pos[1]);
            if (pos[1] < size - 1) res[index++] = new int2(pos[0], pos[1] + 1);

            res = res.GetSubArray(0, index);
        }
    }
}
