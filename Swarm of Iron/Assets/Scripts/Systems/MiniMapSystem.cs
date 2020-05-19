using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Rendering;

namespace Swarm_Of_Iron_namespace {
    public class MiniMapSystem : ComponentSystem {

        const int width = 100;
        const int height = 100;

        RenderTexture green = new RenderTexture { Value = new Color(0, 1, 0, 1) };
        RenderTexture yellow = new RenderTexture { Value = new Color(1, 1, 0, 1) };
        RenderTexture blue = new RenderTexture { Value = new Color(0, 0, 1, 1) };
        RenderTexture white = new RenderTexture { Value = new Color(1, 1, 1, 1) };

        EntityQuery m_UnitQuery, m_WorkerQuery, m_SelectedQuery;

        protected override void OnCreate() {
            m_UnitQuery = GetEntityQuery(new EntityQueryDesc
            {
                None = new ComponentType[] { typeof(WorkerComponent), typeof(UnitSelectedComponent) },
                All = new ComponentType[]{ ComponentType.ReadOnly<UnitComponent>(), ComponentType.ReadOnly<Translation>() }
            });
            m_WorkerQuery = GetEntityQuery(ComponentType.ReadOnly<WorkerComponent>(), ComponentType.ReadOnly<Translation>());
            m_SelectedQuery = GetEntityQuery(ComponentType.ReadOnly<UnitSelectedComponent>(), ComponentType.ReadOnly<Translation>());
        }

        protected override void OnUpdate() {
            var unitPositions = m_UnitQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
            var workerPositions = m_WorkerQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
            var selectedPositions = m_SelectedQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

            Entities
                .WithAllReadOnly<MiniMapComponent>()
                .ForEach((DynamicBuffer<RenderTexture> buffer) =>
                {
                    // Create a texture
                    RenderTexture[] colorArray = new RenderTexture[width * height];

                    for (int x = 0; x < width; x++) {
                        for (int y = 0; y < height; y++) {
                            colorArray[x + (y * width)] = new RenderTexture { Value = new Color(143.0f / 255.0f, 113.0f / 255.0f, 92.0f / 255.0f, 1.0f) };
                        }
                    }

                    for (int i = 0; i != unitPositions.Length; i++){
                        int[] coords = MiniMapHelpers.ConvertWorldToTexture(unitPositions[i].Value, width, height);
                        colorArray[coords[0] + (coords[1] * width)] = blue;
                    }

                    for (int i = 0; i != workerPositions.Length; i++){
                        int[] coords = MiniMapHelpers.ConvertWorldToTexture(workerPositions[i].Value, width, height);
                        colorArray[coords[0] + (coords[1] * width)] = yellow;
                    }

                    for (int i = 0; i != selectedPositions.Length; i++){
                        int[] coords = MiniMapHelpers.ConvertWorldToTexture(selectedPositions[i].Value, width, height);
                        colorArray[coords[0] + (coords[1] * width)] = green;
                    }

                    int[,] outVect = new int[4,2] {
                        {0, 0},
                        {0, height - 1},
                        {width - 1, height - 1},
                        {width - 1, 0},
                    };

                    MiniMapHelpers.ConstructCameraCoordonates(outVect, width, height);

                    for (int i = 0; i < 4; i++) {
                        int idx = (i + 1) % 4;
                        MiniMapHelpers.DrawLine(colorArray, width, height, outVect[i, 0], outVect[i, 1], outVect[idx, 0], outVect[idx, 1], white);
                    }

                    buffer.CopyFrom(colorArray);
                });

            unitPositions.Dispose();
            workerPositions.Dispose();
            selectedPositions.Dispose();
        }
    }
}

// RenderTexture : ISharedComponent     -> GetAllUniqueSharedComponentData<RenderTexture> mais on doit pas update un SharedComponent
// RenderTexture : IComponentData       -> Mais on ne doit pas avoir de tableau
// RenderTexture : IBufferElementData   -> Utilisation de DynamicBuffer