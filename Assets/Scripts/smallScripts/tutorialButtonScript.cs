using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutorialButtonScript : MonoBehaviour
{
    public GameObject menu, menuCanvas, player, playerCanvas, tutorialParent;
	
	//it begins
	void TutorialSetup() {
		menu.SetActive(false);
		player.SetActive(true);
		playerCanvas.SetActive(true);
		menuCanvas.SetActive(false);
		tutorialParent.SetActive(true);
		player.BroadcastMessage("Tutorial");
		player.BroadcastMessage("TSMain");
		player.transform.position = new Vector3(200,130,-50);
	}
}
