using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oppossum : Enemy
{   
    [SerializeField]private float leftCap;
    [SerializeField] private float rightCap;

    [SerializeField] private float length = 3f;
    [SerializeField] private float jumpHeight = 15f;
    [SerializeField] private LayerMask ground;
    private Collider2D coll;
    private Rigidbody2D rb;

    private bool facingLeft = true;

    protected override void Start()
    {
        base.Start();
        coll = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
      /*//Transition from jump to fall
        if (anim.GetBool("Jumping"))
        {
            if(rb.velocity.y < .1)
            {
                anim.SetBool("Falling", true);
                anim.SetBool("Jumping", false);

            }
        }

        //Transition from Fall to Idle
        if(coll.IsTouchingLayers(ground) && anim.GetBool("Falling"))
        {
            anim.SetBool("Falling", false);
        }
        */
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (facingLeft)
        {
            //Test to see of we are beyond the leftCap
            if (transform.position.x > leftCap)
            {

                //Make sure sprite is facing right direction and if it is not make it face that direction
                if (transform.localScale.x != 1)
                {
                    transform.localScale = new Vector3(1, 1);
                }
                //Test to see if I am on the ground, if so jump
               // if (coll.IsTouchingLayers(ground))
               // {
                    //Jump
                    rb.velocity = new Vector2(-length, rb.velocity.y);
                   // anim.SetBool("Jumping", true);
               // }
            }
            else
            {
                facingLeft = false;
            }
        }
        else
        {
            //Test to see of we are beyond the leftCap
            if (transform.position.x < rightCap)
            {
                //Make sure sprite is facing right direction and if it is not make it face that direction
                if (transform.localScale.x != -1)
                {
                    transform.localScale = new Vector3(-1, 1);
                }
                //Test to see if I am on the ground, if so jump
               // if (coll.IsTouchingLayers(ground))
               // {
                    //Jump
                    rb.velocity = new Vector2(length, rb.velocity.y);
                   // anim.SetBool("Jumping", true);
               // }
            }
            else
            {
                facingLeft = true;
            }
        }
    }

  


}
