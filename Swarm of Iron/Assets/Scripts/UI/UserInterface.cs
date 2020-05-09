using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private List<Button> listButton;

    public UserInterface(List<GameObject> listButtonGO) {
        listButton = new List<Button>();

        listButtonGO.ForEach((GameObject element) => {
            var trans = element.GetComponent<RectTransform>();
            Vector3[] v = new Vector3[4];
            trans.GetWorldCorners(v);
            listButton.Add(new Button(v[0].x, v[0].y, v[2].x - v[0].x, v[2].y - v[0].y));
        });
    }

    public bool TryClickInterface(Vector3 pos) {
        for (var i = 0; i < listButton.Count; i++) {
            if (listButton[i].Contains(pos.x, pos.y)) {
                return true;    
            }
        }
        return false;
    }
}
