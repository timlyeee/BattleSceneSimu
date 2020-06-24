using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Use Collider2d to simulate the perception of 2d interface.
     */

public class PlayerDetect : MonoBehaviour
{
    //Movement control parameters
    public int AngleSpeed = 90;
    private bool deviationIsPositive;
    private float deviationTimeout = 0.0f;
    private string obstacleName = "";

    CircleCollider2D cc2d;
    BoxCollider2D colliderBox;
    public float PerceptionRadius = 2f;

    //Paras for wander
    private Vector3 targetPosition;
    public float movementSpeed = 0.5f;
    //private float rotationSpeed = 2.0f;
    public float targetPositionTolerance = 0.5f;
    private Rigidbody2D rb;
    public Animator anim;
    private CharacterCombat combat;

    private GameObject Target=null;
    private float TargetAngle = 0;
    private float AngleDiff = 0;
    public float ViewAngle = 60f;//the angle of view
    //float MinAngle = 3f;
    //float SelfAngle = 0;
     

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
        combat = GetComponent<CharacterCombat>();
        colliderBox = GetComponent<BoxCollider2D>();
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
        Vector3? alternativeDirection;
        Vector3 normalizedDirection;

        // Debug.Log("The state now is" + state);
        switch (state)
        {
            //Normal case, rotate and move as usual
            case 0:
                direction = targetPosition - transform.position;
                //transform.Translate(movementSpeed * direction.normalized);
                alternativeDirection = checkObstacle(targetPosition);
                normalizedDirection = (alternativeDirection != null) ? ((Vector3) alternativeDirection).normalized : direction.normalized;
                rb.AddForce(movementSpeed * direction.normalized);
                anim.SetBool("walking", true);
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

     
                // decide wether to get closer or attack:
                if (Vector3.Distance(targetPosition, transform.position) > targetPositionTolerance) {
                    // before moving, check for obstacles:
                    alternativeDirection = checkObstacle(targetPosition);
                    normalizedDirection = (alternativeDirection != null) ? ((Vector3) alternativeDirection).normalized : direction.normalized;
                    
                    // get closer:
                    rb.AddForce(movementSpeed * 2 * normalizedDirection);
                    walk();

                } else { // attack:
                    combat.strike();
                    strike();
                }


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
                alternativeDirection = checkObstacle(targetPosition);
                normalizedDirection = (alternativeDirection != null) ? ((Vector3) alternativeDirection).normalized : direction.normalized;
                rb.AddForce(movementSpeed * direction.normalized * 0.5f);
                walk();
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
                alternativeDirection = checkObstacle(targetPosition);
                normalizedDirection = (alternativeDirection != null) ? ((Vector3) alternativeDirection).normalized : direction.normalized;
                rb.AddForce(movementSpeed * direction.normalized * 0.5f);
                walk();
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
                    alternativeDirection = checkObstacle(targetPosition);
                    normalizedDirection = (alternativeDirection != null) ? ((Vector3) alternativeDirection).normalized : direction.normalized;
                    rb.AddForce(movementSpeed * direction.normalized);
                    walk();
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

    // --- animations ----------------------------------------------------------------
    private void walk () {
        if (!anim.GetBool("walking")) anim.SetBool("walking", true);
        if (anim.GetBool("striking")) anim.SetBool("striking", false);
    }

    private void strike () {
        if (anim.GetBool("walking")) anim.SetBool("walking", false);
        if (!anim.GetBool("striking")) anim.SetBool("striking", true);
    }

    private void idle () {
        if (anim.GetBool("walking")) anim.SetBool("walking", false);
        if (anim.GetBool("striking")) anim.SetBool("striking", false);
    }

    // --- basic pathfinding ----------------------------------------------------------

    private Vector3? checkObstacle (Vector3 waypoint) {
        // check for obstacles in the way and return other direction if necessary
        
        Vector3[] points = new Vector3[] {
            new Vector3(colliderBox.bounds.min.x,colliderBox.bounds.min.y,0.0f),
            new Vector3(colliderBox.bounds.min.x,colliderBox.bounds.max.y,0.0f),
            new Vector3(colliderBox.bounds.max.x,colliderBox.bounds.min.y,0.0f),
            new Vector3(colliderBox.bounds.max.x,colliderBox.bounds.max.y,0.0f)
        };

        Vector3[] waypointOffsets = new Vector3[] {
            new Vector3(waypoint.x - colliderBox.bounds.extents.x, waypoint.y - colliderBox.bounds.extents.y,0.0f),
            new Vector3(waypoint.x - colliderBox.bounds.extents.x, waypoint.y + colliderBox.bounds.extents.y,0.0f),
            new Vector3(waypoint.x + colliderBox.bounds.extents.x, waypoint.y - colliderBox.bounds.extents.y,0.0f),
            new Vector3(waypoint.x + colliderBox.bounds.extents.x, waypoint.y + colliderBox.bounds.extents.y,0.0f)
        };

        RaycastHit2D obstacleCheck = Physics2D.Raycast(points[0], waypointOffsets[0] - points[0], PerceptionRadius, LayerMask.GetMask("props"));
        Vector3 origin = points[0];


        for (int i = 1; i < 4; ++i) {
            // check from each corner of collider:
            RaycastHit2D obstacle = Physics2D.Raycast(points[i], waypointOffsets[i] - points[i], PerceptionRadius, LayerMask.GetMask("props"));
            if (obstacle.collider != null && (obstacleCheck.collider == null || obstacle.distance < obstacleCheck.distance)) {
                obstacleCheck = obstacle; // keep the hit with smallest distance
                origin = points[i];
            }
        }

        if (!obstacleCheck.collider) return null; // no obstacle in view
        // Debug.Log("obstacle found: " + obstacleCheck.collider.name);
        
        float distanceDelta = Vector3.Distance(transform.position, waypoint) - obstacleCheck.distance;
        if (distanceDelta < 0.0f)  return null; // obstacle is further away than waypoint

        // Determine new, altered direction:
        Vector3 direction = waypoint - origin;

        if (deviationTimeout - Time.deltaTime <= 0.0f || obstacleCheck.collider.name != obstacleName) {
            // commit to a direction for a while (from which side to avoid the obstacle)
            float angleFromHitToObstacleCenter = Vector3.SignedAngle(direction, obstacleCheck.collider.transform.position - origin, Vector3.up);
            deviationIsPositive = angleFromHitToObstacleCenter > 0;
            deviationTimeout = 3.0f;
        } else {
            deviationTimeout -= Time.deltaTime;
        }
        
        Vector3 altDirection = Quaternion.AngleAxis(90, deviationIsPositive ? Vector3.left : Vector3.right) * direction;
        float ratio = ((PerceptionRadius - obstacleCheck.distance) / PerceptionRadius);
        Vector3 newDirection = Vector3.Cross(direction * ratio, altDirection * (1 - ratio));
        // New direction is a mix of old direction and a 90 angle deviation from it.
        // The nearer the obstacle, the bigger the deviation.
        obstacleName = obstacleCheck.collider.name;
        // Debug.Log("new direction found");

        return newDirection;
    }
}
