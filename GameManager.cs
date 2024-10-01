using Godot;
using System;

public partial class GameManager : Node3D
{
    [Export] public PackedScene EntityScene;  // Reference to the entity scene (a KinematicBody3D or Node3D)
    [Export] public int EntityCount = 10;     // Number of entities to spawn
    [Export] public int GridSizeX = 100;      // Size of the terrain or map
    [Export] public int GridSizeZ = 100;      // Size of the terrain or map
    [Export] public float TileSize = 2.0f;    // Tile size for spacing

    public override void _Ready()
    {
        // Check if EntityScene is set
        if (EntityScene == null)
        {
            GD.PrintErr("EntityScene is not assigned. Please assign it in the Inspector.");
            return;
        }

        SpawnEntities();
    }

    private void SpawnEntities()
    {
        for (int i = 0; i < EntityCount; i++)
        {
            // Instantiate the entity
            Node entityInstance = EntityScene.Instantiate();

            // Ensure the entityInstance is a Node3D before casting
            if (entityInstance is Node entity)
            {
                // Randomize the position of the entity
                float randomX = (float)GD.RandRange(0, GridSizeX * TileSize);
                float randomZ = (float)GD.RandRange(0, GridSizeZ * TileSize);

                // If it's a 3D node, position it
                if (entity is Node3D entity3D)
                {
                    entity3D.Position = new Vector3(randomX, 1, randomZ);  // Start just above the terrain
                }

                // Add the entity to the scene
                AddChild(entity);
            }
            else
            {
                GD.PrintErr("Entity instance is not a Node3D.");
            }
        }
    }
}
