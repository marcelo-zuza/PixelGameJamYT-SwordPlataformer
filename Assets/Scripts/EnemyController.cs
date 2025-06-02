using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer enemySprite;
    [SerializeField] private Transform[] enemyPosition;
    [SerializeField] private float enemySpeed = 5f;
    [SerializeField] private float arrivalThreshold = 0.1f;
    private bool isFacingRight = false;
    private int idTarget;

    void Start()
    {
        enemySprite = GetComponent<SpriteRenderer>();
        transform.position = enemyPosition[0].position;
        idTarget = 1;

    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, enemyPosition[idTarget].position, enemySpeed * Time.deltaTime);
        if (gameObject.transform.position == enemyPosition[idTarget].position)
        {
            idTarget += 1;
            if (idTarget == enemyPosition.Length)
            {
                idTarget = 0;
            }
        }
        if (enemyPosition[idTarget].position.x < transform.position.x && isFacingRight == true)
        {
            Flip();
        }
        else if (enemyPosition[idTarget].position.x > transform.position.x && isFacingRight == false)
        {
            Flip();
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        enemySprite.flipX = !enemySprite.flipX;
    }
}
