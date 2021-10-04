using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Pathfinding;
using Wizard.Spells;

public class EnemyRangedAI : MonoBehaviour
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

    [Header("Ranged Attack")]
    public float rangedAttackStopDistance = 12;
    public float rangedAttackFirerate = 1.5f;
    public GameObject rangedAttackPrefab;
    private float rangedAttackTimer = 0;
    public LayerMask playerLayerMask;

    [Header("Custom Behavior")]
    public bool followEnabled = true;
    public bool isJumpEnabled = true;
    public bool dirToNextWaypointLookEnabled = true;
    private Path path;
    private int currentWaypoint = 0;
    public bool isGrounded;
    Seeker seeker;
    Rigidbody2D rb;

    Vector2 dirToNextWaypoint;
    Vector2 dirToPlayer;
    float horizontalDistance;
    float verticalDistance;
    float disToNextWaypoint;
    float disToPlayer;
    bool hasSeenPlayer = false;
    public void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        target = FindObjectOfType<PlayerController>().transform;

        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
    }

    private void FixedUpdate()
    {
        if (TargetInDistance() && hasSeenPlayer && followEnabled)
        {
            PathFollow();
        }
    }

    private void Update()
    {
        if (IsPlayerInView()) hasSeenPlayer = true;
        // Reached end of path or there's no path
        if (path == null || currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }

        // Calculating stuff
        dirToNextWaypoint = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        dirToPlayer = ((Vector2)target.position - rb.position).normalized;


        disToNextWaypoint = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        disToPlayer = Vector2.Distance(rb.position, target.position);

        horizontalDistance = Mathf.Abs(target.position.x - rb.position.x);
        verticalDistance = Mathf.Abs(target.position.y - rb.position.y);

        currentWaypointAngle = Vector2.Angle(dirToNextWaypoint, Vector2.up);
        //Debug.Log($"currentWaypointAngle: {currentWaypointAngle} < 15 {currentWaypointAngle < 15}");

        if (disToPlayer <= rangedAttackStopDistance && IsPlayerInView())
        {
            if (rangedAttackTimer >= 0)
            {
                rangedAttackTimer -= Time.deltaTime;
            }
            else
            {
                ShootRangedAttack();
                rangedAttackTimer = rangedAttackFirerate;
            }
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
        // Reached end of path or there's no path
        if (path == null || currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }


        CheckForGround();

        // Movement
        if (disToPlayer > rangedAttackStopDistance || !IsPlayerInView())
        {

            if (dirToNextWaypoint.x > 0)
            {
                rb.velocity = new Vector2(speed, rb.velocity.y);
            }
            else if (dirToNextWaypoint.x < 0)
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
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        // Next Waypoint
        if (disToNextWaypoint < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        // DirToNextWaypoint Graphics Handling
        if (dirToNextWaypointLookEnabled)
        {
            if (dirToPlayer.x > 0)
            {
                transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (dirToPlayer.x < 0)
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

    private bool IsPlayerInView()
    {
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + Vector2.up, dirToPlayer, rangedAttackStopDistance);
        Debug.DrawRay((Vector2)transform.position + Vector2.up, dirToPlayer * rangedAttackStopDistance, Color.red);


        if (hit.collider != null && hit.collider.gameObject.CompareTag("Player")) return true;
        else return false;
    }

    private void ShootRangedAttack()
    {
        var spell = Instantiate(rangedAttackPrefab, (Vector2)transform.position + Vector2.up, Quaternion.identity);
        spell.GetComponent<SpellBase>().Launch();
    }
}
