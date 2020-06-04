using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Rendering;
using Unity.Jobs;
using Unity.Burst;

namespace Swarm_Of_Iron_namespace {
    public class MiniMapSystem : JobComponentSystem {

        const int width = 100;
        const int height = 100;

        EntityQuery m_MinimapQuery, m_UnitQuery, m_WorkerQuery, m_SelectedQuery;

        [BurstCompile]
        struct ColoringJob : IJobParallelFor
        {
            [ReadOnly] public RenderTexture m_color;
            [ReadOnly] public NativeArray<Translation> m_positions;
            [NativeDisableParallelForRestriction] public NativeArray<RenderTexture> m_results;

            public void Execute(int index)
            {
                int2 coords = MiniMapHelpers.ConvertWorldToTexture(m_positions[index].Value, width, height);
                m_results[coords[0] + (coords[1] * width)] = m_color;
            }
        }

        public static JobHandle Schedule(RenderTexture _color, NativeArray<Translation> _position, NativeArray<RenderTexture> _colors, JobHandle _dependency)
        {
            var job = new ColoringJob()
            {
                m_color = _color,
                m_positions = _position,
                m_results = _colors
            };
            return job.Schedule(_position.Length, 1, _dependency);
        }

        protected override void OnCreate() {
            m_MinimapQuery = GetEntityQuery(typeof(MiniMapComponent));
            m_UnitQuery = GetEntityQuery(new EntityQueryDesc
            {
                None = new ComponentType[] { typeof(WorkerComponent), typeof(UnitSelectedComponent) },
                All = new ComponentType[]{ ComponentType.ReadOnly<UnitComponent>(), ComponentType.ReadOnly<Translation>() }
            });
            m_WorkerQuery = GetEntityQuery(ComponentType.ReadOnly<WorkerComponent>(), ComponentType.ReadOnly<Translation>());
            m_SelectedQuery = GetEntityQuery(ComponentType.ReadOnly<UnitSelectedComponent>(), ComponentType.ReadOnly<Translation>());
        }

        protected override JobHandle OnUpdate(JobHandle dependency) {
            var unitPositions = m_UnitQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
            var workerPositions = m_WorkerQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
            var selectedPositions = m_SelectedQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

            Entity entity = m_MinimapQuery.GetSingletonEntity();
            DynamicBuffer<RenderTexture> buffer = EntityManager.GetBuffer<RenderTexture>(entity);

            NativeArray<RenderTexture> colorArray = new NativeArray<RenderTexture>(width * height, Allocator.TempJob);

            dependency = Job.WithCode(() => {
                for (int x = 0; x < width; x++) {
                    for (int y = 0; y < height; y++) {
                        colorArray[x + (y * width)] = new RenderTexture { Value = new Color(143.0f / 255.0f, 113.0f / 255.0f, 92.0f / 255.0f, 1.0f) };
                    }
                }
            }).Schedule(dependency);

            dependency = Schedule(new RenderTexture { Value = new Color(0, 0, 1, 1) }, unitPositions, colorArray, dependency);
            dependency = Schedule(new RenderTexture { Value = new Color(1, 1, 0, 1) }, workerPositions, colorArray, dependency);
            dependency = Schedule(new RenderTexture { Value = new Color(0, 1, 0, 1) }, selectedPositions, colorArray, dependency);

            NativeArray<int2> outVect = new NativeArray<int2>(4, Allocator.TempJob);
            outVect[0] = new int2(0, 0);
            outVect[1] = new int2(0, height - 1);
            outVect[2] = new int2(width - 1, height - 1);
            outVect[3] = new int2(width - 1, 0);

            MiniMapHelpers.ConstructCameraCoordonates(outVect, width, height);

            dependency = Job.WithCode(() =>
            {
                for (int i = 0; i < 4; i++) {
                    int idx = (i + 1) % 4;
                    MiniMapHelpers.DrawLine(colorArray, width, height, outVect[i][0], outVect[i][1], outVect[idx][0], outVect[idx][1], new RenderTexture { Value = new Color(1, 1, 1, 1) });
                }
            }).Schedule(dependency);

            dependency.Complete();

            dependency = outVect.Dispose(dependency);

            buffer.CopyFrom(colorArray);
            dependency = colorArray.Dispose(dependency);

            dependency = unitPositions.Dispose(dependency);
            dependency = workerPositions.Dispose(dependency);
            dependency = selectedPositions.Dispose(dependency);

            return dependency;
        }
    }
}

// RenderTexture : ISharedComponent     -> GetAllUniqueSharedComponentData<RenderTexture> mais on doit pas update un SharedComponent
// RenderTexture : IComponentData       -> Mais on ne doit pas avoir de tableau
// RenderTexture : IBufferElementData   -> Utilisation de DynamicBuffer