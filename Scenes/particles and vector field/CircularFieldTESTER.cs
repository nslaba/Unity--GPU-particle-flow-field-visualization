using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularFieldTESTER : MonoBehaviour
{

    public ComputeShader circularField;
    public Material fieldMaterial;
    public float rotationSpeed = 5.0f;
    

    int threadGroupsX;
    int threadGroupsY;
    int kernelID;
    // Start is called before the first frame update
    void Start()
    {
        int width = 1920;
        int height = 1080;

        // Create a texture
        RenderTexture circularFieldTexture = new RenderTexture(width, height, 0);
        circularFieldTexture.enableRandomWrite = true;
        circularFieldTexture.Create();
        
        kernelID = circularField.FindKernel("GenerateCircularField");

        // Set the texture as a parameter in the compute shader
        circularField.SetTexture(kernelID, "_VectorField", circularFieldTexture);
        
        Vector2 dimensions = new Vector2(width, height);
        circularField.SetVector("_VectorFieldSize", dimensions);
        circularField.SetFloat("_Time", Time.deltaTime);
        circularField.SetFloat("_RotationSpeed", rotationSpeed);

        threadGroupsX = Mathf.CeilToInt(width/32f);
        threadGroupsY = Mathf.CeilToInt(height/8f);
        

        // Bind the texture to the material
        fieldMaterial.SetTexture("_MainTex", circularFieldTexture);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // Render the texture to the screen
        Graphics.Blit(null, destination, fieldMaterial);
    }
    // Update is called once per frame
    void Update()
    {
        circularField.Dispatch(kernelID, threadGroupsX, threadGroupsY, 1);
    }

}
