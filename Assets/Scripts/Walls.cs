using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walls : MonoBehaviour
{
    public GameObject player;
    public float minY;
    void Start()
    {
        
    }

    void LateUpdate()
    {
        transform.position = new Vector3(transform.position.x, player.transform.position.y, transform.position.z);
        if (transform.position.y < minY)
        {
            transform.position = new Vector2(transform.position.x, minY);
        }
    }
}
