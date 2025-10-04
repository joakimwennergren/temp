using System;
using Shared;
using Entropy.ECS;
using Entropy.ECS.Components;

namespace GameScripts;

public class Sprite : GameScriptBase
{
    public float Speed = 5f;
    public float DirX = 1, DirY = 0;

    public Sprite() {
        Console.WriteLine("Sprite constructor");
        //Entity.AddComponent(new Position(1024/2, 640/2));
        //Entity.AddComponent(new Dimension(1024 * 2, 640 * 2));
        //Entity.AddComponent(new Color(1.0f, 1.0f, 1.0f, 0.0f));
        //Entity.AddComponent(new Rotation(45.0f));
        //Entity.AddComponent(new Texture("assets/sprites/entropy.png"));
    }

    private Entity Entity;
    private Entity SpriteEntity;

    private float angle = 0.0f;

    public override void OnAttach(int entityId)
    {

        Entity = new Entity(12); // Type 1 = Sprite
        Entity.AddComponent(new Position(300, 300, 2.0f));
        Entity.AddComponent(new Dimension(64, 64));
        Entity.AddComponent(new Color(1.0f, 1.0f, 1.0f, 1.0f));
        Entity.AddComponent(new Rotation(0.0f));
        Entity.AddComponent(new Texture("assets/sprites/ametistskivling.png"));

        SpriteEntity = new Entity(12); // Type 12 = Sprite
        SpriteEntity.AddComponent(new Position(600, 300, 1.0f));
        SpriteEntity.AddComponent(new Dimension(256, 256));
        SpriteEntity.AddComponent(new Color(1.0f, 1.0f, 1.0f, 0.5f));
        SpriteEntity.AddComponent(new Rotation(0.0f));
        SpriteEntity.AddComponent(new Texture("assets/sprites/entropy.png"));
    }

    public override void OnUpdate(float dt)
    {
        angle += 0.4f;
        SpriteEntity.Update(new Rotation(angle));
        //Console.WriteLine("IsUpdating. Sprite."); 
        //_e.X += DirX * Speed * dt;
        //_e.Y += DirY * Speed * dt;
        //var state =  Shared.Input.GetMouseState();
        //Console.WriteLine($"mouse cursor = ({state.X}, {state.Y})");

    }

    public override void OnDetach() {}
}