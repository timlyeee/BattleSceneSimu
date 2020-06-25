using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CharacterCombat playerCombatManager;
    private static GameManager instance;
    public Transform enemy;
    public float spawnInterval = 10.0f;
    private float spawnDelay = 0.0f;
    private int spawnNumber = 3;
    private void Awake()
    {
        MakeSingleton();
    }
    private void MakeSingleton()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void FixedUpdate () {
        if (playerCombatManager.hitPoints <= 0) {
            spawnDelay = float.PositiveInfinity; // stop spawns on gameover
        };
        if (spawnDelay <= 0.0f) {
            for (int i = 0; i < spawnNumber; ++i) {
                Instantiate(enemy, new Vector3(Random.Range(-10, 5), Random.Range(0, 5)), Quaternion.identity);
            }
            spawnDelay = spawnInterval;
            spawnInterval *= 0.95f;
        } else {
            spawnDelay -= Time.deltaTime;
        }
    }
}
