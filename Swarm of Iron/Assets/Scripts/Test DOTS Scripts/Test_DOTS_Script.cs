using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class Test_DOTS_Script : MonoBehaviour
{
    void Start()
    {
        // Get the world entity manager
        EntityManager entityManager = World.Active.EntityManager;
        // Create an entity
        entityManager.CreateEntity();
        // Create an entity with type array
        entityManager.CreateEntity(typeof(LevelComponent));
    }
}
