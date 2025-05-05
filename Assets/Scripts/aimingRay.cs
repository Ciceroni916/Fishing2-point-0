using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

//move particle system to first terarin/enemy encountered by physics ray
public class aimingRay : MonoBehaviour
{
	public Camera cam;
	public ParticleSystem redPS, whitePS; 
	public VisualEffect redBeam, whiteBeam;
	
	void Start(){
		whiteBeam.enabled = true;
		redBeam.enabled = false;
	}
	
    // Start is called before the first frame update
    void Update()
    {
		RaycastHit hit;
		LayerMask layerMaskTer = LayerMask.GetMask("Terrain");
		LayerMask layerMaskEne = LayerMask.GetMask("Enemy");
		if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 100f, layerMaskEne)) {
			//enemy in the middle of the screen
			whitePS.transform.position = new Vector3(0,-1000,0);
			whiteBeam.enabled = false;
			redBeam.enabled = true;
			redPS.transform.position = hit.point;
			redPS.transform.LookAt(cam.transform);
		} else if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 100f, layerMaskTer)) {
			//terrain in the middle of the screen
			redPS.transform.position = new Vector3(0,-1000,0);
			whiteBeam.enabled = true;
			redBeam.enabled = false;
			whitePS.transform.position = hit.point;
			whitePS.transform.LookAt(cam.transform);
		} else {
			//hitting nothing in the nearest 100 unity space measuring points (are they meters? who knows at this point...)
			whiteBeam.enabled = true;
			redBeam.enabled = false;
			redPS.transform.position = new Vector3(0,-1000,0);
			whitePS.transform.position = new Vector3(0,-1000,0);
		}
    }
}
