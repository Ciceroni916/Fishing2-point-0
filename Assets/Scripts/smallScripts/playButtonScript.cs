using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playButtonScript : MonoBehaviour
{
    public GameObject menu, menuCanvas, player, playerCanvas, enemiesParent;
	
	//it begins
	void ItBegins() {
		menu.SetActive(false);
		player.SetActive(true);
		playerCanvas.SetActive(true);
		menuCanvas.SetActive(false);
		enemiesParent.SetActive(true);
		player.transform.position = new Vector3(12,90,8);
		player.BroadcastMessage("Easy");
		enemiesParent.BroadcastMessage("Easy");
	}
}
