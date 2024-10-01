using Godot;
using System;

public partial class TerrainGenerator : Node3D
{
    [Export] public int GridSizeX = 100;  // Number of tiles along the X-axis
    [Export] public int GridSizeZ = 100;  // Number of tiles along the Z-axis
    [Export] public float TileSize = 2.0f;  // Size of each tile
    [Export] public float HeightScale = 5.0f;  // How much the noise affects the height
    [Export] public float NoiseScale = 10.0f;  // The "zoom" level of the noise

    private FastNoiseLite noise;

    public override void _Ready()
    {
        noise = new FastNoiseLite();
        noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Simplex;
        noise.Frequency = 1.0f / NoiseScale;

        GenerateTerrain();
    }

    private void GenerateTerrain()
    {
        SurfaceTool surfaceTool = new SurfaceTool();
        surfaceTool.Begin(Mesh.PrimitiveType.Triangles);  // Start creating a mesh with triangles

        for (int x = 0; x < GridSizeX - 1; x++)
        {
            for (int z = 0; z < GridSizeZ - 1; z++)
            {
                // Get heights using FastNoiseLite for smooth transitions
                float h1 = noise.GetNoise2D(x, z) * HeightScale;
                float h2 = noise.GetNoise2D(x + 1, z) * HeightScale;
                float h3 = noise.GetNoise2D(x, z + 1) * HeightScale;
                float h4 = noise.GetNoise2D(x + 1, z + 1) * HeightScale;

                // Define the 4 vertices of the quad
                Vector3 v1 = new Vector3(x * TileSize, h1, z * TileSize);
                Vector3 v2 = new Vector3((x + 1) * TileSize, h2, z * TileSize);
                Vector3 v3 = new Vector3(x * TileSize, h3, (z + 1) * TileSize);
                Vector3 v4 = new Vector3((x + 1) * TileSize, h4, (z + 1) * TileSize);

                // Add the two triangles for the quad
                surfaceTool.AddVertex(v1);
                surfaceTool.AddVertex(v2);
                surfaceTool.AddVertex(v3);

                surfaceTool.AddVertex(v2);
                surfaceTool.AddVertex(v4);
                surfaceTool.AddVertex(v3);
            }
        }

        // Generate the normals (lighting) and commit the mesh
        surfaceTool.GenerateNormals();
        Mesh terrainMesh = surfaceTool.Commit();  // This returns a Mesh, not an Array

        // Create MeshInstance3D to hold the terrain mesh
        MeshInstance3D meshInstance = new MeshInstance3D();
        meshInstance.Mesh = terrainMesh;
        AddChild(meshInstance);  // Add the mesh instance to the scene
    }
}
