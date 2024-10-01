using Godot;
using System;

public partial class Entity : CharacterBody3D
{
    [Export] public float Speed = 5.0f;   // Movement speed of the entity
    private Vector3 direction;  // Stores the current direction of movement
    private Random random = new Random();

    public override void _Ready()
    {
        // Assign a random direction when the entity is ready
        direction = new Vector3((float)(random.NextDouble() * 2 - 1), 0, (float)(random.NextDouble() * 2 - 1)).Normalized();
    }

    public override void _PhysicsProcess(double delta)
    {
        // Update the velocity based on the direction and speed
        Velocity = direction * Speed;

        // Move the entity using MoveAndSlide (no parameters needed in Godot 4.x)
        MoveAndSlide();

        // Randomly change direction with a small chance every frame
        if (random.Next(100) < 1)  // Small chance to change direction
        {
            direction = new Vector3((float)(random.NextDouble() * 2 - 1), 0, (float)(random.NextDouble() * 2 - 1)).Normalized();
        }
    }
}
