using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hardButtonScript : MonoBehaviour
{
    public GameObject menu, menuCanvas, player, playerCanvas, enemiesParent;
	
	//it begins
	void ItBegins() {
		menu.SetActive(false);
		player.SetActive(true);
		playerCanvas.SetActive(true);
		menuCanvas.SetActive(false);
		enemiesParent.SetActive(true);
		player.BroadcastMessage("Hard");
		// enemiesParent.BroadcastMessage("Hard");
	}
}
