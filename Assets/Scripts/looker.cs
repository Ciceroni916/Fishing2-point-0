using UnityEngine;

// this is a script that attached to a tornRotator object in turret prefab.
// it rotates barrel of turrent clockwise(?), captures player if its in turret's pov and turret's barrel follows the player.
// IMPORTANT: TURRET MAY ONLY BE PLACED WITH A ANGLE OF 0,90,180,270. ANY OTHER ANGLES WILL CAUSE ISSUES WITH BARRELT ROTATION that i don't care about beucase what kind of a freak wants a diagonally angled turret????

public class looker : MonoBehaviour
{
    private Transform target;
	//after noticing player speed of change rotation in effort to follow him. hard value = 1.0f; easy value = 0.33f;
    public float speed = 1.0f;
	//passive rotation around itself
	public float rotationSpeed = 0.33f;
	//this value changed in seekplayer.cs. false = rotate around yoursef, true = turret tries to follow 
	private bool targetNoticed;
	
	void Start() {
		target = GameObject.FindWithTag("Player").transform;
		targetNoticed = false;
	}
	
	void FixedUpdate(){
		if (targetNoticed) {
			//Look at player
			// Determine which direction to rotate towards
			Vector3 targetDirection = target.position - transform.position;
			Debug.DrawRay(transform.position, targetDirection, Color.yellow);
	
			// The step size is equal to speed times frame time.
			float singleStep = speed * Time.deltaTime;
	
			// Rotate the forward vector towards the target direction by one step
			Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
			Debug.DrawRay(transform.position, newDirection, Color.blue);
	
			// Calculate a rotation a step closer to the target and applies rotation to this object
			transform.rotation = Quaternion.LookRotation(newDirection);
			
			//draw a threatening line towards player
			//worldspace
			// threat.SetPosition(0, transform.position);
			// threat.SetPosition(1, target.position);
		} else {
			//rotates clockwise
			// Vector3 rotation = transform.eulerAngles;
			// if (onWall) {
				// rotation.y = 0.0f;
				// rotation.x += rotationSpeed;
			// } else {
				// rotation.x = 0.0f;
				// rotation.y += rotationSpeed;
			// }
			// transform.eulerAngles = rotation;
			
			Quaternion startRotation = transform.rotation;
			Quaternion targetRotation;
			startRotation.eulerAngles = new Vector3(0,startRotation.eulerAngles.y,0);
			targetRotation = transform.rotation * Quaternion.Euler(0, rotationSpeed, 0);
			transform.rotation = Quaternion.Slerp(startRotation, targetRotation, 1);
			transform.rotation = targetRotation;
		}
	}
	
	public void TargetNoticed(){
		targetNoticed = true;
	}
	
	public void TargetLost(){
		//when target is lost turret snaps towards its original rotation. It would be better if it was only snapping with its .x and .z coords, but changing values in targetRotation variable does not seems to work
		Quaternion startRotation = transform.rotation;
		Vector3 parentRotation = transform.parent.rotation.eulerAngles;
		Quaternion targetRotation = Quaternion.Euler(parentRotation.x, parentRotation.y, parentRotation.z);
		transform.rotation = Quaternion.Slerp(startRotation, targetRotation, 1);
		targetNoticed = false;
	}
	
	//in case of flies
	public void BoomDisable() {
		this.enabled = false;
	}
}