using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trophy : MonoBehaviour
{
    public GameObject player;
    public GameManager gameManager;
    public GameObject trophyParticle;
    public AudioManager audioManager;

    public int secondTrophyY;
    public int thirdTrophyY;
    void Start()
    {
    }

    void Update()
    {
        if (player.transform.position.y >= transform.position.y && gameManager.trophyCount < 3)
        {
            gameManager.doPlayTrophyAnimation = true;
            Instantiate(trophyParticle, transform.position, transform.rotation);
            audioManager.trophySound.Play();
            if (gameManager.trophyCount == 0)
            {
                transform.position = new Vector3(transform.position.x, secondTrophyY, transform.position.z);
            }
            else if (gameManager.trophyCount == 1)
            {
                transform.position = new Vector3(transform.position.x, thirdTrophyY, transform.position.z);
            }
            else if (gameManager.trophyCount == 2)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
