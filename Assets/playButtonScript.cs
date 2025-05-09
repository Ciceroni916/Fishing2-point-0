using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playButtonScript : MonoBehaviour
{
    public GameObject menu, menuCanvas, player, playerCanvas;
	
	//it begins
	void ItBegins() {
		menu.SetActive(false);
		player.SetActive(true);
		playerCanvas.SetActive(true);
		menuCanvas.SetActive(false);
	}
}
