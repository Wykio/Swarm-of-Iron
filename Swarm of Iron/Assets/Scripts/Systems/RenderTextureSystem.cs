using Unity.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;

namespace SOI {
    [UpdateInGroup(typeof(MiniMapLogicGroup))]
    [UpdateAfter(typeof(MiniMapSystem))]
    public class RenderTextureSystem : ComponentSystem {

        private EntityQuery m_MinimapQuery;

        protected override void OnCreate() {
            m_MinimapQuery = GetEntityQuery(typeof(MiniMapComponent));
        }

        protected override void OnUpdate() {
            Camera.onPostRender = null;

            Camera.onPostRender += (Camera camera) => {
                // Pushes the current matrix onto the stack so that can be restored later
                GL.PushMatrix();

                // Loads a new Projection Matrix, you can also use other methods like LoadOrtho() or GL.LoadPixelMatrix()
                //GL.LoadProjectionMatrix(Matrix4x4.Perspective(90, camera.aspect, -10f, 10f));
                GL.LoadPixelMatrix(0, Screen.width, 0, Screen.height);

                // You can also multiply the current matrix in order to do things like translation, rotation and scaling
                // Here I'm rotating and scaling up the current Matrix
                //GL.MultMatrix(Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 45), new Vector3(2, 2)));
            };

            Entity entity = m_MinimapQuery.GetSingletonEntity();
            DynamicBuffer<RenderTexture> buffer = EntityManager.GetBuffer<RenderTexture>(entity);
            RectComponent pos = EntityManager.GetComponentData<RectComponent>(entity);

            if (buffer.Capacity > buffer.Length)
                buffer.TrimExcess();

            Texture2D tex = new Texture2D(100, 100);
            tex.SetPixels(buffer.Reinterpret<Color>().AsNativeArray().ToArray());
            tex.Apply();

            tex.wrapMode = TextureWrapMode.Clamp;

            Camera.onPostRender += (Camera camera) => {
                if (tex != null)
                    Graphics.DrawTexture(new Rect(pos.x, pos.y, pos.width, pos.height), tex);
            };

            Camera.onPostRender += (Camera camera) => {
                // Pops the matrix that was just loaded, restoring the old matrix
                GL.PopMatrix();
            };
        }
    }
}

