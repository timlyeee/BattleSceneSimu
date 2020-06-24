using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
//using System.Media;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public Transform player;

    void Update()
    {
        // Just hover over the player:
        if (player) transform.position = player.position + new Vector3(0,0,-10);
        
    }
}
