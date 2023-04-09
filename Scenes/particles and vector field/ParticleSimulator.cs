using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script handles particle simulation

public class ParticleSimulator : MonoBehaviour
{
    private Vector2 cursorPos;

    /* STEP 1: create a particle struct and structuredBuffer of Particle to store particle data*/

    public ComputeShader particleShader;
    private int kernelID;
    ComputeBuffer particleBuffer;
    // num of particles per thread wrap
    private const int WARP_SIZE = 256;
    // num of wraps needed
    private int mWarpCount;

   
    // In the future tie the particle's positions to the vector field
    
    public float deltaTime;

    // Particle struct
    struct Particle {
        public Vector3 position;
        public Vector3 velocity;
        public Vector4 color;
        public Vector3 force;
        public float size;
        public float lifetime;
    }
    
    private const int SIZE_PARTICLE = 60;

    private int numParticles = 1000000;

    // Material used to draw particles on screen
    public Material material;


    void Start()
    {
        
        
        // calc how many WARPs there'll be
        mWarpCount = Mathf.CeilToInt((float) numParticles / WARP_SIZE);
        // Initialize the particles 
        // Later figure out how to initialize the particles based on the vector field
        Particle[] particleArray = new Particle[numParticles];
        for (int i=0; i <numParticles; i++)
        {
            float x = Random.value * 2 - 1.0f;
            float y = Random.value * 2 - 1.0f;
            float z = Random.value * 2 - 1.0f;
            Vector3 xyz = new Vector3(x, y, z);
            xyz.Normalize();
            xyz *= Random.value;
            xyz *= 0.5f;

            particleArray[i].position.x = xyz.x;
            particleArray[i].position.y = xyz.y;
            particleArray[i].position.z = xyz.z + 3;

            particleArray[i].velocity.x = 0;
            particleArray[i].velocity.y = 0;
            particleArray[i].velocity.z = 0;

            particleArray[i].lifetime = Random.value * 5.0f + 1.0f;

            particleArray[i].color.x = 1;
            particleArray[i].color.y = 0;
            particleArray[i].color.z = 0;
            particleArray[i].color.w = 1; 

            particleArray[i].force.x = 0;
            particleArray[i].force.y = 0;
            particleArray[i].force.z = 0;
            

            particleArray[i].size = 0.8f;

            particleArray[i].lifetime = 40;
        }
        
        
        particleBuffer = new ComputeBuffer(numParticles, SIZE_PARTICLE);
        particleBuffer.SetData(particleArray);
        // find the id of the kernel
        kernelID = particleShader.FindKernel("UpdateParticles");

        //bind the compute buffer to the shader and the compute shader
        particleShader.SetBuffer(kernelID, "particles", particleBuffer);
        material.SetBuffer("particleBuffer", particleBuffer);
    }

    void OnRenderObject()
    {
        material.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Points, 1, numParticles);
    }

    // Update is called once per frame
    void Update()
    {
        deltaTime = Time.deltaTime;
        // Deal with mouse
        float[] mousePosition2D = {cursorPos.x, cursorPos.y};

        /* STEP 3: Update shader parameters*/
        particleShader.SetFloat("deltaTime", Time.deltaTime);
        particleShader.SetFloats("mousePosition", mousePosition2D);

        // Deal with vector field
       
        // Dispatch the particle shader
        particleShader.Dispatch(kernelID, mWarpCount, 1, 1);
    }

    void OnGui()
    {
        Vector3 p = new Vector3();
        Camera c = Camera.main;
        Event e = Event.current;
        Vector2 mousePos = new Vector2();

        mousePos.x = e.mousePosition.x;
        mousePos.y = c.pixelHeight - e.mousePosition.y;

        p = c.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, c.nearClipPlane + 14)); // z = 3
        cursorPos.x = p.x;
        cursorPos.y = p.y;
    }

    void OnDestroy() {
        /* STEP 4: Release the particle buffer*/
        if (particleBuffer != null)
            particleBuffer.Release();
    }
}
