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
        noise = new FastNoiseLite();
        noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Simplex;
        noise.Frequency = 1.0f / NoiseScale;

        // Generate terrain and place decorations
        GenerateTerrain();
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
                float height = noise.GetNoise2D(x, z) * HeightScale;

                // Handle biomes
                if (height < 0.2f) height = 0;  // Water biome (flat)
                else if (height > 3.0f) height *= 1.5f;  // Mountain biome (raised)

                Vector3 v1 = new Vector3(x * TileSize, height, z * TileSize);
                Vector3 v2 = new Vector3((x + 1) * TileSize, noise.GetNoise2D(x + 1, z) * HeightScale, z * TileSize);
                Vector3 v3 = new Vector3(x * TileSize, noise.GetNoise2D(x, z + 1) * HeightScale, (z + 1) * TileSize);
                Vector3 v4 = new Vector3((x + 1) * TileSize, noise.GetNoise2D(x + 1, z + 1) * HeightScale, (z + 1) * TileSize);

                // Add terrain triangles
                surfaceTool.AddVertex(v1);
                surfaceTool.AddVertex(v2);
                surfaceTool.AddVertex(v3);
                surfaceTool.AddVertex(v2);
                surfaceTool.AddVertex(v4);
                surfaceTool.AddVertex(v3);
            }
        }

        surfaceTool.GenerateNormals();  // Create lighting data
        Mesh terrainMesh = surfaceTool.Commit();
        MeshInstance3D meshInstance = new MeshInstance3D();
        meshInstance.Mesh = terrainMesh;
        AddChild(meshInstance);  // Add terrain to the scene
    }

    private void PlaceObjects()
    {
        for (int x = 0; x < GridSizeX; x++)
        {
            for (int z = 0; z < GridSizeZ; z++)
            {
                float height = noise.GetNoise2D(x, z) * HeightScale;

                // Place trees in forest areas
                if (height > 0.5f && height < 2.0f && TreeScene != null)
                {
                    Node3D treeInstance = (Node3D)TreeScene.Instantiate();
                    treeInstance.Position = new Vector3(x * TileSize, height, z * TileSize);
                    AddChild(treeInstance);  // Add the tree to the scene
                }
            }
        }
    }
}
