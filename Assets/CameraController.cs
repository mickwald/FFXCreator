using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = this.transform.position + this.transform.forward * 0.2f * Input.GetAxis("Vertical");
        this.transform.position = this.transform.position + this.transform.right * 0.2f * Input.GetAxis("Horizontal");
        this.transform.position = this.transform.position + this.transform.up * 0.2f * Input.GetAxis("Up");

        /*if (Input.GetMouseButton(1))
        {
            float x, y;
            x = Input.GetAxis("Mouse X");
            y = Input.GetAxis("Mouse Y");

            this.transform.rotation = this.transform.rotation * Quaternion.Euler(-5*y, x, 0);
        }*/
        if (Input.GetMouseButton(1))
        {
            transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y")*3, Input.GetAxis("Mouse X")*2, 0));
            float X, Y;
            X = transform.rotation.eulerAngles.x;
            Y = transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Euler(X, Y, 0);
        }
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }
}
