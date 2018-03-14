<!------------------------- Release notes ------------------------------------->
### 2.3.3
- **Features**:
   - Updated ZEDCamera.cs script to include GetInternalIMUOrientation function to get access to internal imu quaternion of the ZED-M.
   - Improved VAR (timewarp) when using HTC Vive in stereo pass-through.
   
- **Bug Fixes**:
   - Fixed camera FPS stats display when camera is disconnected. It shows now "disconnected" instead of the last camera FPS.



### 2.3.2
- **Features**:
   - Updated ZEDCamera.cs script to include setIMUPrior(sl::Transform) function used for video pass-through. 
   - Added Camera FPS, Engine FPS, Tracking Status, HMD and Plugin version in ZED Manager panel. These status will help developers see where the application's bottlenecks are.
A Camera FPS below 60FPS indicates a USB bandwidth issue. An Engine FPS below 90FPS indicates that rendering is the limiting factor. Both will induce drop frames.
Note that building and running your application greatly improves performance compared to playing the scene in Unity Editor
   - Added automatic detection of OVR package to avoid having to manually define "ZED_OCULUS" in project settings when using ZEDOculusControllerManager.
   - Added compression settings (RAW, LOSSLESS, LOSSY) in ZEDSVOManager.cs script.

- **Bug Fixes**:
   - Fixed initial camera position when using ZED Mini and an HMD. This had an impact on virtual objects created with physical gravity up and spatial mapping mesh origin.
   - Fixed enable/disable depth occlusions settings in Deferred rendering mode.
   - Fixed resize of halo effect in the Planetarium example.
   - Fixed garbage matte behavior in GreenScreen example that displayed anchor spheres in the scene after loading a matte
   - Fixed ZED Manager instance creation to respect MonoBehavior implementation. Only one ZED manager instance is available at a time for an application.
   - Fix Loading message when ZED tries to open.
   - Remove BallLauncher message instruction as gameobject, as it was not used.
   
### 2.3.1
- **Minor Bug fixes and Features** :
   - Fix GreenScreen broken prefab in Canvas.
   - Fix default spatial memory path when enableTracking is true. Could throw an exception when tracking was activated in green screen prefab.
   - Fix missing but unused script in Planetarium prefab
   - Added Unity Plugin version in ZEDCamera.cs
   
### 2.3.0
- **Features**:
   - Added support for ZED mini.
   - Added beta stereo passthrough feature with optimized rendering in VR headsets (only with ZED mini)
- **Prefabs**:
   - Added ZED_Rig_Stereo prefab, with stereo capture and stereo rendering to VR headsets (beta version)
   - Renamed ZED_Rig_Left in ZED_Rig_Mono, for better mono/stereo distinction.
- **Examples**:
   - Added Planetarium scene to demonstrate how to re-create the solar system in the real world. This is a simplified version of the ZEDWorld app.
   - Added Dark Room scene to demonstrate how to decrease the brightness of the real world using the "Real Light Brightness" setting in ZEDManager.cs.
   - Added Object Placement scene to demonstrate how to place an object on a horizontal plane in the real world.
- **Scripts**:
   - Added ZEDSupportFunctions.cs to help using depth and normals at a screen or world position. Some of these functions are used in ObjectPlacement scene.
   - Added ZEDMixedRealityPlugin.cs to handle stereo passthrough in Oculus Rift or HTC Vive Headset.
- **Renaming**:
  -  ZEDTextureOverlay.cs has been renamed ZEDRenderingPlane.cs.
- **Compatibility**:
  - Supports ZED SDK v2.3.0
  - Supports Unity 2017.x.y (with automatic updates from Unity).
- **Known Issues**:
  - On certain configurations, VRCompositor in SteamVR can freeze when using HTC Vive and ZED. Disabling Async Reprojection in SteamVR's settings can fix the issue.
  - The stereo passthrough experience is highly sensitive to Capture/Engine FPS. Here are some tips:
            * Make sure your PC meets the requirements for stereo pass-through AR (GTx 1070 or better).
            * Make sure you don't have other resource-intensive applications running on your PC at the same time as Unity.
            * Test your application in Build mode, rather than the Unity editor, to have the best FPS available.
             
### 2.2.0/2.1.0/2.0.0
See ZED SDK release notes.