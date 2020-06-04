using Unity.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;

namespace Swarm_Of_Iron_namespace
{
    [UpdateAfter(typeof(MiniMapSystem))]
    public class RenderTextureSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Camera.onPostRender = null;

            Camera.onPostRender += (Camera camera) =>
            {
                // Pushes the current matrix onto the stack so that can be restored later
                GL.PushMatrix();

                // Loads a new Projection Matrix, you can also use other methods like LoadOrtho() or GL.LoadPixelMatrix()
                //GL.LoadProjectionMatrix(Matrix4x4.Perspective(90, camera.aspect, -10f, 10f));
                GL.LoadPixelMatrix(0, Screen.width, 0, Screen.height);

                // You can also multiply the current matrix in order to do things like translation, rotation and scaling
                // Here I'm rotating and scaling up the current Matrix
                //GL.MultMatrix(Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 45), new Vector3(2, 2)));
            };

            Entities.ForEach((DynamicBuffer<RenderTexture> buffer, ref RectComponent pos) =>
            {
                if (buffer.Capacity > buffer.Length)
                    buffer.TrimExcess();

                Texture2D tex = new Texture2D(50, 50);
                tex.SetPixels(buffer.Reinterpret<Color>().AsNativeArray().ToArray());
                tex.Apply();

                tex.wrapMode = TextureWrapMode.Clamp;

                RectComponent p = pos;
                Camera.onPostRender += (Camera camera) =>
                {
                    if (tex != null)
                        Graphics.DrawTexture(new Rect(p.x, p.y, p.width, p.height), tex);
                };
            });

            Camera.onPostRender += (Camera camera) =>
            {
                // Pops the matrix that was just loaded, restoring the old matrix
                GL.PopMatrix();
            };
        }
    }
}

