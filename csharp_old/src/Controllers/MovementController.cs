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
                 Console.WriteLine($"Delta Time: {dt}");
                 p.X += 50.0f * dt;

                 Renderer.Draw(new Sprite(
                     new Position(p.X, 300.0f, 1.0f),
                     new Dimension(150, 200),
                     new Color(1, 1, 1, 1),
                     texture
                 ));
             });
    }

    public void Update()
    {
        Renderer.Draw(new Quad(new Position(300, 300.0f, 1.0f), new Dimension(400, 400), new Color(1, 1, 1, 1)));
    }
}