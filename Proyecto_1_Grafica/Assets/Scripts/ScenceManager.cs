using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
   
    private List<GameObject> objetos;
    
    private GameObject Paredes,Techo;

    private GameObject CamaraOrb, CamaraPp;

    private bool modoPp = false;

    private float velRotMouseOrb, velRotTeclasOrb;

    private Vector3 camPos,camTarget,camUp;

    private float ppRotX = 0f, ppRotY = 0f, velPp = 5f;

    private Vector3 camPosPp, camForwardPp, camUpPp;
    
    private Matrix4x4 projMatrix;

    // Start is called before the first frame update
    void Start()
    {
        float fov = Mathf.Deg2Rad *90;
        float aspectRatio = 16f / 9f;
        float near = 0.1f, far = 1000f;
        projMatrix = CalculatePerspectiveProjectionMatrix(fov, aspectRatio, near, far);

        objetos = new List<GameObject>();
        cargarObjetos();
        
        CreateCamera();
        camPos = new Vector3(0,1.5f,0);
        camTarget = new Vector3(3.5f,1.5f,4.5f);
        camUp = new Vector3(0,1,0);
        camPosPp = new Vector3(3.5f,1.5f,-3);
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

    private void UpdatePpCam()
    {
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

        Vector3 viewPos = modoPp ? camPosPp : camPos;
        Vector3 viewTarget = modoPp ? (camPosPp + camForwardPp) : camTarget;
        Vector3 viewUp = modoPp ? camUpPp : camUp;
        Matrix4x4 viewMatrix = CreateViewMatrix(viewPos, viewTarget, viewUp);

        float fov = Mathf.Deg2Rad *90;
        float aspectRatio = 16f / 9f;
        float near = 0.1f, far = 1000f;
        Matrix4x4 projMatrix = CalculatePerspectiveProjectionMatrix(fov, aspectRatio, near, far);
        foreach(GameObject objeto in objetos)
            objeto.GetComponent<Renderer>().material.SetMatrix("_ViewMatrix", viewMatrix);
            
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

    private void CreateCamera() {
        CamaraOrb = new GameObject();
        CamaraOrb.AddComponent<Camera>();

        CamaraOrb.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;

        CamaraOrb.GetComponent<Camera>().backgroundColor = Color.black;

    }
    
    private void cargarObjetos()
    {
        cargarPiso();
        cargarTecho();
        cargarParedes();
        cargarBed();
        cargarToilet();
    }

    private void cargarPiso(){
        FileReader lector = new FileReader("Piso");
        GameObject Piso = lector.getObject();
        Vector3 vmin = lector.coordenadasMinimas();

        Vector3 newPosition = new Vector3(-vmin.x,0,-vmin.z);
        Vector3 newRotation = new Vector3(0,0,0);
        Vector3 newScale = new Vector3(1,1,1);
        Matrix4x4 modelMatrix = CreateModelMatrix(newPosition, newRotation, newScale);
        Piso.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix", modelMatrix);
        Piso.GetComponent<Renderer>().material.SetMatrix("_ProjectionMatrix", GL.GetGPUProjectionMatrix(projMatrix, true));
        objetos.Add(Piso);
    }

    private void cargarTecho(){
        FileReader lector = new FileReader("Piso");
        Techo = lector.getObject();
        Vector3 vmin = lector.coordenadasMinimas();

        Vector3 newPosition = new Vector3(-vmin.x,3,-vmin.z);
        Vector3 newRotation = new Vector3(180*Mathf.Deg2Rad,0,0);
        Vector3 newScale = new Vector3(1,1,1);
        Matrix4x4 modelMatrix = CreateModelMatrix(newPosition, newRotation, newScale);
        Techo.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix", modelMatrix);
        Techo.GetComponent<Renderer>().material.SetMatrix("_ProjectionMatrix", GL.GetGPUProjectionMatrix(projMatrix, true));
        objetos.Add(Techo);
    }
   

    private void cargarParedes()
    {
        FileReader lector = new FileReader("Paredes");
        lector.setColor(0.8f,0.8f,0.8f);
        Paredes = lector.getObject();

        Vector3 vmin = lector.coordenadasMinimas();
        Vector3 newPosition = new Vector3(-vmin.z,-vmin.y,-vmin.x);
        Vector3 newRotation = new Vector3(0,-90 * Mathf.Deg2Rad,0);
        Vector3 newScale = new Vector3(1,1,1);
        Matrix4x4 modelMatrix = CreateModelMatrix(newPosition, newRotation, newScale);
        Paredes.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix", modelMatrix);
        Paredes.GetComponent<Renderer>().material.SetMatrix("_ProjectionMatrix", GL.GetGPUProjectionMatrix(projMatrix, true));
        objetos.Add(Paredes);
    }

    private void cargarBed()
    {
        FileReader lector = new FileReader("Bed");
        lector.setColor(0.2f,0.2f,0.2f);
        GameObject Bed = lector.getObject();

        Vector3 vmin = lector.coordenadasMinimas();
        Vector3 newPosition = new Vector3(6.998f + vmin.x,-vmin.y - 0.01f,1);
        Vector3 newRotation = new Vector3(0,0,0);
        Vector3 newScale = new Vector3(1,1,0.75f);
        Matrix4x4 modelMatrix = CreateModelMatrix(newPosition, newRotation, newScale);
        Bed.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix", modelMatrix);
        Bed.GetComponent<Renderer>().material.SetMatrix("_ProjectionMatrix", GL.GetGPUProjectionMatrix(projMatrix, true));
        objetos.Add(Bed);
    }

    private void cargarToilet()
    {
        FileReader lector = new FileReader("Toilet");
        lector.setColor(0.9f,0.9f,0.9f);
        GameObject Toilet = lector.getObject();

        Vector3 vmin = lector.coordenadasMinimas();
        Vector3 newPosition = new Vector3(6.998f + vmin.x,-vmin.y - 0.01f,6.5f);
        Vector3 newRotation = new Vector3(0,180*Mathf.Deg2Rad,0);
        Vector3 newScale = new Vector3(1,1,1);
        Matrix4x4 modelMatrix = CreateModelMatrix(newPosition, newRotation, newScale);
        Toilet.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix", modelMatrix);
        Toilet.GetComponent<Renderer>().material.SetMatrix("_ProjectionMatrix", GL.GetGPUProjectionMatrix(projMatrix, true));
        objetos.Add(Toilet);
    }
}