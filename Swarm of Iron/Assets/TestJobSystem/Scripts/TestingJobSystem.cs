using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;

public class TestingJobSystem : MonoBehaviour
{
    [SerializeField] private bool useJobs;

    private void Update() {
        float startTime = Time.realtimeSinceStartup;

        if (useJobs) {

            NativeList<JobHandle> jobHandlesList = new NativeList<JobHandle>(Allocator.Temp);
            // with Job
            for (int i = 0; i < 10; i++) {
                JobHandle jobHandle = ReallyToughTaskJob();
                jobHandlesList.Add(jobHandle);
            }
            JobHandle.CompleteAll(jobHandlesList);
            jobHandlesList.Dispose(); // desaloue la NativeList

        } else {
            // without Job
            // Simulate 10 pathfinding
            for (int i = 0; i < 10; i++) {
                ReallyToughTask();
            }
        }

        Debug.Log(((Time.realtimeSinceStartup - startTime) * 1000.0f) + "ms");
    }

    private void ReallyToughTask() {
        // Represents a tough task like some pathfinding or a really complex calculation
        float value = 0.0f;
        for (int i = 0; i < 50000; i++) {
            value = math.exp10(math.sqrt(value));
        }
    }

    private JobHandle ReallyToughTaskJob() {
        ReallyToughJob job = new ReallyToughJob();
        return job.Schedule();
    }
}

public struct ReallyToughJob : IJob{
    public void Execute() {
        // Represents a tough task like some pathfinding or a really complex calculation
        float value = 0.0f;
        for (int i = 0; i < 50000; i++) {
            value = math.exp10(math.sqrt(value));
        }
    }
}