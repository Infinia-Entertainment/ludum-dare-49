using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movSpeed = 10f;
    public float speedBoostMultiplier = 2.5f;

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

    Transform prevWall;
    [SerializeField]
    AfterImage afterImages;
    IEnumerator coroutine;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        afterImages = GetComponent<AfterImage>();
        dashCounter = dashDuration;
        dashCurrentCD = 0;
    }
    private void Update()
    {
        Dash();
        Jump();
        WallBounce();
    }
    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(playersFeet.position, groundRadius, groundLayer);
        if (isGrounded && lockMovement)
            lockMovement = false;
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (horizontalInput > 0)
            isFacingRight = true;
        else if(horizontalInput < 0)
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
                    //    afterImages.activate = true;
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
                    // afterImages.activate = false;

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
        WallHit = Physics2D.Raycast(transform.position, new Vector2(isFacingRight ? distanceToWall : -distanceToWall, 0), distanceToWall, groundLayer);

            if (WallHit && isGrounded == false && horizontalInput != 0)
            {
                if (!lockMovement || prevWall != WallHit.transform)
                {
                    isSliding = true;
                    rb.velocity = Vector2.zero;
                }
            }
            else if (horizontalInput != 0 && isSliding)
            {
                isSliding = false;
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
            if (WallHit)
            {
                lockMovement = true;
                Vector2 dir = gameObject.transform.position - WallHit.transform.position;
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
        lockMovement = true;
        yield return new WaitForSeconds(lockTime);
        lockMovement = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Speed Boost")
        {
            movSpeed = movSpeed * speedBoostMultiplier;
            afterImages.activate = true;
        }
    }


}
