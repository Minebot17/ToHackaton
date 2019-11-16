using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour {
    public virtual void OnDeath() {
        Destroy(gameObject);
    }
}
