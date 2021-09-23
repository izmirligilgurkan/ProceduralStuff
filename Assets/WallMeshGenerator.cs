using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMeshGenerator : MonoBehaviour
{
	private static int _wallCountName;
	public static Bounds SpawnWalls(Material material, Transform parent, Vector3 pos, Vector3 direction, Vector3 wallScale, float wallTopWidth)
    {
	    
	    return SpawnWall(material, parent, direction, pos, wallScale, wallTopWidth, GetCombinedWall( direction, parent, wallScale, wallTopWidth));
	    
    }
    public static Mesh GenerateWallMesh(Vector3 wallScaleSetting, float wallTopWidthSetting)
    {
	    Mesh mesh = new Mesh();
        //Create a 'Cube' mesh...
        //2) Define the cube's dimensions
        float width = wallScaleSetting.x;
        float height = wallScaleSetting.y;
        float length = wallScaleSetting.z;
        float topWidth = wallTopWidthSetting;


        //3) Define the co-ordinates of each Corner of the cube 
        Vector3[] c = new Vector3[8];

        c[0] = new Vector3(-width * .5f, -height * .5f, length * .5f);
        c[1] = new Vector3(width * .5f, -height * .5f, length * .5f);
        c[2] = new Vector3(width * .5f, -height * .5f, -length * .5f);
        c[3] = new Vector3(-width * .5f, -height * .5f, -length * .5f);

        c[4] = new Vector3((-width * .5f * topWidth), height * .5f, length * .5f);
        c[5] = new Vector3((width * .5f * topWidth) , height * .5f, length * .5f);
        c[6] = new Vector3((width * .5f * topWidth) , height * .5f, -length * .5f);
        c[7] = new Vector3((-width * .5f * topWidth), height * .5f, -length * .5f);


        //4) Define the vertices that the cube is composed of:
        //I have used 16 vertices (4 vertices per side). 
        //This is because I want the vertices of each side to have separate normals.
        //(so the object renders light/shade correctly) 
        Vector3[] vertices = new Vector3[]
        {
	        c[0], c[1], c[2], c[3], // Bottom
	        c[7], c[4], c[0], c[3], // Left
	        c[4], c[5], c[1], c[0], // Front
	        c[6], c[7], c[3], c[2], // Back
	        c[5], c[6], c[2], c[1], // Right
	        c[7], c[6], c[5], c[4]  // Top
        };


        //5) Define each vertex's Normal
        Vector3 up = Vector3.up;
        Vector3 down = Vector3.down;
        Vector3 forward = Vector3.forward;
        Vector3 back = Vector3.back;
        Vector3 left = Vector3.left;
        Vector3 right = Vector3.right;


        Vector3[] normals = new Vector3[]
        {
	        down, down, down, down,             // Bottom
	        left, left, left, left,             // Left
	        forward, forward, forward, forward,	// Front
	        back, back, back, back,             // Back
	        right, right, right, right,         // Right
	        up, up, up, up	                    // Top
        };


        //6) Define each vertex's UV co-ordinates
        Vector2 uv00 = new Vector2(0f, 0f);
        Vector2 uv10 = new Vector2(1f, 0f);
        Vector2 uv01 = new Vector2(0f, 1f);
        Vector2 uv11 = new Vector2(1f, 1f);

        Vector2[] uvs = new Vector2[]
        {
	        uv11, uv01, uv00, uv10, // Bottom
	        uv11, uv01, uv00, uv10, // Left
	        uv11, uv01, uv00, uv10, // Front
	        uv11, uv01, uv00, uv10, // Back	        
	        uv11, uv01, uv00, uv10, // Right 
	        uv11, uv01, uv00, uv10  // Top
        };


        //7) Define the Polygons (triangles) that make up the our Mesh (cube)
        //IMPORTANT: Unity uses a 'Clockwise Winding Order' for determining front-facing polygons.
        //This means that a polygon's vertices must be defined in 
        //a clockwise order (relative to the camera) in order to be rendered/visible.
        int[] triangles = new int[]
        {
	        3, 1, 0,        3, 2, 1,        // Bottom	
	        7, 5, 4,        7, 6, 5,        // Left
	        11, 9, 8,       11, 10, 9,      // Front
	        15, 13, 12,     15, 14, 13,     // Back
	        19, 17, 16,     19, 18, 17,	    // Right
	        23, 21, 20,     23, 22, 21,	    // Top
        };


        //8) Build the Mesh
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.Optimize();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateUVDistributionMetrics();
        return mesh;
    }
    private static Bounds SpawnWall(Material material, Transform parent, Vector3 direction, Vector3 pos, Vector3 wallScale, float wallTopWidth, CombineInstance[] combineInstances = null, int wallCount = 1)
    {
        GameObject wallGameObject = new GameObject();
        var combinedMesh = wallGameObject.AddComponent<MeshFilter>();
        combinedMesh.mesh.CombineMeshes(combineInstances, true, true);
        var meshRenderer = wallGameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = material;
        wallGameObject.name = "wall " + _wallCountName;
        _wallCountName++;
        wallGameObject.transform.position = pos;
        wallGameObject.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        wallGameObject.transform.SetParent(parent, true);
        return combinedMesh.mesh.bounds;
    }
    private static CombineInstance[] GetCombinedWall(Vector3 direction, Transform parent, Vector3 wallScale, float wallTopWidth)
    {
	    Mesh mesh = new Mesh();
        mesh.Clear();
        var wallMeshInstance = GenerateWallMesh(wallScale, wallTopWidth);
        CombineInstance[] combineInstances = new CombineInstance[1];
        combineInstances[0].mesh = wallMeshInstance;
	    combineInstances[0].transform = Matrix4x4.TRS(direction * 0, Quaternion.LookRotation(Vector3.forward, Vector3.up), Vector3.one);
	    mesh.CombineMeshes(combineInstances, true, true);
        return combineInstances;
    }
    
   

}
