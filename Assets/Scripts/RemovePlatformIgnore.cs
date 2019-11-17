using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovePlatformIgnore : MonoBehaviour {
    
    private List<Collider2D> prevColliders = new List<Collider2D>();
    public void FixedUpdate() {
        List<Collider2D> colliders = new List<Collider2D>();
        GetComponent<Collider2D>().OverlapCollider(new ContactFilter2D(), colliders);
        foreach (Collider2D collider2D in prevColliders) {
            if (collider2D != null && collider2D.gameObject != null && !colliders.Contains(collider2D) && collider2D.gameObject.tag.Equals("Platform"))
                collider2D.gameObject.GetComponent<PlatformEffector2D>().colliderMask = 2147483647;
        }

        prevColliders = colliders;
    }
}
