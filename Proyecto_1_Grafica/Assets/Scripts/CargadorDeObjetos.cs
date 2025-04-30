using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargadorDeObjetos
{
    private Matrix4x4 projMatrix;

    private List<GameObject> objetos;
    
    private GameObject Paredes, Techo;

    private GameObject ruedasPcChair, basePcChair, respaldoPcChair;
    
    public CargadorDeObjetos(Matrix4x4 projMatrix)
    {
        this.projMatrix = projMatrix;

        objetos = new List<GameObject>();
        cargaDeObjetos();
        
        objetoJerarquico();
    }

    private void cargaDeObjetos()
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

        objetoJerarquico();
    }

    private void objetoJerarquico()
    {
        ruedasPcChair.transform.SetParent(basePcChair.transform);
        basePcChair.transform.SetParent(respaldoPcChair.transform);
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
        Vector3 newPosition = new Vector3(5.5f,-vmin.y + 0.01f,5.5f);
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
        GameObject pcTable = lector.getObject();

        Vector3 vmin = lector.coordenadasMinimas();
        Vector3 newPosition = new Vector3(5,-vmin.y + 0.001f,0.001f - (vmin.x * 0.5f));
        Vector3 newRotation = new Vector3(0,270 * Mathf.Deg2Rad,0);
        Vector3 newScale = new Vector3(0.5f,1,0.5f);
        Matrix4x4 modelMatrix = CreateModelMatrix(newPosition, newRotation, newScale);
        pcTable.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix", modelMatrix);
        pcTable.GetComponent<Renderer>().material.SetMatrix("_ProjectionMatrix", GL.GetGPUProjectionMatrix(projMatrix, true));
        objetos.Add(pcTable);
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

    private void cargarPcChair()
    {
        cargarRuedasPcChair();
        cargarBasePcChair();
        cargarRespaldoPcChair();
    }

    private void cargarRuedasPcChair()
    {
        FileReader lector = new FileReader("ruedasPcChair");
        lector.setColor(0.1f,0.1f,0.1f);
        ruedasPcChair = lector.getObject();

        Vector3 vmin = lector.coordenadasMinimas();
        Vector3 newPosition = new Vector3(5.2f, 0.001f - (vmin.y * 0.1f), 1);
        Vector3 newRotation = new Vector3(0,270 * Mathf.Deg2Rad,0);
        Vector3 newScale = new Vector3(0.1f,0.1f,0.1f);
        Matrix4x4 modelMatrix = CreateModelMatrix(newPosition, newRotation, newScale);
        ruedasPcChair.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix", modelMatrix);
        ruedasPcChair.GetComponent<Renderer>().material.SetMatrix("_ProjectionMatrix", GL.GetGPUProjectionMatrix(projMatrix, true));
        objetos.Add(ruedasPcChair);
    }

    private void cargarBasePcChair()
    {
        FileReader lector = new FileReader("basePcChair");
        lector.setColor(0.1f,0.1f,0.1f);
        basePcChair = lector.getObject();

        Vector3 vmin = lector.coordenadasMinimas();
        Vector3 newPosition = new Vector3(5.2f, 0.001f + 0.03f - (vmin.y * 0.1f), 1);
        Vector3 newRotation = new Vector3(0,270 * Mathf.Deg2Rad,0);
        Vector3 newScale = new Vector3(0.1f,0.1f,0.1f);
        Matrix4x4 modelMatrix = CreateModelMatrix(newPosition, newRotation, newScale);
        basePcChair.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix", modelMatrix);
        basePcChair.GetComponent<Renderer>().material.SetMatrix("_ProjectionMatrix", GL.GetGPUProjectionMatrix(projMatrix, true));
        objetos.Add(basePcChair);
    }

    private void cargarRespaldoPcChair()
    {
        FileReader lector = new FileReader("respaldoPcChair");
        lector.setColor(0.1f,0.1f,0.1f);
        respaldoPcChair = lector.getObject();

        Vector3 vmin = lector.coordenadasMinimas();
        Vector3 newPosition = new Vector3(5.2f, 0.001f + 0.03f + 0.53f - (vmin.y * 0.1f), 1);
        Vector3 newRotation = new Vector3(0,270 * Mathf.Deg2Rad,0);
        Vector3 newScale = new Vector3(0.1f,0.1f,0.1f);
        Matrix4x4 modelMatrix = CreateModelMatrix(newPosition, newRotation, newScale);
        respaldoPcChair.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix", modelMatrix);
        respaldoPcChair.GetComponent<Renderer>().material.SetMatrix("_ProjectionMatrix", GL.GetGPUProjectionMatrix(projMatrix, true));
        objetos.Add(respaldoPcChair);
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
            new Vector4(1f, 0f, 0f, 0f),
            new Vector4(0f, Mathf.Cos(newRotation.x), -Mathf. Sin (newRotation.x), 0f), 
            new Vector4(0f, Mathf. Sin (newRotation.x), Mathf.Cos(newRotation.x), 0f), 
            new Vector4(0f, 0f, 0f, 1f)
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
            new Vector4(0f, 0f, 1f, 0f), 
            new Vector4(0f, 0f, 0f, 1f) 
        );
    
        Matrix4x4 rotationMatrix = rotationMatrixZ * rotationMatrixY * rotationMatrixX;
        rotationMatrix = rotationMatrix.transpose;
    
        Matrix4x4 scaleMatrix = new Matrix4x4( 
            new Vector4(newScale.x, 0f, 0f, 0f),
            new Vector4(0f, newScale.y, 0f, 0f), 
            new Vector4(0f, 0f, newScale.z, 0f),
            new Vector4(0f, 0f, 0f, 1f)
        );
        scaleMatrix = scaleMatrix.transpose;
    
        Matrix4x4 finalMatrix = positionMatrix;
    
        finalMatrix *= rotationMatrix;
        finalMatrix *= scaleMatrix;
        return (finalMatrix);
    }

    public List<GameObject> getObjetos()
    {
        return objetos;
    }
    
    public GameObject getParedes()
    {
        return Paredes;
    }

    public GameObject getTecho()
    {
        return Techo;
    }
}
