using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class seekplayer : MonoBehaviour
{
	private Transform target;
	public float perceptionLength = 50.0f;
	private float noticedIterator = 0.01f;
	private LineRenderer threat;
	
	public float cycleNumber;
	
	private GameObject rememberedTarget, debugtarget;
	private float noticedTimer;
	private bool targetNoticed;
	private float exterminationTimer = 0.0f;
	
	private looker look;
	
    // Start is called before the first frame update
    void Start()
    {
		look = transform.parent.parent.GetComponent<looker>();
		target = look.target;
		threat = look.threat;
		debugtarget = gameObject;
		
		//bruh don't even ask
		// float startingNumber = 5000.0f;
		// float calculatedNumber = startingNumber;
		// float mult = 1.075f;
		// float finalNumber = 0;
		// for (int i = 0; i < cycleNumber; i++) {
			// calculatedNumber *= mult;
			// finalNumber += calculatedNumber;
		// }
		// Debug.Log(finalNumber);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		Vector3 targetDirection = target.position - transform.position;
		RaycastHit potentialTarget;
		float alignment = Vector3.Dot(transform.parent.up.normalized, targetDirection.normalized);
		LayerMask mask = LayerMask.GetMask("Terrain", "Player", "Enemy");
		
		//if noticed nothing; outside of visible range
		if (!Physics.Raycast(transform.position, targetDirection, out potentialTarget, perceptionLength, mask) && targetNoticed) {
			if (noticedTimer < 5.0f) noticedTimer += noticedIterator;
			if (noticedTimer > 5.0f && targetNoticed) {
				look.TargetLost();
				Debug.Log("TARGET LOST: OUTSIDE OF VISIBLE RANGE.");
				rememberedTarget.SendMessage("BecomeTargeted", gameObject);
			}
			return;
		}
		
		if (targetNoticed && potentialTarget.transform.gameObject.tag.Equals("Player") && alignment > 0.9f) exterminationTimer += 0.01f;
		//check if angle between barrel and Player is small enough; check if raycast towards the player is not interrupted by a terr
        if (alignment > 0.85f){
			if (potentialTarget.transform.gameObject.tag == "Player") {
				//player is in front of turret and not behind cover
				noticedTimer = 0.0f;
				if (!targetNoticed){
					look.TargetNoticed();
					Debug.Log("TARGET LOCKED");
					//message is sending to the parent because parent has UIController
					rememberedTarget = potentialTarget.transform.parent.gameObject;
				}
				targetNoticed = true;
				rememberedTarget.SendMessage("BecomeTargeted", gameObject);
			} else {
				//player is behind cover
				if (noticedTimer < 5.0f) noticedTimer += noticedIterator;
				if (noticedTimer >= 5.0f && targetNoticed) {
					look.TargetLost();
					Debug.Log("TARGET LOST: HIDING BEHIND COVER.");
					rememberedTarget.SendMessage("UnbecomeTargeted", gameObject);
				}
			}
		}
		if (noticedTimer > 5.0f) {
			targetNoticed = false;
			exterminationTimer = 0.0f;
			threat.SetPosition(0, new Vector3(0,0,0));
			threat.SetPosition(1, new Vector3(0,0,0));
		}
		
		if (exterminationTimer > 5.0f) {
			Debug.Log("TARGET DESTROYED.");
			Destroy(target.gameObject);
			gameObject.SetActive(false);
		}
    }
	
	public float GetExterminationTimer() {
		return exterminationTimer;
	}
}
