using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script is called via broadcast message from boom_sphere and tells turret to unparent all objects and disable all scripts.
public class dissasembler : MonoBehaviour
{
	private float deathTimer, liveTimer;
	
	void Awake() {
		liveTimer = 3.0f;
	}
	
	void Disassemble(Vector3 boomSpherePos) {
		//1. Make every rigidbody-containing part of turret affected by unity physics.
		foreach (Transform child in transform) {
			if (child.TryGetComponent<Rigidbody>(out Rigidbody rb)) {
				rb.isKinematic = false;
			}
		}
		//2. Disable all scripts by calling a function within them.
		this.BroadcastMessage("BoomDisable",null,SendMessageOptions.DontRequireReceiver);
		//2. In 3 seconds, end it all.
		deathTimer = Time.time;
		this.enabled = true;
	}
	
	void Update() {
		if (Time.time > deathTimer + liveTimer) {
			while (transform.childCount > 0) {
				DestroyImmediate(transform.GetChild(0).gameObject);
			}
			Destroy(this.gameObject);
		}
	}
}
