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

    private float horizontalInput;
    private Rigidbody2D rb;
    private float jumpCounter;
    bool isGrounded;
    bool releaseJump;

    [SerializeField]
    AfterImage afterImages;

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
    }
    private void FixedUpdate()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        if (isDashing == false)
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
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
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
                    if (rb.velocity.x < 0)
                        dashDir = -1;
                    else if (rb.velocity.x > 0)
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
    IEnumerator RefreshDashCD()
    {
        yield return new WaitForSeconds(dashCoolDown);
        dashCurrentCD = 0;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
            isGrounded = true;
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
