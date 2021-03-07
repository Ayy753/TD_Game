using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float horizontalMove;
    float verticalMove;
    public float runSpeed = 1f;

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        verticalMove = Input.GetAxisRaw("Vertical") * runSpeed;

//        gameObject.transform.position = new Vector3(transform.position.x + horizontalMove*runSpeed*Time.deltaTime, transform.position.y + verticalMove * runSpeed * Time.deltaTime, 0);

        rb.MovePosition(new Vector2(transform.position.x + horizontalMove * runSpeed * Time.deltaTime, transform.position.y + verticalMove * runSpeed * Time.deltaTime));

    }
}
