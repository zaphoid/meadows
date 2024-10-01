using Godot;
using System;

public partial class Entity : CharacterBody3D  // Use CharacterBody3D in Godot 4.x
{
    [Export] public float Speed = 5.0f;  // Movement speed of the entity
    private Vector3 velocity = Vector3.Zero;  // Entity movement velocity
    private Vector3 direction;  // Movement direction
    private Random random = new Random();

    public override void _Ready()
    {
        // Assign a random initial movement direction
        direction = new Vector3((float)(random.NextDouble() * 2 - 1), 0, (float)(random.NextDouble() * 2 - 1)).Normalized();
    }

    public override void _PhysicsProcess(double delta)
    {
        // Apply movement to the entity
        velocity = direction * Speed;

        // Use MoveAndSlide for smooth movement and collision detection
        velocity = MoveAndSlide(velocity, Vector3.Up);

        // Randomly change direction after some time
        if (random.Next(100) < 1)  // Small chance to change direction
        {
            direction = new Vector3((float)(random.NextDouble() * 2 - 1), 0, (float)(random.NextDouble() * 2 - 1)).Normalized();
        }
    }

    private Vector3 MoveAndSlide(Vector3 velocity, Vector3 up)
    {
        throw new NotImplementedException();
    }

}
