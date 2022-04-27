using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public int Speed = 50;
    public LayerMask obstacleMask;
    Camera viewCamera;

    public float sensitivity = 10f;
    public float maxYAngle = 80f;
    private Vector2 currentRotation;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        viewCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        float xAxisValue = Input.GetAxis("Horizontal") * Speed;
        float zAxisValue = Input.GetAxis("Vertical") * Speed;
        float yValue = 0.0f;

        if (Input.GetKey(KeyCode.Q))
        {
            yValue = 30 * Time.deltaTime * -Speed;
        }
        if (Input.GetKey(KeyCode.E))
        {
            yValue = 30 * Time.deltaTime * Speed;
        }

        Vector3 direction = transform.right * xAxisValue + transform.forward * zAxisValue + transform.up * yValue;

        //transform.position = new Vector3(transform.position.x + xAxisValue, transform.position.y + yValue, transform.position.z + zAxisValue);
        if (Physics.Raycast(transform.position, new Vector3(direction.x, 0, 0), 5, obstacleMask))
        {
            direction.x = 0;
        }

        if (Physics.Raycast(transform.position, new Vector3(0, 0, direction.z), 5, obstacleMask))
        {
            direction.z = 0;
        }

        if (Physics.Raycast(transform.position, new Vector3(0, direction.y, 0), 5, obstacleMask))
        {
            direction.y = 0;
        }

        transform.position += direction;


        //transform.LookAt(viewCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, viewCamera.nearClipPlane)), Vector3.up);
        //float mouseX = (Input.mousePosition.x / Screen.width) - 0.5f;
        //float mouseY = (Input.mousePosition.y / Screen.height) - 0.5f;

        //transform.localRotation = Quaternion.Euler(new Vector4(-1f * (mouseY * 180f), mouseX * 360f, transform.localRotation.z));

        currentRotation.x += Input.GetAxis("Mouse X") * sensitivity;
        currentRotation.y -= Input.GetAxis("Mouse Y") * sensitivity;
        currentRotation.x = Mathf.Repeat(currentRotation.x, 360);
        currentRotation.y = Mathf.Clamp(currentRotation.y, -maxYAngle, maxYAngle);
        Camera.main.transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
    }
}
