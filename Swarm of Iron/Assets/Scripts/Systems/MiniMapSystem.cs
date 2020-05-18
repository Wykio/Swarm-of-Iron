using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;

namespace Swarm_Of_Iron_namespace {
    public class MiniMapSystem : ComponentSystem {

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

            // Create a texture
            Texture2D tex = new Texture2D(100, 100);
            Color[] colorArray = new Color[tex.width * tex.height];

            for (int x = 0; x < tex.width; x++) {
                for (int y = 0; y < tex.height; y++) {
                    colorArray[x + (y * tex.width)] = new Color(143.0f / 255.0f, 113.0f / 255.0f, 92.0f / 255.0f);
                }
            }

            Entities.WithAnyReadOnly<UnitComponent>().ForEach((Entity entity, ref Translation trans) => {
                int[] coords = ImageHerlpers.ConvertWorldToTexture(trans.Value, tex);
                colorArray[coords[0] + (coords[1] * tex.width)] = EntityManager.HasComponent<UnitSelectedComponent>(entity) ? 
                    Color.green :
                    EntityManager.HasComponent<WorkerComponent>(entity) ? 
                        Color.yellow : Color.blue;
            });

            int[,] outVect = new int[4,2] {
                {0, 0},
                {0, tex.height - 1},
                {tex.width - 1, tex.height - 1},
                {tex.width - 1, 0},
            };

            ImageHerlpers.ConstructCameraCoordonates(outVect, tex);

            for (int i = 0; i < 4; i++) {
                int idx = (i + 1) % 4;
                ImageHerlpers.DrawLine(colorArray, tex, outVect[i, 0], outVect[i, 1], outVect[idx, 0], outVect[idx, 1], Color.white);
            }

            tex.SetPixels(colorArray);
            tex.Apply();

            tex.wrapMode = TextureWrapMode.Clamp;

            Camera.onPostRender += (Camera camera) => {
                // Draws your texture onto the screen using the matrix you just loaded in
                Graphics.DrawTexture(new Rect(15, 15, 155, 155), tex);

                // Pops the matrix that was just loaded, restoring the old matrix
                GL.PopMatrix();
            };
        }
    }
}

