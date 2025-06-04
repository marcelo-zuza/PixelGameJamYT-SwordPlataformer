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
        isGrounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
        playerAnimator.SetBool("isGrounded", isGrounded);
        float verticalSpeed = playerRigidBody.velocity.y;
        playerAnimator.SetFloat("verticalSpeed", verticalSpeed);
        PlayerCommands();


    }

    void FixedUpdate()
    {
        MovePlayer(xAxis);
        if (jump)
        {
            JumpPlayer();
        }
        SetAnimations();
    }

    void PlayerCommands()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonUp("Jump"))
        {
            jump = true;
            playerAnimator.SetTrigger("jump");
        }

        if (Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                playerAnimator.SetTrigger("attack");
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
            playerRigidBody.AddForce(new Vector2(0f, jumpForce));
            isGrounded = false;
            jump = false;
        }
    }

    void AjustColliderForCrouch(bool crouch)
    {
        if (crouch)
        {
            playerMainCollider.offset = new Vector2(playerMainCollider.offset.x, playerMainCollider.offset.y / 2);
            playerMainCollider.size = new Vector2(playerMainCollider.size.x, playerMainCollider.size.y / 2);
        }
        else
        {
            playerMainCollider.offset = new Vector2(playerMainCollider.offset.x, playerMainCollider.offset.y * 2);
            playerMainCollider.size = new Vector2(playerMainCollider.size.x, playerMainCollider.size.y * 2);
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
                if (playerLives > 0)
                {
                    playerAnimator.SetTrigger("hurt");
                    StartCoroutine(PlayerDamage());
                    playerLives -= 1;
                    Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                    playerRigidBody.AddForce(knockbackDirection * 10f, ForceMode2D.Impulse);

                    
                }
                else
                {
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
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        playerLives = 3;
    }
    
    
}
