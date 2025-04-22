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
	private float startTime;
	private Vector3 trScale, trIterator;
	private List<int> idArray;
	
    // Start is called before the first frame update
    void Awake()
    {
		col = GetComponent<SphereCollider>();
		startTime = Time.fixedTime;
		trScale = new Vector3(1f,1f,1f);
		trIterator = new Vector3(0.25f,0.25f,0.25f);
		idArray = new List<int>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		// in africa every 5 seconds a boom_sphere dies. together we can stop this.
		if (Time.fixedTime > startTime + 1.0f) {
			Destroy(this.gameObject);
		}
		trScale += trIterator;
        transform.localScale = trScale;
    }
	
	void OnTriggerEnter(Collider collision) {
		// if (
		//todo: only check/add parent that is first from above
		//preventing pushing one object more then once.
		int id = collision.transform.gameObject.GetInstanceID();
		GameObject colGO = collision.transform.gameObject;
		if (!idArray.Contains(id)) {
			idArray.Add(id);
			//telling turrent to dissassemble itself
			colGO.SendMessageUpwards("Disassemble","",SendMessageOptions.DontRequireReceiver);
			//
		}
	}
}
