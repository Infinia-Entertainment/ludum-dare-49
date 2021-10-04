using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movSpeed = 10f;
    public float speedBoostMultiplier = 2.5f;
    bool hasSpeedBoost = false;
    [Header("Jump")]
    public float jumpForce = 1f;
    //public float jumpDuration = 0.5f;
    public float fallMultiplier = 3f;
    public float lowJumpMultiplier = 2f;

    [Header("Dash")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCoolDown = 5f;
    private float dashCurrentCD;
    private float dashCounter;
    private float dashDir = 0;
    private bool isDashing = false;

    [Header("Wall Bounce")]
    public float wallBounceTime = 0.7f;
    public float slideSpeed = 0.5f;
    public float distanceToWall = 0.5f;
    public float bounceStrength = 30f;
    public float bounceHeight = 1f;
    bool isSliding = false;
    RaycastHit2D WallHit;
    private float bounceTime;

    [Header("Ground")]
    public Transform playersFeet;
    public LayerMask groundLayer;
    public float groundRadius;
    bool isGrounded;


    private float horizontalInput;
    private Rigidbody2D rb;
    private float jumpCounter;
    bool releaseJump;
    bool isFacingRight;

    bool lockMovement = false;
    bool unlockInProcess = false;
    Transform prevWall;
    [SerializeField]
    AfterImage afterImages;
    IEnumerator coroutine;

    Animator animator;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        afterImages = GetComponent<AfterImage>();
        animator = GetComponent<Animator>();
        dashCounter = dashDuration;
        dashCurrentCD = 0;
    }
    private void Update()
    {
        Dash();
        Jump();
        WallBounce();

        if (isGrounded && rb.velocity.x != 0)
            animator.SetBool("Walking", true);
        else
            animator.SetBool("Walking", false);
    }
    private void FixedUpdate()
    {

        isGrounded = Physics2D.OverlapCircle(playersFeet.position, groundRadius, groundLayer);

        if (isGrounded && (animator.GetCurrentAnimatorStateInfo(0).IsName("Jump up") || animator.GetCurrentAnimatorStateInfo(0).IsName("Fall")))
        {
            print("landed");
            animator.SetTrigger("Land");
        }
        if (isGrounded && lockMovement)
            lockMovement = false;
        if(isSliding && lockMovement && unlockInProcess == false)
        {
            coroutine = LockMovement(0.5f);
            StartCoroutine(coroutine);
        }
        horizontalInput = Input.GetAxisRaw("Horizontal");



        if (horizontalInput > 0)
            isFacingRight = true;
        else if (horizontalInput < 0)
            isFacingRight = false;

        Slide();

        if (isDashing == false && !lockMovement)
        {
            MoveHorizontally();
        }
    }

    void MoveHorizontally()
    {
        rb.velocity = new Vector2(horizontalInput * movSpeed, rb.velocity.y);
    }
    void Jump()
    {
        if (rb.velocity.y < 0) //if falling down
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump") && lockMovement)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && isGrounded && !isSliding)
        {
            // releaseJump = false;
            // jumpCounter = jumpDuration;
            rb.velocity = Vector2.up * jumpForce;
            isGrounded = false;
            animator.SetTrigger("Jump");
        }
        //if (Input.GetKey(KeyCode.Space) && !releaseJump)
        //{
        //    if (jumpCounter > 0)
        //    {
        //        Jump();
        //        jumpCounter -= Time.deltaTime;
        //    }
        //    else
        //        releaseJump = true;
        //}

        //if (Input.GetKeyUp(KeyCode.Space))
        //    releaseJump = true;
    }
    void Dash()
    {
        if (dashCurrentCD == 0)
        {
            if (dashDir == 0)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    afterImages.activate = false;
                    if (!isFacingRight)
                        dashDir = -1;
                    else if (isFacingRight)
                        dashDir = 1;
                }
            }
            else
            {
                if (dashCounter <= 0)
                {
                    dashDir = 0;
                    dashCounter = dashDuration;
                    rb.velocity = Vector2.zero;
                    isDashing = false;
                    dashCurrentCD = dashCoolDown;
                    StartCoroutine("RefreshDashCD");
                    if (hasSpeedBoost)
                        afterImages.activate = true;

                }
                else
                {
                    isDashing = true;
                    dashCounter -= Time.deltaTime;
                    rb.velocity = new Vector2(dashDir * dashSpeed, rb.velocity.y);
                }
            }
        }
    }

    void Slide()
    {

        if (WallHit)
        {
            prevWall = WallHit.transform;
        }
        if(rb.velocity.x < 0 )
        {
            WallHit = Physics2D.Raycast(transform.position, new Vector2(-distanceToWall, 0), distanceToWall, groundLayer);
            Debug.DrawRay(transform.position, new Vector2(-distanceToWall, 0), Color.blue);
        }
        else if(rb.velocity.x > 0)
        {
            WallHit = Physics2D.Raycast(transform.position, new Vector2(distanceToWall, 0), distanceToWall, groundLayer);
            Debug.DrawRay(transform.position, new Vector2(distanceToWall, 0), Color.blue);
        }

        if (WallHit && isGrounded == false && horizontalInput != 0)
        {
            if (!lockMovement || prevWall != WallHit.transform)
            {
                    animator.SetBool("Slide", true);
                    isSliding = true;
                    rb.velocity = Vector2.zero;
            }
        }
        else if (horizontalInput != 0 && isSliding)
        {
            isSliding = false;
            animator.SetBool("Slide", false);
        }

        if (isSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, slideSpeed, float.MaxValue));
        }

    }
    void WallBounce()
    {
        if (Input.GetButtonDown("Jump") && isSliding)
        {
            isSliding = false;
            animator.SetBool("Slide", false);
            if (WallHit)
            {
                lockMovement = true;
                Vector2 dir = (Vector2)gameObject.transform.position - WallHit.point;
                dir = new Vector2(dir.x < 0 ? -1 : 1, bounceHeight); ;
                rb.AddForce(dir * bounceStrength, ForceMode2D.Impulse);
            }
        }
    }
    IEnumerator RefreshDashCD()
    {
        yield return new WaitForSeconds(dashCoolDown);
        dashCurrentCD = 0;
    }
    IEnumerator LockMovement(float lockTime)
    {
        unlockInProcess = true;
        lockMovement = true;
        yield return new WaitForSeconds(lockTime);
        lockMovement = false;
        unlockInProcess = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Speed Boost")
        {
            movSpeed = movSpeed * speedBoostMultiplier;
            hasSpeedBoost = true;
            afterImages.activate = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {

        if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle") && isSliding)
        {
            isSliding = false;
        } 
    }

}
