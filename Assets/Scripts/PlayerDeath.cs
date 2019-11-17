using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : Death {

    public static bool isDead = false;
    public override void OnDeath() {
        if (!isDead) {
            if (Gravitation.gravityUp)
                Gravitation.changeGravity();
            Gravitation.mobs.Clear();
            isDead = true;
            Destroy(gameObject);
            LevelGenerator.Singleton.CurrentPhase++;
            LevelGenerator.Singleton.GenerateLevel(LevelGenerator.Singleton.CurrentPhase);
        }
    }
}
