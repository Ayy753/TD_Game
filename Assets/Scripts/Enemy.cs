using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GameObject exitGate;

    void Start()
    {
        exitGate = GameManager.Instance.ExitGate;  
    }

    /// <summary>
    /// Just blindly moves towards exit gate
    /// Pathfinding to come later
    /// </summary>
    void Update()
    {
        if (exitGate != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, exitGate.transform.position, 3f * Time.deltaTime);
        }
    }
}
