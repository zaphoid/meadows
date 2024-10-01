using Godot;
using System;

public partial class GameManager : Node3D
{
    [Export] public PackedScene EntityScene;  // Reference to the entity scene (a KinematicBody3D)
    [Export] public int EntityCount = 10;  // Number of entities to spawn

    public override void _Ready()
    {
        SpawnEntities();
    }

		private void SpawnEntities()
	{
		for (int i = 0; i < EntityCount; i++)
		{
			// Instantiate the entity (use Instantiate instead of Instance in Godot 4.x)
			Node entityInstance = EntityScene.Instantiate();  // Correct method name
			
			// Ensure the entityInstance is a Node3D before casting
			if (entityInstance is Node3D entity)
			{
				// Randomize the position of the entity
				float randomX = (float)GD.RandRange(0, 100);
				float randomZ = (float)GD.RandRange(0, 100);
				entity.Position = new Vector3(randomX, 1, randomZ);  // Start just above the terrain

				// Add the entity to the scene
				AddChild(entity);
			}
		}
	}


}
