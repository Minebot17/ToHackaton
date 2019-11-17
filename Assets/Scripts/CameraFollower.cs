using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour {

	public static CameraFollower Singleton;
	public float speed;
	
	private GameObject target;
	private Vector2 max;
	
	public void Start() {
		Singleton = this;
	}

	public void SetTarget(GameObject target, Vector2 max) {
		this.target = target;
		this.max = max - new Vector2(640, 360);
	}

	private void FixedUpdate() {
		if (target == null)
			return;
		
		Vector2 min = new Vector2(640, 360);
		if (transform.position != target.transform.position) {
			Vector2 delta = ((target.transform.position - transform.position) * speed) + transform.position;
			
			if (delta.x < min.x)
				delta.x = min.x;
			else if (delta.x > max.x)
				delta.x = max.x;
			
			if (delta.y < min.y)
				delta.y = min.y;
			else if (delta.y > max.y)
				delta.y = max.y;

			if (transform.position != new Vector3(delta.x, delta.y, transform.position.z))
				transform.position = new Vector3(delta.x, delta.y, transform.position.z);
		}
	}
}
