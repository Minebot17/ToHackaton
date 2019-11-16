using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour {
    public void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.GetComponent<Death>() != null)
            other.gameObject.GetComponent<Death>().OnDeath();
    }
}
