using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public GameManager gameManager;

    private Camera mainCamera;
    void Start()
    {
        mainCamera = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (gameManager.isGameActive || gameManager.deathScreenUI.gameObject.activeSelf)
        {
            transform.position = player.transform.position + new Vector3(0, 0, transform.position.z);
            mainCamera.orthographicSize = 12;
        }
        else
        {
            transform.position = new Vector3(0, -100, transform.position.z);
            mainCamera.orthographicSize = 4;
        }
    }
}
