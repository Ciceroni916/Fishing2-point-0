using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script is called via broadcast message from boom_sphere and tells turret to unparent all objects and disable all scripts.
public class dissasembler : MonoBehaviour
{
	void Disassemble(Vector3 boomSpherePos) {
		foreach (Transform child in transform) {
			if (child.TryGetComponent<Rigidbody>(out Rigidbody rb)) {
				rb.isKinematic = false;
			}
		}
		transform.DetachChildren();
	}
}
