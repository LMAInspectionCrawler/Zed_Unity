using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;


/*
 * LMA CRAWLER TEAM
 * Author: Orlando Gordillo
 * Date: 03/13/2018
 * 
 * Class: ComputerVisionAlgorithms
 * 
 * Description:
 * The ComputerVisionAlgorithms class should be used as a way to 
 * communicate with our OpenCV couterpart written in C++ through a 
 * Visual Studio generated DLL file [LMA_OPENCV_PLUGIN.dll]
 * 
 * OpenCV and Unity communicate images through byte pointers (byte[])
 * Zed doesn't process it's images through the CPU. Instead Zed
 * processes all frames through the GPU. In order to pass the raw
 * byte data of a Zed image to OpenCV, we must first get the data 
 * from the GPU onto the CPU. Unity can do this through compute
 * shaders or the readpixels method. We used the readpixels method.
 * Then we can convert that data into raw byte data and pass it into
 * OpenCV. OpenCV returns raw data which we convert back into an 
 * usable Unity 2DTexture. 
 * 
 */

public class ComputerVisionAlgorithms : MonoBehaviour {
    //OpenCV DLL Wrapper Functions
    //In order to get the EntryPoints of the LMA_OPENCV_PLUGIN functions 
    //I used a tool called Dependency Walker to inspect the dll

    /// <summary>
    /// Frees up memory used by a frequently accessed pointer in the OpenCV C++ side of things
    /// </summary>
    [DllImport("LMA_OPENCV_PLUGIN", EntryPoint = "?FreeMem@Functions@LMA_OPENCV_PLUGIN@@SAXXZ")]
    private static extern void FreeMem();

    /// <summary>
    /// Returns a Sobel Image (still in development)
    /// </summary>
    [DllImport("LMA_OPENCV_PLUGIN", EntryPoint = "?SobelFrame@Functions@LMA_OPENCV_PLUGIN@@SAPEAEPEAEHH@Z")]
    private static extern byte[] SobelFrame(byte[] img, int zcol, int zrow);


    /// <summary>
    /// Material for this gameobject
    /// </summary>
    Material m;

    /// <summary>
    /// A byte pointer that we pass back and forth between Unity and OpenCV for the current image 
    /// </summary>
    byte[] imgData;

    /// <summary>
    /// texture that we use to store the shader corrected 2D texture 
    /// </summary>
    Texture2D tex;
    
    /// <summary>
    /// The current texture being fed to the GPU
    /// </summary>
    RenderTexture currentActiveRT;

    /// <summary>
    /// Instance of the ZED
    /// </summary>
    private sl.ZEDCamera zed;

    /// <summary>
    /// 3D positions in x y z 
    /// </summary>
    private Texture2D XYZTexture;

    /// <summary>
    /// color of each pixel (no actual pixel color data actually stored on here)
    /// </summary>
    private Texture2D colorTexture;

    /// <summary>
    /// RenderTexture of the x y z 2Dtexture being pushed into the GPU
    /// </summary>
    private RenderTexture XYZTextureCopy = null;

    /// <summary>
    /// RenderTexture of the r g b 2Dtexture being pushed into the GPU
    /// </summary>
    private RenderTexture ColorTextureCopy = null;



    void Start()
    {
        //Mat ZED Unlit BGRA shader converts our RGBA32 raw data into pretty looking BGRA32
        m = Resources.Load("Materials/Unlit/Mat_ZED_Unlit_BGRA") as Material;

        //set our current gameobjects material to the new BGRA one
        this.GetComponent<Renderer>().material = m;

        //Get the zed camera instance (it is a singleton)
        zed = sl.ZEDCamera.GetInstance();

        //initialize our 2Dtexture placeholder
        tex = new Texture2D(1920, 1080, TextureFormat.RGBA32, false);
    }


    void OnApplicationQuit()
    {
        //reset material
        m = null;

        //Release x y z data on GPU
        if (XYZTextureCopy != null)
            XYZTextureCopy.Release();
        
        //Release r g b data on GPU
        if (ColorTextureCopy != null)
            ColorTextureCopy.Release();
    }

    void Update()
    {
       
        //Wait until the zed is awake *rise n shine*
        if (zed.IsCameraReady)
        {
            //this 2D color texture from zed but has no usuable color values until
            //it is passed into the GPU. Then a Unity shader can correct it into BGRA32
            colorTexture = zed.CreateTextureImageType(sl.VIEW.LEFT);

            //if our GPU is hungry
            if (ColorTextureCopy == null)
            {
                //create a rendertexture with the same specs as our color texture
                ColorTextureCopy = new RenderTexture(colorTexture.width, colorTexture.height, 0, RenderTextureFormat.ARGB32);
            }

            //feed the GPU a colortexture from the CPU *nom nom nom nom*
            Graphics.Blit(colorTexture, ColorTextureCopy);

            //make sure we found our material and loaded it. 
            if (m != null)
            {
                //set our materials main texture to the rgb values stored in the GPU
                m.SetTexture("_ColorTex", ColorTextureCopy);

                //Get the GPU corrected pixel data and store it into a 2D texture
                tex = GetRTPixels(ColorTextureCopy);

                //Apply those values to the 2D texture
                tex.Apply();

                //Turn the RGBA32 pixel data into a byte array
                imgData = tex.GetRawTextureData();

                //Pass that byte array to our OpenCV counterpart through the wrapper function SobelFrame
                imgData = SobelFrame(imgData, 1920, 1080);
                
                //Load whatever OpenCV returned into a RGBA32 2D Texture
                tex.LoadRawTextureData(imgData);

                //Apply those values to the 2D texture
                tex.Apply();

                //Free the byte array on the OpenCV counterpart through the wrapper function FreeMem
                FreeMem();

                //Set our material's main texture to whatever OpenCV returned
                m.mainTexture = tex;
            }

        }

    }

    /// <summary>
    /// Gets the GPU (rendertexture) data and puts it
    /// on the CPU into aa a Texture2D
    /// </summary>
    /// <param name="rt"></param>
    /// <returns></returns>
    public Texture2D GetRTPixels(RenderTexture rt)
    {
        //Currently active render texture on GPU
        RenderTexture currentActiveRT = RenderTexture.active;

        //Set the supplied RenderTexture as the active one
        RenderTexture.active = rt;

        //Create a new Texture2D and read the RenderTexture image into it
        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);

        //Restore the previously active render texture
        RenderTexture.active = currentActiveRT;

        //Return the Texture2D with the corrected color data now living on the CPU
        return tex;
    }

}
