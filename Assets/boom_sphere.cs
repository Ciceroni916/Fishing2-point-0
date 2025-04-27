using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script is part of shooting system.
//after shooting sequence (charactercontrollerscipt) instantiates this script it will expand sphere and any enemy (or player) that got touched by this sphere's collider-trigger
//will be launched away from center of this sphere.
//todo: check for los to stop wallbang
//todo: push down effects camera to not see effects that are blocked by upper/lower blockers on drone
public class boom_sphere : MonoBehaviour
{
	private SphereCollider col;
	private float startTime, deathTimer;
	private Vector3 trScale, trIterator;
	private List<int> idArray;
	
    // Start is called before the first frame update
    void Awake()
    {
		col = GetComponent<SphereCollider>();
		startTime = Time.fixedTime;
		trScale = new Vector3(1f,1f,1f);
		trIterator = new Vector3(0.35f,0.35f,0.35f);
		idArray = new List<int>();
		deathTimer = 0.5f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		// in africa every deathTimer seconds a boom_sphere dies. together we can stop this.
		if (Time.fixedTime > startTime + deathTimer) {
			Destroy(this.gameObject);
		}
		//OUR INFLUENCE EXPANDED
		trScale += trIterator;
        transform.localScale = trScale;
    }
	
	//disassemble.cs unparents all childen and disables their rb-s kinematic mode so that turret would exploded without launching itself in unpredictable directions
	void OnTriggerEnter(Collider collision) {
		// if (
		//todo: only check/add parent that is first from above
		//preventing pushing one object more then once.
		int id = collision.transform.gameObject.GetInstanceID();
		GameObject colGO = collision.transform.gameObject;
		if (!idArray.Contains(id)) {
			idArray.Add(id);
			//telling turrent to dissassemble itself
			colGO.SendMessageUpwards("Disassemble",transform.position,SendMessageOptions.DontRequireReceiver);
			Vector3 colPos = collision.transform.position;
			collision.attachedRigidbody.AddTorque(colPos.normalized * 1.5f, ForceMode.Impulse);
		}
	}
	
	//push every rb away from the center of the sphere 
	void OnTriggerStay(Collider other)
    {
        if (other.attachedRigidbody) {
			//todo: fix this calculation. As it stands now objects further away from center are launched faster.
			// Vector3 pushDirection = other.transform.position - this.GetComponent<Collider>().ClosestPoint(other.transform.position);
			Vector3 pushDirection = other.transform.position - transform.position;
            other.attachedRigidbody.AddForce(pushDirection, ForceMode.Impulse);
		}
    }
}
