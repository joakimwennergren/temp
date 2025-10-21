using System;
using Entropy.ECS;
using Entropy.ECS.Components;
using System.Runtime.InteropServices;
using Svampjakt;

public static class Pointer {
  public static float X { get; set; }
  public static float Y { get; set; }
  public static bool IsDown { get; set; }
}

public class Game
{
  private static Level1 level1;
  
  //static private Entity leaf;

  [UnmanagedCallersOnly(EntryPoint = "CSharpMain")]
  public static void Main()
  {
    //level1 = new Level1();
    /*
    // Create a new entity
    leaf = new Entity();

    var col = new Color
    {
        R = 1.0f,
        G = 1.0f,
        B = 1.0f,
        A = 1.0f
    };

    // Create a component instance
    var pos = new Position
    {
        X = 300.0f,
        Y = 300.0f,
        Z = 0.0f
    };

    var dim = new Dimension
    {
        Width = 100.0f,
        Height = 100.0f
    };

    var rot = new Rotation
    {
        X = 0.0f,
        Y = 0.0f,
        Z = 0.0f,
        W = 1.0f
    };

    var tex = new Texture("assets/textures/leaf1.png");

    // Assign (or replace) the Position component on this entity
    leaf.Set(col);
    leaf.Set(pos);
    leaf.Set(dim);
    leaf.Set(rot);
    leaf.Set(tex);
    */

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
    /*
    leaf.Mutate<Position>((ref Position p) =>
    {
        p.X += 10.0f;
    });
    */
    //level1.Update(deltaTime, screenWidth, screenHeight);
  } 
}