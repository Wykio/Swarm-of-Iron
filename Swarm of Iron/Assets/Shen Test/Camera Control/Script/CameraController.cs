using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform cameraTransform;

    public float normalSpeed;
    public float fastSpeed;
    public float movementSpeed;
    public float movementTimes;
    public float rotationAmount;
    public float ScreenBorder;
    public Vector3 mapLimit;
    public Vector3 zoomAmount;

    public Vector3 newPosition;
    public Quaternion newRotation;
    public Vector3 newZoom;

    // Start is called before the first frame update
    void Start()
    {
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;

    }

    // Update is called once per frame
    void Update()
    {
        HandleMovementInput();
        HandleMouseInput();
    }

    //Using Mouse
    void HandleMouseInput()
    {
        //Zoom with mouse

        if (Input.mouseScrollDelta.y != 0)
        {
            newZoom += Input.mouseScrollDelta.y * zoomAmount;
        }

    //    newZoom.y = Mathf.Clamp(newZoom.y, -mapLimit.z, mapLimit.z);
    //    newZoom.z = Mathf.Clamp(newZoom.z, -mapLimit.z, mapLimit.z);

        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTimes);

   /*     if (newZoom.y >= 1 && newZoom.y <= 15)
        {
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTimes);
        } 
   */

    }

    //Using Keyboard
    void HandleMovementInput()
    {
        //control the speed of camera movement Using Shift Key
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movementSpeed = fastSpeed;
        }
        else
        {
            movementSpeed = normalSpeed;
        }

        //Movement of cam Using Key
        if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow) || Input.mousePosition.y >= Screen.height - ScreenBorder)
        {
            newPosition += (transform.forward * movementSpeed);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || Input.mousePosition.y <= ScreenBorder)
        {
            newPosition += (transform.forward * -movementSpeed);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || Input.mousePosition.x >= Screen.width - ScreenBorder)
        {
            newPosition += (transform.right * movementSpeed);
        }
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow) || Input.mousePosition.x <= ScreenBorder)
        {
            newPosition += (transform.right * -movementSpeed);
        }

        //Rotation of cam Using Key
        if (Input.GetKey(KeyCode.A))
        {
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);

        }
        if (Input.GetKey(KeyCode.E))
        {
            newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);

        }

        //Zoom Using Key
        /*
                if (Input.GetKey(KeyCode.R))
                {
                    newZoom += zoomAmount;
                }
                if (Input.GetKey(KeyCode.F))
                {
                    newZoom -= zoomAmount;
                }
        */

        newPosition.x = Mathf.Clamp(newPosition.x, -mapLimit.x, mapLimit.x);
        newPosition.z = Mathf.Clamp(newPosition.z, -mapLimit.y, mapLimit.y);


        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTimes);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTimes);

    }
}
