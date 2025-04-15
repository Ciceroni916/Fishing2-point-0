using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//move particle system to first terarin/enemy encountered by physics ray
//todo: change ray and particle colors to red when targeting enemy and keep it as system color (white/green?) otherwise.
public class aimingRay : MonoBehaviour
{
	public Camera cam;
	public ParticleSystem ps; 
	
    // Start is called before the first frame update
    void Update()
    {
		RaycastHit hit;
		LayerMask layerMask = LayerMask.GetMask("Terrain", "Enemy");
		if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 100f, layerMask)) {
			ps.transform.position = hit.point;
			ps.transform.LookAt(cam.transform);
		} else {
			ps.transform.position = new Vector3(0,-1000,0);
		}
    }
}
