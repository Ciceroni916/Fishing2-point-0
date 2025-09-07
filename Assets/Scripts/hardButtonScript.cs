using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hardButtonScript : MonoBehaviour
{
    public GameObject menu, menuCanvas, player, playerCanvas, enemiesParent;
	
	//it begins
	void ItBegins() {
		menu.SetActive(false);
		menuCanvas.SetActive(false);
		
		playerCanvas.SetActive(true);
		player.SetActive(true);
		enemiesParent.SetActive(true);
		
		player.BroadcastMessage("Hard");
		// enemiesParent.BroadcastMessage("Hard");
	}
}
