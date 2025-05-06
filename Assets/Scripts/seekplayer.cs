using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//todo: make a proper pointing laser, lower turret range, change color depending on whether turret sees player or not 
//todo: dark clouds + killzone beneath certain point
public class seekplayer : MonoBehaviour
{
	
	public float perceptionLength = 50.0f;
	
	private float noticedIterator = 0.01f;
	private Transform target;
	// private LineRenderer threat;
	private GameObject rememberedTarget;
	private float noticedTimer;
	private bool targetNoticed;
	private float exterminationTimer = 0.0f;
	
	private looker look;
	
    // Start is called before the first frame update
    void Start()
    {
		look = transform.parent.GetComponent<looker>();
		target = look.target;
		// threat = look.threat;
		rememberedTarget = null;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		Vector3 targetDirection = target.position - transform.position;
		RaycastHit potentialTarget;
		float alignment = Vector3.Dot(transform.up.normalized, targetDirection.normalized);
		LayerMask mask = LayerMask.GetMask("Terrain", "Player", "Enemy");
		
		Debug.DrawRay(this.transform.position, Vector3.forward * perceptionLength, new Color(255,0,0,100), 0.1f);
		
		//if noticed nothing; outside of visible range
		if (!Physics.Raycast(transform.position, targetDirection, out potentialTarget, perceptionLength, mask) && targetNoticed) {
			if (noticedTimer < 5.0f) noticedTimer += noticedIterator;
			if (noticedTimer > 5.0f && targetNoticed) {
				look.TargetLost();
				Debug.Log("TARGET LOST: OUTSIDE OF VISIBLE RANGE.");
				LoseTarget();
			}
			return;
		}
		
		if (targetNoticed && potentialTarget.transform.gameObject.tag.Equals("Player") && alignment > 0.9f) exterminationTimer += 0.01f;
		//check if angle between barrel and Player is small enough; check if raycast towards the player is not interrupted by a terrain
        if (alignment > 0.85f){
			if (potentialTarget.transform != null && potentialTarget.transform.gameObject.tag == "Player") {
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
					LoseTarget();
				}
			}
		}
		if (noticedTimer > 5.0f) {
			targetNoticed = false;
			exterminationTimer = 0.0f;
			// threat.SetPosition(0, new Vector3(0,0,0));
			// threat.SetPosition(1, new Vector3(0,0,0));
		}
		
		if (exterminationTimer > 5.0f) {
			Destroy(target.gameObject);
			Debug.Log("TARGET DESTROYED.");
		}
    }
	
	public float GetExterminationTimer() {
		return exterminationTimer;
	}
	
	//in case of flies
	public void BoomDisable() {
		this.enabled = false;
	}
	
	public void LoseTarget() {
		targetNoticed = false;
		exterminationTimer = 0.0f;
		if (rememberedTarget != null) rememberedTarget.SendMessage("UnbecomeTargeted", gameObject);
		// threat.SetPosition(0, new Vector3(0,0,0));
		// threat.SetPosition(1, new Vector3(0,0,0));
	}
	
	void OnDisable() {
		LoseTarget();
		Debug.Log("TARGET LOST: I AM HOLLOW.");
	}
	
	void OnDestroy() {
		LoseTarget();
		Debug.Log("TARGET LOST: I AM DEAD.");
	}
}
