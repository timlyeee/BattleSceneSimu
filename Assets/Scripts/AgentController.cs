using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour
{

    private List<EnemyMovement> enemies = new List<EnemyMovement>();
    Transform player;

    // Start is called before the first frame update
    void Start()
    {
        // Get reference to player location:
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        // Get enemies movement control scripts:
        GameObject[] controlled = GameObject.FindGameObjectsWithTag("watermelon");
        foreach (GameObject enemy in controlled) {
            enemies.Add(enemy.GetComponent<EnemyMovement>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        // For now just tell enemies to go to the player in a straight line:
        foreach (EnemyMovement enemy in enemies) {
            enemy.setWaypoint(player.position);
        }
    }
}
