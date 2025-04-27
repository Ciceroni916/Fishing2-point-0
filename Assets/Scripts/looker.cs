using UnityEngine;

// this is a script that attached to a tornRotator object in turret prefab.
// it rotates barrel of turrent clockwise(?), captures player if its in turret's pov and turret's barrel follows the player.
// IMPORTANT: TURRET MAY ONLY BE PLACED WITH A ANGLE OF 0,90,180,270. ANY OTHER ANGLES WILL CAUSE ISSUES WITH BARRELT ROTATION that i don't care about beucase what kind of a freak wants a diagonally angled turret????

public class looker : MonoBehaviour
{
    public Transform target;
	//after noticing player speed of looking at him. does nothing ;c
    public float speed = 1.0f;
	//passive rotation around itself
	public float rotationSpeed = 1.0f;
	public LineRenderer threat;
	
	private bool targetNoticed, isAngled;
	
	void Start() {
		targetNoticed = false;
		if ((transform.eulerAngles.z > 89.0f && transform.eulerAngles.z < 91) || (transform.eulerAngles.z > 269 && transform.eulerAngles.z < 271)) {
			//turret in | position
			isAngled = true;
		} else {
			//turret in - position
			isAngled = false;
		}
	}
	
	void FixedUpdate(){
		Vector3 targetDirection = target.position - transform.position;
		if (targetNoticed) {
			//Look at player
			// Determine which direction to rotate towards
			//Vector3 targetDirection = target.position - transform.position;
	
			// The step size is equal to speed times frame time.
			float singleStep = speed * Time.deltaTime;
	
			// Rotate the forward vector towards the target direction by one step
			Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
	
			// Draw a ray pointing at our target in
			Debug.DrawRay(transform.position, newDirection, Color.red);
	
			// Calculate a rotation a step closer to the target and applies rotation to this object
			transform.rotation = Quaternion.LookRotation(newDirection);
			
			//draw a threatening line towards player
			threat.SetPosition(0, transform.position);
			threat.SetPosition(1, target.position);
		} else {
			//rotates clockwise
			Vector3 rotation = transform.eulerAngles;
			if (!isAngled) {
				//turret is angled vertically
				rotation.x = 0.0f;
				rotation.y += rotationSpeed;
			} else {
				rotation.y = 0.0f;
				rotation.x += rotationSpeed;
			}
			transform.eulerAngles = rotation;
		}
	}
	
	public void TargetNoticed(){
		targetNoticed = true;
	}
	
	public void TargetLost(){
		targetNoticed = false;
	}
	
	//in case of flies
	public void BoomDisable() {
		this.enabled = false;
	}
}