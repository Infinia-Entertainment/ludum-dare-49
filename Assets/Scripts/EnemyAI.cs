using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    [Header("Pathfinding")]
    public Transform target;
    public float activateDistance = 50f;
    public float pathUpdateSeconds = 0.25f;

    [Header("Jump")]
    public float jumpForce = 1f;
    public float fallMultiplier = 3f;
    public float groundRaycastLength = 0.65f;
    public LayerMask groundLayerMask;
    private float currentWaypointAngle = 0;
    [Header("Physics")]
    public float speed = 2.5f;
    public float nextWaypointDistance = 3f;
    public float jumpNodeHeightRequirement = 1f;
    public float jumpPlatformProximityRequirement = 1.5f;
    public float jumpPlatformAngleRequirement = 15;

    [Header("Custom Behavior")]
    public bool followEnabled = true;
    public bool isJumpEnabled = true;
    public bool directionLookEnabled = true;
    private Path path;
    private int currentWaypoint = 0;
    public bool isGrounded;
    Seeker seeker;
    Rigidbody2D rb;


    public void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        target = FindObjectOfType<PlayerController>().transform;

        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
    }

    private void FixedUpdate()
    {
        if (TargetInDistance() && followEnabled)
        {
            PathFollow();
        }
    }

    private void UpdatePath()
    {
        if (followEnabled && TargetInDistance() && seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    private void PathFollow()
    {
        if (path == null)
        {
            return;
        }

        // Reached end of path
        if (currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }

        // Calculating stuff
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        float horizontalDistance = Mathf.Abs(target.position.x - rb.position.x);
        float verticalDistance = Mathf.Abs(target.position.y - rb.position.y);
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        currentWaypointAngle = Vector2.Angle(direction, Vector2.up);
        //Debug.Log($"currentWaypointAngle: {currentWaypointAngle} < 15 {currentWaypointAngle < 15}");


        CheckForGround();

        // Movement
        if (direction.x > 0)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
        }
        else if (direction.x < 0)
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
        }


        // Jump
        if (isJumpEnabled)
        {
            if (isGrounded)
            {
                //has to be high enough to actually need to jump
                //needs to be close enough need to jump
                //or at and angle where it seems like you're above on a platform lol
                if (verticalDistance >= jumpNodeHeightRequirement &&
                 (horizontalDistance <= jumpPlatformProximityRequirement || currentWaypointAngle < jumpPlatformAngleRequirement))
                {
                    rb.velocity = Vector2.up * jumpForce;
                    isGrounded = false;
                }

            }
            else
            {
                //if falling down
                if (rb.velocity.y < 0)
                {
                    rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
                }
            }

        }

        // Next Waypoint

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        // Direction Graphics Handling
        if (directionLookEnabled)
        {
            if (rb.velocity.x > 0.05f)
            {
                transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (rb.velocity.x < -0.05f)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }

    private void CheckForGround()
    {
        //Debug.DrawRay(transform.position, Vector2.down * groundRaycastLength, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundRaycastLength, groundLayerMask);


        if (hit.collider != null)
            isGrounded = true;
        else
            isGrounded = false;

    }

    private bool TargetInDistance()
    {
        return Vector2.Distance(transform.position, target.transform.position) < activateDistance;
    }

    void OnDrawGizmos()
    {
        //Handles.Label(transform.position, $"{currentWaypointAngle}");
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }


}
