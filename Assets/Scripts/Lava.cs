using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    public float speed;
    public float maxScale;
    public float riseDelay;
    public bool doRise;

    public GameManager gameManager;

    void Start()
    {
        
    }

    void Update()
    {
        float dt = Time.deltaTime;

        if (doRise && gameManager.isGameActive)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + speed * dt, transform.position.z);
            if (transform.localScale.y < maxScale)
            {
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y + speed * dt, transform.localScale.z);
                transform.position = new Vector3(transform.position.x, transform.position.y - speed * dt / 2, transform.position.z);
            }
        }
    }
    public void StartLavaTimer()
    {
        StartCoroutine("StartLava");
    }
    IEnumerator StartLava()
    {
        yield return new WaitForSeconds(riseDelay);
        doRise = true;
    }
}
