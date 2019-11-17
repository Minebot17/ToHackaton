using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : Death {
    public override void OnDeath() {
        Destroy(gameObject);
        LevelGenerator.Singleton.CurrentPhase++;
        LevelGenerator.Singleton.GenerateLevel( LevelGenerator.Singleton.CurrentPhase);
    }
}
