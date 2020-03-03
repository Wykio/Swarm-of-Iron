using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class TestingJobSystem : MonoBehaviour
{
    private void Update()
    {
        float startTime = Time.realtimeSinceStartup;
        ReallyToughTask();
        Debug.Log(((Time.realtimeSinceStartup - startTime) * 1000.0f) + "ms");
    }

    private void ReallyToughTask()
    {
        // Represents a tough task like some pathfinding or a really complex calculation
        float value = 0.0f;
        for (int i = 0; i < 50000; i++) {
            value = math.exp10(math.sqrt(value));
        }
    }
}
