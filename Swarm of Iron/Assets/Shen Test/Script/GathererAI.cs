using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Swarm_Of_Iron_namespace
{

    public class GathererAI : MonoBehaviour
    {
        [SerializeField] private Transform goldNodeTransform;
        [SerializeField] private Transform storageTransform;

        //private Worker worker;

        private void Awake()
        {
            //worker = gameObject.GetComponent<Worker>();
/*
            worker.MoveTo(goldNodeTransform.position, 10f, () =>
             {
                 worker.MoveTo(storageTransform.position, 5f, null);
             });

    */
        }




    }
}