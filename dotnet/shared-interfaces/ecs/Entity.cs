using System;
using System.Runtime.CompilerServices;
using Entropy.Bindings;

namespace Entropy.ECS;

public class Entity : IDisposable
{
    public ulong EntityId {get; set;}

    public Entity(int type)
    {
        EntityId = NativeBindings.EntityCreate(type);
    }

    ~Entity()
    {
        Dispose();
    }

    public void Dispose()
    {
        NativeBindings.EntityDestroy(EntityId);
    }

    public void Update(IComponent component)
    {
        component.Update(EntityId);
    }

    public void AddComponent(IComponent component)
    {
        component.AddTo(EntityId);
    }
}