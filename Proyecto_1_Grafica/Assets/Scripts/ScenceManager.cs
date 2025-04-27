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
        float near = 0.1f, far = 500f;
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
        Quaternion rotY = Quaternion.AngleAxis(rotHorizontal, camUp);
        camPos = rotY * (camPos - camTarget) + camTarget;

        // Rotaci�n en el eje X
        Vector3 right = Vector3.Cross(camUp, camPos - camTarget).normalized;
        Quaternion rotX = Quaternion.AngleAxis(rotVertical, right);
        camPos = rotX * (camPos - camTarget) + camTarget;
        camUp = Vector3.Cross(camPos - camTarget, right).normalized;
    }

    private void RecalcularMatrices()
    {

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

    private void CreateCamera() 
    {
        CamaraOrb = new GameObject();
        CamaraOrb.AddComponent<Camera>();

        CamaraOrb.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;

        CamaraOrb.GetComponent<Camera>().backgroundColor = Color.black;

    }
    
    private void cargarObjetos()
    {
        //cargo estructura
        cargarPiso();
        cargarTecho();
        cargarParedes();
        
        //cargo muebles del baño
        cargarToilet();
        cargarShower();
        cargarSink();
        cargarMirror();

        //cargo muebles cocina
        cargar90Degrees();
        cargarKitchenCabinetRounded();
        cargarKitchenStoveWhithOven();
        cargarFridge();

        //cargo muebles sala de estar
        cargar90DegreesSofa();
        cargarSofaWhithLegs();

        //cargo muebles comedor
        cargarTable();
        cargarChairMesaCocina();
        cargarChairMesaSofa();
        cargarChairMesaVentana();
        cargarChairMesaBanio();

        //cargo muebles habitacion
        cargarBed();
        cargarLittleOne();
        cargarCloset();
        cargarPcTable();
        cargarPcChair();
        cargarPc();
    }

    private void cargarPiso()
    {
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

    private void cargarTecho()
    {
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

    private void cargarToilet()
    {
        FileReader lector = new FileReader("Toilet");
        lector.setColor(0.9f,0.9f,0.9f);
        GameObject Toilet = lector.getObject();

        Vector3 vmin = lector.coordenadasMinimas();
        Vector3 newPosition = new Vector3(6.998f + vmin.x,-vmin.y - 0.01f,7);
        Vector3 newRotation = new Vector3(0,180*Mathf.Deg2Rad,0);
        Vector3 newScale = new Vector3(1,1,1);
        Matrix4x4 modelMatrix = CreateModelMatrix(newPosition, newRotation, newScale);
        Toilet.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix", modelMatrix);
        Toilet.GetComponent<Renderer>().material.SetMatrix("_ProjectionMatrix", GL.GetGPUProjectionMatrix(projMatrix, true));
        objetos.Add(Toilet);
    }

    private void cargarShower()
    {
        FileReader lector = new FileReader("Shower");
        lector.setColor(0.3f,0.3f,0.4f);
        GameObject Shower = lector.getObject();

        Vector3 vmin = lector.coordenadasMinimas();
        Vector3 newPosition = new Vector3(5.5f,-vmin.y - 0.01f,5.5f);
        Vector3 newRotation = new Vector3(0,270*Mathf.Deg2Rad,0);
        Vector3 newScale = new Vector3(1,1,1);
        Matrix4x4 modelMatrix = CreateModelMatrix(newPosition, newRotation, newScale);
        Shower.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix", modelMatrix);
        Shower.GetComponent<Renderer>().material.SetMatrix("_ProjectionMatrix", GL.GetGPUProjectionMatrix(projMatrix, true));
        objetos.Add(Shower);
    }

    private void cargarSink()
    {
        FileReader lector = new FileReader("Sink");
        lector.setColor(0.3f,0.3f,0.4f);
        GameObject Sink = lector.getObject();

        Vector3 vmin = lector.coordenadasMinimas();
        Vector3 newPosition = new Vector3(5.5f,-vmin.y + 0.01f,8.999f + vmin.x);
        Vector3 newRotation = new Vector3(0,90*Mathf.Deg2Rad,0);
        Vector3 newScale = new Vector3(1,1,1);
        Matrix4x4 modelMatrix = CreateModelMatrix(newPosition, newRotation, newScale);
        Sink.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix", modelMatrix);
        Sink.GetComponent<Renderer>().material.SetMatrix("_ProjectionMatrix", GL.GetGPUProjectionMatrix(projMatrix, true));
        objetos.Add(Sink);
    }

    private void cargarMirror()
    {
        FileReader lector = new FileReader("Mirror");
        lector.setColor(0,1,1);
        GameObject Mirror = lector.getObject();

        Vector3 vmin = lector.coordenadasMinimas();
        Vector3 newPosition = new Vector3(5.5f,-vmin.y + 1.2f ,8.999f + vmin.x);
        Vector3 newRotation = new Vector3(0,90*Mathf.Deg2Rad,0);
        Vector3 newScale = new Vector3(1,1,1);
        Matrix4x4 modelMatrix = CreateModelMatrix(newPosition, newRotation, newScale);
        Mirror.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix", modelMatrix);
        Mirror.GetComponent<Renderer>().material.SetMatrix("_ProjectionMatrix", GL.GetGPUProjectionMatrix(projMatrix, true));
        objetos.Add(Mirror);
    }

    private void cargar90Degrees()
    {
        FileReader lector = new FileReader("90DegreesUpperCabinet");
        lector.setColor(0.5f, 0.2f, 0.2f);
        GameObject DegreesUpperCabinet = lector.getObject();

        Vector3 vmin = lector.coordenadasMinimas();
        Vector3 newPosition = new Vector3(0.001f-(vmin.x*0.75f), 1.9f - vmin.y, 8.999f + (vmin.z*0.75f));
        Vector3 newRotation = new Vector3(0,0,0);
        Vector3 newScale = new Vector3(0.75f,1,0.75f);
        Matrix4x4 modelMatrix = CreateModelMatrix(newPosition, newRotation, newScale);
        DegreesUpperCabinet.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix", modelMatrix);
        DegreesUpperCabinet.GetComponent<Renderer>().material.SetMatrix("_ProjectionMatrix", GL.GetGPUProjectionMatrix(projMatrix, true));
        objetos.Add(DegreesUpperCabinet);
    }

    private void cargarKitchenCabinetRounded()
    {
        FileReader lector = new FileReader("KitchenCabinetRounded");
        lector.setColor(0.5f, 0.2f, 0.2f);
        GameObject KitchenCabinetRounded = lector.getObject();

        Vector3 vmin = lector.coordenadasMinimas();
        Vector3 newPosition = new Vector3(0.001f - (vmin.x*0.75f), 0.001f - vmin.y, 8.999f + (vmin.z*0.75f));
        Vector3 newRotation = new Vector3(0,0,0);
        Vector3 newScale = new Vector3(0.75f,1,0.75f);
        Matrix4x4 modelMatrix = CreateModelMatrix(newPosition, newRotation, newScale);
        KitchenCabinetRounded.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix", modelMatrix);
        KitchenCabinetRounded.GetComponent<Renderer>().material.SetMatrix("_ProjectionMatrix", GL.GetGPUProjectionMatrix(projMatrix, true));
        objetos.Add(KitchenCabinetRounded);
    }

    private void cargarKitchenStoveWhithOven()
    {
        FileReader lector = new FileReader("KitchenStoveWhithOven");
        lector.setColor(0.6f, 0.6f, 0.6f);
        GameObject KitchenStoveWhithOven = lector.getObject();

        Vector3 vmin = lector.coordenadasMinimas();
        Vector3 newPosition = new Vector3(2.25f, 0.001f - vmin.y, 8.999f + (vmin.x*0.7f));
        Vector3 newRotation = new Vector3(0, 90*Mathf.Deg2Rad , 0);
        Vector3 newScale = new Vector3(0.7f,1,1);
        Matrix4x4 modelMatrix = CreateModelMatrix(newPosition, newRotation, newScale);
        KitchenStoveWhithOven.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix", modelMatrix);
        KitchenStoveWhithOven.GetComponent<Renderer>().material.SetMatrix("_ProjectionMatrix", GL.GetGPUProjectionMatrix(projMatrix, true));
        objetos.Add(KitchenStoveWhithOven);
    }

    private void cargarFridge()
    {
        FileReader lector = new FileReader("Fridge");
        lector.setColor(0.4f, 0.4f, 0.4f);
        GameObject Fridge = lector.getObject();

        Vector3 vmin = lector.coordenadasMinimas();
        Vector3 newPosition = new Vector3(0.001f - vmin.x, 0.001f - vmin.y, 6.9f);
        Vector3 newRotation = new Vector3(0, 0 , 0);
        Vector3 newScale = new Vector3(1,1,1);
        Matrix4x4 modelMatrix = CreateModelMatrix(newPosition, newRotation, newScale);
        Fridge.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix", modelMatrix);
        Fridge.GetComponent<Renderer>().material.SetMatrix("_ProjectionMatrix", GL.GetGPUProjectionMatrix(projMatrix, true));
        objetos.Add(Fridge);
    }

    private void cargar90DegreesSofa()
    {
        FileReader lector = new FileReader("90DegreesSofa");
        lector.setColor(0, 0.4f, 0.4f);
        GameObject DegreesSofa = lector.getObject();

        Vector3 vmin = lector.coordenadasMinimas();
        Vector3 newPosition = new Vector3(0.001f - (vmin.x*0.7f), 0.001f - vmin.y, 0.001f - (vmin.z*0.6f));
        Vector3 newRotation = new Vector3(0, 0 , 0);
        Vector3 newScale = new Vector3(0.7f,1,0.6f);
        Matrix4x4 modelMatrix = CreateModelMatrix(newPosition, newRotation, newScale);
        DegreesSofa.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix", modelMatrix);
        DegreesSofa.GetComponent<Renderer>().material.SetMatrix("_ProjectionMatrix", GL.GetGPUProjectionMatrix(projMatrix, true));
        objetos.Add(DegreesSofa);
    }

    private void cargarSofaWhithLegs()
    {
        FileReader lector = new FileReader("sofaWhithLegs");
        lector.setColor(0, 0.4f, 0.4f);
        GameObject sofaWhithLegs = lector.getObject();

        Vector3 vmin = lector.coordenadasMinimas();
        Vector3 newPosition = new Vector3(0.5f - (vmin.z*0.6f), 0.001f - vmin.y, 1.5f - (vmin.z*0.6f));
        Vector3 newRotation = new Vector3(0, 90 * Mathf.Deg2Rad , 0);
        Vector3 newScale = new Vector3(0.6f,1,0.6f);
        Matrix4x4 modelMatrix = CreateModelMatrix(newPosition, newRotation, newScale);
        sofaWhithLegs.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix", modelMatrix);
        sofaWhithLegs.GetComponent<Renderer>().material.SetMatrix("_ProjectionMatrix", GL.GetGPUProjectionMatrix(projMatrix, true));
        objetos.Add(sofaWhithLegs);
    }

    private void cargarTable()
    {
        FileReader lector = new FileReader("Table");
        lector.setColor(0.7f, 0.4f, 0.4f);
        GameObject Table = lector.getObject();

        Vector3 vmin = lector.coordenadasMinimas();
        Vector3 newPosition = new Vector3(1.5f , 0.001f - (vmin.y*0.8f), 5);
        Vector3 newRotation = new Vector3(0, 0 , 0);
        Vector3 newScale = new Vector3(0.7f,0.8f,0.7f);
        Matrix4x4 modelMatrix = CreateModelMatrix(newPosition, newRotation, newScale);
        Table.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix", modelMatrix);
        Table.GetComponent<Renderer>().material.SetMatrix("_ProjectionMatrix", GL.GetGPUProjectionMatrix(projMatrix, true));
        objetos.Add(Table);
    }

    private void cargarChairMesaCocina()
    {
        FileReader lector = new FileReader("chair3");
        lector.setColor(0.7f, 0.3f, 0.3f);
        GameObject Chair = lector.getObject();

        Vector3 vmin = lector.coordenadasMinimas();
        Vector3 newPosition = new Vector3(1.5f, 0.001f - (vmin.y*1), 6);
        Vector3 newRotation = new Vector3(0, 90*Mathf.Deg2Rad , 0);
        Vector3 newScale = new Vector3(0.7f,1,0.7f);
        Matrix4x4 modelMatrix = CreateModelMatrix(newPosition, newRotation, newScale);
        Chair.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix", modelMatrix);
        Chair.GetComponent<Renderer>().material.SetMatrix("_ProjectionMatrix", GL.GetGPUProjectionMatrix(projMatrix, true));
        objetos.Add(Chair);
    }

    private void cargarChairMesaSofa()
    {
        FileReader lector = new FileReader("chair3");
        lector.setColor(0.7f, 0.3f, 0.3f);
        GameObject Chair = lector.getObject();

        Vector3 vmin = lector.coordenadasMinimas();
        Vector3 newPosition = new Vector3(1.5f, 0.001f - (vmin.y*1), 4);
        Vector3 newRotation = new Vector3(0, 270*Mathf.Deg2Rad , 0);
        Vector3 newScale = new Vector3(0.7f,1,0.7f);
        Matrix4x4 modelMatrix = CreateModelMatrix(newPosition, newRotation, newScale);
        Chair.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix", modelMatrix);
        Chair.GetComponent<Renderer>().material.SetMatrix("_ProjectionMatrix", GL.GetGPUProjectionMatrix(projMatrix, true));
        objetos.Add(Chair);
    }

    private void cargarChairMesaVentana()
    {
        FileReader lector = new FileReader("chair3");
        lector.setColor(0.7f, 0.3f, 0.3f);
        GameObject Chair = lector.getObject();

        Vector3 vmin = lector.coordenadasMinimas();
        Vector3 newPosition = new Vector3(0.5f, 0.001f - (vmin.y*1), 5);
        Vector3 newRotation = new Vector3(0,0,0);
        Vector3 newScale = new Vector3(0.7f,1,0.7f);
        Matrix4x4 modelMatrix = CreateModelMatrix(newPosition, newRotation, newScale);
        Chair.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix", modelMatrix);
        Chair.GetComponent<Renderer>().material.SetMatrix("_ProjectionMatrix", GL.GetGPUProjectionMatrix(projMatrix, true));
        objetos.Add(Chair);
    }

    private void cargarChairMesaBanio()
    {
        FileReader lector = new FileReader("chair3");
        lector.setColor(0.7f, 0.3f, 0.3f);
        GameObject Chair = lector.getObject();

        Vector3 vmin = lector.coordenadasMinimas();
        Vector3 newPosition = new Vector3(2.5f, 0.001f - (vmin.y*1), 5);
        Vector3 newRotation = new Vector3(0,180*Mathf.Deg2Rad,0);
        Vector3 newScale = new Vector3(0.7f,1,0.7f);
        Matrix4x4 modelMatrix = CreateModelMatrix(newPosition, newRotation, newScale);
        Chair.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix", modelMatrix);
        Chair.GetComponent<Renderer>().material.SetMatrix("_ProjectionMatrix", GL.GetGPUProjectionMatrix(projMatrix, true));
        objetos.Add(Chair);
    }

    private void cargarBed()
    {
        FileReader lector = new FileReader("Bed");
        lector.setColor(0.2f,0.2f,0.2f);
        GameObject Bed = lector.getObject();

        Vector3 vmin = lector.coordenadasMinimas();
        Vector3 newPosition = new Vector3(6.998f + vmin.x,-vmin.y + 0.001f,4.999f + (vmin.z * 0.75f));
        Vector3 newRotation = new Vector3(0,0,0);
        Vector3 newScale = new Vector3(1,1,0.75f);
        Matrix4x4 modelMatrix = CreateModelMatrix(newPosition, newRotation, newScale);
        Bed.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix", modelMatrix);
        Bed.GetComponent<Renderer>().material.SetMatrix("_ProjectionMatrix", GL.GetGPUProjectionMatrix(projMatrix, true));
        objetos.Add(Bed);
    }

    private void cargarLittleOne()
    {
        FileReader lector = new FileReader("littleOne");
        lector.setColor(0.3f,0.3f,0.2f);
        GameObject littleOne = lector.getObject();

        Vector3 vmin = lector.coordenadasMinimas();
        Vector3 newPosition = new Vector3(5,-vmin.y + 0.001f,4.999f + vmin.z);
        Vector3 newRotation = new Vector3(0,90 * Mathf.Deg2Rad,0);
        Vector3 newScale = new Vector3(1,1,1);
        Matrix4x4 modelMatrix = CreateModelMatrix(newPosition, newRotation, newScale);
        littleOne.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix", modelMatrix);
        littleOne.GetComponent<Renderer>().material.SetMatrix("_ProjectionMatrix", GL.GetGPUProjectionMatrix(projMatrix, true));
        objetos.Add(littleOne);
    }

    private void cargarCloset()
    {
        FileReader lector = new FileReader("closet");
        lector.setColor(0.15f,0.3f,0.2f);
        GameObject Closet = lector.getObject();

        Vector3 vmin = lector.coordenadasMinimas();
        Vector3 newPosition = new Vector3(6.999f + vmin.x,-vmin.y + 0.001f, 0.15f - vmin.z);
        Vector3 newRotation = new Vector3(0,0,0);
        Vector3 newScale = new Vector3(1,1,1);
        Matrix4x4 modelMatrix = CreateModelMatrix(newPosition, newRotation, newScale);
        Closet.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix", modelMatrix);
        Closet.GetComponent<Renderer>().material.SetMatrix("_ProjectionMatrix", GL.GetGPUProjectionMatrix(projMatrix, true));
        objetos.Add(Closet);
    }

    private void cargarPcTable()
    {
        FileReader lector = new FileReader("pcTable1");
        lector.setColor(0.3f,0.3f,0.2f);
        GameObject pcTable1 = lector.getObject();

        Vector3 vmin = lector.coordenadasMinimas();
        Vector3 newPosition = new Vector3(5,-vmin.y + 0.001f,0.001f - (vmin.x * 0.5f));
        Vector3 newRotation = new Vector3(0,270 * Mathf.Deg2Rad,0);
        Vector3 newScale = new Vector3(0.5f,1,0.5f);
        Matrix4x4 modelMatrix = CreateModelMatrix(newPosition, newRotation, newScale);
        pcTable1.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix", modelMatrix);
        pcTable1.GetComponent<Renderer>().material.SetMatrix("_ProjectionMatrix", GL.GetGPUProjectionMatrix(projMatrix, true));
        objetos.Add(pcTable1);
    }

    private void cargarPcChair()
    {
        FileReader lector = new FileReader("pcChair");
        lector.setColor(0.1f,0.1f,0.1f);
        GameObject pcChair = lector.getObject();

        Vector3 vmin = lector.coordenadasMinimas();
        Vector3 newPosition = new Vector3(5.2f, 0.001f - (vmin.y * 0.1f), 1);
        Vector3 newRotation = new Vector3(0,270 * Mathf.Deg2Rad,0);
        Vector3 newScale = new Vector3(0.1f,0.1f,0.1f);
        Matrix4x4 modelMatrix = CreateModelMatrix(newPosition, newRotation, newScale);
        pcChair.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix", modelMatrix);
        pcChair.GetComponent<Renderer>().material.SetMatrix("_ProjectionMatrix", GL.GetGPUProjectionMatrix(projMatrix, true));
        objetos.Add(pcChair);
    }

    private void cargarPc()
    {
        FileReader lector = new FileReader("PC");
        lector.setColor(0.1f,0.1f,0.1f);
        GameObject PC = lector.getObject();

        Vector3 vmin = lector.coordenadasMinimas();
        Vector3 newPosition = new Vector3(5.2f, 1.05f - (vmin.y * 0.1f), 0.01f -(vmin.z * 0.08f));
        Vector3 newRotation = new Vector3(0,0,0);
        Vector3 newScale = new Vector3(0.08f,0.1f,0.08f);
        Matrix4x4 modelMatrix = CreateModelMatrix(newPosition, newRotation, newScale);
        PC.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix", modelMatrix);
        PC.GetComponent<Renderer>().material.SetMatrix("_ProjectionMatrix", GL.GetGPUProjectionMatrix(projMatrix, true));
        objetos.Add(PC);
    }
}