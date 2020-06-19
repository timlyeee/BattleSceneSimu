using UnityEngine;
public class Wander : MonoBehaviour
{
    private Vector3 targetPosition;

    private float movementSpeed = 0.03f;
    private float rotationSpeed = 2.0f;
    private float targetPositionTolerance = 3.0f;
    private float minX;
    private float maxX;
    private float minY;
    private float maxY;
    void Start()
    {
        minX = -8.0f;
        maxX = 8.0f;

        minY = -6.0f;
        maxY = 6.0f;

        //Get Wander Position
        GetNextPosition();
    }

    void Update()
    {
        if (Vector3.Distance(targetPosition, transform.position) < targetPositionTolerance) {
            GetNextPosition();
        }
        move2nextPosition();

    }
    void GetNextPosition()
    {
        targetPosition = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY),0) ;
    }
    void move2nextPosition()
    {
        
        Vector3 direction = targetPosition - transform.position;

        //Quaternion tarRot = Quaternion.LookRotation(direction);
        //transform.rotation = Quaternion.Slerp(transform.rotation, tarRot, rotationSpeed * Time.deltaTime);

        transform.Translate(movementSpeed*direction.normalized);
    }
}
