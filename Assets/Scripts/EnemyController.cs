using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer enemySprite;
    [SerializeField] private Transform[] enemyPosition;
    [SerializeField] private float enemySpeed = 5f;
    [SerializeField] private float arrivalThreshold = 0.1f;
    [SerializeField] private float enemyLifePoints = 1f;
    private bool isFacingRight = true;
    private int idTarget;
    private Animator enemyAnimator;

    void Start()
    {
        enemySprite = GetComponent<SpriteRenderer>();
        enemyAnimator = GetComponent<Animator>();
        transform.position = enemyPosition[0].position;
        idTarget = 1;

        CheckAndFlip();

    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, enemyPosition[idTarget].position, enemySpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, enemyPosition[idTarget].position) < arrivalThreshold)
        {
            idTarget = (idTarget + 1) % enemyPosition.Length;

            CheckAndFlip();
        }

    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        enemySprite.flipX = !enemySprite.flipX;
    }

    void CheckAndFlip()
    {
        if (enemyPosition[idTarget].position.x < transform.position.x && isFacingRight == true)
        {
            Flip();
        }
        else if (enemyPosition[idTarget].position.x > transform.position.x && isFacingRight == false)
        {
            Flip();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "HurtBox")
        {  
            StartCoroutine(DestroyEnemy());
        }
    }

    IEnumerator DestroyEnemy()
    {
        enemyAnimator.SetTrigger("Death");
        yield return new WaitForSeconds(0.6f);
        Destroy(gameObject);
    }
}
