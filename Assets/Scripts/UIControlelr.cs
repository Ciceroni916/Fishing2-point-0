using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
rest of UI:
battery: static number, never blinks.
blinking red dot
satellite: unavalibale
wifi: ping 0
*/

public class UIControlelr : MonoBehaviour
{
	public TMP_Text horizontal, vertical, altitude, altitudeMeasure, altitudeSign, xCoordSign, zCoordSign, xCoord, zCoord, lockedOn, turretCount;
	public Transform Enemies;
	public Rigidbody rb;
	public RectTransform raw;
	public AudioSource noise, backgroundStatic, dangerousBeep;
	public bool sound;
	
	//beingTargeted related variables
	private List<GameObject> turretsGO;
	private List<TMP_Text> warningsTMP;
	private List<float> turretsExtermination;
	private int lockedOnFrequency = 50;
	private int counter, turretAmount;
	
    // Start is called before the first frame update
    void Start()
    {
		turretsGO = new List<GameObject>();
		warningsTMP = new List<TMP_Text>();
		for(int i = 0; i < lockedOn.gameObject.transform.childCount; i++){
			warningsTMP.Add(lockedOn.gameObject.transform.GetChild(i).gameObject.GetComponent<TMP_Text>());
		}
		counter = 0;
		if (!sound) {
			noise.enabled = false;
			backgroundStatic.enabled = false;
			dangerousBeep.enabled = false;
		}
    }
	
	void FixedUpdate(){
		LockedOnInteractions();
	}

    // Update is called once per frame
    void Update()
    {
		Vector3 dronePos = rb.transform.position;
		float xEuler = rb.transform.localEulerAngles.x;
		float yEuler = rb.transform.localEulerAngles.y;
		if (xEuler > 180) xEuler -= 360;
		float xRemainder = xEuler % 5.0f;
		float yRemainder = yEuler % 5.0f;
		string xString = (-1 * (xEuler - xRemainder )).ToString();
		string yString = (yEuler - yRemainder).ToString();
		horizontal.text = yString;
		vertical.text = xString;
		//altitude
		float altitudeNumber = dronePos.y;
		string altitudeString = "";
		if (altitudeNumber >= 0) {
			altitudeSign.text = "pos";
		} else {
			altitudeSign.text = "neg";
		}
		altitudeNumber = Mathf.Abs(altitudeNumber);
		if (altitudeNumber > 1000) {
			altitudeNumber /= 1000;
			altitudeMeasure.text = "km";
		} else if (altitudeNumber > 100) {
			altitudeNumber /= 100;
			altitudeMeasure.text = "hm";
		} else if (altitudeNumber > 10) {
			altitudeNumber /= 10;
			altitudeMeasure.text = "dm";
		} else {
			altitudeMeasure.text = "m";
		}
		altitudeString = altitudeNumber.ToString("0.00");
		altitude.text = altitudeString;
		//X coord
		//Z coord
		if (dronePos.x >= 0) {
			xCoordSign.text = "pos";
		} else {
			xCoordSign.text = "neg";
		}
		if (dronePos.z >= 0) {
			zCoordSign.text = "pos";
		} else {
			zCoordSign.text = "neg";
		}
		xCoord.text = (Mathf.Abs(dronePos.x)/10000000).ToString("0.0000000");
		zCoord.text = (Mathf.Abs(dronePos.z)/10000000).ToString("0.0000000");
		//ah yes the update loop abuse
		if (Enemies.childCount != turretAmount) {
			turretAmount = Enemies.childCount;
			if (turretAmount <= 5) {
				turretCount.gameObject.SetActive(true);
				// turretCount.text = ("Turret remaining " + turretAmount + " Closest turret + );
			}
		}
    }
	
	public void BecomeTargeted(GameObject go){
		if (!turretsGO.Contains(go)) {
			turretsGO.Add(go);
		}
	}
	
	public void UnbecomeTargeted(GameObject go){
		turretsGO.Remove(go);
	}
	
	//called every fixed update to lit up corresponding part of interface when targeted by an enemy
	private void LockedOnInteractions(){
		//visuals
		//turretsGO is a list containing IDs of objects that are currently targeting the player. When its empty player is not threatened by a ranged weaponary.
		if (turretsGO.Count > 0) {
			if (counter > lockedOnFrequency / 2) {
				//flashing red
				lockedOn.color = new Color(255, 0, 0, 100);
				for (int i = 0; i < turretsGO.Count && i < warningsTMP.Count; i++) {
					warningsTMP[i].color = new Color(255, 0, 0, 100);
				}
			} else {
				//flashing black
				lockedOn.color = new Color(0, 0, 0, 100);
				for (int i = 0; i < turretsGO.Count && i < warningsTMP.Count; i++) {
					warningsTMP[i].color = new Color(0, 0, 0, 100);
				}
			}
			counter++;
			if (counter > lockedOnFrequency) counter = 0;
			//blacking out not needed interface elements
			for (int i = turretsGO.Count; i < warningsTMP.Count; i++) {
				warningsTMP[i].color = new Color(0, 0, 0, 100);
			}
		} else {
			//not being targeted
			counter = 0;
			lockedOn.color = new Color(0, 0, 0, 100);
			for (int i = 0; i < warningsTMP.Count; i++) {
				warningsTMP[i].color = new Color(0, 0, 0, 100);
			}
		}
		//sounds
		//increase noise volume, add static
		if (turretsGO.Count > 0) {
			noise.volume = 0.0f;
			backgroundStatic.volume = 0.1f;
		} else {
			noise.volume = 0.1f;
			backgroundStatic.volume = 0.0f;
		}
		//update exterminationTimers array
		if (turretsGO.Count > 0) {
			//get extermination counter that is closest to killing player drone
			float extermination = 0.0f;
			for (int i = 0; i < turretsGO.Count; i++) {
				seekplayer sp = turretsGO[i].GetComponent<seekplayer>();
				if (sp.GetExterminationTimer() > extermination) {
					extermination = sp.GetExterminationTimer();
				}
			}
			if (extermination < 4.0) {
				dangerousBeep.volume = 0.5f;
				if (!dangerousBeep.isPlaying && dangerousBeep.enabled) dangerousBeep.Play();
			} else {
				//todo: faster beeps
			}
		} else {
			dangerousBeep.volume = 0.0f;
			dangerousBeep.Stop();
		}
	}
}
