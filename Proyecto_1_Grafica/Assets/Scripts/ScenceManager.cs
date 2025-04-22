using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    private Vector3[] vertices;

    private int[] triangles;

    private GameObject objetoCuadrado;

    private GameObject miCamara;
    
    private Vector3 camPos,camTarget,camUp;
    
    private Color[] colores;
    // Start is called before the first frame update
    void Start()
    {
        objetoCuadrado = new GameObject();

        objetoCuadrado.AddComponent<MeshFilter>();

        objetoCuadrado.GetComponent<MeshFilter>().mesh = new Mesh();

        objetoCuadrado.AddComponent<MeshRenderer>();

        CreateModel();
        UpdateMesh();
        CreateMaterial();

        CreateCamera();
        camPos = new Vector3(0,5,0);
        camTarget = new Vector3(0,0,0);
        camUp = new Vector3(0,0,1);
        RecalcularMatrices();
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void CreateModel() 
    {
        
        vertices = new Vector3[]
        {
            new Vector3(1,1,1),
            new Vector3(-1,1,1),
            new Vector3(1,1,-1),
            new Vector3(-1,1,-1),
            new Vector3(1,-1,1),
            new Vector3(-1,-1,1),
            new Vector3(1,-1,-1),
            new Vector3(-1,-1,-1),
        };

        triangles = new int[]{
            // Cara superior
            0,1,2,
            3,2,1,
            // Cara inferior
            4,6,5,
            7,5,6,
            // Cara frontal
            0,2,4,
            6,4,2,
            // Cara trasera
            1,5,3,
            7,3,5,
            // Cara izquierda
            1,0,5,
            4,5,0,
            // Cara derecha
            2,3,6,
            7,6,3
        };
        colores = new Color[]{
            new Color(0,0,0),
            new Color(0,0,1),
            new Color(0,1,0),
            new Color(0,1,1),
            new Color(1,0,0),
            new Color(1,0,1),
            new Color(1,1,0),
            new Color(1,1,1)
        };
    }

    private void UpdateMesh()
    {
        objetoCuadrado.GetComponent<MeshFilter>().mesh.vertices = vertices;

        objetoCuadrado.GetComponent<MeshFilter>().mesh.triangles = triangles;

        objetoCuadrado.GetComponent<MeshFilter>().mesh.colors = colores;
    }

    private void CreateCamera() {
        miCamara = new GameObject();
        miCamara.AddComponent<Camera>();

        miCamara.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;

        miCamara.GetComponent<Camera>().backgroundColor = Color.black;

    }

    private void CreateMaterial() {
        Material newMaterial = new Material(Shader.Find("ShaderBasico"));

        objetoCuadrado.GetComponent<MeshRenderer>().material = newMaterial;
    }

    private Matrix4x4 CreateModelMatrix(Vector3 newPosition, Vector3 newRotation, Vector3 newScale)
    {

    Matrix4x4 positionMatrix = new Matrix4x4(
        new Vector4(1f, 0f, 0f, newPosition.x), 
        new Vector4(0f, 1f, 0f, newPosition.y), 
        new Vector4(0f, 0f, 1f, newPosition.z),
        new Vector4(0f, 0f, 0f, 1f)
    );
    positionMatrix = positionMatrix.transpose;

    Matrix4x4 rotationMatrixX = new Matrix4x4(
        new Vector4(1f, 0f, 0f, 0f), // Primera columna
        new Vector4(0f, Mathf.Cos(newRotation.x), -Mathf. Sin (newRotation.x), 0f), 
        new Vector4(0f, Mathf. Sin (newRotation.x), Mathf.Cos(newRotation.x), 0f), 
        new Vector4(0f, 0f, 0f, 1f) // Cuarta columna
    );

    Matrix4x4 rotationMatrixY= new Matrix4x4(
        new Vector4(Mathf.Cos(newRotation.y), 0f, Mathf.Sin (newRotation.y), 0f), 
        new Vector4(0f, 1f, 0f, 0f),
        new Vector4(-Mathf.Sin (newRotation.y), 0f, Mathf.Cos(newRotation.y), 0f), 
        new Vector4(0f, 0f, 0f, 1f) 
    );

    Matrix4x4 rotationMatrixZ = new Matrix4x4(
        new Vector4(Mathf.Cos(newRotation.z), -Mathf.Sin (newRotation.z), 0f, 0f),
        new Vector4(Mathf.Sin (newRotation.z), Mathf.Cos(newRotation.z), 0f, 0f), 
        new Vector4(0f, 0f, 1f, 0f), // Tercera columna
        new Vector4(0f, 0f, 0f, 1f) // Cuarta columna
    );
    
    Matrix4x4 rotationMatrix = rotationMatrixZ * rotationMatrixY * rotationMatrixX;
    rotationMatrix = rotationMatrix.transpose;
    
    Matrix4x4 scaleMatrix = new Matrix4x4( 
        new Vector4(newScale.x, 0f, 0f, 0f),
        new Vector4(0f, newScale.y, 0f, 0f), 
        new Vector4(0f, 0f, newScale.z, 0f),
        new Vector4(0f, 0f, 0f, 1f) // Cuarta columna
    );
    scaleMatrix = scaleMatrix.transpose;
    
    Matrix4x4 finalMatrix = positionMatrix;
    
    finalMatrix *= rotationMatrix;
    finalMatrix *= scaleMatrix;
    return (finalMatrix);
    }

    private Matrix4x4 CreateViewMatrix(Vector3 pos, Vector3 target, Vector3 up)
    {
        Vector3 forward = (target-pos).normalized;
        up = up.normalized;
        Vector3 rigt = Vector3.Cross(forward,up).normalized;
        
        Matrix4x4 finalMatrix = new Matrix4x4(
            new Vector4(rigt.x, rigt.y, rigt.z, -Vector3.Dot(rigt,pos)), 
            new Vector4(up.x, up.y, up.z, -Vector3.Dot(up,pos)), 
            new Vector4(-forward.x, -forward.y, -forward.z, Vector3.Dot(forward,pos)),
            new Vector4(0f, 0f, 0f, 1f)
        );
        finalMatrix = finalMatrix.transpose;
        return (finalMatrix);
    }
    
    private Matrix4x4 CalculatePerspectiveProjectionMatrix(float fov, float asp, float near, float far)
    {
        Matrix4x4 finalMatrix = new Matrix4x4(
            new Vector4(1/asp*Mathf.Tan(fov/2), 0, 0, 0), 
            new Vector4(0, 1/Mathf.Tan(fov/2), 0,0), 
            new Vector4(0,0,(far+near)/(near-far),2*far*near/(near-far)),
            new Vector4(0f, 0f, -1f, 0f)
        );
        
        finalMatrix = finalMatrix.transpose;
        return (finalMatrix);
    }

    private void RecalcularMatrices()
    {
        Vector3 newPosition = new Vector3(0,0,0);
        Vector3 newRotation = new Vector3(0,0,0);
        Vector3 newScale = new Vector3(1,1,1);
        Matrix4x4 modelMatrix = CreateModelMatrix(newPosition, newRotation, newScale);
        objetoCuadrado.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix",modelMatrix);
        
        Matrix4x4 viewMatrix = CreateViewMatrix(camPos,camTarget,camUp);
        objetoCuadrado.GetComponent<Renderer>().material.SetMatrix("_ViewMatrix",viewMatrix);
       
        float fov = 90 * Mathf.Deg2Rad;
        float aspectoRatio = 16/(float)9;
        float nearClipPlane = 0.1f;
        float farClipPlane = 1000 ;
        Matrix4x4 projectionMatrix = CalculatePerspectiveProjectionMatrix(fov, aspectoRatio,nearClipPlane,farClipPlane);
        objetoCuadrado.GetComponent<Renderer>().material.SetMatrix("_ProjectionMatrix",GL.GetGPUProjectionMatrix(projectionMatrix,true));
                
    }
}