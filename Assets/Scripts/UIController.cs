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

public class UIController : MonoBehaviour
{
	public TMP_Text horizontal, vertical, altitude, altitudeMeasure, altitudeSign, xCoordSign, zCoordSign, xCoord, zCoord, lockedOn, turretCount;
	public Transform Enemies;
	public GameObject player, playerCanvas;
	public Rigidbody rb;
	public RectTransform raw;
	public AudioSource noise, backgroundStatic, dangerousBeep;
	public static float globalVolume = 1.0f;
	public bool sound;
	
	//beingTargeted related variables
	private List<GameObject> AllAggroedTurrets;
	private List<TMP_Text> warningsTMP;
	private List<float> turretsExtermination;
	private int counter, turretAmount, lockedOnFrequency = 50;
	private static float soundFadeInOutTime = 80.0f, maxVolume = 0.1f;
	private Coroutine corCalm, corPanic;
	
    // Start is called before the first frame update
    void Start()
    {
		AllAggroedTurrets = new List<GameObject>();
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
	
	void FixedUpdate() {
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
		//set part of interace active that says where the closest enemy is and i am so tired
		if (Enemies.childCount != turretAmount) {
			turretAmount = Enemies.childCount;
			if (turretAmount <= 10) {
				if (!turretCount.gameObject.activeSelf) {
					turretCount.gameObject.SetActive(true);
				}
			}
		}
		//updating turret count when almost all turrets are dead
		//updating distance to closest turret
		//ah yes do it in the update loop G R E A T   J O B
		if (turretCount.gameObject.activeSelf) {
			GameObject closestEnemy = this.gameObject;
			float distanceToClosestEnemy = 10000.0f;
			for (int i = 0; i < turretAmount; i++) {
				float newDistance = Vector3.Distance(Enemies.GetChild(i).gameObject.transform.position, player.transform.position);
				if (newDistance < distanceToClosestEnemy) {
					distanceToClosestEnemy = newDistance;
					closestEnemy = Enemies.GetChild(i).gameObject;
				}
			}
			Vector3 closestEnemyPos = closestEnemy.transform.position;
			turretCount.text = ("Turret remaining " + turretAmount + ".\nClosest turret position x: " + closestEnemyPos.x.ToString("0") + " y: " + closestEnemyPos.y.ToString("0") + " z: " + closestEnemyPos.z.ToString("0") + "\nDistance to closest turret: " + distanceToClosestEnemy.ToString("0.00"));
		}
    }
	
	public void BecomeTargeted(GameObject go){
		if (!AllAggroedTurrets.Contains(go)) {
			AllAggroedTurrets.Add(go);
		}
	}
	
	public void UnbecomeTargeted(GameObject go){
		AllAggroedTurrets.Remove(go);
	}
	
	//called every fixed update to lit up corresponding part of interface when targeted by an enemy
	private void LockedOnInteractions(){
		//visuals
		//AllAggroedTurrets is a list containing IDs of objects that are currently targeting the player. When its empty player is not threatened by a ranged weaponary.
		lockedOn.outlineColor = new Color(255, 0, 0, 0.5f);
		if (AllAggroedTurrets.Count > 0) {
			//note: activating both canvas and player object at the same time from main menu causes a bug when canvas in not updated. Fix:
			lockedOn.gameObject.SetActive(false);
			lockedOn.gameObject.SetActive(true);
			if (counter > lockedOnFrequency / 2) {
				//flashing red
				// lockedOn.color = new Color(255, 0, 0, 100);
				lockedOn.outlineColor = new Color(255, 0, 0, 0.5f);
				for (int i = 0; i < AllAggroedTurrets.Count && i < warningsTMP.Count; i++) {
					// warningsTMP[i].color = new Color(255, 0, 0, 100);
					warningsTMP[i].outlineColor = new Color(1, 0, 0, 0.5f);
				}
			} else {
				//flashing green
				// lockedOn.color = new Color(255, 255, 255, 100);
				lockedOn.outlineColor = new Color(0, 255, 0, 0.5f);
				for (int i = 0; i < AllAggroedTurrets.Count && i < warningsTMP.Count; i++) {
					// warningsTMP[i].color = new Color(255, 255, 255, 100);
					warningsTMP[i].outlineColor = new Color(0, 1, 0, 0.5f);
				}
			}
			counter++;
			if (counter > lockedOnFrequency) counter = 0;
			//blacking out not needed interface elements
			for (int i = AllAggroedTurrets.Count; i < warningsTMP.Count; i++) {
				// warningsTMP[i].color = new Color(255, 255, 255, 100);
				warningsTMP[i].outlineColor = new Color(0, 1, 0, 0.5f);
			}
		} else {
			//not being targeted 
			counter = 0;
			// lockedOn.color = new Color(255, 255, 255, 100);
			lockedOn.outlineColor = new Color(0, 255, 0, 0.5f);
			for (int i = 0; i < warningsTMP.Count; i++) {
				// warningsTMP[i].color = new Color(255, 255, 255, 100);
				warningsTMP[i].outlineColor = new Color(0, 1, 0, 0.5f);
			}
		}
		//sounds
		//increase noise volume, add static
		if (AllAggroedTurrets.Count > 0) {
			if (corPanic != null) StopCoroutine(corPanic);
			corCalm = StartCoroutine(SoundFadeinOut(backgroundStatic, noise));
		} else {
			if (corCalm != null) StopCoroutine(corCalm);
			corPanic = StartCoroutine(SoundFadeinOut(noise, backgroundStatic));
		}
		//update exterminationTimers array
		if (AllAggroedTurrets.Count > 0) {
			//get extermination counter that is closest to killing player drone
			float extermination = 0.0f;
			for (int i = 0; i < AllAggroedTurrets.Count; i++) {
				seekplayer sp = AllAggroedTurrets[i].GetComponent<seekplayer>();
				if (sp.GetExterminationTimer() > extermination) {
					extermination = sp.GetExterminationTimer();
				}
			}
			//if extermination in 4 or more seconds
			if (extermination < 4.0) {
				dangerousBeep.volume = 0.2f * globalVolume;
				dangerousBeep.pitch = 1.0f;
				if (!dangerousBeep.isPlaying && dangerousBeep.enabled) dangerousBeep.Play();
			} else {
				dangerousBeep.volume = 0.2f * globalVolume;
				dangerousBeep.pitch = 2.0f;
				if (!dangerousBeep.isPlaying && dangerousBeep.enabled) dangerousBeep.Play();
			}
		} else {
			dangerousBeep.volume = 0.0f * globalVolume;
			dangerousBeep.Stop();
		}
	}
	
	//sounds fadeout/fadein
	private static IEnumerator SoundFadeinOut(AudioSource fadeIn, AudioSource fadeOut) {
		while (fadeIn.volume < maxVolume * globalVolume || fadeOut.volume >= 0.01) {
			//nice double ifs idiot
			if (fadeIn.volume < maxVolume * globalVolume) fadeIn.volume += Time.deltaTime / soundFadeInOutTime * maxVolume;
			//can't go lower then 0, unlike me
			fadeOut.volume -= Time.deltaTime / soundFadeInOutTime * maxVolume;
			yield return null;
		}
	}
	
	//broadcasted from audioSlider.cs
	private void VolumeChanged(float newVolume) {
		globalVolume = newVolume;
		//when sound is updated, background noise is not affected. A fix:
		backgroundStatic.volume = 0.0f;
		noise.volume = 0.0f;
		if (AllAggroedTurrets.Count > 0) {
			if (corPanic != null) StopCoroutine(corPanic);
			corCalm = StartCoroutine(SoundFadeinOut(backgroundStatic, noise));
		} else {
			if (corCalm != null) StopCoroutine(corCalm);
			corPanic = StartCoroutine(SoundFadeinOut(noise, backgroundStatic));
		}
		Debug.Log(globalVolume);
	}
}
