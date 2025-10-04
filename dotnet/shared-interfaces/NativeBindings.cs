using System;
using Entropy.ECS.Components;
using System.Runtime.InteropServices;

namespace Entropy.Bindings;

internal static partial class NativeBindings
{
    [LibraryImport("Entropy", EntryPoint = "Entity_Create")]
    public static partial ulong EntityCreate(int type);

    [LibraryImport("Entropy", EntryPoint = "Entity_Destroy")]
    public static partial void EntityDestroy(ulong entityId);

    [LibraryImport("Entropy", EntryPoint = "Entity_AddPosition")]
    public static partial void EntityAddPosition(ulong entityId, Position position);

    [LibraryImport("Entropy", EntryPoint = "Entity_UpdatePosition")]
    public static partial void EntityUpdatePosition(ulong entityId, Position position);

    [LibraryImport("Entropy", EntryPoint = "Entity_AddDimension")]
    public static partial void EntityAddDimension(ulong entityId, Dimension dimension);

    [LibraryImport("Entropy", EntryPoint = "Entity_UpdateDimension")]
    public static partial void EntityUpdateDimension(ulong entityId, Dimension dimension);

    [LibraryImport("Entropy", EntryPoint = "Entity_AddRotation")]
    public static partial void EntityAddRotation(ulong entityId, Rotation rotation);

    [LibraryImport("Entropy", EntryPoint = "Entity_UpdateRotation")]
    public static partial void EntityUpdateRotation(ulong entityId, Rotation rotation);

    [LibraryImport("Entropy", EntryPoint = "Entity_AddColor")]
    public static partial void EntityAddColor(ulong entityId, Color color);

    [LibraryImport("Entropy", EntryPoint = "Entity_UpdateColor")]
    public static partial void EntityUpdateColor(ulong entityId, Color color);

    [LibraryImport("Entropy", EntryPoint = "Entity_AddTexture")]
    public static partial void EntityAddTexture(ulong entityId, IntPtr texture);
}