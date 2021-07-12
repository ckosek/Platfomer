using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private float movementInputDir;

    private bool isFacingRight = true;
    private bool isWalking;
    private bool isGrounded;
    private bool canJump;
    private bool isTouchingWall;
    private bool isWallSlide;

    private int jumpsLeft;
    private int dirFacing = 1;
    public float movementSpeed = 10.0f;
    public float jumpForce = 8.0f;
    public Transform GroundCheck;
    public Transform wallCheck;
    public float GroundCheckRadius;
    public float wallCheckDistance;
    public float wallSlideSpeed;
    public float varJumpHeightMultiplier = 0.5f;
    public float wallJumpForce;
    public int jumpNum = 1;

    public LayerMask whatIsGround;
    public Vector2 wallJumpDir;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        jumpsLeft = jumpNum;
        wallJumpDir.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        CheckMoveDir();
        UpdateAnimations();
        CheckIfCanJump();
        CheckIfWallSlide();
    }

    void FixedUpdate()
    {
        Movement();
        CheckGround();
    }


    private void CheckIfWallSlide()
    {
        if(isTouchingWall && !isGrounded && rb.velocity.y < 0)
        {
            isWallSlide = true;
        }
        else
        {
            isWallSlide = false;
        }
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(GroundCheck.position, GroundCheckRadius, whatIsGround);
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
    }

    private void CheckIfCanJump()
    {
        if(isGrounded && rb.velocity.y <= 0 || isWallSlide)
        {
            jumpsLeft = jumpNum;
        }
        if(jumpsLeft <= 0)
        {
            canJump = false;
        }
        else
        {
            canJump = true;
        }
    }

    private void CheckMoveDir()
    {
        if(isFacingRight && movementInputDir < 0)
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
            dirFacing *= -1;
        }
        else if( !isFacingRight && movementInputDir > 0)
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
            dirFacing *= -1;
        }
        if(rb.velocity.x > 0.1 || rb.velocity.x < -0.1)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }

    private void UpdateAnimations()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isWallSlide", isWallSlide);
    }

    private void CheckInput()
    {
        // Maybe remove raw for momementum feeling?
        movementInputDir = Input.GetAxisRaw("Horizontal");
        if(Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        if(Input.GetButtonUp("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * varJumpHeightMultiplier);
        }

    }

    private void Movement()
    {
        if(!isWallSlide)
        {
            rb.velocity = new Vector2(movementSpeed * movementInputDir, rb.velocity.y);
        }

        if(isWallSlide)
        {
            if(rb.velocity.y < -wallSlideSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            }
        }
    }

    private void Jump()
    {
        if (canJump && !isWallSlide)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpsLeft--;
        }

        else if ((isWallSlide || isTouchingWall) && movementInputDir != 0 && canJump)
        {
            isWallSlide = false;
            //jumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDir.x * movementInputDir, wallJumpForce * wallJumpDir.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
        }
    }


    private void onDrawGizmos()
    {
        Gizmos.DrawWireSphere(GroundCheck.position, GroundCheckRadius);
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }
}
