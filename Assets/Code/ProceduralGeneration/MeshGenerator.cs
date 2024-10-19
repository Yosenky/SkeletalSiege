using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))] // makes sure that we have to have a mesh filter to run
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;

    public int xSize = 50;
    public int zSize = 50;
    public int numOctaves = 4;
    public float lacunarity = 0.5f;
    public float persistence = 1.4f;
    public float scale = 1f;
    void Start()
    {
        mesh = new Mesh(); 
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        
    }

    private void Update()
    {
        CreateShape();
        UpdateMesh();
    }

    
    float CalculateY(float x, float z, int numOctaves, float persistence, float lacunarity, float scale)
    {
        float y = 0;
        float amplitude = 1;
        float frequency = 1;
        for(int i = 0; i < numOctaves; i++)
        {
            float perlinValue = Mathf.PerlinNoise(x * frequency / scale, z * frequency / scale);
            y += perlinValue * amplitude;

            amplitude *= persistence;
            frequency *= lacunarity;
        }
        
        return y - numOctaves; // this makes it centered at y = 0
    }
    

    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for(int i = 0, z = 0; z < zSize+1; z++)
        {
            for(int x = 0; x < xSize+1; x++)
            {
                float y = CalculateY(x,z, numOctaves, persistence, lacunarity, scale);
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        int vert = 0; // which starting vertex are we at
        int tris = 0; // number of points we make for triangles
        triangles = new int[xSize * zSize * 6];
        for(int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++; // move to next starting vertex
                tris += 6; // add 6 because we have 6 vertices

            }
            // need to increment vert by 1 so that we dont make a triangle from the end of one row to the start of another
            vert++;
        }
        
        

    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals(); // this fixes weird lighting
    }
    /**
    private void OnDrawGizmos()
    {
        // make sure that we dont try to draw vertices if we have none
        if(vertices == null)
        {
            return;
        }
        for(int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], .1f);
        }
    }
    */
}
