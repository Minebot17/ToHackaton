using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevelTrigger : MonoBehaviour {
    private List<Collider2D> prevColliders = new List<Collider2D>();
    public void FixedUpdate() {
        List<Collider2D> colliders = new List<Collider2D>();
        GetComponent<Collider2D>().OverlapCollider(new ContactFilter2D(), colliders);
        foreach (Collider2D collider2D in prevColliders) {
            if (collider2D.gameObject.layer == 9) {
                if (Gravitation.gravityUp)
                    Gravitation.changeGravity();
                Gravitation.mobs.Clear();
                Destroy(collider2D.gameObject);
                Destroy(gameObject);
                LevelGenerator.Singleton.CurrentPhase--;
                LevelGenerator.Singleton.GenerateLevel( LevelGenerator.Singleton.CurrentPhase);
            }
        }

        prevColliders = colliders;
    }
}
