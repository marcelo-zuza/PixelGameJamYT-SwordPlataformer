using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Components
    Rigidbody2D playerRigidBody;
    Animator playerAnimator;
    public Transform groundCheck;
    // Colliders
    [SerializeField] private CapsuleCollider2D playerMainCollider;

        
    [SerializeField] private GameObject attackCollider;
    [SerializeField] private float attackCoolDown = 0.5f;
    private float nextAttackTime = 0f;
    //Variables
    [SerializeField] float playerSpeed = 15f;
    [SerializeField] float jumpForce = 100f;
    private float xAxis;
    // bools
    private bool isFacingRight = false;
    public bool isGrounded = true;
    public bool jump = false;
    public bool isCrouching = false;


    void Start()
    {
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerMainCollider = GetComponent<CapsuleCollider2D>();
        playerAnimator = GetComponent<Animator>();
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

        // if (Input.GetKeyDown(KeyCode.DownArrow))
        // {
        //     isCrouching = true;
        //     playerAnimator.SetBool("isCrouching", isCrouching);
        //     AjustColliderForCrouch(true);
        // }

        // if (Input.GetKeyUp(KeyCode.DownArrow))
        // {
        //     isCrouching = false;
        //     playerAnimator.SetBool("isCrouching", isCrouching);
        //     AjustColliderForCrouch(false);
        // }
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
}
