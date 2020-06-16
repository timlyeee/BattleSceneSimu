using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private CharacterCombat combat;

    public KeyCode moveUp;
    public KeyCode moveDown;
    public KeyCode moveLeft;
    public KeyCode moveRight;
    public KeyCode strike;

    public float baseSpeed;

    private string lastAnimDirection = "front";
    private string lastAnimType = "idle";
    private string newAnimDirection = "front";
    private string newAnimType = "idle";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        combat = GetComponent<CharacterCombat>();
    }


    void FixedUpdate()
    {
        bool movingUp = Input.GetKey(moveUp);
        bool movingDown = Input.GetKey(moveDown);
        bool movingLeft = Input.GetKey(moveLeft);
        bool movingRight = Input.GetKey(moveRight);
        bool striking = Input.GetKey(strike);
        bool movingX = movingUp != movingDown;
        bool movingY = movingLeft != movingRight;

        float speed = baseSpeed;

        if (movingX && movingY)
        {
            speed *= 0.80f; // ◄ lower the speed when moving in diagonals so it does not feel too different
        }

        if (striking)
        {
            speed *= 0.0f; // stop moving
        }

        if (movingUp)
        {
            rb.AddForce(transform.up * speed);
            newAnimDirection = "back";
            newAnimType = "walk";
        }
        if (movingDown)
        {
            rb.AddForce(transform.up * speed * (-1.0f));
            newAnimDirection = "front";
            newAnimType = "walk";
        }
        if (movingLeft)
        {
            rb.AddForce(transform.right * speed * (-1.0f));
            newAnimDirection = "left";
            newAnimType = "walk";
        }
        if (movingRight)
        {
            rb.AddForce(transform.right * speed);
            newAnimDirection = "right";
            newAnimType = "walk";
        }
        if (!movingX && !movingY)
        {
            newAnimType = "idle";
        }
        if (striking)
        {
            newAnimType = "strike";
            combat.strike((lastAnimDirection != newAnimDirection) ? newAnimDirection : lastAnimDirection);
        }



        anim.SetBool(lastAnimDirection + "-" + lastAnimType, false);

        if (lastAnimDirection != newAnimDirection || lastAnimType != newAnimType)
        { // animate when necessary:

            anim.SetBool(newAnimDirection + "-" + newAnimType, true);
            lastAnimDirection = newAnimDirection;
            lastAnimType = newAnimType;
        }
    }
}