using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCombat : MonoBehaviour
{
    public int hitPoints = 3;
    private int oldValue = 3;
    public LayerMask targets;

    public float range = 0.5f;
    public int damage = 1;

    public float attackDelay = 0.5f;
    private float nextAttackIn = 0.0f;

    private Dictionary<string, Vector2> DirectionnalStrikeOffsets = new Dictionary<string, Vector2>();

    void Start()
    {
        DirectionnalStrikeOffsets.Add("back", new Vector2(0.0f, 0.1f));      // UP
        DirectionnalStrikeOffsets.Add("front", new Vector2(0.0f, -0.4f));    // FRONT
        DirectionnalStrikeOffsets.Add("left", new Vector2(-0.3f, -0.2f));    // LEFT
        DirectionnalStrikeOffsets.Add("right", new Vector2(0.3f, -0.2f));    // RIGHT
    }


    void Update()
    {
        if (hitPoints <= 0) {
            Destroy(gameObject);
        }
        if (hitPoints != oldValue) {
            Debug.Log("i have " + hitPoints + "hp left");
            oldValue = hitPoints;
        }
    }

    public bool strike (string direction = "") {
        // Hit an area and deal damage.
        // (Player gets to strike in a specific direction).

        if (nextAttackIn <= 0.0f) {

            Vector2 offset = new Vector2(0.0f, 0.0f);

            if (!string.IsNullOrWhiteSpace(direction)) {
                offset = DirectionnalStrikeOffsets[direction];
            }

            foreach (Collider2D hit in objectsHit(offset)) {
                if (!hit.isTrigger) { // don't use script triggers as hitboxes
                    hit.GetComponent<CharacterCombat>().hitPoints--;
                    Vector3 knockBackDirection = (hit.transform.position - transform.position).normalized;
                    Debug.Log(hit.name + " was pushed back: " + knockBackDirection);
                    hit.transform.position += (knockBackDirection * 0.1f);
                }
            } 
            nextAttackIn = attackDelay;
            return true;
        } else {
            nextAttackIn -= Time.deltaTime;
            return false;
        }
    }


    private Collider2D[] objectsHit (Vector2 offset) {
        // Find all objects with their colliders in the area.
        return Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y) + offset, range, targets);
    }

}

