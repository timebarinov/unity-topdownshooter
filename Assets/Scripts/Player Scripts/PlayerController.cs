using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody myBody;
    private Vector3 velocity;

    private void Awake()
    {
        myBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        myBody.MovePosition(myBody.position + velocity * Time.deltaTime);
    }

    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    public void LookAt(Vector3 lookPoint)
    {
        Vector3 heightCorrected = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        transform.LookAt(heightCorrected);
    }

} // class
