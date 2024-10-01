using Godot;
using System;

public partial class GameManager : Node3D
{
    [Export] public PackedScene EntityScene;  // Reference to the entity scene (e.g., a CharacterBody3D or custom NPC scene)
    [Export] public int EntityCount = 10;  // Number of entities to spawn
    [Export] public int GridSizeX = 100;   // Size of the terrain or grid
    [Export] public int GridSizeZ = 100;   // Size of the terrain or grid
    [Export] public float TileSize = 2.0f; // Tile size for spacing entities

    public override void _Ready()
    {
        // Ensure that EntityScene is not null before instantiating
        if (EntityScene == null)
        {
            GD.PrintErr("EntityScene is not assigned. Please assign it in the Inspector.");
            return;
        }

        // Spawn entities when the scene is ready
        SpawnEntities();
    }

    // Spawns the specified number of entities across the terrain
    private void SpawnEntities()
    {
        for (int i = 0; i < EntityCount; i++)
        {
            Node entityInstance = EntityScene.Instantiate();  // Instantiate the PackedScene

            // Ensure the entity is a Node3D before continuing
            if (entityInstance is Node3D entity)
            {
                // Randomize the position of each entity
                float randomX = (float)GD.RandRange(0, GridSizeX * TileSize);
                float randomZ = (float)GD.RandRange(0, GridSizeZ * TileSize);

                entity.Position = new Vector3(randomX, 1, randomZ);  // Adjust the Y position as necessary
                
                AddChild(entity);  // Add the entity to the scene
            }
            else
            {
                GD.PrintErr("Entity instance is not a Node3D.");
            }
        }
    }
}
