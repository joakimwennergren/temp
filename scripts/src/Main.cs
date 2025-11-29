using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Entropy;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Flecs.NET.Core;
using Microsoft.Extensions.DependencyInjection;

public class Game
{
    private static World world;
    private static IServiceCollection services;
    private static IServiceProvider serviceProvider;

    [UnmanagedCallersOnly(EntryPoint = "CSharpProgress")]
    public static void Progress(float deltaTime)
    {
        if (world != null)
            world.Progress(deltaTime);

        foreach (var c in serviceProvider.GetServices<IController>())
            c.Update();

    }

    [UnmanagedCallersOnly(EntryPoint = "CSharpStart")]
    public static void Start()
    {
        world = World.Create();

        services = new ServiceCollection();

        services.AddSingleton(new WorldContainer(world));
        services.AddSingleton<IController, MovementController>();

        serviceProvider = services.BuildServiceProvider();

        foreach (var c in serviceProvider.GetServices<IController>())
            c.Setup();
    }

    /*
     private static Level1 level1;
     private static List<Tuple<DynamicBody, Entity>> boxes = new List<Tuple<DynamicBody, Entity>>();

     [UnmanagedCallersOnly(EntryPoint = "CSharpMain")]
     public static void Main()
     {
         Entropy.Pointer.Install();
         Renderer.EnablePhysicsDebugging(false);
         Random rand = new Random();


         int screenWidth = 2024;  // adjust to your screen width
         int screenHeight = 640; // adjust to your screen height

         for (int i = 0; i < 1400; i++)
         {
             float x = (float)(rand.NextDouble() * screenWidth);
             float y = (float)(rand.NextDouble() * screenHeight * 6);
             float size = 10;

             var box = new DynamicBody(
                 new Position { X = x, Y = y, Z = 0 },
                 new Dimension { Width = size, Height = size },
                 1.0f
             );

             var entity = new Entity();
             entity.Set(new Position(x, y, 1.0f));
             entity.Set(new Dimension(20.0f, 20.0f));
             entity.Set(new Color(1.0f, 1.0f, 1.0f, 1.0f));
             entity.Set(new Rotation(0.0f, 1.0f, 0.0f, 0.0f));
             entity.Set(new Texture("assets/sprites/flare.png"));
             entity.Set(new Type2D(12));

             boxes.Add(Tuple.Create(box, entity));
         }

         level1 = new Level1();
     }

     [UnmanagedCallersOnly(EntryPoint = "CSharpOnUpdate")]
     public static void OnUpdate(float deltaTime, int screenWidth, int screenHeight)
     {
       Console.WriteLine($"Mouse Position: {Pointer.X}, {Pointer.Y}");
       foreach (var box in boxes) {
           var position = box.Item1.GetPosition();
           var rotation = box.Item1.GetRotation();
           box.Item2.Set(new Rotation(0.0f, 0.0f, 1.0f, rotation));
           box.Item2.Set(new Position(position.X, position.Y, 1.0f));
       }
       level1.Update(deltaTime, screenWidth, screenHeight);
     } 
     */
}