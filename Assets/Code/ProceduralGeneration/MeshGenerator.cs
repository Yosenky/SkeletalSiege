using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MeshFilter))] // makes sure that we have to have a mesh filter to run
[RequireComponent(typeof(NavMeshSurface))]
public class MeshGenerator : MonoBehaviour
{
    // generating map
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;

    public int xSize = 100;
    public int zSize = 100;
    public float noise1Scale = 3.26f;
    public float noise1Amp = 18.76f;
    public float noise2Scale = 11.04f;
    public float noise2Amp = -4.2f;
    public float noise3Scale = 80.35f;
    public float noise3Amp = 1f;
    public float noiseStrength = 1f;

    // coloring map
    Color[] colors;
    public Gradient gradient;
    float minTerrainHeight;
    float maxTerrainHeight;

    // unit gameobjects
    public GameObject barbarian;

    private NavMeshSurface navMeshSurface;


    void Start()
    {
        mesh = new Mesh(); 
        GetComponent<MeshFilter>().mesh = mesh;
        navMeshSurface = GetComponent<NavMeshSurface>();
        CreateShape();
        UpdateMesh();
        BakeNavMesh();
        //Instantiate(barbarian, new Vector3(10, 1, 10), Quaternion.identity);
        
    }

    private void Update()
    {
        CreateShape();
        UpdateMesh();

        
        BakeNavMesh();
    }

    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for(int i = 0, z = 0; z <= zSize; z++)
        {
            for(int x = 0; x <= xSize; x++)
            {
                float y =
                      noise1Amp * Mathf.PerlinNoise(x * noise1Scale, z * noise1Scale)
                    + noise2Amp * Mathf.PerlinNoise(x * noise2Scale, z * noise2Scale)
                    + noise3Amp * Mathf.PerlinNoise(x * noise3Scale, z * noise3Scale)
                        * noiseStrength - 4;
                if (x == 0 || x == xSize) {
                    y = 0;
                }
                else if (x < 5 || x > xSize - 5)
                {
                    y /= 2; // dampen 
                }
                vertices[i] = new Vector3(x, y, z);

                if (y > maxTerrainHeight) maxTerrainHeight = y;
                if (y < minTerrainHeight) minTerrainHeight = y;
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

        colors = new Color[vertices.Length];
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);
                colors[i] = gradient.Evaluate(height);
                i++;
            }
        }


    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateNormals(); // this fixes weird lighting
    }

    void BakeNavMesh()
    {
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
        }
    }

    // Normalizes a value within a specified range
    // Used to make the edge of the procedural generation always connect with the platforms
    public static float NormalizeToRange(float value, float min, float max, float newMin, float newMax)
    {
        float normalizedValue = (value - min) / (max - min);

        return normalizedValue * (newMax - newMin) + newMin;
    }
}
