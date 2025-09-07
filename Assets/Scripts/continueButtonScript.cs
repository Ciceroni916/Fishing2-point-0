using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class continueButtonScript : MonoBehaviour
{
	public GameObject gameCanvas, pauseCanvas, MakeshiftUILinebreaker;
	
    void ItContinues() {
		pauseCanvas.SetActive(false);
		gameCanvas.SetActive(true);
		MakeshiftUILinebreaker.SetActive(true);
		Time.timeScale = 1;
	}
}
