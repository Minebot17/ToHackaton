using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public static PlayerController Singleton;
    public float maxVelocityY = 100f;
    public float maxSpeed = 10f;
    public float jumpForce = 700f;
    public int gravityChangeTicks = 1000;
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
    private bool attackCD;
    private bool laserCD;
    private GameObject lineRenderer;
    private bool gravityCD = false;
    private int gravityTimer;

    public void Start() {
        Singleton = this;
        rb = GetComponent<Rigidbody2D>();
        countOfJumps = maxJumpsAvaible;
        Gravitation.mobs.Add(this.transform);
        PlayerDeath.isDead = false;
        gravityTimer = gravityChangeTicks;
        if (LevelGenerator.Singleton == null)
            maxJumpsAvaible = 1;
        else
            maxJumpsAvaible = LevelGenerator.Singleton.CurrentPhase != 0 ? 2 : 1;
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
                if (!alreadyJump)
                    countOfJumps = maxJumpsAvaible;
            }

            if (col.gameObject.tag.Equals("JumpPlatfrom"))
                onJumpPlatfrom = true;
            if (col.gameObject.tag.Equals("Platform"))
                platfrom = col.gameObject;
        }

        if (((countOfJumps == maxJumpsAvaible && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space))) || (countOfJumps > 0 && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)))) && !alreadyJump) {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0f, (onJumpPlatfrom ? jumpForce * 1.5f : jumpForce) * (Gravitation.gravityUp ? -1 : 1)));
            countOfJumps--;
            alreadyJump = true;
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
        
        GetComponent<Animator>().SetBool("Run", Mathf.Abs(move) > 0.1f);
        GetComponent<Animator>().SetBool("Jump", !grounded);
        if (Input.GetMouseButtonDown(0) && !attackCD && !laserCD && LevelGenerator.Singleton.CurrentPhase != 1) {
            GetComponent<Animator>().SetTrigger("Attack");
            attackCD = true;
        }
        
        if (Input.GetMouseButton(1) && !laserCD && !attackCD && LevelGenerator.Singleton.CurrentPhase != 2) {
            Vector3 mousePosition = GameObject.Find("Main Camera").GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
            Vector3 toMouse = (mousePosition - transform.position).normalized*64f;
            Vector3 toLaser;
            toMouse = new Vector3(toMouse.x, toMouse.y, -1);
            Vector3 xVec = transform.localScale.x > 0 ? Vector3.right : Vector3.left;
            if (toMouse.x > 0 != transform.localScale.x > 0)
                Flip();
            float angle = Vector3.Angle(xVec, toMouse);
            if (angle < 40) {
                GetComponent<Animator>().SetTrigger("AttackCenter");
                toLaser = toMouse.x > 0 ? Vector3.right : Vector3.left;
            }
            else if (angle > 40 && toMouse.y > 0) {
                GetComponent<Animator>().SetTrigger("AttackUp");
                toLaser = toMouse.x > 0 ? new Vector3(Mathf.Cos(52f * (Mathf.PI / 180f)), Mathf.Sin(52f * (Mathf.PI / 180f))) : new Vector3(-Mathf.Cos(52f * (Mathf.PI / 180f)), Mathf.Sin(52f * (Mathf.PI / 180f)));
            }
            else {
                GetComponent<Animator>().SetTrigger("AttackDown");
                toLaser = toMouse.x > 0 ? new Vector3(Mathf.Cos(52f * (Mathf.PI / 180f)), -Mathf.Sin(52f * (Mathf.PI / 180f))) : new Vector3(-Mathf.Cos(52f * (Mathf.PI / 180f)), -Mathf.Sin(52f * (Mathf.PI / 180f)));
            }

            laserCD = true;
            Invoke(nameof(RemoveLine), 0.6f);
            lineRenderer = new GameObject("line");
            RaycastHit2D ray = Physics2D.Raycast(transform.position + toLaser*64f, toLaser, 600, 1 << 10);
            lineRenderer.AddComponent<LineRenderer>().SetPositions(new Vector3[] { transform.position + toLaser*64f, ray.collider != null ? new Vector3(ray.point.x, ray.point.y, -3) : toLaser * 640f + transform.position });
            lineRenderer.GetComponent<LineRenderer>().SetWidth(2, 2);
            lineRenderer.GetComponent<LineRenderer>().SetColors(Color.red, Color.red);
            if (ray.collider != null && ray.collider.gameObject.GetComponent<Health>() != null)
                ray.collider.gameObject.GetComponent<Health>().Damage(3);
        }

        if (LevelGenerator.Singleton.CurrentPhase == 2) {
            gravityTimer--;
            if (gravityTimer < 0) {
                gravityTimer = gravityChangeTicks;
                Gravitation.changeGravity();
            }
        }
    }

    private void Update() {
        if (LevelGenerator.Singleton.CurrentPhase == 0 && !gravityCD && Input.GetKeyDown(KeyCode.Q)) {
            Gravitation.changeGravity();
            gravityCD = true;
            Invoke(nameof(RemoveGravityCD), 5f);
        }

        if (LevelGenerator.Singleton.CurrentPhase == 2) {
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
    }

    private void Flip() {
        facingRight = !facingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        //transform.position += new Vector3(facingRight ? -playerWidth : playerWidth, 0, 0);
    }

    private void RemoveGravityCD() {
        gravityCD = false;
    }

    private void RemoveLine() {
        if (lineRenderer != null) {
            Destroy(lineRenderer);
            lineRenderer = null;
        }

        laserCD = false;
    }

    private void RemoveAlreadyJump() {
        alreadyJump = false;
    }

    private void NotJerk() {
        toJerk = false;
    }

    private void StopJerk() {
        rb.velocity = new Vector2(0, rb.velocity.y);
        isJerk = false;
    }

    public static bool IsTouchingLayers(Collider2D collider, int layer) {
        List<Collider2D> colliders = new List<Collider2D>();
        collider.OverlapCollider(new ContactFilter2D(), colliders);
        foreach (Collider2D col in colliders)
            if (col.gameObject.layer == layer)
                return true;
        return false;
    }
    
    public void Attack() {
        List<Collider2D> colliders = new List<Collider2D>();
        Physics2D.OverlapCircle(transform.position + new Vector3((transform.localScale.x < 0 ? -1 : 1) * 64, 0, 0),10, new ContactFilter2D(), colliders);
        foreach (Collider2D collider2D1 in colliders) {
            if (collider2D1.gameObject.layer == 10)
                collider2D1.gameObject.GetComponent<Health>().Damage(5);
        }

        attackCD = false;
    }
}