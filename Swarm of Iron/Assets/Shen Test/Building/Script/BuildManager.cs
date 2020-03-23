using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuildManager_namespace
{

    public class BuildManager : MonoBehaviour
    {
        public enum CursorState { Building, Rotating }

        public CursorState state = CursorState.Building;
        public Transform VirtualObj;
        public GameObject RealObj;
        public LayerMask mask;
        GameObject builtObject;
        float LastPosX, LastPosY, LastPosZ;
        Vector3 mousePos;
    //    Structure structure;


        // Update is called once per frame
        void Update()
        {
            mousePos = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
            {
                float PosX = hit.point.x;
                float PosY = hit.point.y;
                float PosZ = hit.point.z;

                if (PosX != LastPosX || PosY != LastPosY || PosZ != LastPosZ)
                {
                    LastPosX = PosX;
                    LastPosY = PosY;
                    LastPosZ = PosZ;
                    VirtualObj.position = new Vector3(LastPosX, .5f, LastPosZ ); // LastPosY - .5f == or .5f
                }
                if (Input.GetMouseButton(1))
                {
                    if (state == CursorState.Building)
                    {
                        GameObject building = (GameObject)Instantiate(RealObj, VirtualObj.position, Quaternion.identity);
                        builtObject = building;
                        state = CursorState.Rotating;
                    }
                    if (state == CursorState.Rotating)
                    {
                        builtObject.transform.LookAt(new Vector3(LastPosX, builtObject.transform.position.y, LastPosZ));
                    }
                }
                if (Input.GetMouseButtonUp(1))
                {
                    builtObject = null;
                    state = CursorState.Building;
                }
            }
        }
    }
}