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
	public Transform barrel;
	public float shootSpeed, moveSpeed = 2.0f;
	public GameObject BOOM, gameCanvas, pauseCanvas;
	public Camera cam;
	
	private bool shooting;
	private Rigidbody rb;
	private InputAction moveAction, spacebar, shift, z, x, q, e, lmb, rmb, esc;
	private float moveAmount, maxForcedAngularVelocity, shootingStartTime, shootingEndTime, shootLength ;
	private Vector3 shootBeginPosition, shootEndPosition;

    void Start()
    {
		//no kings, no gods.
		if (shootSpeed > 5 || shootSpeed <= 0.1f) {
			shootSpeed = 5f;
		}
		if (moveSpeed < 2.0f) {
			moveSpeed = 2.0f;
		}
		//not yet ferb
		shooting = false;
		
		moveAction = playerInput.actions.FindAction("Move");
		spacebar = playerInput.actions.FindAction("Space");
		shift = playerInput.actions.FindAction("Shift");
		z = playerInput.actions.FindAction("Freeze");
		x = playerInput.actions.FindAction("Upright");
		q = playerInput.actions.FindAction("BarrelRollLeft");
		e = playerInput.actions.FindAction("BarrelRollRight");
		rmb = playerInput.actions.FindAction("RMB");
		lmb = playerInput.actions.FindAction("LMB");
		esc = playerInput.actions.FindAction("ESC");
		
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
		float LMB = lmb.ReadValue<float>();
		float pause = esc.ReadValue<float>();
		
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
			passiveForce *= moveSpeed;
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
		if (pause > 0) {
			Time.timeScale = 0;
			gameCanvas.SetActive(false);
			pauseCanvas.SetActive(true);
		}
		
		//shooting sequence
		if (LMB > 0 && !shooting) {
			//if not shooting, initiate sequence
			OpenFire();
			CreateVisualExplosionsEffect();
		}
		if (shooting) {
			//If already shooting, continue sequence
			Shoot();
		}
		
		//passively push drone upwards
		rb.AddRelativeForce(passiveForce, ForceMode.Acceleration);
    }
	
	//initiate shooting sequence
	private void OpenFire() {
		shootingStartTime = Time.time;
		shootBeginPosition = barrel.transform.localPosition;
		shootEndPosition = barrel.transform.localPosition;
		shootEndPosition.y -= 1.6f;
		shootLength = Vector3.Distance(shootBeginPosition, shootEndPosition);
		//GO
		shooting = true;
	}
	
	//continue shooting sequence
	private void Shoot(){
        float distCovered = (Time.time - shootingStartTime) * shootSpeed;
		float fractionOfJourney = distCovered / shootLength;
		if (fractionOfJourney < 0.01) {
			//1. Push barrel back.
			barrel.localPosition = Vector3.Lerp(shootBeginPosition, shootEndPosition, fractionOfJourney * 100);
		} else {
			//2. Push barrel forward.
			barrel.localPosition = Vector3.Lerp(shootEndPosition, shootBeginPosition, fractionOfJourney);
		}
		if (fractionOfJourney > 1.0f) {
			shooting = false;
			return;
		}
	}
	
	//makes things LOOKS like they go boom
	private void CreateVisualExplosionsEffect() {
		RaycastHit hit;
		LayerMask layerMask = LayerMask.GetMask("Terrain", "Enemy");
		if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 100f, layerMask)) {
			GameObject currentParticleSystem = Instantiate(BOOM, hit.point, new Quaternion(0,0,0,0));
		}
	}
}
