using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script handles particle simulation

public class ParticleSimulator : MonoBehaviour
{
    /* STEP 1: create a particle struct and structuredBuffer of Particle to store particle data*/

    public ComputeShader particleShader;
   
    public float perlinFieldSize;
    public float deltaTime;

    private ComputeBuffer particleBuffer;

    struct Particle {
        public Vector3 position;
        public Vector3 velocity;
        public Vector4 color;
        public Vector3 force;
        public float size;
        public float lifetime;
    }
    

    void Start()
    {
        /* STEP 2: populate particle buffer*/
        int numParticles = 1000;
        particleBuffer = new ComputeBuffer(numParticles, sizeof(float) * 6);
        
    }

    // Update is called once per frame
    void Update()
    {
        /* STEP 3: Update shader parameters*/
        particleShader.SetFloat("deltaTime", deltaTime);
        particleShader.SetVector("perlinFieldSize", new Vector3( perlinFieldSize, perlinFieldSize, perlinFieldSize));
        particleShader.SetVector("inversePerlinFieldSize", new Vector3(1f/perlinFieldSize, 1f/perlinFieldSize, 1f/perlinFieldSize));
        particleShader.SetBuffer(0, "particles", particleBuffer);

        // Dispatch the particle shader
        particleShader.Dispatch(0, particleBuffer.count/64, 1, 1);
    }

    void OnDestroy() {
        /* STEP 4: Release the particle buffer*/
        particleBuffer.Release();
    }
}
