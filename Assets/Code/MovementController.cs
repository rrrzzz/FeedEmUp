using System;
using Code;
using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovementController : MonoBehaviour
{
    [NotNull] private const string Player1Hor = "HorizontalP1";
    [NotNull] private const string Player2Hor = "HorizontalP2";
    [NotNull] private const string Player1Ver = "VerticalP1";
    [NotNull] private const string Player2Ver = "VerticalP2";
    
    
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speed = 1000;
    private bool _isFirstPlayer;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        _isFirstPlayer = GetComponent<PlayerCollisionsController>().isFirstPlayer;
    }

    // Update is called once per frame
    void Update()
    {

        float hor;
        float vert;
        if (_isFirstPlayer)
        {
            hor = Input.GetAxis(Player1Hor);
            vert = Input.GetAxis(Player1Ver);
        }
        else
        {
            hor = Input.GetAxis(Player2Hor);
            vert = Input.GetAxis(Player2Ver);
        }
        
        var dir = Vector3.back * vert + Vector3.left * hor;
        dir.Normalize();
        rb.AddForce(dir * (speed * Time.deltaTime));
    }
}
