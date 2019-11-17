using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {
    public int health;
    public int maxHealth;
    public GameObject healthBar;
    public bool isPlayer;
    private long lastDamage;

    public void Start() {
        UpdateHealthBar(health);
    }

    public int Heal(int heal) {
        int preHealth = health;
        health += heal;
        if (health > maxHealth)
            health = maxHealth;
        UpdateHealthBar(health);
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
        UpdateHealthBar(health);
        return health - preDamage;
    }

    private void UpdateHealthBar(int newHealth) {
        if (healthBar != null) {
            healthBar.transform.GetChild(0).GetComponent<SpriteRenderer>().size = new Vector2((int)(newHealth/(float)maxHealth*62f), 6);
            if (!isPlayer) {
                healthBar.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
                lastDamage = System.DateTime.Now.Ticks;
                Invoke(nameof(HideHealthBar), 2f);
            }
            else
                healthBar.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    private void HideHealthBar() {
        if (System.DateTime.Now.Ticks - lastDamage > 15000000)
            healthBar.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
    }
}
