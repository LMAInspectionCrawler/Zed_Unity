using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceOnScreen : MonoBehaviour {

    ZEDManager ZedManager;
    Camera LeftCamera;

    public GameObject Object;

    // Use this for initialization
    void Awake() {
        ZedManager = FindObjectOfType<ZEDManager>();
        LeftCamera = ZedManager.GetLeftCameraTransform().gameObject.GetComponent<Camera>();

        // Show cursor
        Cursor.visible = true;

        // Hide bunny
		Object.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
		if(!sl.ZEDCamera.GetInstance().IsCameraReady)
        {
            return;
        }

        if (Input.GetButtonDown("Fire1"))
        {			
			/// Mouse Input gives the screen pixel position
            Vector2 ScreenPosition = Input.mousePosition;

            Vector3 Normal;
            Vector3 WorldPos;

			// Get Normal and real world position defined by the pixel 
			ZEDSupportFunctions.GetNormalAtPixel(ScreenPosition,sl.REFERENCE_FRAME.WORLD,LeftCamera,out Normal);
			ZEDSupportFunctions.GetWorldPositionAtPixel(ScreenPosition, LeftCamera,out WorldPos);

			// To consider the location as a floor, we check that the normal is valid and is closely aligned with the gravity
            bool validFloor = Normal.x != float.NaN && Vector3.Dot(Normal, Vector3.up) > 0.85f;

			// If we've found a floor to place the bunny, then set its location and show it.
			if (validFloor) {
				Object.transform.localPosition = WorldPos;
				Object.transform.LookAt (ZedManager.transform);
				Object.SetActive(true);
			} else {
				if (Normal.x == float.NaN)
				Debug.Log ("cannot place object at this position. Normal vector not detected.");
				if (Vector3.Dot(Normal, Vector3.up) <= 0.85f)
					Debug.Log ("cannot place object at this position. Normal vector does not fit the gravity enough : "+Vector3.Dot(Normal, Vector3.up));
				Object.SetActive(false);
			}
        }
    }
}
