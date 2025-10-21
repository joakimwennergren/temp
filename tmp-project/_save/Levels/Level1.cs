using System;
using System.Collections.Generic;
using Entropy.ECS;
using Entropy.ECS.Components;

namespace Svampjakt;

public class Level1 : BaseLevel
{
    private Random Random = new Random();
    private List<Tuple<Entity, float>> layers = new List<Tuple<Entity, float>>();
    private Random rng = new Random();
    private List<Mushroom> ActiveMushrooms = new List<Mushroom>();
    
    private Entity Flare = new Entity();
    private float FlareOpacity = 0.0f;
    private bool FlareDirection = false;
    private float ZOffset = 0.001f;

    private FallingLeavesSystem _LeafSystem;

    private List<Position> positions = new List<Position>()
    {
        // Layer 1 positions
        new Position(25.0f, 11.0f, 37.0f),
        new Position(35.0f, 10.0f, 38.0f),
        new Position(45.0f, 11.0f, 39.0f),
        new Position(55.0f, 6.0f, 35.0f),
        new Position(65.0f, 6.0f, 35.0f),
        new Position(75.0f, 7.0f, 35.0f),
        new Position(85.0f, 4.0f, 35.0f),
        new Position(95.0f, 4.0f, 35.0f),

        /*
        // Layer 2 positions
        new Position(5.0f,  21.0f, 25.0f),
        new Position(15.0f, 17.0f, 25.0f),
        new Position(25.0f, 14.0f, 25.0f),
        new Position(35.0f, 17.0f, 25.0f),
        new Position(45.0f, 17.0f, 25.0f),
        new Position(55.0f, 18.0f, 25.0f),
        new Position(65.0f, 17.0f, 25.0f),
        new Position(75.0f, 16.0f, 25.0f),
        new Position(85.0f, 12.0f, 25.0f),
        new Position(95.0f, 12.0f, 25.0f),

        /*

        // Layer 3 positions
        new Position(10.0f,  40.0f, 15.0f),
        new Position(20.0f,  42.0f, 15.0f),
        new Position(30.0f,  32.0f, 15.0f),
        new Position(40.0f,  35.0f, 15.0f),
        new Position(50.0f,  33.0f, 15.0f),
        new Position(60.0f,  33.0f, 15.0f),
        new Position(70.0f,  33.0f, 15.0f),
        new Position(80.0f,  31.0f, 15.0f),
        new Position(90.0f,  31.0f, 15.0f),
        */
    };
    
    public Level1()
    {
        Flare.AddComponent(new Position(0.0f, 0.0f, 100.0f));
        Flare.AddComponent(new Dimension(0.0f, 0.0f));
        Flare.AddComponent(new Color(1.0f, 1.0f, 1.0f, 1.0f));
        Flare.AddComponent(new Rotation(0.0f, 1.0f, 0.0f, 0.0f));
        Flare.AddComponent(new Texture("assets/sprites/flare.png"));

        Name = "Level 1";
        Description = "This is the first level of the game.";
        Difficulty = DifficultyLevel.Easy;
        for(int i = 0; i < 5; i++)
        {
            CreateLayer($"assets/sprites/level1_layer{i + 1}.png", (i * 10.0f)); 
            Console.WriteLine($"Created layer {i + 1} with zIndex  {(i * 10.0f)}");
        }

        _LeafSystem = new FallingLeavesSystem(seed: 42);
    }

    public override void Update(float deltaTime, int screenWidth, int screenHeight)
    {
        _LeafSystem.Update(deltaTime, screenWidth, screenHeight);
        MushroomSpawner.Update(deltaTime, screenWidth, screenHeight);

        // Scale and position the layers based on the screen size
        foreach (var layer in layers)
        {
            layer.Item1.Update(new Dimension(screenWidth / 2.0f, screenHeight / 2.0f));
            layer.Item1.Update(new Position(screenWidth / 2.0f, screenHeight / 2.0f, layer.Item2));
        }

        if(MushroomSpawner.ShouldSpawn)
        {
            Position randomPosition = positions[Random.Next(positions.Count)];
            Mushroom randomMushroom = MushroomSpawner.GetRandomMushroom();
            randomPosition.z += ZOffset; // Slightly offset in z to avoid z-fighting
            randomMushroom.Position = randomPosition;
            ZOffset += 0.0001f; // Slightly increase zIndex to avoid z-fighting

            ActiveMushrooms.Add(randomMushroom);

            if(MushroomSpawner.SpawnedMushrooms < 10)
            {
                MushroomSpawner.SpawnedMushrooms++;
            } else {
                MushroomSpawner.ShouldSpawn = false;
            }
        }

        foreach (var am in ActiveMushrooms)
        {
            am.Update(deltaTime, screenWidth, screenHeight);
        }

        Flare.Update(new Position((screenWidth / 100.0f) * 33.0f, (screenHeight / 100.0f) * 90.0f, 100.0f));
        Flare.Update(new Dimension((screenWidth / 100.0f) * 15.0f, (screenHeight / 100.0f) * 25.0f));
        Flare.Update(new Color(1.0f, 1.0f, 1.0f, FlareOpacity));

        if(FlareDirection == false)
        {
            if(FlareOpacity > 1.0f)
            {
                FlareDirection = true;
            }
            FlareOpacity += deltaTime * 0.5f;
        } else {
            if(FlareOpacity < 0.0f)
            {
                FlareDirection = false;
            }
            FlareOpacity -= deltaTime * 0.5f;
        }
    }

    private void CreateLayer(string name, float zIndex)
    {
        Entity layer = new Entity();
        layer.AddComponent(new Position(0.0f, 0.0f, zIndex));
        layer.AddComponent(new Dimension(0.0f, 0.0f));
        layer.AddComponent(new Color(1.0f, 1.0f, 1.0f, 1.0f));
        layer.AddComponent(new Rotation(0.0f, 1.0f, 0.0f, 0.0f));
        layer.AddComponent(new Texture(name));
        layer.AddComponent(new Type2D(12));
        layers.Add(new Tuple<Entity, float>(layer, zIndex));
    }
}