using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float maxVelocityY = 100f;
    public float maxSpeed = 10f;
    public float jumpForce = 700f;
    public Collider2D groundCollider;
    public Collider2D forwardCollider;
    public Collider2D backCollider;
    private bool facingRight = true;
    private Rigidbody2D rb;
    public bool grounded;
    private bool onJumpPlatfrom;
    public float move;
    private bool alreadyJump;
    public int countOfJumps;
    private int maxJumpsAvaible = 2;
    public bool toJerk = false;
    private bool lastLeftJerk;
    public bool isJerk = false;

    public void Start() {
        rb = GetComponent<Rigidbody2D>();
        countOfJumps = maxJumpsAvaible;
        Gravitation.mobs.Add(this.transform);
    }

    public void FixedUpdate() {
        grounded = false;
        onJumpPlatfrom = false;
        GameObject platfrom = null;
        List<Collider2D> colliders = new List<Collider2D>();
        groundCollider.OverlapCollider(new ContactFilter2D(), colliders);
        foreach (Collider2D col in colliders) {
            if (col.gameObject.layer == 8) {
                grounded = true;
                if (alreadyJump) {
                    countOfJumps = maxJumpsAvaible;
                    alreadyJump = false;
                }
            }

            if (col.gameObject.tag.Equals("JumpPlatfrom"))
                onJumpPlatfrom = true;
            if (col.gameObject.tag.Equals("Platform"))
                platfrom = col.gameObject;
        }

        if (countOfJumps > 0 && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))) {
            rb.AddForce(new Vector2(0f, onJumpPlatfrom ? jumpForce * 1.8f : jumpForce));
            countOfJumps--;
            Invoke(nameof(RemoveAlreadyJump), 0.1f);
        }

        if (grounded && Input.GetKeyDown(KeyCode.S) && platfrom != null)
            platfrom.GetComponent<PlatformEffector2D>().colliderMask = 2147483647 - (1 << 9);

        move = Input.GetAxis("Horizontal");
        Collider2D colliderToCheck = facingRight
            ? (move > 0 ? forwardCollider : backCollider)
            : (move > 0 ? backCollider : forwardCollider);

        if (!colliderToCheck.IsTouchingLayers(8) && !isJerk)
            rb.velocity = new Vector2(move * maxSpeed, rb.velocity.y);

        if (move > 0 && !facingRight)
            Flip();
        else if (move < 0 && facingRight)
            Flip();

        if (Mathf.Abs(rb.velocity.y) > maxVelocityY)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y > 0 ? maxVelocityY : -maxVelocityY);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            Gravitation.changeGravity();
        }
        
        if (Input.GetKeyDown(KeyCode.A)) {
            lastLeftJerk = true;
            if (toJerk && !isJerk && lastLeftJerk) {
                rb.AddForce(new Vector2(-20000, 0));
                isJerk = true;
                Invoke(nameof(StopJerk), 0.5f);
            }
            
            toJerk = true;
            Invoke(nameof(NotJerk), 0.2f);
        }

        if (Input.GetKeyDown(KeyCode.D)) {
            lastLeftJerk = false;
            if (toJerk && !isJerk && !lastLeftJerk) {
                rb.AddForce(new Vector2(20000, 0));
                isJerk = true;
                Invoke(nameof(StopJerk), 0.5f);
            }
            
            toJerk = true;
            Invoke(nameof(NotJerk), 0.2f);
        }
    }

    private void Flip() {
        facingRight = !facingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        //transform.position += new Vector3(facingRight ? -playerWidth : playerWidth, 0, 0);
    }

    private void RemoveAlreadyJump() {
        alreadyJump = true;
    }

    private void NotJerk() {
        toJerk = false;
    }

    private void StopJerk() {
        rb.velocity = new Vector2(0, rb.velocity.y);
        isJerk = false;
    }
}