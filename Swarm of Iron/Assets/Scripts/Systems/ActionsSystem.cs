using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using Unity.Jobs;

namespace Swarm_Of_Iron_namespace {
    public class ActionsSystem : ComponentSystem {
        protected override void OnUpdate() {
            // Construct All EntityQuery
            var soldierQuery = new EntityQueryDesc {
                All = new ComponentType[] { typeof(UnitComponent), typeof(UnitSelectedComponent) },
                None = new ComponentType[] { typeof(WorkerComponent) }
            };
            EntityQuery soldierEntityQuery = GetEntityQuery(soldierQuery);

            var workerQuery = new EntityQueryDesc {
                All = new ComponentType[] { typeof(WorkerComponent), typeof(UnitSelectedComponent) }
            };
            EntityQuery workerEntityQuery = GetEntityQuery(workerQuery);

            // Get count of entities for all query
            int soldierNumber = soldierEntityQuery.CalculateEntityCount();
            int workerNumber = workerEntityQuery.CalculateEntityCount();

            List<Texture2D> layers = new List<Texture2D>();
            for (int i = 0; i < Swarm_Of_Iron.instance.layers.Count; i++) {
                Texture2D texture = Swarm_Of_Iron.instance.layers[i];
                if (texture.name == "ArrowIcon.svg" && (workerNumber > 0 || soldierNumber > 0)) {
                    layers.Add(texture);
                } else if (texture.name == "HouseIcon.svg" && workerNumber > 0) {
                    layers.Add(texture);
                }
            }

            Image rend = Swarm_Of_Iron.instance.listButtonGO.Find(el => el.name == "Actions").GetComponent<Image>();

            // Create a texture
            Texture2D tex = new Texture2D(100 , 100);
            Color[] colorArray = new Color[tex.width * tex.height];

            int dimx = 50;
            int dimy = 50;

            int factorx = tex.width / dimx;

            int x = 0, y = 0;
            for (int idx = 0; idx < layers.Count; idx++) {
                int scalex = layers[idx].width / dimx;
                int scaley = layers[idx].height / dimy;
                for (int i = 0; i < dimx; i++) {
                    for (int j = 0; j < dimy; j++) {
                        if (i * scalex < layers[idx].width && j * scaley < layers[idx].height) {
                            int pixelIndex = x + (y * tex.width);
                            Color srcPixel = layers[idx].GetPixel(i * scalex, j * scaley);
                            if (srcPixel.a == 1) {
                                colorArray[pixelIndex] = srcPixel;
                            }
                        }
                        int tmp = (int)Mathf.Floor(idx / factorx);
                        if (y == dimy * (tmp + 1) - 1) {
                            x++;
                            y = dimy * tmp;
                        } else {
                            y++;
                        }
                    }
                    if (x == tex.height - 1) {
                        x = 0;
                    }
                }
            }

            tex.SetPixels(colorArray);
            tex.Apply();

            tex.wrapMode = TextureWrapMode.Clamp;

            // Create a sprite
            Sprite newSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);

            // Assign our procedural sprite
            rend.sprite = newSprite;
        }
    }
}

