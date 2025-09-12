using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

//This script writes stuff on player's screen. Very not videogame, wcyd.

public class tutorialScript : MonoBehaviour
{
	public PlayerInput playerInput;
	public GameObject gameCanvas, MakeshiftUILinebreaker, tutorialCanvas, vertical, horizontal, altitude, altitudeS, altitudeM, lockedOn, coords;
	public GameObject[] tutorialTriggers;
	public TMP_Text tutorialText;
	
	private int iterator = 0;
	private InputAction enter;
	private bool isReading = false;
	
	private void OnEnable() {
		enter = playerInput.actions.FindAction("Enter");
		vertical.SetActive(false);
		horizontal.SetActive(false);
		altitude.SetActive(false);
		altitudeS.SetActive(false);
		altitudeM.SetActive(false);
		lockedOn.SetActive(false);
		coords.SetActive(false);
	}
	
	private void Update() {
		float cont = enter.ReadValue<float>();
		
		if (cont > 0f && isReading) {
			EnableOtherHud();
			isReading = false;
		}
	}
	
	private void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag.Equals("TutorialTrigger")) {
			other.gameObject.SetActive(false);
			if (iterator < tutorialTriggers.Length) tutorialTriggers[iterator].SetActive(true);
			TSMain();
		}
	}
	
	private void TSMain() {
		this.enabled = true;
		DisableOtherHud();
		switch (iterator) {
			//0: enable Vertical
			case 0:
			vertical.SetActive(true);
			tutorialText.text = "You are currently operating aerial drone that can only move by tilting fans.\n\nWhite circle in the middle of the screen designates safe mouse cursor borders. Moving mouse cursor ouside Safe Circle borders tilts\\rotates drone.\n\nRight now, your drone is \"frozen\", meaning it cannot move, only rotate left-right.\n\nRead these instructions, then execute them in order they are written:\n\n1. Press [Enter] to close this tutorial prompt.\n2. Press [x] to \"thaw\" drone, allowing you to move.\n3. Move mouse cursor below Safe Circle until number to the left of Safe Circle will become \"-10\".\n4. Move mouse cursor back inside Safe Circle and watch as your drone slowly drifts towards its objective.\n\n\nTip: Line connecting mouse cursor and center of the screen will turns green when mouse cursor affects drone and white when it does not.";
			break;
			
			//1: enable horizontal
			case 1:
			horizontal.SetActive(true);
			tutorialText.text = "Good job on reaching your first objective. It must have been very tiring.\nNow, do not panic, but there is numbers on your screen. Number to the left of Safe Circle is related to drone tilt. If that number is negative, it means drone is tilted forward and its propelled forward.\nWhich is good. If you want to move forward.\n\n\nRead these instructions, then execute them in order they are written:\n\n1. Press [Enter], then [x].\n2. Stop drone movement by moving cursor above safe circle and hold it there until value to the left of Safe Circle becomes [0]. Then put mouse cursor inside Safe Circle.\n3. Turn around and locate a large see-through green cubic object. Your goal is to move drone inside it.\n\nTip: While holding [RMB] use mouse to look around while not influencing drone tilt.";
			break;
			
			//2: enable altitude related variables
			case 2:
			altitude.SetActive(true);
			altitudeS.SetActive(true);
			altitudeM.SetActive(true);
			tutorialText.text = "Now, speed things up.\n\nPressing [Space] will increase fans strength. Should drone be balanced it will gain height. Should drone be tilted it will speed up its movement.\nPressng [L.Shift] will reverse fans movement. For your safety, remove this button from keyboard.\nPressing [Q] or [E] will affect drone's roll. \n\n\nRead these instructions, then execute them in order they are written:\n\n1. Press [Enter], then [x].\n2. Stop drone movement and rotation.\n3. Hold [Space] and reach another goal.";
			break;
			
			//3: enable lockedon and coords
			case 3:
			lockedOn.SetActive(true);
			coords.SetActive(true);
			tutorialText.text = "Its me, wall of text, scarer of tictoc children.\n\nEnd goal of this game is to locate and destroy every turret.\nTurrets will retaliate by locking onto the drone. This is harmless, but after 5 seconds of beeping noises drone is destroyed. (Its not a coincidence).\nPressing [LMB] will fire your only weapon. It has infinite ammo, limited reach, requires time to recharge.\n\nLast task: turn around and destroy the only one tutorial turret. Feel free to explore map and die by wandering off too far.\n\nTip: only in tutorial, press [Z] to freeze drone in place, and [X] to thaw it.\nTip: explosion from weapon will not destroy drone but it will push it, and it can be used to lift drone should it fall onto the ground .\n\nTip: there is several ways to remove turret lock-on. Well, other then destroying turret.";
			break;
			
			//4: tips
			case 4:
			tutorialText.text = "Secret 1: fastest way to move forward is to put drone to a negative 45 degree angle and hold [Space].\n\nSecret 2: In hard mode, dark sides of pyramid have exactly one turret.\n\nSecret 3: when only 5 turrets are remaining their locations will be revelaed via dedicated ui element.\n\n Secret 4: in easy mode, every turret has a 50% chance of self-destruction and drone moves slower.";
			break;
			
			
			// case 5:
			// break;
			// case 6:
			// break;
			// case 7:
			// break;
		}
		iterator++;
		isReading = true;
	}
	
	private void DisableOtherHud() {
		Time.timeScale = 0;
		this.SendMessage("FreezeDrone");
		gameCanvas.SetActive(false);
		MakeshiftUILinebreaker.SetActive(false);
		tutorialCanvas.SetActive(true);
	}
	
	private void EnableOtherHud() {
		Time.timeScale = 1;
		gameCanvas.SetActive(true);
		MakeshiftUILinebreaker.SetActive(true);
		tutorialCanvas.SetActive(false);
	}
}
