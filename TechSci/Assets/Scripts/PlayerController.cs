using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    //Start Variables
    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D coll;
    private BoxCollider2D boxCollider2d;

    //FSH
    private enum State {idle, running, jumping, falling, hurt, climb}
    private State state = State.idle;

    // Ladder Variables
    [HideInInspector] public bool canClimb = false;
    [HideInInspector]  public bool topLadder = false;
    [HideInInspector]  public bool bottomLadder = false;
    public ladder lad;
    private float naturalGravity;
    [SerializeField] float climbSpeed = 3f;

    //Inspector variables
    [SerializeField] private LayerMask ground;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpforce = 10f;
    [SerializeField] private int cherries = 0;
    [SerializeField] private Text cherryText;
    [SerializeField] private float hurtforce = 5f;
    public Vector3 respawnPoint;
    // private float hDirection;

    /*
    [Header("Wall Jump")]
    public float wallSlidingSpeed;
    public float checkRadius;
    bool isTouchingFront;
    bool isTouchingWall;
    bool wallSliding;
    public Transform groundCheck;
    public Transform frontCheck;
    */

    private void Awake()
    {
        boxCollider2d = transform.GetComponent<BoxCollider2D>();
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        respawnPoint = transform.position;
        naturalGravity = rb.gravityScale;
    }
    private void Update()
    {
        if(state == State.climb)
        {
            Climb();
        }
        else if(state != State.hurt)
        {
            Movement(IsGrounded());
        }
        AnimationState(IsGrounded());
        anim.SetInteger("state", (int)state); //sets animation based on enumerator state

     
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Collectable")
        {
            Destroy(other.gameObject);
            cherries += 1;
            cherryText.text = cherries.ToString();
        }

        if(other.tag == "FallDetector")
        {
            transform.position = respawnPoint;
        }
        if(other.tag == "Checkpoint")
        {
            respawnPoint = other.transform.position;
        }

        if(other.tag == "Trap")
        {
            transform.position = respawnPoint;
        }



    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if(state == State.falling)
            {
                enemy.JumpedOn();
                Jump();
            }
            else
            {
                state = State.hurt;
                if(other.gameObject.transform.position.x > transform.position.x)
                {
                    //Enemy is to my right therefore i should be damaged and move left
                    rb.velocity = new Vector2(-hurtforce, rb.velocity.y);
                }
                else
                {
                    //Enemy is to my left therefore i should be damaged and move right
                    rb.velocity = new Vector2(hurtforce, rb.velocity.y);
                }
            }
        }

    }

    private bool IsGrounded()
    {
        float extraHeightText = .05f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size - new Vector3(0.1f, 0f, 0f), 0f, Vector2.down, extraHeightText, ground);
        Color rayColor;
        if(raycastHit.collider != null)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }
        Debug.DrawRay(boxCollider2d.bounds.center + new Vector3(boxCollider2d.bounds.extents.x, 0), Vector2.down * (boxCollider2d.bounds.extents.y + extraHeightText), rayColor);
        Debug.DrawRay(boxCollider2d.bounds.center - new Vector3(boxCollider2d.bounds.extents.x, 0), Vector2.down * (boxCollider2d.bounds.extents.y + extraHeightText), rayColor);
        Debug.DrawRay(boxCollider2d.bounds.center - new Vector3(boxCollider2d.bounds.extents.x, boxCollider2d.bounds.extents.y + extraHeightText), Vector2.right * (boxCollider2d.bounds.extents.x * 2f), rayColor);

        Debug.Log(raycastHit.collider);
        return raycastHit.collider != null;
    }
    private void Movement(bool isGrounded)
    {

        float hDirection = Input.GetAxisRaw("Horizontal");

        if(canClimb && Mathf.Abs(Input.GetAxisRaw("Vertical")) > .1f)
        {
            state = State.climb;
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            transform.position = new Vector3(lad.transform.position.x, rb.position.y);
            rb.gravityScale = 0f;
        }

        //Moving Left
        if (hDirection == -1)
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            transform.localScale = new Vector2(-1, 1);
        }

        //Moving RIght
        else if (hDirection == 1)
        {
           // isFacingRight = true;
            rb.velocity = new Vector2(speed, rb.velocity.y);
            transform.localScale = new Vector2(1, 1);

        }
        //stops movement when A or D key is released
        else if(hDirection == 0 && /*coll.IsTouchingLayers(ground)*/ isGrounded)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        //Jumping
        if (Input.GetButtonDown("Jump") && /*coll.IsTouchingLayers(ground)*/ isGrounded)
        {
            Jump();
        }
    }

    private void Climb()
    {

        if (Input.GetButtonDown("Jump"))
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            canClimb = false;
            transform.position = new Vector3(lad.transform.position.x, rb.position.y);
            rb.gravityScale = naturalGravity;
            anim.speed = 1f;
            Jump();
            return;
        }

        float vDirection = Input.GetAxis("Vertical");

        //Climbing up
        if(vDirection > .1f && !topLadder)
        {
            rb.velocity = new Vector2(0f, vDirection * climbSpeed);
            anim.speed = 1f;
        }

        //Climbing Down
        else if(vDirection < -.1f && !bottomLadder)
        {
            rb.velocity = new Vector2(0f, vDirection * climbSpeed);
            anim.speed = 1f;
        }

        //Still
        else
        {
            anim.speed = 0f;
            rb.velocity = Vector2.zero;
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpforce);
        state = State.jumping;
    }


    private void AnimationState(bool isGrounded)
    {

        if(state == State.climb)
        {

        }

        else if(state == State.jumping)
        {
            if(rb.velocity.y < .1f)
            {
                state = State.falling;
            }
        }
        else if(state == State.falling)
        {
            if (/*coll.IsTouchingLayers(ground)*/ isGrounded)
            {
                state = State.idle;
            }
        }

        else if (state == State.hurt)
        {
            if (Mathf.Abs(rb.velocity.x) < .1f)
            {
                state = State.idle;
            }
        }

        else if(Mathf.Abs(rb.velocity.x ) > 2f)
        // if(Input.GetButtonDown("Horizontal"))
        {
            //Moving
            state = State.running;

        }

        else
        {
            state = State.idle;
        }
    }

}
