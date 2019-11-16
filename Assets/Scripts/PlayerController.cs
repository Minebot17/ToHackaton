using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float maxSpeed = 10f;
    public float jumpForce = 700f;
    public Collider2D groundCollider;
    public Collider2D forwardCollider;
    public Collider2D backCollider;
    private bool facingRight = true;
    private Rigidbody2D rb;
    private bool grounded;
    private bool onJumpPlatfrom;
    private bool onPlatfrom;
    private float move;
    
    public void Start() {
        rb = GetComponent<Rigidbody2D>();
    }
    
    public void FixedUpdate() {
        grounded = false;
        onJumpPlatfrom = false;
        onPlatfrom = false;
        List<Collider2D> colliders = new List<Collider2D>();
        groundCollider.OverlapCollider(new ContactFilter2D(), colliders);
        foreach (Collider2D col in colliders) {
            if (col.gameObject.layer == 8)
                grounded = true;
            if (col.gameObject.tag.Equals("JumpPlatfrom"))
                onJumpPlatfrom = true;
            if (col.gameObject.tag.Equals("Platform"))
                onPlatfrom = true;
        }
        
        if (grounded && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)))
            rb.AddForce(new Vector2(0f, onJumpPlatfrom ? jumpForce*1.5f : jumpForce));

        if (grounded && onPlatfrom && Input.GetKeyDown(KeyCode.S)) {
            gameObject.layer = 9;
            Invoke(nameof(ReturnLayer), 1);
        }

        move = Input.GetAxis("Horizontal");
        Collider2D colliderToCheck = facingRight ? (move > 0 ? forwardCollider : backCollider) : (move > 0 ? backCollider: forwardCollider);
        if (!colliderToCheck.IsTouchingLayers(8))
            rb.velocity = new Vector2(move * maxSpeed, rb.velocity.y);

        if (move > 0 && !facingRight)
            Flip();
        else if (move < 0 && facingRight)
            Flip();
    }

    private void Flip() {
        facingRight = !facingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        //transform.position += new Vector3(facingRight ? -playerWidth : playerWidth, 0, 0);
    }

    private void ReturnLayer() {
        gameObject.layer = 0;
    }
}