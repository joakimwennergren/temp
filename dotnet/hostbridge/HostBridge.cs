using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Collections.Generic;
using System.Linq;
using Entropy.ECS;
using Shared;

namespace Entropy;

internal sealed class HostImpl : IHost
{
    private static readonly object _lock = new();

    public void RegisterInstance(ulong entityId, IGameScript script)
    {
        Console.WriteLine($"Registering instance for entity {entityId}");
        if (HostBridge.TryGetContext(out var ctx))
        {
            lock (_lock) ctx.Instances[(int)entityId] = script;
        }
        else
        {
            throw new InvalidOperationException("Script context not initialized.");
        }
    }

    public void UnregisterInstance(ulong entityId)
    {
        if (HostBridge.TryGetContext(out var ctx))
        {
            lock (_lock) ctx.Instances.Remove((int)entityId);
        }
    }
}

public static class HostBridge
{
    internal sealed class ScriptContext
    {
        public AssemblyLoadContext ALC;
        public Assembly ScriptsAsm;
        public Dictionary<int, IGameScript> Instances = new();
        public Dictionary<int, object?> SavedState = new();
        public ScriptContext(AssemblyLoadContext alc, Assembly asm) { ALC = alc; ScriptsAsm = asm; }
    }

    private static ScriptContext? _ctx; 

    // Safe accessor for other host code (not for Shared)
    internal static bool TryGetContext(out ScriptContext ctx)
    {
        ctx = _ctx!;
        return _ctx is not null;
    }


    static void EnsureSharedInDefault()
    {
        // Option 1: if Bridge/Bootstrap references Shared.csproj, this line forces load:
        // _ = typeof(Shared.IGameScript).Assembly;

        // Option 2: explicitly load from a known absolute path:
        var sharedPath = "/Users/joakimwennergren/Projects/Entropy-Application/dotnet/shared-interfaces/bin/Debug/net9.0/SharedInterfaces.dll";
        if (!File.Exists(sharedPath))
            throw new FileNotFoundException("SharedInterfaces.dll not found", sharedPath);

        // If not already loaded, load it into Default ALC
        foreach (var a in AssemblyLoadContext.Default.Assemblies)
            if (string.Equals(a.GetName().Name, "SharedInterfaces", StringComparison.Ordinal))
                return;

        AssemblyLoadContext.Default.LoadFromAssemblyPath(sharedPath);
    }

    static bool InheritsFromFullName(Type t, string baseFullName)
    {
        for (var cur = t; cur != null; cur = cur.BaseType)
            if (cur.FullName == baseFullName)
                return true;
        return false;
    }

    public static int LoadScripts(IntPtr arg, int argLen)
    {
        EnsureSharedInDefault();

        // IMPORTANT: provide the host implementation to Shared before loading scripts
        Shared.Runtime.Init(new HostImpl());

        string scriptsPath = Path.GetFullPath(Marshal.PtrToStringUTF8(arg)!);
        var alc = new ScriptLoadContext(scriptsPath);
        var asm = alc.LoadFromAssemblyPath(scriptsPath);        
        _ctx = new ScriptContext(alc, asm);

        // If you can reference the base type here, this gives you the correct full name:
        string gameScriptBaseFullName = typeof(GameScriptBase).FullName!;
        // If you cannot reference it (ALC issues), hardcode the name, e.g.:
        // string gameScriptBaseFullName = "MyGame.Scripting.GameScriptBase";

        var scriptTypes = asm.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && InheritsFromFullName(t, gameScriptBaseFullName))
            .ToList();

        if (scriptTypes.Count == 0)
            throw new InvalidOperationException($"No script classes inheriting from {gameScriptBaseFullName} found in {asm.GetName().Name}.");

        foreach (var t in scriptTypes)
        {
            string ns   = t.Namespace ?? "";    // empty if global namespace
            string name = t.Name;
            string full = t.FullName ?? name;

            Console.WriteLine($"Script => Namespace: '{ns}', Class: '{name}', FullName: '{full}'");

            // If you still want to instantiate it:
            var instance = Activator.CreateInstance(t);

            // Your existing call; unrelated to discovery of name/namespace:
            Attach(_ctx.Instances.Count() + 1, Marshal.StringToCoTaskMemUTF8(full));
        }
        
        return 0;
    }

    // Add this nested delegate type
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int AttachDelegate(int entityId, IntPtr typeNameUtf8);

    public static int Attach(int entityId, IntPtr typeNameUtf8)
    {
        if (_ctx is null) {
            Console.WriteLine("Attach called but _ctx is null");
            return -1;
        }
        string typeName = Marshal.PtrToStringUTF8(typeNameUtf8)!;
        var t = _ctx.ScriptsAsm.GetType(typeName, throwOnError: true)!;
        var inst = (IGameScript)Activator.CreateInstance(t)!;
        inst.OnAttach(entityId);
        _ctx.Instances[entityId] = inst;
        return 0;
    }

    // Add this nested delegate type
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void TickDelegate(float dt);
    public static void Tick(float dt)
    {  
        if (_ctx is null) {
            Console.WriteLine("Tick called but _ctx is null");
            return;
        }
        //Console.WriteLine($"Tick called, {_ctx.Instances.Count} instances");
        foreach (var s in _ctx.Instances.Values) s.OnUpdate(dt);
    }

    // Add this nested delegate type
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void MouseDelegate(float x, float y);
    public static void SetMousePosition(float x, float y)
    {  
        //Shared.Input.SetMousePosition((float)x, (float)y);
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void UnloadDelegate();
    public static void Unload()
    {
        Console.WriteLine("Unload called");
        if (_ctx is null) return;
        foreach (var s in _ctx.Instances.Values) s.OnDetach();
        _ctx.Instances.Clear();

        var alc = _ctx.ALC;
        _ctx = null;
        alc.Unload();
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }
}
