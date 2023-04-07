using System.Collections;
using TMPro;
// using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MandelbrotCS : MonoBehaviour
{
    // Define variables
    double width = 5;
    double height = 2.5;
    double rStart = -3.25;
    double iStart = -1.4;
    int maxIterations = 1024;
    float zoom;

    // Compute Shader stuff
    public ComputeShader shader;
    ComputeBuffer buffer;
    RenderTexture texture;
    public RawImage image;

    //GUI Resources
    public TextMeshProUGUI real, imag, w, h, ite, frame;
    public int increment = 3;
    //public float zoomSpeed = 0.5f;

    // Mandelbrot param
    


    // Data for the Compute Buffer
    public struct DataStruct
    {
        public double w, h, r, i;
        public int screenWidth, screenHeight;
    }

    DataStruct[] data;

    // Start is called before the first frame update
    void Start()
    {
        width = 4.5;
        height = width * Screen.height / Screen.width;
        rStart = -2.0;
        iStart = -1.25;
        maxIterations = 500;
        increment = 3;
        zoom = 0.5f;

        data = new DataStruct[1];

        data[0] = new DataStruct{
            w = width,
            h = height,
            r = rStart,
            i = iStart,
            screenWidth = Screen.width,
            screenHeight = Screen.height
        };

        buffer = new ComputeBuffer(data.Length, 40); // second parameter is size of pacage: double = 8 bytes, int = 4 bytes
        texture = new RenderTexture(Screen.width, Screen.height, 0);
        texture.enableRandomWrite = true;
        texture.Create();

        Mandelbrot();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Mandelbrot()
    {
        int kernelHandle = shader.FindKernel("CSMain");

        buffer.SetData(data);
        shader.SetBuffer(kernelHandle, "buffer", buffer);

        shader.SetInt("maxIterations", maxIterations);
        shader.SetTexture(kernelHandle, "Result", texture);

        shader.Dispatch(kernelHandle, Screen.width / 24, Screen.height / 24, 1);

        RenderTexture.active = texture;
        image.material.mainTexture = texture; 
    }

    private void OnDestroy()
    {
        buffer.Dispose();
    }
}
