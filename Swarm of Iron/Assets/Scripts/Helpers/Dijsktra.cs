using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;
using Unity.Transforms;

namespace SOI {
    public class Dijkstra {

        public struct Neighbour {
            public int2 position;
            public int distance;
        }

        [BurstCompile]
        struct InitDijkstraGridJob : IJobForEach<Translation> {
            [ReadOnly] public int _max, _width, _height;
            [NativeDisableParallelForRestriction] public NativeArray<int> _dijkstraGrid;
            public void Execute([ReadOnly] ref Translation translation) {
                int2 pos = MiniMapHelpers.ConvertWorldCoord(translation.Value, _width, _height);
                _dijkstraGrid[pos[0] + (pos[1] * _width)] = _max;
            }
        }

        public static JobHandle Construct(NativeArray<int> dijkstraGridBase, int _width, int _height, int _max, EntityQuery _query, JobHandle dependency) {
            for (var i = 0; i < dijkstraGridBase.Length; i++) dijkstraGridBase[i] = -1;

            return new InitDijkstraGridJob() {
                _max = _max,
                _width = _width,
                _height = _height,
                _dijkstraGrid = dijkstraGridBase
            }.Schedule(_query, dependency);
        }

        public static void Explore(NativeArray<int> dijkstraGrid, int2 target, int _width, int _height) {
            Neighbour pathEnd = new Neighbour { position = target, distance = 0 };
            dijkstraGrid[target[0] + (target[1] * _width)] = 0;

            int toVisitIndex = 0;
            NativeArray<Neighbour> toVisit = new NativeArray<Neighbour>(_width * _height, Allocator.Temp);
            toVisit[toVisitIndex++] = pathEnd;

            for (var i = 0; i < toVisit.Length; i++) {
                NativeArray<int2> neighbours = new NativeArray<int2>(4, Allocator.Temp);
                straightNeighboursOf(toVisit[i].position, _width, neighbours);

                for (var j = 0; j < neighbours.Length; j++) {
                    int2 n = neighbours[j];

                    var dist = toVisit[i].distance + 1;
                    if (dijkstraGrid[n[0] + (n[1] * _width)] == -1) {
                        dijkstraGrid[n[0] + (n[1] * _width)] = dist;
                        toVisit[toVisitIndex++] = new Neighbour { position = n, distance = dist };
                    }
                }
                neighbours.Dispose();
            }
            toVisit.Dispose();
        }

        private static void straightNeighboursOf(int2 pos, int size, NativeArray<int2> res)
        {
            var index = 0;

            if (pos[0] > 0) res[index++] = new int2(pos[0] - 1, pos[1]);
            if (pos[1] > 0) res[index++] = new int2(pos[0], pos[1] - 1);

            if (pos[0] < size - 1) res[index++] = new int2(pos[0] + 1, pos[1]);
            if (pos[1] < size - 1) res[index++] = new int2(pos[0], pos[1] + 1);

            res = res.GetSubArray(0, index);
        }
    }
}
