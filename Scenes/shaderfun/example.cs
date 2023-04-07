using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public class example : MonoBehaviour
// {
//     public Transform[] instances;
    
//     public int numAmount = 14;
//     public ComputeShader computeTest;
    
//     ComputeBuffer bufferOutputTest;
//     int kernel;
//     uint threadGroupSize;
//     Vector3[] outputArray;


//     // Start is called before the first frame update
//     void Start()
//     {
//         // First find the program we're executing
//         kernel = computeTest.FindKernel("CSMain");
//         // kernel ID will give us the size of each thread in the group
//         computeTest.GetKernelThreadGroupSizes(kernel, out threadGroupSize, out _, out _); // kernel ID, x, y, z


//         // Attach compute buffer to an object
//         // second element is the size of the buffer. first elem is the number of elements it'll hold
//         bufferOutputTest = new ComputeBuffer(numAmount, sizeof(float) * 3); 
//         outputArray = new Vector3[numAmount];

//         //spheres we use for visualisation
//         instances = new Transform[numAmount];
//         for (int i = 0; i < numAmount; i++)
//         {
//             instances[i] = Instantiate(Prefab, transform).transform;
//         }
        
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         computeTest.SetBuffer(kernel, "Output", bufferOutputTest);

//         // count how many thread groups are needed -> wanna have this be the same size as length of the array that's being updated
//         int threadGroups = (int)((numAmount + (threadGroupSize - 1)) / threadGroupSize);

//         computeTest.Dispatch(kernel, threadGroups, 1, 1);

//         // extract the data from GPU buffer to CPU
//         bufferOutputTest.GetData(outputArray);
        
//         //in update method
//         for (int i = 0; i < instances.Length; i++) 
//         {
//             instances[i].localPosition = outputArray[i];
//         }
//     }

//     void OnDestroy()
//     {
//         bufferOutputTest.Dispose();

//     }
        
// }
