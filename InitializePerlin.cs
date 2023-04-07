using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
**
** This script creates a new render texture to store the vector field data, sets
** the texture as the target of the compute shader, and dispatches the compute shader
** to initialize the vector field. Lastly it reads the render texture into a Texture2D 
** to use as the vector field texture in the particle simulation.
**/

public class InitializePerlin : MonoBehaviour
{
    // create a new compute shader object
    public ComputeShader perlinCompute;
    public Vector2Int perlinSize = new Vector2Int(256, 256);
    public Texture2D perlinTexture;

    private RenderTexture _perlinTexture;
 
    // Compute buffers
    private ComputeBuffer perlinBuffer;

    // define the kernel ID for the compute shader function for perlin
    private int perlinKernel;


    // Start is called before the first frame update
    void Start()
    {
        // Create a new render texture for the vector field
        _perlinTexture = new RenderTexture(perlinSize.x, perlinSize.y, 0);
        _perlinTexture.enableRandomWrite = true;
        _perlinTexture.Create();

        // Set the render texture as the target of the compute shader
        perlinCompute.SetTexture(0, "Result", _perlinTexture);

        // Set the resolution of the vector field in the compute shader
        perlinCompute.SetVector("Resolution", new Vector2(perlinSize.x, perlinSize.y));

        // Dispatch the compute shader to initialize the vector field
        int threadGroupsX = Mathf.CeilToInt(perlinSize.x / 8f);
        int threadGroupsY = Mathf.CeilToInt(perlinSize.y / 8f);
        perlinCompute.Dispatch(0, threadGroupsX, threadGroupsY, 1);

        // Read the render texture into a Texture2D to use as the vector field texture
        perlinTexture = new Texture2D(perlinSize.x, perlinSize.y, TextureFormat.RGBAFloat, false);
        RenderTexture.active = _perlinTexture;
        perlinTexture.ReadPixels(new Rect(0, 0, perlinSize.x, perlinSize.y), 0, 0);
        perlinTexture.Apply();
        RenderTexture.active = null;
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        // Clean up the render texture when the script is destroyed
        if (_perlinTexture != null)
        {
            _perlinTexture.Release();
            Destroy(_perlinTexture);
        }
    }

}
