using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Rotator : MonoBehaviour
{
	public PlayerInput playerInput;
	public Rigidbody rb;
	
	private InputAction rmb, lmb;
	
    // Start is called before the first frame update
    void Start()
    {
        rmb = playerInput.actions.FindAction("RMB");
		lmb = playerInput.actions.FindAction("LMB");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		Vector3 center = new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0f);
		float rotateY = rmb.ReadValue<float>();
		float rotateX = lmb.ReadValue<float>();
		
		// if (rotateY > 0) {
			// if (Input.mousePosition.x > center.x) {
				// rb.AddRelativeTorque(0, 0.05f, 0);
			// } else {
				// rb.AddRelativeTorque(0, -0.05f, 0);
			// }
		// }
		
		// if (rotateX > 0) {
			// if (Input.mousePosition.y > center.y) {
				// rb.AddRelativeTorque(-0.05f, 0, 0);
			// } else {
				// rb.AddRelativeTorque(0.05f, 0, 0);
			// }
		// }
    }
}
