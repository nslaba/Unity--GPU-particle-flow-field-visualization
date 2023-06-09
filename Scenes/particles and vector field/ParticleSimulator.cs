using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using TMPro;


// This script handles particle simulation

public class ParticleSimulator : MonoBehaviour
{
    /* STEP 1 : Vector Field Related Declarations*/
    public ComputeShader circularField;
    public RenderTexture circularFieldTexture;
    public float rotationSpeed = 5.0f;


    int threadGroupsX;
    int threadGroupsY;
    int fieldKernelID;

    int fieldWidth = 1920;
    int fieldHeight = 1080;


    /*********************************************************/
    private Vector2 cursorPos;

    /* STEP 2 : create a particle struct and structuredBuffer of Particle to store particle data*/

    public ComputeShader particleShader;
    private int kernelID;
    ComputeBuffer particleBuffer;
    // num of particles per thread wrap
    private const int WARP_SIZE = 256;
    // num of wraps needed
    private int mWarpCount;
   
    // In the future tie the particle's positions to the vector field
    
    public float deltaTime;
    public float time;
    Vector2 dimensions;

    // Particle struct
    struct Particle {
        public Vector3 positionCur;
        public Vector3 positionPrev;
        public Vector3 positionNew;
        public Vector3 velocity;
        public Vector3 accelaration;
        public Vector4 color;
        public Vector3 force; // Will be calculated by the vector field. I also need acceleration to calc velocity and position
        public float mass;
        public float lifetime;
    }
    
    private const int SIZE_PARTICLE = 96;
    // change # of particles to be 2073600 = 1920 x 1080. This way I can initialize the particles' positions per pixel
    private int numParticles = 2073600;

    // Material used to draw particles on screen
    public Material material;

    /* Step 3 : Declare mouse pos to pass to vert/frag shader*/
    [SerializeField]
    private Vector2 mousePosVF;


     /* FUNCTION : InitializeVectorField *******************
     *
     * use      : helper function to keep things organized.
     *            - Initializes the field stuff such as
     *              -> shader: texture, struct, thread wraps
     *
     *******************************************************/
    void InitVectorField()
    {
        // Debug.Log("At the start of InitVectorField");
        /* STEP 1 :  calc and update the nescessary vars */
        // 1) a. define dimensions
        dimensions = new Vector2(fieldWidth, fieldHeight);
        // Debug.Log("in InitVectorField : after defining dimensions (1a)");

         // 1) b. define threadGroups
        threadGroupsX = Mathf.CeilToInt(fieldWidth/64f);
        threadGroupsY = Mathf.CeilToInt(fieldHeight/8f);
        // Debug.Log("in InitVectorField : after defining thread groups (1b)");


        // 1) b. get the kernel ID
        fieldKernelID = circularField.FindKernel("GenerateCircularField");
        if (fieldKernelID == -1) Debug.Log("the fieldKernelID found is not null"); // My code doesn't even get here

        // Debug.Log("in InitVectorField : after step 1");

        /* STEP 2 :  Create texture and other vars */
        // Create a texture
        circularFieldTexture = new RenderTexture(fieldWidth, fieldHeight, 0);
        circularFieldTexture.enableRandomWrite = true;
        circularFieldTexture.Create();

        // Debug.Log("in InitVectorField : after step 2");

        /* STEP 3 : Bind the nescessary vars to shader & material*/
        // Set the texture as a parameter in the compute shader
        circularField.SetTexture(fieldKernelID, "_ForceField", circularFieldTexture);
        circularField.SetVector("_ForceFieldSize", dimensions);
        circularField.SetFloat("_Time", Time.time);
        circularField.SetFloat("_RotationSpeed", rotationSpeed);

        // Debug.Log("in InitVectorField : after step 3");

        /* STEP 4 : Dispatch only once since dealing with a steady Vector field*/
        circularField.Dispatch(fieldKernelID, threadGroupsX, threadGroupsY, 1);

    }


    /* FUNCTION : InitializeParticles **********************
     *
     * use      : helper function to keep things organized.
     *            - Initializes the particle stuff such as
     *              -> shader: buffer, struct, thread wraps
     *
     *******************************************************/
    void InitParticles()
    {
        /* STEP 1 :  calc and update the nescessary vars */
        // 1) a. WRAPS of threads
        mWarpCount = Mathf.CeilToInt((float) numParticles / WARP_SIZE);


        // 1) b. Set particle Buffer based on Particle size and # of particles
        particleBuffer = new ComputeBuffer(numParticles, SIZE_PARTICLE);

        // 1) c. Find & update the correct KernelID
        kernelID = particleShader.FindKernel("UpdateParticles");
        

        /* STEP 2 :  Set initial vals for each Particle in cpu particleArray*/
        Particle[] particleArray = new Particle[numParticles];
        for (int i=0; i <numParticles; i++)
        {
            // What I actually want is to initialize the position of 1 particle per pixel
           
            float row = Mathf.Floor(i/1920f); // 1920 is screen width
            float column = i - row * 1920f;
            row -= (1080f/2f);
            column -= (1920f/2f);
            

            // Temporarily initialize particles to a random pos and see if they move in a circle
            // particleArray[i].positionCur.x = UnityEngine.Random.value * 1.0f - 1.0f;
            // particleArray[i].positionCur.y = UnityEngine.Random.value - 2.0f; // 1080 is the screen height
            // particleArray[i].positionCur.z = 1;

            // // Initialize for Verlet
            // particleArray[i].positionPrev.x = UnityEngine.Random.value * 1.0f - 1.0f;
            // particleArray[i].positionPrev.y = UnityEngine.Random.value - 2.0f;
            // particleArray[i].positionPrev.z = 1;

            // particleArray[i].positionNew.x = UnityEngine.Random.value * 1.0f - 1.0f;
            // particleArray[i].positionNew.y = UnityEngine.Random.value - 2.0f;
            // particleArray[i].positionNew.z = 1;

            particleArray[i].positionCur.x = column / 100f;
            particleArray[i].positionCur.y = row / 100f; // 1080 is the screen height
            particleArray[i].positionCur.z = 1;

            // Initialize for Verlet
            particleArray[i].positionPrev.x = column / 100f;
            particleArray[i].positionPrev.y = row / 100f; 
            particleArray[i].positionPrev.z = 1;

            particleArray[i].positionNew.x = column / 100f;
            particleArray[i].positionNew.y = row / 100f; 
            particleArray[i].positionNew.z = 1;

            particleArray[i].velocity.x = 0;
            particleArray[i].velocity.y = 0;
            particleArray[i].velocity.z = 0;

            particleArray[i].accelaration.x = 0;
            particleArray[i].accelaration.y = 0;
            particleArray[i].accelaration.z = 0;

            particleArray[i].lifetime = UnityEngine.Random.value * 5.0f + 1.0f;

            particleArray[i].color.x = 1;
            particleArray[i].color.y = 0;
            particleArray[i].color.z = 0;
            particleArray[i].color.w = 1; 

            particleArray[i].force.x = 0;
            particleArray[i].force.y = 0;
            particleArray[i].force.z = 0;
            

            particleArray[i].mass = 0.8f;

            
        }

        /* STEP 3 : Bind the nescessary vars to shader & material*/
        // 3) a. Particle array to shader
        particleBuffer.SetData(particleArray);

        // 3) b. Compute buffer to shader
        particleShader.SetBuffer(kernelID, "particles", particleBuffer);

        // 3) c. Compute buffer to material
        material.SetBuffer("particleBuffer", particleBuffer);

        // 3) d. Set the data shared between the two shaders
        particleShader.SetTexture(kernelID, "_ForceField", circularFieldTexture);
        particleShader.SetVector("_ForceFieldSize", dimensions);
    }


    void Start()
    {
        time = Time.time;
        deltaTime = Time.deltaTime;
        Debug.Log("Start function called");
        /* STEP 1 : Initialize Vector Field */
        InitVectorField();

        if (circularField == null ) Debug.Log("Vector field is null");
        if (circularField != null) Debug.Log("Vector field is not null");
        /* STEP 2 : Initialize Particles*/
        InitParticles();

    }

    void OnRenderObject()
    {
        // Update and Pass mouse position
        mousePosVF = cursorPos;
        material.SetVector("_MousePosition", new Vector4(mousePosVF.x, mousePosVF.y, 0.0f, 0.0f));

        material.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Points, 1, numParticles);        
    }

    // Update is called once per frame
    void Update()
    {
        time = Time.time;
        deltaTime = Time.deltaTime;
        // Deal with mouse
        Vector3 p = new Vector3();
        Camera c = Camera.main;
        Vector2 mousePos = Input.mousePosition;

        p = c.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, c.nearClipPlane + 14)); // z = 3
        cursorPos.x = p.x;
        cursorPos.y = p.y;


        float[] mousePosition2D = {cursorPos.x, cursorPos.y};
        Debug.Log("mouse pos is");
        Debug.Log(cursorPos.x);
        Debug.Log(cursorPos.y);

        /* STEP 3: Update shader parameters*/
        // 3) a. regarding the particle shader
        particleShader.SetFloat("deltaTime", deltaTime);
        particleShader.SetFloats("mousePosition", mousePosition2D);

        // 3) b. regarding the vector field
        circularField.SetFloat("_Time", time);

        /* STEP 4: Dispatch */
        // circularField.Dispatch(fieldKernelID, threadGroupsX, threadGroupsY, 1);
       
        particleShader.Dispatch(kernelID, mWarpCount, 1, 1);
        

    }

    void OnGUI()
    {
        int fps = Mathf.RoundToInt(1.0f / deltaTime);
        string text = string.Format("{0} FPS", fps);
        GUI.Label(new Rect(10, 10, 100, 20), text);
    }

    void OnDestroy() {
        /* STEP 4: Release the particle buffer*/
        if (particleBuffer != null)
            particleBuffer.Release();
    }
}
