using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/*
up:		0/+
down:	0/-
left:	-/0
right:	+/0

how to get towards 0 0 0 rotation:
1. If x rotation > 0, rotate.x = 1f;
else, rotate.x = 1f;
2. If y rotation > 0, rotate.y = 1f;
else, rotate.y = 1f;
2. If z rotation > 0, rotate.z = 1f;
else, rotate.z = 1f;

snap rotation:
Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.fixedDeltaTime);
rb.MoveRotation(rb.rotation * deltaRotation);

or

//use this for making things smooth
rb.maxAngularVelocity = 1.0f;
//then rotate using rb.addtorque
//somehow

or 
Vector3 relativePos = target.position - transform.position;
Quaternion rotation = Quaternion.LookRotation(Vector3.right, Vector3.up);
*/

public class CharacterControllerScript : MonoBehaviour
{
    public PlayerInput playerInput;
	public Rigidbody rb;
	
	private InputAction moveAction, spacebar, shift, z, x, q, e, lmb, rmb;
	private float moveAmount;	
	private float maxForcedAngularVelocity;

    void Start()
    {
		moveAction = playerInput.actions.FindAction("Move");
		spacebar = playerInput.actions.FindAction("Space");
		shift = playerInput.actions.FindAction("Shift");
		z = playerInput.actions.FindAction("Freeze");
		x = playerInput.actions.FindAction("Upright");
		q = playerInput.actions.FindAction("BarrelRollLeft");
		e = playerInput.actions.FindAction("BarrelRollRight");
		rmb = playerInput.actions.FindAction("RMB");
		lmb = playerInput.actions.FindAction("LMB");
		
		rb = GetComponent<Rigidbody>();
		rb.maxLinearVelocity = 50.0f;
		moveAmount = 0.01f;
    }

    void FixedUpdate()
    {
		Vector2 move = moveAction.ReadValue<Vector2>();
		move *= moveAmount;
		Vector3 passiveForce = -Physics.gravity;
		float up = spacebar.ReadValue<float>();
		float down = shift.ReadValue<float>();
		float upright = z.ReadValue<float>();
		float stopRotation = x.ReadValue<float>();
		float barrelRollLeft = q.ReadValue<float>();
		float barrelRollRight = e.ReadValue<float>();
		
		if (move.x > 0) {
			rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
		}
		if (move.x < 0) {
			rb.constraints = RigidbodyConstraints.None;
		}
		if (move.y > 0) {
			// rb.AddForce(transform.forward);	
		}
		if (move.y < 0) {
			// rb.AddForce(transform.forward * -1f);	
		}
		if (down > 0) {
			passiveForce = new Vector3(0,0,0);
		}
		if (up > 0) {
			passiveForce *= 2.0f;
		}
		if (barrelRollLeft > 0) {
			rb.AddRelativeTorque(new Vector3(0,0,0.05f));
		}
		if (barrelRollRight > 0) {
			rb.AddRelativeTorque(new Vector3(0,0,-0.05f));
		}
		if (upright > 0) {
			//todo: button that sets rotation to 0,0,0 and sets drone upright
		}
		if (stopRotation > 0) {
			Vector3 curRot = rb.angularVelocity;
			Vector3 rotation = new Vector3(0,0,0);
			
			//it speed within 0.1 to -0.1 do nothing
			if (curRot.x > 0.1 || curRot.x < -0.1) {
				rotation.x = (curRot.x < 0) ? 0.01f : -0.01f;
			}
			if (curRot.y > 0.1 || curRot.y < -0.1) {
				rotation.y = (curRot.y < 0) ? 0.01f : -0.01f;
			}
			if (curRot.z > 0.1 || curRot.z < -0.1) {
				rotation.z = (curRot.z < 0) ? 0.01f : -0.01f;
			}
			rb.AddTorque(rotation);
		}
		//passively push drone upwards
		rb.AddRelativeForce(passiveForce, ForceMode.Acceleration);
    }
}
