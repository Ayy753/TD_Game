using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GameObject exitGate;

    // Start is called before the first frame update
    void Start()
    {
        exitGate = GameObject.Find("Exit");   
    }

    // Update is called once per frame
    void Update()
    {
        if (exitGate != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, exitGate.transform.position, 1f * Time.deltaTime);
        }
    }
}
