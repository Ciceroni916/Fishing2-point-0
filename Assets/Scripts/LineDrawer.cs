using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LineDrawer : MonoBehaviour
{
	public PlayerInput playerInput;
	public LineRenderer lrCrosshair, lrCircle, lrHorizon, lrAltitude;
	public GameObject lrAltitudeParent, lrCompassParent;
	public Rigidbody rb;
	public Transform crosshairPlane;
	public GraphicRaycaster m_Raycaster;
	public float radius;
	
	private Camera cam;
	private InputAction rmb;

	void Start() {
		cam = Camera.main;
		rmb = playerInput.actions.FindAction("RMB");
		
		//drawing circle (I stole the formula i know no math smile)
		float ThetaScale = 0.005f;
		int Size;
		float Theta = 0f;
        Size = (int)((1f / ThetaScale) + 1f);
        lrCircle.positionCount = Size;
        for (int i = 0; i < Size; i++) {
            Theta += (2.0f * Mathf.PI * ThetaScale);
            float x = radius * Mathf.Cos(Theta);
            float y = radius * Mathf.Sin(Theta);
            lrCircle.SetPosition(i, new Vector3(x, y, 0));
        }
	}

    // Update is called once per frame
    void Update()
    {
		float RMB = rmb.ReadValue<float>();
		//crosshair
		Vector3 crosshairPosition = ScreenToLine(Input.mousePosition);
		Ray ray = cam.ViewportPointToRay(crosshairPosition);
        RaycastHit hit;
		LayerMask layerMask = LayerMask.GetMask("MakeshiftUI");
		if (RMB > 0) {
			//player is currently looking around, remove crosshair movement to let player know that they cannot rotate drone while looking around
			lrCrosshair.positionCount = 0;
		} else if (Physics.Raycast(ray, out hit, 10.0f, layerMask)) {
			if (hit.collider.gameObject.tag.Equals("MakeshiftUILinebreaker")) {
				lrCrosshair.positionCount = 2;
				Vector3 localHit = crosshairPlane.InverseTransformPoint(hit.point);
				float scaleModifier = crosshairPlane.transform.lossyScale.x;
				float radiusActual = radius * scaleModifier;
				float distance = Vector3.Distance(crosshairPlane.position, hit.point);
				float distanceX = Vector3.Distance(new Vector3(0, 0, 0), new Vector3(localHit.x, 0, 0));
				float distanceY = Vector3.Distance(new Vector3(0, 0, 0), new Vector3(0, localHit.y, 0));
				
				lrCrosshair.SetPosition(0, crosshairPlane.position);
				lrCrosshair.SetPosition(1, hit.point);
				
				if (distance < radiusActual) {
					//cursor is inside of circle, make it white
					lrCrosshair.startColor = Color.white;
					lrCrosshair.endColor = Color.white;
				} else if (RMB < 1.0f) {
					/*cursor is outside of circle.
					1. Check by how much verically and horizontally.
					2. rotate around y coordinate by how further away cursor.x from circle.
					3. rotate around x coordinate by how further away cursor.y from circle.*/
					lrCrosshair.startColor = Color.green;
					lrCrosshair.endColor = Color.green;
					float verticalSpeed = CalculateSpeed(distanceY, radiusActual);
					float horizontalSpeed = CalculateSpeed(distanceX, radiusActual);;
					if (localHit.y < 0) {
						rb.AddRelativeTorque(verticalSpeed, 0, 0);
					} else {
						rb.AddRelativeTorque(-verticalSpeed, 0, 0);
					}
					if (localHit.x < 0){
						rb.AddRelativeTorque(0, -horizontalSpeed, 0);
					} else {
						rb.AddRelativeTorque(0, horizontalSpeed, 0);
					}
				}
			}
		}
		//horizon line
		int angledLine = CalculatePointOnStaticCircle();
		Vector3 pos1 = lrCircle.GetPosition(angledLine);
		lrHorizon.SetPosition(0, pos1);
		angledLine = (angledLine + (int)((lrCircle.positionCount + 1)/2) > lrCircle.positionCount) ? angledLine - (int)(lrCircle.positionCount/2) : angledLine + (int)(lrCircle.positionCount/2);
		Vector3 pos2 = lrCircle.GetPosition(angledLine);
		lrHorizon.SetPosition(1, pos2);
		//horizon line: change color depending on drone rotation: upwards = green, downwards = red
		if (Quaternion.Angle(rb.transform.rotation, Quaternion.LookRotation(rb.transform.forward, Vector3.up)) > 89) {
			lrHorizon.startColor = new Color(1,0,0,1);
			lrHorizon.endColor = new Color(1,0,0,1);
		} else {
			lrHorizon.startColor = new Color(0,1,0,1);
			lrHorizon.endColor = new Color(0,1,0,1);
		}
		//setting line positions: angling to the left by z also angles line; counter clockwise (0/360 degrees means that line is horizontal, 90/270 degrees means that line is strictly vertical)
		//creating vertical offset: looking down means line goes up and vice versa
		float offset = CalculateHorizontalLineAngle();
		pos1.y += offset;
		pos2.y += offset;
		lrHorizon.SetPosition(0, pos1);
		lrHorizon.SetPosition(1, pos2);
		//Attitude scroller
		for (int j = 0; j < lrAltitudeParent.transform.childCount; j++) {
			LineRenderer lr = lrAltitudeParent.transform.GetChild(j).GetComponent<LineRenderer>();
			float dif = 0.25f / 0.3f;
			float scrollerOffset = CalculateAltitudeScroller(j * dif, 10.0f);
			for (int i = 0; i < lr.positionCount; i++) {
				lr.SetPosition(i, new Vector2(lr.GetPosition(i).x, scrollerOffset));
			}
		}
		//CompassScroller
		for (int j = 0; j < lrCompassParent.transform.childCount; j++) {
			//gap between lines is circle divided by amount of lines
			float dif = 360.0f / lrCompassParent.transform.childCount;
			float scrollerOffset = CalculateCompassScroller(j * dif, 180.0f);
			lrCompassParent.transform.GetChild(j).localPosition = new Vector3(scrollerOffset, lrCompassParent.transform.GetChild(j).localPosition.y, lrCompassParent.transform.GetChild(j).localPosition.z);
		}
    }
	
	private Vector3 ScreenToLine(Vector3 coords) {
		coords.x = coords.x / Screen.width;
		coords.y = coords.y / Screen.height;
		return coords;
	}
	
	private float CalculateSpeed(float distance, float radiusActual) {
		//if its 3 times as far from edges of the circle, rotate it with fastest speed of 0.01 . Otherwise rotate it by 0.01 * 
		//distance of 3 * radiusActual is 0.01
		//if distance lower then 3 * radiusActual then 0.01 * howMuchLowerPercent
		float multiplier = distance / (3 * radiusActual);
		if (multiplier > 1.0f) multiplier = 1.0f;
		//change this to increase/decrease speed of rotation using line controls
		float standartSpeed = 0.01f;
		float speedOfRotation = standartSpeed * multiplier;
		return speedOfRotation;
	}
	
	private float CalculateHorizontalLineAngle() {
		//todo check values for other resolutions
		float xEuler = rb.transform.localEulerAngles.x;
		if (xEuler > 180) xEuler -= 360;
		// xEuler *= -1;
		xEuler /= -330;
		return xEuler;
	}
	
	private int CalculatePointOnStaticCircle() {
		int zEuler = (int)rb.transform.localEulerAngles.z;
		//0 = 0, 360 = 100;
		zEuler = (int) (zEuler / (360.0f/lrCircle.positionCount));
		return zEuler;
	}
	
	private float CalculateAltitudeScroller(float offset, float tiling) {
		//offset: visual spacing between two lrScrollers
		//tiling: tiling of 10 means that one line will cross the screen from top to borrom every 10 units; higher tiling means faster altitude lose/gain required to make scroller go fast
		float yPos = (rb.transform.position.y + offset) / tiling;
		if (yPos < 0) yPos += 1;
		yPos -= Mathf.Floor(yPos);
		return (yPos - 0.5f) * 0.3f;	//-0.5 due to lr needed to be scrolled from -0.5 to 0.5, which is where bottom and top of screen are; * 0.15 due to requiring to take less screen space
	}
	
	private float CalculateCompassScroller(float offset, float tiling) {
		//offset: visual spacing between two lrScrollers
		//-1.0 makes it go from -1.0f to +1.0f on LineBreaker due to angle being in 0 to 360 range and tiling always 180; which is about double the size of screen on 1920*1080 resolution.
		//This number is too big and it puts what suppose to be a UI element outside of screen. So then number is multiplied by nerf (default: 0.15), which makes it go from -0.15f to +0.15f; which os about quater of the screen on 1920*1080
		float yEuler = (rb.transform.localEulerAngles.y + offset) / tiling - 1.0f;
		float nerf = 0.4f;
		yEuler *= nerf;
		if (yEuler > nerf) {
			yEuler = yEuler - nerf * 2.0f;
		}
		return yEuler;
	}
}
