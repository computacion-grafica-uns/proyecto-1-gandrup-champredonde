using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Globalization;

public class FileReader
{   
    private float xMax, yMax, zMax, xMin, yMin, zMin;
    private GameObject obj;
    private Vector3[] vertices;
    private int[] triangles;
    private Color[] colores;
    
    public FileReader(string fileName)
    {
        ReadObj(fileName);
    }

    private void ReadObj(string fileName)
    {
           string path = "Assets/Modelos3d/"+ fileName + ".obj";

           StreamReader reader = new StreamReader(path);
           string fileData = (reader.ReadToEnd());
    
           reader.Close();

           ReadEachLine(fileData);
           CentrarObjeto();

           obj = new GameObject();

           obj.AddComponent<MeshFilter>();

           obj.GetComponent<MeshFilter>().mesh = new Mesh();

           obj.AddComponent<MeshRenderer>();

           UpdateMesh();
           CreateMaterial();
    }

    private void ReadEachLine(string fileData)
    {   
        int indiceVertices = 0;
        int indiceTriangles = 0;
        string[] lines = fileData.Split('\n');

        int longVertices = fileData.Split(new string[] { "v " }, StringSplitOptions.None).Length - 1;
        vertices = new Vector3[longVertices];

        colores = new Color[longVertices];

        int longTriangles = (fileData.Split(new string[] { "f " }, StringSplitOptions.None).Length - 1) * 3;
        triangles = new int[longTriangles];

        bool primerVertice = true;

        for(int i = 0; i < lines.Length; i++)
        {
            if(lines[i].StartsWith("v "))
            {
                string[] palabras = lines[i].Split(new char[]{' '},StringSplitOptions.RemoveEmptyEntries);
                float x = float.Parse(palabras[1],CultureInfo.InvariantCulture);
                float y = float.Parse(palabras[2],CultureInfo.InvariantCulture);
                float z = float.Parse(palabras[3],CultureInfo.InvariantCulture);
                vertices[indiceVertices] = new Vector3(x,y,z);

                colores[indiceVertices] = new Color(1,0,0);
                indiceVertices++;

                if(primerVertice)
                {
                    xMin = xMax = x;
                    yMin = yMax = y;
                    zMin = zMax = z;
                    primerVertice = false;
                }else
                {
                    if (x < xMin) xMin = x; 
                    if (x > xMax) xMax = x;
                    if (y < yMin) yMin = y; 
                    if (y > yMax) yMax = y;
                    if (z < zMin) zMin = z;
                    if (z > zMax) zMax = z;
                }
               
            }else if(lines[i].StartsWith("f "))
            {
                string[] triangulos = lines[i].Split(new char[]{' '},StringSplitOptions.RemoveEmptyEntries);
                for(int j=1 ; j<triangulos.Length ; j++)
                {
                    string[] verticesCara = triangulos[j].Split('/');
                    triangles[indiceTriangles] = int.Parse(verticesCara[0]) - 1;
                    if(j == 4) // si habia 4 vertices para formar la cara
                    {
                        triangles[indiceTriangles + 1] = triangles[indiceTriangles - 1] ;
                        triangles[indiceTriangles + 2] = triangles[indiceTriangles - 2] ;
                        indiceTriangles += 2;
                    }
                    indiceTriangles++;
                }
            }
        }
    }
    
    private void CentrarObjeto()
    {
        float xProm = (xMax+xMin)/2;
        float yProm = (yMax+yMin)/2;
        float zProm = (zMax+zMin)/2;
        for(int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(vertices[i].x - xProm , vertices[i].y - yProm , vertices[i].z - zProm);
        }
    }

    private void UpdateMesh()
    {
        obj.GetComponent<MeshFilter>().mesh.vertices = vertices;

        obj.GetComponent<MeshFilter>().mesh.triangles = triangles;

        obj.GetComponent<MeshFilter>().mesh.colors = colores;
    }

    private void CreateMaterial() 
    {
        Material newMaterial = new Material(Shader.Find("ShaderBasico"));

        obj.GetComponent<MeshRenderer>().material = newMaterial;
    }

    public GameObject getObject()
    {
        return obj;
    }

    public GameObject getNewObject(string fileName)
    {
        ReadObj(fileName);
        return obj;
    }

    public Vector3 coordenadasMinimas()
    {
        float xProm = (xMax+xMin)/2;
        float yProm = (yMax+yMin)/2;
        float zProm = (zMax+zMin)/2;
        
        Vector3 minimos = new Vector3(xMin-xProm, yMin-yProm, zMin-zProm); 
        return minimos;
    }
}
