using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeMob : MonoBehaviour {
    public float speed = 0.5f;
    public float ammoSpeed = 5f;
    public int damage = 3;
    public float radiusVision = 100;
    public Collider2D forwardCollider;
    public Collider2D downCollider;
    public GameObject ammoPrefab;

    private Vector3 fireVector;
    private bool canFire = true;
    private static System.Random rnd = new System.Random();
    
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
        
        if (state != 0) {
            int side = transform.localScale.x > 0 ? 2 : 1;
            if (side == state)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

            if (!PlayerController.IsTouchingLayers(forwardCollider, 8) && PlayerController.IsTouchingLayers(downCollider, 8))
                transform.position += new Vector3(speed * (state == 1 ? 1 : -1), 0, 0);
        }

        if (canFire) {
            Vector3 vector = (PlayerController.Singleton.transform.position - transform.position).normalized;
            RaycastHit2D fireRay = Physics2D.Raycast(transform.position + vector * 80f, vector, radiusVision);
            if (fireRay.collider != null && fireRay.collider.gameObject.layer == 9) {
                canFire = false;
                fireVector = vector;
                GetComponent<Animator>().SetTrigger("Attack");
            }
        }
    }

    public void DamagePlayer() {
        GameObject ammo = Object.Instantiate(ammoPrefab);
        ammo.transform.position = transform.position + new Vector3(0, 16);
        ammo.GetComponent<Ammo>().Init(damage, /*Quaternion.Euler(0, 0, rnd.Next(10)-5) * */fireVector, ammoSpeed);
        Invoke(nameof(CanFire), 0.5f);
    }

    private void CanFire() {
        canFire = true;
    }
}
