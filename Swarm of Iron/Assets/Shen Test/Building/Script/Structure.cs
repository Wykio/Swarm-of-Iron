using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour
{
    public Material matOK, matFail, matDefault;
    public bool okay = false;
    public Renderer rend;
    private int objCount;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void FixedUpdate()
    {
        if (objCount > 1)
        {
            rend.material = matFail;
            okay = false;
        }
        else if (objCount <= 1)
        {
            rend.material = matOK;
            okay = true;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        objCount++;
    }
    void OnCollisionExit(Collision col)
    {
        objCount--;
    }

}
