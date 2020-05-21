using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Swarm_Of_Iron_namespace
{
    public class UserInterface {
        struct Button {
            float x, y, width, height;

            public Button(float x, float y, float width, float height) {
                this.x = x;
                this.y = y;
                this.width = width;
                this.height = height;
            }

            public bool Contains (float x, float y) {
                if (this.width <= 0 || this.height <= 0) {
                    return false;
                }

                return (this.x <= x && this.x + this.width >= x && this.y <= y && this.y + this.height >= y);
            }
        } 

        static public bool TryClickInterface(Vector3 pos) {
            for (var i = 0; i < Swarm_Of_Iron.instance.listButtonGO.Count; i++) {
                if (UserInterface.TryClickInterface(pos, Swarm_Of_Iron.instance.listButtonGO[i])) {
                    return true;
                }
            }
            return false;
        }

        static public bool TryClickInterface(Vector3 pos, string name) {
            return UserInterface.TryClickInterface(pos, Swarm_Of_Iron.instance.listButtonGO.Find(el => el.name == name));
        }

        static public bool TryClickInterface(Vector3 pos, GameObject element) {
            var trans = element.GetComponent<RectTransform>();
            Vector3[] v = new Vector3[4];
            trans.GetWorldCorners(v);
            Button button = new Button(v[0].x, v[0].y, v[2].x - v[0].x, v[2].y - v[0].y);
            return button.Contains(pos.x, pos.y);
        }
    }
}
