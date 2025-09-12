using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

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
	public TMP_Text droneFrozen;
	public Transform barrel;
	public float shootSpeed, moveSpeed = 2.0f;
	public GameObject BOOM, MakeshiftUILinebreaker, gameCanvas, pauseCanvas, gameOverScreen, victoryScreen;
	public Camera cam;
	public AudioSource droneFanSound, droneShootSound, droneReloadSound;
	public float fanPitchNormal = 1.0f, fanPitchHigh = 1.5f, fanPitchLow = 0.5f, fanPitchNegative = 0.25f;
	
	private bool shooting, tutorial, sound;
	private Rigidbody rb;
	private InputAction spacebar, shift, z, x, q, e, lmb, rmb, esc;
	private float maxForcedAngularVelocity, shootingStartTime, shootingEndTime, shootLength;
	public static float globalVolume = 1.0f, standardVolumeFan, standardVolumeShoot, standardVolumeReload;
	private Vector3 shootBeginPosition, shootEndPosition;

    void OnEnable()
    {
		//no kings, no gods.
		if (shootSpeed > 5 || shootSpeed <= 0.1f) {
			shootSpeed = 5f;
		}
		if (moveSpeed < 2.0f) {
			moveSpeed = 2.0f;
		}
		tutorial = false;
		//not yet ferb
		shooting = false;
		
		sound = transform.parent.GetComponent<UIController>().sound;
		
		spacebar = playerInput.actions.FindAction("Space");
		shift = playerInput.actions.FindAction("Shift");
		z = playerInput.actions.FindAction("Freeze");
		x = playerInput.actions.FindAction("Thaw");
		q = playerInput.actions.FindAction("BarrelRollLeft");
		e = playerInput.actions.FindAction("BarrelRollRight");
		rmb = playerInput.actions.FindAction("RMB");
		lmb = playerInput.actions.FindAction("LMB");
		esc = playerInput.actions.FindAction("ESC");
		
		rb = GetComponent<Rigidbody>();
		rb.maxLinearVelocity = 50.0f;
		
		//drone fan sound
		if (sound) {
			droneFanSound.enabled = true;
			droneShootSound.enabled = true;
			droneReloadSound.enabled = true;
			standardVolumeFan = droneFanSound.volume;
			standardVolumeShoot = droneShootSound.volume;
			standardVolumeReload = droneReloadSound.volume;
		} else {
			droneFanSound.enabled = false;
			droneShootSound.enabled = false;
			droneReloadSound.enabled = false;
		}
    }

    void FixedUpdate()
    {
		Vector3 passiveForce = -Physics.gravity;
		float up = spacebar.ReadValue<float>();
		float down = shift.ReadValue<float>();
		float upright = z.ReadValue<float>();
		float freeze = z.ReadValue<float>();
		float thaw = x.ReadValue<float>();
		float barrelRollLeft = q.ReadValue<float>();
		float barrelRollRight = e.ReadValue<float>();
		float LMB = lmb.ReadValue<float>();
		float pause = esc.ReadValue<float>();
		
		//standard fan pitch
		if (sound) {
			droneFanSound.pitch = fanPitchLow;
			droneFanSound.volume = standardVolumeFan * globalVolume;
		}
		if (barrelRollLeft > 0) {
			//flat
			rb.AddRelativeTorque(new Vector3(0,0,0.05f));
			if (sound) droneFanSound.pitch = fanPitchNormal;
		}
		if (barrelRollRight > 0) {
			rb.AddRelativeTorque(new Vector3(0,0,-0.05f));
			if (sound) droneFanSound.pitch = fanPitchNormal;
		}
		if (down > 0) {
			passiveForce *= -moveSpeed;
			if (sound) droneFanSound.pitch = fanPitchNegative;
		}
		if (up > 0) {
			passiveForce *= moveSpeed;
			if (sound) droneFanSound.pitch = fanPitchHigh;
		}
		if (freeze > 0 && tutorial) {
			rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
			droneFrozen.gameObject.SetActive(true);
		}
		if (thaw > 0 && tutorial) {
			rb.constraints = RigidbodyConstraints.None;
			droneFrozen.gameObject.SetActive(false);
		}
		if (pause > 0) {
			Time.timeScale = 0;
			gameCanvas.SetActive(false);
			MakeshiftUILinebreaker.SetActive(false);
			pauseCanvas.SetActive(true);
		}
		
		//shooting sequence
		if (LMB > 0 && !shooting) {
			//if not shooting, initiate sequence
			OpenFire();
			CreateVisualExplosionsEffect();
			if (sound) {
				droneShootSound.volume = standardVolumeShoot * globalVolume;
				droneShootSound.Play();
			}
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
			if (sound) {
				droneReloadSound.volume = standardVolumeReload * globalVolume;
				droneReloadSound.Play();
			}
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
	
	//receiver for tutorila menu button
	private void Tutorial() {
		rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
		droneFrozen.gameObject.SetActive(true);
		tutorial = true;
		moveSpeed = 2.0f;
	}
	
	private void FreezeDrone() {
		rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
		droneFrozen.gameObject.SetActive(true);
	}
	
	//received from easy button
	private void Easy() {
		rb.constraints = RigidbodyConstraints.None;
		tutorial = false;
		droneFrozen.gameObject.SetActive(false);
		moveSpeed = 2.0f;
	}
	
	private void Hard() {
		rb.constraints = RigidbodyConstraints.None;
		tutorial = false;
		droneFrozen.gameObject.SetActive(false);
		moveSpeed = 4.0f;
	}
	
	private void GameOverSequenceDeathZone() {
		gameOverScreen.SetActive(true);
		gameOverScreen.BroadcastMessage("SetGameOverReason", "Connection with drone reached 100% packet loss.\nDrone went outside connection zone.\nGame Over.");
		transform.parent.gameObject.SetActive(false);
	}
	
	private void GameOverSequence(GameObject turret) {
		gameOverScreen.SetActive(true);
		Vector3 pos = turret.transform.position;
		gameOverScreen.BroadcastMessage("SetGameOverReason", "Connection with drone reached 100% packet loss.\nLast intercepted message: \"Target destroyed\". \nMessage origin: " + pos.x + " " + pos.y + " " + pos.z + "\nGame Over.");
		transform.parent.gameObject.SetActive(false);
	}
	
	private void VictorySequence() {
		victoryScreen.SetActive(true);
		transform.parent.gameObject.SetActive(false);
	}
	
	private void VolumeChanged(float newVolume) {
		globalVolume = newVolume;
	}
}
