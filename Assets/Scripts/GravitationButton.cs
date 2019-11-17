using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitationButton : MonoBehaviour {
    public void FixedUpdate() {
        if (Input.GetKeyDown(KeyCode.Q) && GetComponent<BoxCollider2D>().IsTouchingLayers(9)) {
            Gravitation.changeGravity();
        }
    }
}
