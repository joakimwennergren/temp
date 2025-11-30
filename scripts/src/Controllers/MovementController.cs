using Flecs.NET.Core;
using System;
using Entropy;

public class MovementController(WorldContainer world) : IController
{
    private readonly World world = world.World;
    private Texture texture;
    public void Setup()
    {
        texture = AssetLoader.LoadTexture("assets/sprites/blek_kantarell.png");
        // Create some entities
        Entity entity = world.Entity()
            .Set(new Position(10, 20, 0));

        world.System<Position>("Move")
             .Each((ref Position p) =>
             {
                 float dt = world.DeltaTime();
                 p.X += 50.0f * dt;
             });
    }

    public void Update()
    {
        /*
        Renderer.Draw(new Sprite(
            new Position(300.0f, 300.0f, 1.1f),
            new Dimension(150, 200),
            new Color(1, 1, 1, 1),
            texture
        ));
        */  
        for(var x = 0; x < 5; x++)
        {
            for(var y = 0; y < 5; y++)
            {
                Renderer.Draw(new Cube(
                    new Position(100 + x * 55, 0.0f, 100 + y * 55),
                    new Dimension(50, 50, 50),
                    new Color(1.0f, 0.8f, 1.0f, 0.2f)
                ));
            }
        }

    }
}