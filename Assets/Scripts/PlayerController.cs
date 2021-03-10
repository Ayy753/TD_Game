using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A temp character controller for testing tilemap collision
/// </summary>
public class PlayerController : MonoBehaviour
{
    float horizontalMove;
    float verticalMove;
    public float runSpeed = 1f;

    Rigidbody2D rb;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        verticalMove = Input.GetAxisRaw("Vertical") * runSpeed;
        rb.MovePosition(new Vector2(transform.position.x + horizontalMove * runSpeed * Time.deltaTime, transform.position.y + verticalMove * runSpeed * Time.deltaTime));
    }
}
