using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravitation
{
    public static List<Transform> mobs = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void changeGravity()
    {
        Physics2D.gravity = new Vector2(0, -Physics2D.gravity.y);
        mobs.ForEach(t => t.localScale = new Vector3(t.localScale.x, -t.localScale.y, t.localScale.z));
    }
}
