using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    private Vector3[] vertices;

    private int[] triangles;

    private GameObject objetoCuadrado;

    private GameObject CamaraOrb, CamaraPp;

    private bool modoPp = false;

    private float velRotMouseOrb, velRotTeclasOrb;

    private Vector3 camPos,camTarget,camUp;

    private float ppRotX = 0f, ppRotY = 0f, velPp = 5f;

    private Vector3 camPosPp, camForwardPp, camUpPp;
    
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
        camPosPp = new Vector3(0,1,-3);
        camForwardPp = new Vector3(0,0,1);
        camUpPp = new Vector3(0,1,0);
        velRotMouseOrb = 100f;
        velRotTeclasOrb = 30f;
        RecalcularMatrices();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
            modoPp = !modoPp;
        if(modoPp)
            UpdatePpCam();
        else
            CamaraOrbital();
        RecalcularMatrices();
    }

    private void UpdatePpCam(){
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 right = Vector3.Cross(camForwardPp, camUpPp).normalized;
        Vector3 moveDir = camForwardPp * v + right * h;
        camPosPp += moveDir * velPp * Time.deltaTime;
        if(Input.GetMouseButton(0)){
           ppRotX += Input.GetAxis("Mouse X") * 2f;
           ppRotY -= Input.GetAxis("Mouse Y") * 2f;
           ppRotY = Mathf.Clamp(ppRotY, -85f, 85f);
        }
        Quaternion rotX = Quaternion.AngleAxis(ppRotX, Vector3.up);
        Quaternion rotY = Quaternion.AngleAxis(ppRotY, Vector3.right);
        camForwardPp = (rotY * rotX* Vector3.forward).normalized;
        camUpPp = Vector3.up;
    }

    private void CamaraOrbital(){
        float rotHorizontal = 0f;
        float rotVertical = 0f;
        if(Input.GetMouseButton(0)){
            rotHorizontal = Input.GetAxis("Mouse X") * velRotMouseOrb * Time.deltaTime;
            rotVertical = Input.GetAxis("Mouse Y") * velRotMouseOrb * Time.deltaTime;
        }
        // Rotaci�n con el teclado
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            rotHorizontal -= velRotTeclasOrb * Time.deltaTime;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            rotHorizontal += velRotTeclasOrb * Time.deltaTime;
        if (Input.GetKey(KeyCode.UpArrow)|| Input.GetKey(KeyCode.W))
            rotVertical += velRotTeclasOrb * Time.deltaTime;
        if (Input.GetKey(KeyCode.DownArrow)|| Input.GetKey(KeyCode.S))
            rotVertical -= velRotTeclasOrb * Time.deltaTime;

        // Rotaci�n en el eje Y
        Quaternion rotY = Quaternion.AngleAxis(rotHorizontal, camUp);
        camPos = rotY * (camPos - camTarget) + camTarget;

        // Rotaci�n en el eje X
        Vector3 right = Vector3.Cross(camUp, camPos - camTarget).normalized;
        Quaternion rotX = Quaternion.AngleAxis(rotVertical, right);
        camPos = rotX * (camPos - camTarget) + camTarget;
        camUp = Vector3.Cross(camPos - camTarget, right).normalized;
    }

    private void RecalcularMatrices(){
        Vector3 newPosition = Vector3.zero;
        Vector3 newRotation = Vector3.zero;
        Vector3 newScale = Vector3.one;
        Matrix4x4 modelMatrix = CreateModelMatrix(newPosition, newRotation, newScale);
        objetoCuadrado.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix", modelMatrix);

        Vector3 viewPos = modoPp ? camPosPp : camPos;
        Vector3 viewTarget = modoPp ? (camPosPp + camForwardPp) : camTarget;
        Vector3 viewUp = modoPp ? camUpPp : camUp;
        Matrix4x4 viewMatrix = CreateViewMatrix(viewPos, viewTarget, viewUp);
        objetoCuadrado.GetComponent<Renderer>().material.SetMatrix("_ViewMatrix", viewMatrix);

        float fov = Mathf.Deg2Rad *90;
        float aspectRatio = 16f / 9f;
        float near = 0.1f, far = 1000f;
        Matrix4x4 projMatrix = CalculatePerspectiveProjectionMatrix(fov, aspectRatio, near, far);
        objetoCuadrado.GetComponent<Renderer>().material.SetMatrix("_ProjectionMatrix", GL.GetGPUProjectionMatrix(projMatrix, true));
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
        CamaraOrb = new GameObject();
        CamaraOrb.AddComponent<Camera>();

        CamaraOrb.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;

        CamaraOrb.GetComponent<Camera>().backgroundColor = Color.black;

    }

    private void CreateMaterial() {
        Material newMaterial = new Material(Shader.Find("ShaderBasico"));

        objetoCuadrado.GetComponent<MeshRenderer>().material = newMaterial;
    }
   
}