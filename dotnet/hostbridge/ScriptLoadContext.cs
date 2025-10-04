using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Collections.Generic;


namespace Entropy;

public sealed class ScriptLoadContext : AssemblyLoadContext
{
    private readonly string _baseDir;
    private readonly Assembly _sharedAsm;               
    private readonly string _sharedName;

    public ScriptLoadContext(string mainAssemblyPath)
        : base("ScriptsALC", isCollectible: true)
    {
        _baseDir = Path.GetDirectoryName(Path.GetFullPath(mainAssemblyPath))!;
        _sharedAsm = typeof(Shared.IGameScript).Assembly;
        _sharedName = _sharedAsm.GetName().Name!;

        Resolving += OnResolving;
        ResolvingUnmanagedDll += OnResolvingUnmanaged;
    }

    protected override Assembly? Load(AssemblyName name)
    {
        // Always use the exact Default ALC instance of Shared
        if (string.Equals(name.Name, _sharedName, StringComparison.Ordinal))
            return _sharedAsm;

        var candidate = Path.Combine(_baseDir, name.Name + ".dll");
        if (File.Exists(candidate))
            return LoadFromAssemblyPath(candidate);

        return null;
    }

    private Assembly? OnResolving(AssemblyLoadContext _, AssemblyName name) => Load(name);

    private IntPtr OnResolvingUnmanaged(Assembly _, string libName)
    {
        var candidate = Path.Combine(_baseDir, libName);
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) candidate += ".dylib";
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) candidate += ".so";
        else candidate += ".dll";
        return File.Exists(candidate) ? LoadUnmanagedDllFromPath(candidate) : IntPtr.Zero;
    }
}