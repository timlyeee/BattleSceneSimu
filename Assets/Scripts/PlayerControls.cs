using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
// using System.Diagnostics;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    private Rigidbody2D rb;

    public KeyCode moveUp;
    public KeyCode moveDown;
    public KeyCode moveLeft;
    public KeyCode moveRight;

    public float baseSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    
    void FixedUpdate()
    {
        bool movingUp = Input.GetKey(moveUp);
        bool movingDown = Input.GetKey(moveDown);
        bool movingLeft = Input.GetKey(moveLeft);
        bool movingRight = Input.GetKey(moveRight);
        bool movingX = movingUp != movingDown;
        bool movingY = movingLeft != movingRight;

        float speed = baseSpeed;

        if (movingX && movingY)
        {
            speed *= 0.80f; // ◄ lower the speed when moving in diagonals so it does not feel too different
        }

        if (movingUp)
        {
            rb.AddForce(transform.up * speed);
        }
        if (movingDown)
        {
            rb.AddForce(transform.up * speed * (-1.0f));
        }
        if (movingLeft)
        {
            rb.AddForce(transform.right * speed * (-1.0f));
        }
        if (movingRight)
        {
            rb.AddForce(transform.right * speed);
        }
    }
}