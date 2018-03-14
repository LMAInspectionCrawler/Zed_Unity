using UnityEngine;
/// <summary>
/// Functions support to help to retrieve depth and normals
/// </summary>
public class ZEDSupportFunctions
{
    /// <summary>
	/// Get the Normal vector at a given pixel (i,j). the Normal can be given regarding camera reference or in the world reference.
    /// </summary>
	/// <param name="pixel"> position of the pixel</param>
	/// <param name="reference_frame"> Reference frame given by the enum sl.REFERENCE_FRAME</param>
	/// <param name="cam"> Unity Camera (to access world to camera transform</param>
	/// <out> normal that will be filled </out>
    /// <returns> true if success, false otherwie</returns>
	public static bool GetNormalAtPixel(Vector2 pixel, sl.REFERENCE_FRAME reference_frame,Camera cam, out Vector3 normal)
    {
        Vector4 n;
		bool r = sl.ZEDCamera.GetInstance().GetNormalValue(new Vector3(pixel.x,pixel.y, 0), out n);
        
		switch (reference_frame) {
		case sl.REFERENCE_FRAME.CAMERA: 
			normal = n;
			break;

		case sl.REFERENCE_FRAME.WORLD: 
			normal = cam.worldToCameraMatrix.inverse * n;
			break;
		default :
			normal = Vector3.zero;
			break;
		}

        return r;
    }

	/// <summary>
	/// Get the Normal vector at a world position (x,y,z). the Normal can be given regarding camera reference or in the world reference.
	/// </summary>
	/// <param name="position"> world position</param>
	/// <param name="reference_frame"> Reference frame given by the enum sl.REFERENCE_FRAME</param>
	/// <param name="cam"> Unity Camera (to access world to camera transform</param>
	/// <out> normal vector that will be filled </out>
	/// <returns> true if success, false otherwie</returns>
	public static bool GetNormalAtWorldLocation(Vector3 position, sl.REFERENCE_FRAME reference_frame,Camera cam, out Vector3 normal)
    {
 		Vector4 n;
		bool r = sl.ZEDCamera.GetInstance().GetNormalValue(cam.WorldToScreenPoint(position), out n);

		switch (reference_frame) {
		case sl.REFERENCE_FRAME.CAMERA: 
			normal = n;
			break;

		case sl.REFERENCE_FRAME.WORLD : 
			normal = cam.worldToCameraMatrix.inverse*  n;
			break;

		default :
			normal = Vector3.zero;
			break;
		}

		return r;

    }

    /// <summary>
    /// Get depth value at a given image pixel
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public static bool GetDepthAtPixel(Vector2 pixel, out float depth)
    {
		float d = sl.ZEDCamera.GetInstance().GetDepthValue(new Vector3(pixel.x, pixel.y, 0));
        depth = d;

        if (d == -1) return false;
        return true;
    }


	/// <summary>
	/// Get the depth value at a world position (x,y,z). 
	/// </summary>
	/// <param name="position"> (x,y,z) world location</param>
	/// <param name="Camera"> Camera object</param>
    /// <out> Depth value in float </out>
	/// <returns></returns>
	public static bool GetDepthAtWorldLocation(Vector3 position, Camera cam,out float depth)
	{
		Vector3 pixelPosition = cam.WorldToScreenPoint (position);

		float d = sl.ZEDCamera.GetInstance().GetDepthValue(new Vector3(pixelPosition.x, pixelPosition.y, 0));
		depth = d;

		if (d == -1) return false;
		return true;
	}



	/// <summary>
	/// Get euclidean distance value at a given image pixel
	/// </summary>
	/// <param name="position"></param>
	/// <returns></returns>
	public static bool GetDistanceAtPixel(Vector2 pixel, out float distance)
	{
		float d = sl.ZEDCamera.GetInstance().GetDistanceValue(new Vector3(pixel.x, pixel.y, 0));
		distance = d;

		if (d == -1) return false;
		return true;
	}


	/// <summary>
	/// Get the euclidean distance value from a point at a world position (x,y,z) to the camera 
	/// </summary>
	/// <param name="position"> (x,y,z) world location</param>
	/// <param name="Camera"> Camera object</param>
	/// <out> Depth value in float </out>
	/// <returns></returns>
	public static bool GetDistanceAtWorldLocation(Vector3 position, Camera cam,out float distance)
	{
		Vector3 pixelPosition = cam.WorldToScreenPoint (position);

		float d = sl.ZEDCamera.GetInstance().GetDistanceValue(new Vector3(pixelPosition.x, pixelPosition.y, 0));
		distance = d;

		if (d == -1) return false;
		return true;
	}
    /// <summary>
    /// Get the world position of the given image pixel.
    /// </summary>
    /// <param name="pixelPos"></param>
    /// <param name="cam"></param>
    /// <returns>true if success, false otherwise</returns>
    public static bool GetWorldPositionAtPixel(Vector2 pixel, Camera cam, out Vector3 worldPos)
    {
 		float d;
		worldPos = Vector3.zero;
		if (!GetDepthAtPixel(pixel, out d)) return false;

		//convert regarding screen size
		float xp = pixel.x * sl.ZEDCamera.GetInstance().ImageWidth / Screen.width;
		float yp = pixel.y * sl.ZEDCamera.GetInstance().ImageHeight / Screen.height;
		//Extract world position using S2W
		worldPos = cam.ScreenToWorldPoint(new Vector3(xp, yp,d));
	    return true;
    }


    /// <summary>
	/// Checks if a world location is visible from the camera (true) or masked by a virtual object (with collider)
	/// This uses the raycast to check if we hit something or not
    /// </summary>
	/// <warning> This only occurs with the virtual objects that have a collider </warning>
	/// <param name="position"> (x,y,z) world location</param>
    /// <param name="cam"></param>
    /// <returns></returns>
    public static bool IsLocationVisible(Vector3 position, Camera cam)
    {
        RaycastHit hit;
		float d;
		GetDepthAtWorldLocation(position, cam,out d);
        if (Physics.Raycast(cam.transform.position, position - cam.transform.position, out hit))
        {
            if (hit.distance < d) return false;
        }
        return true;
    }


	/// <summary>
	/// Checks if an image pixel is visible from the camera (true) or masked by a virtual object (with collider)
	/// This uses the raycast to check if we hit something or not
	/// </summary>
	/// <warning> This only occurs with the virtual objects that have a collider </warning>
	/// <param name="pixel"></param>
	/// <param name="cam"></param>
	/// <returns></returns>
	public static bool IsPixelVisible(Vector2 pixel, Camera cam)
	{
		RaycastHit hit;
		float d;
		GetDepthAtPixel(pixel,out d);
		Vector3 position = cam.ScreenToWorldPoint(new Vector3(pixel.x, pixel.y, d));
		if (Physics.Raycast(cam.transform.position, position - cam.transform.position, out hit))
		{
			if (hit.distance < d) return false;
		}
		return true;
	}


}
