using System;
using Entropy.ECS;
using Entropy.ECS.Components;
using System.Runtime.InteropServices;
using Svampjakt;

public class Game
{
  private static Level1 level1;


  [UnmanagedCallersOnly(EntryPoint = "CSharpMain")]
  public static void Main()
  {
    level1 = new Level1();
  }

  [UnmanagedCallersOnly(EntryPoint = "UpdateMousePosition")]
  public static void UpdateMousePosition(float x, float y)
  {
  }

  [UnmanagedCallersOnly(EntryPoint = "CSharpOnUpdate")]
  public static void OnUpdate(float deltaTime, int screenWidth, int screenHeight)
  {
    level1.Update(deltaTime, screenWidth, screenHeight);
  } 
}