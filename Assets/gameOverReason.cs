using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//this script is called by characterController.cs in GameOverSequence
public class gameOverReason : MonoBehaviour
{
	private void SetGameOverReason(string reason) {
		gameObject.GetComponent<TMP_Text>().text = reason;
	}
}
