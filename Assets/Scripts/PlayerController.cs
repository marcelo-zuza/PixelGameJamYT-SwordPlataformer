using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Components
    Rigidbody2D playerRigidBody;
    Animator playerAnimator;
    public Transform groundCheck;
    [SerializeField] SpriteRenderer playerSpriteRenderer;
    // Colliders
    [SerializeField] private CapsuleCollider2D playerMainCollider;
    [SerializeField] private AudioSource gameAudioSource;
    [SerializeField] private AudioClip fxJump;
    [SerializeField] private AudioClip fxHurt;
    [SerializeField] private AudioClip fxAttack;
    [SerializeField] private AudioClip fxDie;


    [SerializeField] private GameObject attackCollider;
    [SerializeField] private float attackCoolDown = 0.5f;
    private float nextAttackTime = 0f;
    //Variables
    [SerializeField] float playerSpeed = 15f;
    [SerializeField] float jumpForce = 100f;
    [SerializeField] public float playerLives = 3f;
    private float xAxis;
    // bools
    private bool isFacingRight = false;
    public bool isGrounded = true;
    public bool jump = false;
    public bool isCrouching = false;
    [SerializeField] private bool playerInvulnerability = false;


    void Start()
    {
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerMainCollider = GetComponent<CapsuleCollider2D>();
        playerAnimator = GetComponent<Animator>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        playerAnimator.SetBool("isGrounded", isGrounded);
        float verticalSpeed = playerRigidBody.velocity.y;
        playerAnimator.SetFloat("verticalSpeed", verticalSpeed);
        PlayerCommands();


    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
        MovePlayer(xAxis);
        SetAnimations();
    }

    void PlayerCommands()
    {
        if (Input.GetButtonDown("Jump"))
        {
            //jump = true;
            JumpPlayer();
            playerAnimator.SetTrigger("jump");
        }

        if (Time.time >= nextAttackTime)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                playerAnimator.SetTrigger("attack");
                gameAudioSource.PlayOneShot(fxAttack);
                StartCoroutine(Attack());
                nextAttackTime = Time.time + attackCoolDown;
            }
        }
    }


    void MovePlayer(float xMovement)
    {
        playerRigidBody.velocity = new Vector2(xMovement * playerSpeed, playerRigidBody.velocity.y);
        if (isFacingRight && xAxis > 0 || !isFacingRight && xAxis < 0)
        {
            Flip();
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    void SetAnimations()
    {
        playerAnimator.SetBool("walking", xAxis != 0);
    }

    void JumpPlayer()
    {
        if (isGrounded)
        {
            playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, 0f);
            gameAudioSource.PlayOneShot(fxJump);
            playerRigidBody.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            isGrounded = false;
            jump = false;
        }
    }

    IEnumerator Attack()
    {
        attackCollider.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        attackCollider.SetActive(false);
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.gameObject != gameObject && playerInvulnerability == false)
            {
                if (playerLives > 1)
                {
                    playerAnimator.SetTrigger("hurt");
                    StartCoroutine(PlayerDamage());
                    playerLives -= 1;
                    Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                    playerRigidBody.AddForce(knockbackDirection * 10f, ForceMode2D.Impulse);

                    
                }
                else
                {
                    playerLives -= 1;
                    StartCoroutine(PlayerDeath());
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "DeathZone")
        {
            StartCoroutine(PlayerDeath());
        }
    }

    IEnumerator PlayerDamage()
    {
        playerInvulnerability = true;
        gameAudioSource.PlayOneShot(fxHurt);
        yield return new WaitForSeconds(0.2f);
        for (float i = 0f; i < 1f; i += 0.1f)
        {
            playerSpriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.1f);
            playerSpriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
        playerInvulnerability = false;
    }

    IEnumerator PlayerDeath()
    {
        playerAnimator.SetTrigger("died");
        gameAudioSource.PlayOneShot(fxDie);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        playerLives = 3;
    }
    
    
}
