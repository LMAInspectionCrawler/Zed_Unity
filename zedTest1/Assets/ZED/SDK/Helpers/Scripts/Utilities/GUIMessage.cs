//======= Copyright (c) Stereolabs Corporation, All rights reserved. ===============

using UnityEngine;
using UnityEngine.VR;

/// <summary>
/// Controls the message displayed during the opening and disconnection of the ZED
/// </summary>
public class GUIMessage : MonoBehaviour
{
    /// <summary>
    /// Text under the loading sign
    /// </summary>
    private UnityEngine.UI.Text text;

    /// <summary>
    /// Event to say the ZED is ready, start the timer, to wait the textures to be initialized
    /// </summary>
    private bool ready = false;

    /// <summary>
    /// Warning container, contains the text, background and loading sign
    /// </summary>
    private GameObject warning;

    /// <summary>
    /// Add few time before stopping the ZED launching screen to let time to the other textures to be initialized 
    /// </summary>
    private float timerWarning = 0.0f;

    /// <summary>
    /// Stop the update of the script when the flag is set to true
    /// </summary>
    private bool init = false;

	/// <summary>
    /// Timer to stop the gif from rotating
    /// </summary>
    private float timer;

    /// <summary>
    /// Reference to the spinner
    /// </summary>
    private GameObject image;


    private void Awake()
    {
        warning = Instantiate(Resources.Load("PrefabsUI/Warning" + (ZEDManager.IsStereoRig ? "_VR" :"")) as GameObject, transform);
        warning.SetActive(true);
        text = warning.GetComponentInChildren<UnityEngine.UI.Text>();
        text.color = Color.white;

        if (!sl.ZEDCamera.CheckPlugin())
        {
            text.text = ZEDLogMessage.Error2Str(ZEDLogMessage.ERROR.SDK_NOT_INSTALLED);
        }
        image = warning.transform.GetChild(0).GetChild(1).gameObject;
        image.transform.parent.gameObject.SetActive(true);
        ready = false;
    }

    private void OnEnable()
    {
        ZEDManager.OnZEDReady += Ready;
		ZEDManager.OnZEDDisconnected += ZEDDisconnected;
    }

    private void OnDisable()
    {
        ZEDManager.OnZEDReady -= Ready;
		ZEDManager.OnZEDDisconnected -= ZEDDisconnected;
    }

    /// <summary>
    /// Event if ZED is disconnected
    /// </summary>
    void ZEDDisconnected()
    {
        warning.SetActive(true);
		warning.transform.GetChild(0).gameObject.SetActive(true);
        text.GetComponent<RectTransform>().localPosition = Vector3.zero;
        text.text = ZEDLogMessage.Error2Str(ZEDLogMessage.ERROR.ZED_IS_DISCONNECETD);
		text.alignment = TextAnchor.MiddleCenter;
		text.fontSize = 200;
        warning.layer = 30;
    }

    // Update is called once per frame
    void Update()
    {
        if (!init)
        {
            sl.ERROR_CODE e = ZEDManager.LastInitStatus;

			if (e == sl.ERROR_CODE.SUCCESS) {
				timer += Time.deltaTime;
				if (timer > 0.2f) {
					text.text = "";
				}
				image.gameObject.SetActive (false);


			} else if (e == sl.ERROR_CODE.ERROR_CODE_LAST) {
				text.text = ZEDLogMessage.Error2Str (ZEDLogMessage.ERROR.CAMERA_LOADING);
				text.color = Color.white;
			} else if (e == sl.ERROR_CODE.CAMERA_NOT_DETECTED) {
				text.text = ZEDLogMessage.Error2Str (ZEDLogMessage.ERROR.UNABLE_TO_OPEN_CAMERA);
			} else {
				text.text = ZEDLogMessage.Error2Str (ZEDLogMessage.ERROR.CAMERA_NOT_INITIALIZED);
			}

		    if (ready)
            {
                timerWarning += Time.deltaTime;
                if (timerWarning > 0.5f)
                {
                   warning.SetActive(false);
                   init = true;
                   timerWarning = 0;
                }
               image.gameObject.transform.parent.gameObject.SetActive(false);
            }
        }
    }

    private void Ready()
    {
        ready = true;
    }
}
