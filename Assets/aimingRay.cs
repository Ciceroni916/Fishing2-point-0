using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aimingRay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		Ray ray = new Ray(transform.position, transform.forward);
    }
}
