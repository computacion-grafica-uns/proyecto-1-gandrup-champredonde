using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    private float ppRotX = 0f, ppRotY = 0f, velPp = 5f;    

    private List<GameObject> objetos;
    
    private GameObject Paredes, Techo;

    private GameObject CamaraOrb, CamaraPp;

    private bool modoPp = false;

    private float velRotMouseOrb, velRotTeclasOrb;

    private Vector3 camPosOrb,camTargetOrb,camUpOrb;

    private Vector3 camPosPp, camForwardPp, camUpPp;
    
    private Matrix4x4 projMatrix;

    // Start is called before the first frame update
    void Start()
    {
        float fov = Mathf.Deg2Rad *90;
        float aspectRatio = 16f / 9f;
        float near = 0.1f, far = 500f;
        projMatrix = CalculatePerspectiveProjectionMatrix(fov, aspectRatio, near, far);

        CargadorDeObjetos cargador = new CargadorDeObjetos(projMatrix);

        objetos = cargador.getObjetos();
        
        Paredes = cargador.getParedes();

        Techo = cargador.getTecho();
        
        CreateCamera();
        camPosOrb = new Vector3(3.5f,1.5f,-2);
        camTargetOrb = new Vector3(3.5f,1.5f,4.5f);
        camUpOrb = new Vector3(0,1,0);
        camPosPp = new Vector3(3.5f,1.7f,-3);
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

        // Desaparecer Paredes
        if (Input.GetKeyDown(KeyCode.P) && objetos.Contains(Paredes))
            Paredes.SetActive(!Paredes.activeSelf);

        // Desaparecer Techo
        if (Input.GetKeyDown(KeyCode.T) && objetos.Contains(Techo))
            Techo.SetActive(!Techo.activeSelf);

        if (modoPp)
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
        camForwardPp = (rotY * rotX * Vector3.forward).normalized;
        camUpPp = Vector3.up;
    }

    private void CamaraOrbital()
    {
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
        Quaternion rotY = Quaternion.AngleAxis(rotHorizontal, camUpOrb);
        camPosOrb = rotY * (camPosOrb - camTargetOrb) + camTargetOrb;

        // Rotaci�n en el eje X
        Vector3 right = Vector3.Cross(camUpOrb, camPosOrb - camTargetOrb).normalized;
        Quaternion rotX = Quaternion.AngleAxis(rotVertical, right);
        camPosOrb = rotX * (camPosOrb - camTargetOrb) + camTargetOrb;
        camUpOrb = Vector3.Cross(camPosOrb - camTargetOrb, right).normalized;
    }

    private void RecalcularMatrices()
    {

        Vector3 viewPos = modoPp ? camPosPp : camPosOrb;
        Vector3 viewTarget = modoPp ? (camPosPp + camForwardPp) : camTargetOrb;
        Vector3 viewUp = modoPp ? camUpPp : camUpOrb;
        Matrix4x4 viewMatrix = CreateViewMatrix(viewPos, viewTarget, viewUp);

        foreach(GameObject objeto in objetos)
            objeto.GetComponent<Renderer>().material.SetMatrix("_ViewMatrix", viewMatrix);
            
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

    private void CreateCamera() 
    {
        CamaraOrb = new GameObject();
        CamaraOrb.AddComponent<Camera>();

        CamaraOrb.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;

        CamaraOrb.GetComponent<Camera>().backgroundColor = Color.black;

    }
    
}