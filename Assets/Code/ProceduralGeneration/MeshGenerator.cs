using System;
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
    private NavMeshSurface navMeshSurface;
    Vector3[] vertices;
    int[] triangles;

    public int xSize = 300;
    public int zSize = 100;
    public float noise1Scale = 0.06f;
    public float noise1Amp = 11f;
    public float noise2Scale = 0f;
    public float noise2Amp = -0.82f;
    public float noise3Scale = 0f;
    public float noise3Amp = 0f;
    public float noiseStrength = 1f;

    // coloring map
    Color[] colors;
    public Gradient gradient;
    float minTerrainHeight;
    float maxTerrainHeight;


    // Spawning in lumber mills
    public GameObject lumberMillPrefab;
    private Vector3 leftLumberMillSpawnPoint = new Vector3(Int32.MaxValue, Int32.MaxValue, Int32.MaxValue);
    private Vector3 rightLumberMillSpawnPoint = new Vector3(Int32.MaxValue, Int32.MaxValue, Int32.MaxValue);
    private Vector3 middleLumberMillSpawnPoint = new Vector3(Int32.MinValue, Int32.MinValue, Int32.MinValue);

    // Deposit zones
    public GameObject team1DepositZone;
    public GameObject team2DepositZone;





    void Start()
    {
        /**
        if (seed == 0)
        {
            seed = UnityEngine.Random.Range(1, 100000);
        }
        // Initialize the random number generator with the seed
        UnityEngine.Random.InitState(seed);
        Debug.Log("Using seed: " + seed);
        noise1Scale = UnityEngine.Random.Range(0.50f, 0.63f);
        noise1Amp = UnityEngine.Random.Range(4f, 6f);
        */


        mesh = new Mesh { indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 }; // Ensure it supports large meshes
        mesh.MarkDynamic(); // Marks the mesh as dynamic
        GetComponent<MeshFilter>().mesh = mesh;
        navMeshSurface = GetComponent<NavMeshSurface>();
        CreateShape();
        UpdateMesh();
        
        BakeNavMesh();
        SpawnLumberMills();
        team1DepositZone.SetActive(true);
        team2DepositZone.SetActive(true);
        //Instantiate(barbarian, new Vector3(10, 1, 10), Quaternion.identity);

    }

    /**
    private void Update()
    {
        CreateShape();
        UpdateMesh();
        if (Input.GetKeyDown(KeyCode.B)) BakeNavMesh();
    }
    */
    

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


                // if in base, make flat
                if (x < 50 || x > xSize - 50)
                {
                    y = 0;
                }

                // find spawn point for left side lumber mill
                int leftThirdStart = 50;
                int battlefieldSize = xSize - (leftThirdStart * 2);
                int leftThirdEnd = leftThirdStart + (battlefieldSize /3);
                int rightThirdStart = leftThirdEnd + (battlefieldSize / 3);
                int rightThirdEnd = xSize - leftThirdStart;

                if (x > leftThirdStart && x < leftThirdEnd)
                {
                    if(y < leftLumberMillSpawnPoint.y)
                    {
                        leftLumberMillSpawnPoint = new Vector3(x, y, z);
                    }
                }

                // find spawn point for middle lumber mill
                if(x > leftThirdEnd && x < rightThirdStart)
                {
                    if((y-1) > middleLumberMillSpawnPoint.y)
                    {
                        middleLumberMillSpawnPoint = new Vector3(x, y-1, z);
                    }
                }

                // find spawn point for right side lumber mill
                if(x > rightThirdStart && x < rightThirdEnd)
                {
                    if (y < rightLumberMillSpawnPoint.y)
                    {
                        rightLumberMillSpawnPoint = new Vector3(x, y, z);
                    }
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

        // Update the mesh collider
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            meshCollider.sharedMesh = mesh; // Assign the updated mesh
        }
        //BakeNavMesh();
    }

    void BakeNavMesh()
    {
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (navMeshSurface != null)
        {
            if (mesh.vertexCount > 0 && meshCollider.sharedMesh != null)
            {
                Debug.Log("Baking NavMesh with vertex count: " + mesh.vertexCount);
                navMeshSurface.BuildNavMesh();
            }
            else
            {
                Debug.LogWarning("Mesh is invalid or empty. Cannot bake NavMesh.");
            }
        }
        else
        {
            Debug.LogWarning("NavMeshSurface component is missing!");
        }
    }

    void SpawnLumberMills()
    {
        Quaternion lumberMillRotation = Quaternion.Euler(-90, 0, 0);

        Instantiate(lumberMillPrefab, leftLumberMillSpawnPoint, lumberMillRotation);
        Instantiate(lumberMillPrefab, middleLumberMillSpawnPoint, lumberMillRotation);
        Instantiate(lumberMillPrefab, rightLumberMillSpawnPoint, lumberMillRotation);
    }
}
