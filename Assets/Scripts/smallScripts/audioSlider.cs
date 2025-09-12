using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class audioSlider : MonoBehaviour
{
	GameObject player;
	
	void Start() {
		//object tagged player is actually the part that moves. it has a parent that hosts scripts
		player = GameObject.FindWithTag("Player").transform.parent.gameObject;
	}
	
	public void ValueChanged() {
		player.BroadcastMessage("VolumeChanged", GetComponent<Slider>().value);
	}
}
