using System;
using Entropy.ECS;
using Entropy.ECS.Components;
using System.Runtime.InteropServices;
using Entropy.Physics.TwoD;
using Svampjakt;
using Entropy;
using System.Collections.Generic;

public static class Pointer {
  public static float X { get; set; }
  public static float Y { get; set; }
  public static bool IsDown { get; set; }
}

public class Game
{
  private static Level1 level1;
  private static List<Tuple<DynamicBody, Entity>> boxes = new List<Tuple<DynamicBody, Entity>>();
  //static private Entity leaf;

  [UnmanagedCallersOnly(EntryPoint = "CSharpMain")]
  public static void Main()
  {
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

  [UnmanagedCallersOnly(EntryPoint = "UpdateMousePosition")]
  public static void UpdateMousePosition(float x, float y)
  {
    Pointer.X = x;
    Pointer.Y = y;
  }

  [UnmanagedCallersOnly(EntryPoint = "UpdateMouseButton")]
  public static void UpdateMouseButton(float x, float y)
  {
  
  }

  [UnmanagedCallersOnly(EntryPoint = "CSharpOnUpdate")]
  public static void OnUpdate(float deltaTime, int screenWidth, int screenHeight)
  {
    foreach (var box in boxes) {
        var position = box.Item1.GetPosition();
        var rotation = box.Item1.GetRotation();
        box.Item2.Set(new Rotation(0.0f, 0.0f, 1.0f, rotation));
        box.Item2.Set(new Position(position.X, position.Y, 1.0f));
    }
    level1.Update(deltaTime, screenWidth, screenHeight);
  } 
}