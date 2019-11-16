using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {
    public int health;
    public int maxHealth;

    public int Heal(int heal) {
        int preHealth = health;
        health += heal;
        if (health > maxHealth)
            health = maxHealth;
        return health - preHealth;
    }

    public int Damage(int damage) {
        int preDamage = health;
        health -= damage;
        if (health <= 0) {
            health = 0;
            if (GetComponent<Death>() != null)
                GetComponent<Death>().OnDeath();
        }
        return health - preDamage;
    }
}
