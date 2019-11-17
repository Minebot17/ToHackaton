using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour {

    private float speed;
    private int damage;
    private Vector3 vector;

    public void Init(int damage, Vector3 vector, float speed) {
        this.damage = damage;
        this.vector = vector;
        this.speed = speed;
        float angle = Vector3.Angle(Vector3.right, vector);
        transform.localEulerAngles = new Vector3(0, 0, vector.y > 0 ? angle : -angle);
    }

    public void FixedUpdate() {
        if (damage == 0)
            return;
        
        if (PlayerController.IsTouchingLayers(GetComponent<Collider2D>(), 8))
            Destroy(gameObject);
        else if (PlayerController.IsTouchingLayers(GetComponent<Collider2D>(), 9)) {
            PlayerController.Singleton.GetComponent<Health>().Damage(damage);
            Destroy(gameObject);
        }
        else transform.position += vector * speed;
    }
}
