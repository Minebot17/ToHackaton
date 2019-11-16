using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour {

    public int damageEveryTick = 50;
    public int damageValue = 1;
    public Collider2D trigger;
    private int timer;
    
    public void Start() {
        Collider2D[] allColliders = GetComponents<Collider2D>();
        timer = damageEveryTick;
    }

    public void FixedUpdate() {
        timer--;
        if (timer <= -1) {
            timer = damageEveryTick;
            List<Collider2D> colliders = new List<Collider2D>();
            trigger.OverlapCollider(new ContactFilter2D(), colliders);
            foreach (Collider2D collider in colliders) {
                if (collider.GetComponent<Health>() != null)
                    collider.GetComponent<Health>().Damage(damageValue);
            }
        }
    }
}
