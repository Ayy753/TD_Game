using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private int panLeft;
    private int panDown;
    private int panRight;
    private int panUp;

    private float PanSpeed = 25f;

    void Start()
    {
        panLeft = Mathf.FloorToInt(Screen.width * 0.05f);
        panDown = Mathf.FloorToInt(Screen.height * 0.05f);
        panRight = Mathf.FloorToInt(Screen.width * 0.95f);
        panUp = Mathf.FloorToInt(Screen.height * 0.95f);
    }

    void Update()
    {
        Vector3 mousePos = Input.mousePosition;

        //if ((mousePos.x < panLeft && mousePos.x > 0) || Input.GetAxis("Horizontal") < 0)
        //{
        //    transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.left, PanSpeed*Time.deltaTime);

        //}
        //else if ((mousePos.x > panRight && mousePos.x < Screen.width) || Input.GetAxis("Horizontal") > 0)
        //{
        //    transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.right, PanSpeed * Time.deltaTime);
        //}

        //if ((mousePos.y < panDown && mousePos.y > 0 )|| Input.GetAxis("Vertical") < 0)
        //{
        //    transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.down, PanSpeed * Time.deltaTime);
        //}
        //else if ((mousePos.y > panUp && mousePos.y < Screen.height) || Input.GetAxis("Vertical") > 0)
        //{
        //    transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.up, PanSpeed * Time.deltaTime);
        //}


        if (Input.GetAxis("Horizontal") < 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.left, PanSpeed * Time.deltaTime);

        }
        else if (Input.GetAxis("Horizontal") > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.right, PanSpeed * Time.deltaTime);
        }

        if (Input.GetAxis("Vertical") < 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.down, PanSpeed * Time.deltaTime);
        }
        else if (Input.GetAxis("Vertical") > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.up, PanSpeed * Time.deltaTime);
        }

        //  Scroll wheel zoom
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + Input.mouseScrollDelta.y * -5f, 5, 75);
    }
}
