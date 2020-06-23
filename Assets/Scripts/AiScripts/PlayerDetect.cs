﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Use Collider2d to simulate the perception of 2d interface.
     */

public class PlayerDetect : MonoBehaviour
{
    //Movement control parameters
    public int AngleSpeed = 90;

    CircleCollider2D cc2d;
    public float PerceptionRadius = 2f;

    //Paras for wander
    private Vector3 targetPosition;
    public float movementSpeed = 0.5f;
    private float rotationSpeed = 2.0f;
    float targetPositionTolerance = 1.0f;
    private Rigidbody2D rb;

    private GameObject Target=null;
    private float TargetAngle = 0;
    private float AngleDiff = 0;
    public float ViewAngle = 60f;//the angle of view
    float MinAngle = 3f;
    float SelfAngle = 0;
     

    private int state = 0;

    bool isAlert = false;
    bool isInView = false;   //if the player is in the view of the AI
    bool isRayHit = false;
    bool isWait = false;

    float WaitTime = 4f;//Time to detect if the wait time is beyond this limitation.
    float AlertTime= 2f;//Time to stop alert mode 

    float time_wait = 0;//Time has been waiting
    float time_alert = 0;
    // Start is called before the first frame update
    void Start()
    {
       
        cc2d = this.GetComponent<CircleCollider2D>();
        cc2d.radius = PerceptionRadius;
        targetPosition = new Vector3(Random.Range(-8, 8), Random.Range(-6, 6), 0);
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
        //Wider the perception field when it's alert;
        cc2d.radius = PerceptionRadius * (isAlert ? 1.5f : 1f);


        {
        /*
         * Functions to call when the parameter is concerned with isAlert.
         */
        }
        CalculateAngle();
        JudgeView();
        StateSwitcher();
        //Debug.Log("Eular Angle is" + this.transform.rotation.eulerAngles.z);
        /*float eularAngle = (transform.rotation.ToEulerAngles() * 180 / Mathf.PI).z;
        Vector3 dir = new Vector3(Mathf.Cos(eularAngle), Mathf.Sin(eularAngle), 0);
        
        this.//transform.Translate(dir.normalized*Time.deltaTime);
        */
    }
    void CalculateAngle()
    {
        if (Target)
        {
            float diffPositionY = Target.transform.position.y - this.transform.position.y;
            float diffPositionX = Target.transform.position.x - this.transform.position.x;
            //Debug.Log("Target is not null");
            float AtanAngle = Mathf.Atan(diffPositionY /
                diffPositionX) * 180f / Mathf.PI;
            //Debug.Log("Atan angle is" + AtanAngle);
            //Calculate the angle with different field math.
            if (diffPositionY > 0 && diffPositionX <= 0)
            {
                TargetAngle = 270f - AtanAngle;
            }
            else
                TargetAngle = 90f - AtanAngle;
            //Debug.Log("Target Angle is " + TargetAngle);
            //Modify the angle difference by importing self's eular angle
            float OriginTargetAngle = TargetAngle;
            if (Mathf.Abs(TargetAngle + 360 - this.transform.rotation.eulerAngles.z)
               <
            Mathf.Abs(TargetAngle - this.transform.rotation.eulerAngles.z)
               )
            {
                TargetAngle += 360f;
            }
            if (Mathf.Abs(TargetAngle - 360 - this.transform.rotation.eulerAngles.z)
               <
            Mathf.Abs(TargetAngle - this.transform.rotation.eulerAngles.z)
               )
            {
                TargetAngle -= 360f;
            }

            //Output the difference of angles.
            AngleDiff = Mathf.Abs(TargetAngle - this.transform.rotation.eulerAngles.z);
            //Debug.Log("Difference of angles:" + TargetAngle + "(" + OriginTargetAngle + ")-" + this.transform.rotation.eulerAngles.z + "=" + AngleDiff);
        }

    }
    //Use RaycastHit to detect if the target is really can be seen?
    void JudgeView()
    {
        if (Target)
        {
            //The vector which aims to the player
            Vector3 vec = new Vector3(Target.transform.position.x - this.transform.position.x, Target.transform.position.y - this.transform.position.y, 0);
            RaycastHit2D hitInfo = Physics2D.Raycast(this.transform.position, vec, PerceptionRadius * 1.5f, LayerMask.GetMask("player"));

            if (hitInfo.collider != null)
            {
                // Debug.Log("The name of target is" + hitInfo.collider.gameObject);//name os W2.

                if (hitInfo.collider.gameObject.CompareTag("Player"))
                {
                    // Debug.Log("Hit!");
                    isRayHit = true;
                }
                else
                    isRayHit = false;

            }

            // Debug.DrawLine(this.transform.position, hitInfo.point, Color.red,1);
            if (AngleDiff * 2 < (isAlert ? ViewAngle * 1.5 : ViewAngle))
            {
                isInView = true;

            }
            else
                isInView = false;
            // Debug.Log("Is in view?" + isInView);
        }
        
    }
    void StateSwitcher()
    {
        Vector3 direction;
        // Debug.Log("The state now is" + state);
        switch (state)
        {
            //Normal case, rotate and move as usual
            case 0:
                direction = targetPosition - transform.position;
                //transform.Translate(movementSpeed * direction.normalized);
                rb.AddForce(movementSpeed * direction.normalized);
                if (Vector3.Distance(targetPosition, transform.position) < targetPositionTolerance)
                {
                    targetPosition = new Vector3(Random.Range(-8, 8), Random.Range(-6, 6), 0);
                }
                if (isRayHit && isInView)
                {
                    isAlert = true;
                    time_wait = 0;
                    state = 1;
                    targetPosition = Target.transform.position;
                }
                break;
             
            //When the target is inside, stable the angle and aiming to the target.
            case 1:
                //Debug.Log("Angle diff erence is" + AngleDiff);

                //               Debug.Log("Eular Angle is" + this.transform.rotation.eulerAngles.z);
                if(Target)
                targetPosition = Target.transform.position;
                direction = targetPosition - transform.position;
                //transform.Translate(movementSpeed *2 * direction.normalized);
                rb.AddForce(movementSpeed * 2 * direction.normalized);
                // Debug.Log("Is RayHit?" + isRayHit);
                if (!isRayHit)
                {
                    isWait = true;
                    time_wait = 0;
                    state = 2;
                    //targetPosition keep unchanged
                }
                break;

            case 2:
                if(Target)
                targetPosition = Target.transform.position;
                
                direction = targetPosition - transform.position;
                //transform.Translate(movementSpeed * direction.normalized/2);
                rb.AddForce(movementSpeed * direction.normalized * 0.5f);
                time_wait += Time.deltaTime * 1;
                if(time_wait >= WaitTime)
                {
                    isWait = false;
                    time_alert = 0;
                    state = 3;
                }
                if(isRayHit && isInView)
                {
                    isAlert = true;
                    time_wait = 0;
                    state = 1;
                }
                break;

            case 3:
                if (Target)
                    targetPosition = Target.transform.position;
                direction = targetPosition - transform.position;
                //transform.Translate(movementSpeed * direction.normalized / 2);
                rb.AddForce(movementSpeed * direction.normalized * 0.5f);
                time_alert += Time.deltaTime;
                // Debug.Log("Alert time" + time_alert);
                if (time_alert >= AlertTime)
                {
                    //Quit alert mode
                    time_alert = 0;
                    isAlert = false;
                    state = 0;

                    //Back to normal : Get a random target position which is inside the map
                    targetPosition = new Vector3(Random.Range(-8, 8), Random.Range(-6, 6), 0);
                    direction = targetPosition - transform.position;
                    //transform.Translate(movementSpeed * direction.normalized);
                    rb.AddForce(movementSpeed * direction.normalized);
                }
                if(isRayHit && isInView)
                {
                    isAlert = true;
                    time_wait = 0;
                    state = 1;
                }
               
                break;
            default:
                break;
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Target = collision.gameObject;
            isAlert = true;
            
            // Debug.Log("The target gets is " + Target.name);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Target == null)
            {
                Target = collision.gameObject;
                // Debug.Log("The target in stay gets is " + Target.name);
                isAlert = true;
                
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            
            // Debug.Log("The target getting out is " + Target.name);
            Target = null;
            isAlert = false;
        
        {
                isRayHit = false;
                isInView = false;
            }


        }
    }
   
}
