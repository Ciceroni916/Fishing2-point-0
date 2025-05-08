using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script kills player drone when it goes outside of boundaries

//good programmer would do it throug trigger zones
//i am not a good programmer 
public class deathZoneCheck : MonoBehaviour
{
    // Update is called once per frame
    void FixedUpdate()
    {
		Vector3 pos = transform.position;
        if (pos.y < 0 || pos.y > 750
		|| pos.x > 500 || pos.x < -500
		|| pos.z > 500 || pos.z < -500) {
			Destroy(gameObject);
		}
    }
}
