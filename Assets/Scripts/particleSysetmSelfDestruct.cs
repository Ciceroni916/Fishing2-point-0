using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraBordersParticles : MonoBehaviour
{
	private ParticleSystem YAxisBlockingParticles;
	
	void Start() {
		YAxisBlockingParticles = this.GetComponent<ParticleSystem>();
	}
	
    void FixedUpdate()
    {
		if (YAxisBlockingParticles.particleCount <= 0) {
			//kys
			Destroy(this.gameObject);
		}
    }
}
