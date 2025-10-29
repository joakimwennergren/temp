using System;
using Entropy;

public class PhysicsTestScript : EntropyScript
{
    static PhysicsTestScript()
    {
        // Register a factory that creates an instance
        ScriptContainer.RegisterFactory(() => new PhysicsTestScript());
    }

    public PhysicsTestScript() { /* setup */ }

    public override void OnStart() => Console.WriteLine("PhysicsTestScript Started");

    public override void OnUpdate(float deltaTime, int screenWidth, int screenHeight)
    {
        // Optional: update logic
    }
}