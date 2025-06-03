using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float offsetX = 3f;
    [SerializeField] float smooth = 0.1f;
    [SerializeField] float limitedUp = 2f;
    [SerializeField] float limitedDown = 2f;
    [SerializeField] float limitedLeft = 2f;
    [SerializeField] float limitedRight = 2f;

    [SerializeField] Transform player;
    private float playerX;
    private float playerY;
    void Start()
    {
        //player = GameObject.Find("Player").transform;
        playerX = player.position.x;
        playerY = player.position.y;
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        if (player != null)
        {
            playerX = Mathf.Clamp(player.position.x, limitedLeft, limitedRight);
            playerY = Mathf.Clamp(player.position.y, limitedDown, limitedUp);
            transform.position = Vector3.Lerp(transform.position, new Vector3(playerX + offsetX, playerY, transform.position.z), smooth);
        }
    }
}
