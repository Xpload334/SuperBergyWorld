using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawTriangleMesh : MonoBehaviour
{
    Mesh m;
    MeshFilter mf;

    private Vector3[] VerticesArray;
    // Use this for initialization
    void Start()
    {
        mf = GetComponent<MeshFilter>();
        m = new Mesh();
        mf.mesh = m;
        drawTriangle();
    } 
    //This draws a triangle
    void drawTriangle()
    { 
        //We need two arrays one to hold the vertices and one to hold the triangles
        VerticesArray = new Vector3[3];
        int[] trianglesArray = new int[3]; 
        
        //add 3 vertices in the 3d space
        VerticesArray[0] = new Vector3(0, 1, 0);
        VerticesArray[1] = new Vector3(-1, 0, 0);
        VerticesArray[2] = new Vector3(1, 0, 0); 
        
        //define the order in which the vertices in the VerteicesArray should be used to draw the triangle
        trianglesArray[0] = 0;
        trianglesArray[1] = 1;
        trianglesArray[2] = 2; 
        
        //add these two triangles to the mesh
        m.vertices = VerticesArray;
        m.triangles = trianglesArray;
    }
}
