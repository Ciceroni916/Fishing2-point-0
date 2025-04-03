using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine;

//todo: energy weapon charges from drifring near walls, walls are electrified and heavily throw back drone when its touching walls

public class cameraRotator : MonoBehaviour
{
	public PlayerInput playerInput;
	public bool sound;
	public AudioSource cameraRot;
	
	private InputAction rmb, mmb;
	//cameraPostPity is amount of frames script still plays sound of camera rotating after player has stopped rotating camera. Its purpose is to make that sound smoother and keep playing it even when there is a slight pause between changeing rotations of camera.
	private int cameraPostPity;
	private Quaternion oldRot;
	
    // Start is called before the first frame update
    void Start()
    {
        rmb = playerInput.actions.FindAction("RMB");
		mmb = playerInput.actions.FindAction("MMB");
		cameraRot.volume = 0.0f;
		cameraPostPity = 0;
		oldRot = gameObject.transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 targetMouseDelta = Mouse.current.delta.ReadValue()*Time.smoothDeltaTime;
		float RMB = rmb.ReadValue<float>();
		float MMB = mmb.ReadValue<float>();
		float mult = 150f;
		Vector3 rot = gameObject.transform.localEulerAngles;
		if (RMB > 0.0f) {
			rot.y += targetMouseDelta.x*mult;
			rot.x += targetMouseDelta.y*-mult;
			//rotation of camera is limited to this number
			float rad = 50f;
			Vector2 rodOff = new Vector2(rot.x, rot.y);
			//there is an issue with drawing circle: local euler angles looks at 0,0 but -1,-1 becomes 359,359. For proper geometric calculations range must be -180,-180 to 180,180
			if (rodOff.x + 180 >= 360) rodOff.x -= 360;
			if (rodOff.y + 180 >= 360) rodOff.y -= 360;
			// If player attempts to look turn camera too much to the side this check won't let him
			Debug.Log(rodOff.y);
			// if (rodOff.y < 
			if ((rodOff.x) * (rodOff.x) + (rodOff.y) * (rodOff.y) <= rad * rad) {
				gameObject.transform.localEulerAngles = rot;
			}
			//todo: when offset value is out of circle create negatively looking visual effects that mask my inability to snap camera to the point on circle instead of fully negating camera movement.
		} else if (gameObject.transform.localEulerAngles != Vector3.zero) {
			//note: could be issues with warping cursor if ANYTHING else changes camera rotation
			gameObject.transform.localEulerAngles = Vector3.zero;
			float sw = Screen.width / 2;
			//let's be honest this check won't help anyone
			if (Input.mousePosition.x != sw) {
				Mouse.current.WarpCursorPosition(new Vector2(sw, Screen.height / 2));
			}
		}
    }
	
	void FixedUpdate() {
		if (sound) {
			Quaternion rot = gameObject.transform.localRotation;
			if (Quaternion.Angle(rot, oldRot) > 2.0f) {
				cameraPostPity = 10;
			}
			if  (cameraPostPity > 0) {
				cameraRot.volume = 0.35f;
				cameraPostPity--;
			} else {
				cameraRot.volume = 0.0f;
			}
			oldRot = rot;
		}
	}
}
