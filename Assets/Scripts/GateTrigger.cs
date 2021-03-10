using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (OnEnemyReachedGate != null)
        {
            if (collision.tag == "Enemy")
            {
                OnEnemyReachedGate(collision.gameObject);
            }
        }
    }

    public delegate void EnemyReachedExit(GameObject enemy);
    public event EnemyReachedExit OnEnemyReachedGate;
}
