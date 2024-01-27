using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovementController : MonoBehaviour
{
    
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speed = 1000;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        var hor = Input.GetAxis("Horizontal");
        var vert = Input.GetAxis("Vertical");

        var dir = Vector3.back * vert + Vector3.left * hor;
        dir.Normalize();
        rb.AddForce(dir * (speed * Time.deltaTime));
    }
}
