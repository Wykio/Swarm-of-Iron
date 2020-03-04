using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;

public class TestingJobSystem : MonoBehaviour
{
    [SerializeField] private int soldierNumber = 10;
    [SerializeField] private bool useJobs;
    // Parameters for IJobParallelFor
    [SerializeField] private Transform pfSoldier;
    private List<Soldier> soldierList;

    public class Soldier {
        public Transform transform;
        public float moveY;
    }

    private void Start() {
        soldierList = new List<Soldier>();
        for (int i = 0; i < soldierNumber; i++) {
            Transform soldierTransform = Instantiate(pfSoldier, new Vector3(UnityEngine.Random.Range(-8.0f, 8.0f), UnityEngine.Random.Range(-5.0f, 5.0f)), Quaternion.identity);

            soldierList.Add(new Soldier {
                transform = soldierTransform,
                moveY = UnityEngine.Random.Range(1.0f, 2.0f)
            });
        }
    }

    private void Update() {
        float startTime = Time.realtimeSinceStartup;
        if (useJobs) {
            //NativeArray<float3> positionArray = new NativeArray<float3>(soldierList.Count, Allocator.TempJob);
            NativeArray<float> moveYArray = new NativeArray<float>(soldierList.Count, Allocator.TempJob);
            TransformAccessArray transformAccessArray = new TransformAccessArray(soldierList.Count);

            for (int i = 0; i < soldierList.Count; i++) {
                //positionArray[i] = soldierList[i].transform.position;
                moveYArray[i] = soldierList[i].moveY;
                transformAccessArray.Add(soldierList[i].transform);
            }
            /*
            ReallyToughParallelJob reallyToughParallelJob = new ReallyToughParallelJob {
                deltaTime = Time.deltaTime,
                positionArray = positionArray,
                moveYArray = moveYArray
            };

            JobHandle jobHandle = reallyToughParallelJob.Schedule(soldierList.Count, soldierNumber / 10);
            jobHandle.Complete();
            */
            ReallyToughParallelJobTransforms reallyToughParallelJobTransforms = new ReallyToughParallelJobTransforms {
                deltaTime = Time.deltaTime,
                //positionArray = positionArray,
                moveYArray = moveYArray
            };

            JobHandle jobHandle = reallyToughParallelJobTransforms.Schedule(transformAccessArray);
            jobHandle.Complete();

            for (int i = 0; i < soldierList.Count; i++) {
                //soldierList[i].transform.position = positionArray[i];
                soldierList[i].moveY = moveYArray[i];
            }

            //positionArray.Dispose();
            moveYArray.Dispose();
            transformAccessArray.Dispose();
        } else {
            foreach (Soldier soldier in soldierList)
            {
                soldier.transform.position += new Vector3(0, soldier.moveY * Time.deltaTime);
                if (soldier.transform.position.y > 5.0f)
                {
                    soldier.moveY = -math.abs(soldier.moveY);
                }
                if (soldier.transform.position.y < -5.0f)
                {
                    soldier.moveY = +math.abs(soldier.moveY);
                }
                ReallyToughTask();
            }
        }
        /*
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
        */
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

[BurstCompile]
public struct ReallyToughJob : IJob {
    public void Execute() {
        // Represents a tough task like some pathfinding or a really complex calculation
        float value = 0.0f;
        for (int i = 0; i < 50000; i++) {
            value = math.exp10(math.sqrt(value));
        }
    }
}


[BurstCompile]
public struct ReallyToughParallelJob : IJobParallelFor {
    public NativeArray<float3> positionArray;
    public NativeArray<float> moveYArray;
    public float deltaTime;

    public void Execute(int index) {
        positionArray[index] += new float3(0, moveYArray[index] * deltaTime, 0);
        if (positionArray[index].y > 5.0f)
        {
            moveYArray[index] = -math.abs(moveYArray[index]);
        }
        if (positionArray[index].y < -5.0f)
        {
            moveYArray[index] = +math.abs(moveYArray[index]);
        }
        float value = 0.0f;
        for (int i = 0; i < 50000; i++)
        {
            value = math.exp10(math.sqrt(value));
        }
    }
}

[BurstCompile]
public struct ReallyToughParallelJobTransforms : IJobParallelForTransform
{
    public NativeArray<float> moveYArray;
    public float deltaTime;

    public void Execute(int index, TransformAccess transform)
    {
        transform.position += new Vector3(0, moveYArray[index] * deltaTime, 0);
        if (transform.position.y > 5.0f)
        {
            moveYArray[index] = -math.abs(moveYArray[index]);
        }
        if (transform.position.y < -5.0f)
        {
            moveYArray[index] = +math.abs(moveYArray[index]);
        }
        float value = 0.0f;
        for (int i = 0; i < 50000; i++)
        {
            value = math.exp10(math.sqrt(value));
        }
    }
}