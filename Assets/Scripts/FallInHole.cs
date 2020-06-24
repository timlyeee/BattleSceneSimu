using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallInHole : MonoBehaviour
{
    private Collider2D hitbox;
    public float fallTime = 0.75f;
    private List<float> characterFallingTimes;
    private List<GameObject> charactersFalling;
    void Start()
    {
        characterFallingTimes = new List<float>();
        charactersFalling = new List<GameObject>();
    }


    void FixedUpdate()
    {
        // Manage characters that are falling:
        for (int i = 0; i < characterFallingTimes.Count; ++i) {
            if (characterFallingTimes[i] <= 0.0f) {
                Destroy(charactersFalling[i]); // delete after fall time
                charactersFalling[i] = null;
            } else {
                // fall effect:
                characterFallingTimes[i] -= Time.deltaTime;
                charactersFalling[i].transform.localScale *=  0.95f;
                float fallValue = - (characterFallingTimes[i] - fallTime) * 0.15f;
                charactersFalling[i].transform.position = transform.position - new Vector3(0, fallValue, 0);
            }
        }
    }

    void OnTriggerEnter2D (Collider2D other) {
        if (other.isTrigger) return;
        // Get necessary data about character falling in the hole:
        GameObject chara = other.gameObject;
        PlayerControls pc = other.GetComponent<PlayerControls>();
        PlayerDetect pd = other.GetComponent<PlayerDetect>();
        // Stop any future movement:
        if (pc) pc.baseSpeed = 0.0f;
        if (pd) pd.movementSpeed = 0.0f;
        // Record them:
        characterFallingTimes.Add(fallTime);
        charactersFalling.Add(chara);
        // Move them to the middle of the hole:
        chara.transform.position = transform.position;
    }
}

