using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularFieldTESTER : MonoBehaviour
{

    public RenderTexture circularFieldTexture;
    public ComputeShader circularField;
    //public Material fieldMaterial;
    public float rotationSpeed = 5.0f;

    // Rendering
    public ParticleSystem particleSys;
    public float particleSpeedMultiplier = 1f;
    private Texture2D texture2D;
    

    int threadGroupsX;
    int threadGroupsY;
    int fieldKernelID;

    int fieldWidth = 1920;
    int fieldHeight = 1080;

    /* FUNCTION : InitializeVectorField *******************
     *
     * use      : helper function to keep things organized.
     *            - Initializes the field stuff such as
     *              -> shader: texture, struct, thread wraps
     *
     *******************************************************/
    void InitVectorField()
    {
        Debug.Log("At the start of InitVectorField");
        /* STEP 1 :  calc and update the nescessary vars */
        // 1) a. define dimensions
        Vector2 dimensions = new Vector2(fieldWidth, fieldHeight);
        Debug.Log("in InitVectorField : after defining dimensions (1a)");

         // 1) b. define threadGroups
        threadGroupsX = Mathf.CeilToInt(fieldWidth/32f);
        threadGroupsY = Mathf.CeilToInt(fieldHeight/8f);
        Debug.Log("in InitVectorField : after defining thread groups (1b)");

        

        // 1) b. get the kernel ID
        fieldKernelID = circularField.FindKernel("GenerateCircularField");
        if (fieldKernelID == -1) Debug.Log("the fieldKernelID found is not null"); // My code doesn't even get here

        Debug.Log("in InitVectorField : after step 1");

        /* STEP 2 :  Create texture and other vars */
        // Create a texture
        circularFieldTexture = new RenderTexture(fieldWidth, fieldHeight, 0);
        circularFieldTexture.enableRandomWrite = true;
        circularFieldTexture.Create();

        Debug.Log("in InitVectorField : after step 2");

        /* STEP 3 : Bind the nescessary vars to shader & material*/
        // Set the texture as a parameter in the compute shader
        circularField.SetTexture(fieldKernelID, "_VectorField", circularFieldTexture);
        circularField.SetVector("_VectorFieldSize", dimensions);
        circularField.SetFloat("_Time", Time.deltaTime);
        circularField.SetFloat("_RotationSpeed", rotationSpeed);

        Debug.Log("in InitVectorField : after step 3");
    }



    // Start is called before the first frame update
    void Start()
    {
        
        // InitVectorField();
        
        // circularField.Dispatch(fieldKernelID, threadGroupsX, threadGroupsY, 1);
        // texture2D = new Texture2D(circularFieldTexture.width, circularFieldTexture.height, TextureFormat.RGBAFloat, false);
        // Graphics.Blit(texture2D, circularFieldTexture);

        
        // Graphics.CopyTexture(circularFieldTexture, texture2D);

        // var velModule = particleSys.velocityOverLifetime;
        // var curveX = new AnimationCurve();
        // var curveY = new AnimationCurve();
        // var curveZ = new AnimationCurve();

        // for (int x = 0; x < texture2D.width; x++)
        // {
        //     for (int y = 0; y < texture2D.height; y++)
        //     {
        //         var color = texture2D.GetPixel(x, y);
        //         var direction = new Vector3(color.r, color.g, color.b);
        //         var magnitude = color.a * particleSpeedMultiplier;
        //         var position = new Vector3(x, y, 0f);

        //         curveX.AddKey(new Keyframe(0f, direction.x * magnitude, 0f, 0f));
        //         curveY.AddKey(new Keyframe(0f, direction.y * magnitude, 0f, 0f));
        //         curveZ.AddKey(new Keyframe(0f, direction.z * magnitude, 0f, 0f));
        //     }
        // }

        // velModule.x = new ParticleSystem.MinMaxCurve(0f, curveX);
        // velModule.y = new ParticleSystem.MinMaxCurve(0f, curveY);
        // velModule.z = new ParticleSystem.MinMaxCurve(0f, curveZ);
    
        // Bind the texture to the material
        //fieldMaterial.SetTexture("_MainTex", circularFieldTexture);
    }

    // private void OnRenderImage(RenderTexture source, RenderTexture destination)
    // {
    //     // Render the texture to the screen
    //     Graphics.Blit(null, destination, fieldMaterial);
    // }
    // Update is called once per frame
    void Update()
    {
    //     circularField.Dispatch(fieldKernelID, threadGroupsX, threadGroupsY, 1);
    //     Graphics.CopyTexture(circularFieldTexture, texture2D);


    //     var velModule = particleSys.velocityOverLifetime;
    //     var curveX = new AnimationCurve();
    //     var curveY = new AnimationCurve();
    //     var curveZ = new AnimationCurve();

    //     for (int x = 0; x < texture2D.width; x++)
    //     {
    //         for (int y = 0; y < texture2D.height; y++)
    //         {
    //             var color = texture2D.GetPixel(x, y);
    //             var direction = new Vector3(color.r, color.g, color.b);
    //             var magnitude = color.a * particleSpeedMultiplier;
    //             var position = new Vector3(x, y, 0f);

    //             curveX.AddKey(new Keyframe(0f, direction.x * magnitude, 0f, 0f));
    //             curveY.AddKey(new Keyframe(0f, direction.y * magnitude, 0f, 0f));
    //             curveZ.AddKey(new Keyframe(0f, direction.z * magnitude, 0f, 0f));
    //         }
    //     }

    //     velModule.x = new ParticleSystem.MinMaxCurve(0f, curveX);
    //     velModule.y = new ParticleSystem.MinMaxCurve(0f, curveY);
    //     velModule.z = new ParticleSystem.MinMaxCurve(0f, curveZ);
     }
    

}
