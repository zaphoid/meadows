using Godot;
using System;

public partial class TerrainGenerator : Node3D
{
    [Export] public int GridSizeX = 100;  // Number of tiles along the X-axis
    [Export] public int GridSizeZ = 100;  // Number of tiles along the Z-axis
    [Export] public float TileSize = 2.0f;  // Size of each tile
    [Export] public float HeightScale = 5.0f;  // How much the noise affects the height
    [Export] public float NoiseScale = 10.0f;  // The "zoom" level of the noise
    [Export] public PackedScene TreeScene;  // Scene for trees or decorations

    private FastNoiseLite noise;

    public override void _Ready()
    {
        // Initialize noise generator
        noise = new FastNoiseLite();
        noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Simplex;
        noise.Frequency = 1.0f / NoiseScale;

        // Generate terrain with biomes
        GenerateTerrain();

        // Place trees or decorations based on terrain height or biome
        PlaceObjects();
    }

    private void GenerateTerrain()
    {
        SurfaceTool surfaceTool = new SurfaceTool();
        surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

        for (int x = 0; x < GridSizeX - 1; x++)
        {
            for (int z = 0; z < GridSizeZ - 1; z++)
            {
                // Generate base terrain using noise
                float baseHeight = noise.GetNoise2D(x, z) * HeightScale;

                // Define biomes based on height values
                if (baseHeight > 3.0f)
                {
                    // Mountain biome
                    baseHeight *= 1.5f;  // Scale up mountains
                }
                else if (baseHeight < 0.2f)
                {
                    // Water biome
                    baseHeight = 0;  // Flat water surface
                    // Set water material here if needed
                }
                else
                {
                    // Plains or forest biome
                    baseHeight *= 0.5f;  // Scale down plains
                    // Add logic for forests or grass if needed
                }

                // Generate the terrain mesh using height data
                Vector3 v1 = new Vector3(x * TileSize, baseHeight, z * TileSize);
                Vector3 v2 = new Vector3((x + 1) * TileSize, noise.GetNoise2D(x + 1, z) * HeightScale, z * TileSize);
                Vector3 v3 = new Vector3(x * TileSize, noise.GetNoise2D(x, z + 1) * HeightScale, (z + 1) * TileSize);
                Vector3 v4 = new Vector3((x + 1) * TileSize, noise.GetNoise2D(x + 1, z + 1) * HeightScale, (z + 1) * TileSize);

                // Add the two triangles forming the quad
                surfaceTool.AddVertex(v1);
                surfaceTool.AddVertex(v2);
                surfaceTool.AddVertex(v3);
                surfaceTool.AddVertex(v2);
                surfaceTool.AddVertex(v4);
                surfaceTool.AddVertex(v3);
            }
        }

        surfaceTool.GenerateNormals();  // Generate normals for lighting
        Mesh terrainMesh = surfaceTool.Commit();  // Create the final mesh

        // Create MeshInstance3D to hold the terrain mesh
        MeshInstance3D meshInstance = new MeshInstance3D();
        meshInstance.Mesh = terrainMesh;
        AddChild(meshInstance);  // Add the terrain to the scene
    }

    // Smooth terrain by averaging neighbor heights
    private float GetSmoothedHeight(int x, int z)
    {
        // Get neighboring heights and average them for smoothing
        float totalHeight = noise.GetNoise2D(x, z);
        totalHeight += noise.GetNoise2D(x + 1, z);
        totalHeight += noise.GetNoise2D(x - 1, z);
        totalHeight += noise.GetNoise2D(x, z + 1);
        totalHeight += noise.GetNoise2D(x, z - 1);

        return totalHeight / 5.0f;  // Average height of neighbors
    }

    // Place objects like trees or rocks based on height or biome
    private void PlaceObjects()
    {
        for (int x = 0; x < GridSizeX; x++)
        {
            for (int z = 0; z < GridSizeZ; z++)
            {
                float height = noise.GetNoise2D(x, z) * HeightScale;

                // Place trees in specific biome areas (e.g., forests)
                if (height > 0.5f && height < 2.0f)
                {
                    // Instantiate a tree scene and place it on the terrain
                    if (TreeScene != null)
                    {
                        Node3D treeInstance = (Node3D)TreeScene.Instantiate();
                        treeInstance.Position = new Vector3(x * TileSize, height, z * TileSize);
                        AddChild(treeInstance);  // Add the tree to the scene
                    }
                }
            }
        }
    }
}
