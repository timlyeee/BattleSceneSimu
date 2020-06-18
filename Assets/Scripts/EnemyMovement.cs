using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{

    /*private Transform player;*/
    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector3 waypoint;
    public float moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(Random.Range(0, 12), Random.Range(0, 12),0 );
        rb = this.GetComponent<Rigidbody2D>();
        /*player = GameObject.FindGameObjectWithTag ("Player").transform;*/
    }

    // Update is called once per frame
    void Update()
    {
        /*Vector3 direction = player.position - transform.position;*/
        Vector3 direction = waypoint - transform.position;
        //Debug.Log(direction);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.rotation = angle;
        direction.Normalize();
        movement = direction;
    }

    private void FixedUpdate()
    {
        moveCharacter(movement);
    }

    void moveCharacter(Vector2 direction)
    {
        rb.MovePosition((Vector2)transform.position + (direction * moveSpeed * Time.deltaTime));
    }

    public void setWaypoint(Vector3 waypoint) {
        this.waypoint = waypoint;
     }
}
