using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearMob : MonoBehaviour {

    public float speed = 0.5f;
    public int damage = 3;
    public float radiusVision = 100;
    public Collider2D forwardCollider;
    public Collider2D downCollider;

    public void Start() {
        Gravitation.mobs.Add(transform);
    }

    public void OnDestroy() {
        Gravitation.mobs.Remove(transform);
    }

    public void FixedUpdate() {
        int state = 0;
        RaycastHit2D ray = Physics2D.Raycast(transform.position + new Vector3(-45, 0, 0), Vector2.left, radiusVision);
        if (ray.collider != null && ray.collider.gameObject.layer == 9)
            state = 1;
        if (state == 0) {
            ray = Physics2D.Raycast(transform.position + new Vector3(45, 0, 0), Vector2.right, radiusVision);
            if (ray.collider != null && ray.collider.gameObject.layer == 9)
                state = 2;
        }

        GetComponent<Animator>().SetBool("Run", state != 0);
        if (state != 0) {
            int side = transform.localScale.x > 0 ? 2 : 1;
            if (side != state)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

            if (ray.distance < 8) 
                GetComponent<Animator>().SetTrigger("Attack");
            else if (!PlayerController.IsTouchingLayers(forwardCollider, 8) && PlayerController.IsTouchingLayers(downCollider, 8))
                transform.position += new Vector3(speed * (state == 1 ? -1 : 1), 0, 0);
        }
    }

    public void DamagePlayer() {
        List<Collider2D> colliders = new List<Collider2D>();
        Physics2D.OverlapCircle(transform.position + new Vector3((transform.localScale.x < 0 ? -1 : 1) * 48, 0, 0),10, new ContactFilter2D(), colliders);
        foreach (Collider2D collider2D1 in colliders) {
            if (collider2D1.gameObject.layer == 9)
                collider2D1.gameObject.GetComponent<Health>().Damage(damage);
        }
    }
}
