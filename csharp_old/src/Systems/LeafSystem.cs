using System;
using System.Collections.Generic;
using Entropy.ECS;
using Entropy.ECS.Components;

public sealed class FallingLeavesSystem
{
    static class Mathx { public const float PI = (float)Math.PI; public const float TAU = (float)(Math.PI * 2.0); public static float Clamp(float v, float min, float max) => v < min ? min : (v > max ? max : v); public static float Lerp(float a, float b, float t) => a + (b - a) * t; public static float Frac(float x) => x - (float)Math.Floor(x); }
    public readonly struct RangeF { public readonly float Min; public readonly float Max; public RangeF(float min, float max) { Min = min; Max = max; } }
    struct Leaf
    {
        public Entity Entity;      // the ECS entity representing this leaf
        public Position Pos;       // local-space position (x,y in field, z for draw order)
        public float VY;           // downward speed (local space)
        public float BaseDriftX;   // base horizontal drift speed (local space)
        public float RotDeg;       // current rotation in degrees
        public float AngularDegS;  // angular speed in degrees per second
        public float Scale;        // uniform scale factor
        public float Seed;         // random seed for noise functions
        public float alpha;
    }
    
    // --- configuration ---
    public int MaxLeaves = 100;
    public Dimension SpawnAreaSize = new Dimension(600.0f, 300.0f); // local units around Origin
    public float RecycleMargin = 2f;
    public RangeF FallSpeed = new RangeF(4.0f, 6.8f);
    public RangeF BaseDriftX = new RangeF(-1.0f, 1.0f);
    public RangeF TumbleDegPerSec = new RangeF(30f, 120f);
    public RangeF LeafScale = new RangeF(1.0f, 2.0f);

    // flutter & wind
    public float NoiseSwayAmplitude = 0.6f;
    public float NoiseTimeScale = 0.4f;
    public float NoiseSpaceScale = 0.35f;
    public float GustStrength = 1.0f;
    public float GustFrequencyHz = 0.08f;

    // render mapping: local → world
    public Position Origin = new Position(0.0f, 0.0f); // world-space origin of the field
    public float WorldScale = 10f;                         // scale local units to world

    // tint kept for renderer if you need it later
    public uint LeafTint = 0xFFFFFFFF;

    readonly Random _rng;
    readonly List<Leaf> _pool = new List<Leaf>();
    float _time;
    float _zCursor = 120.0f; // incremental Z for draw order

    public FallingLeavesSystem(int seed = 1337)
    {
        _rng = new Random(seed);
        for (int i = 0; i < MaxLeaves; i++)
            CreateLeaf(initialRandomY: true);
    }

    public void Update(float dt, int screenWidth, int screenHeight)
    {
        if (dt <= 0f) return;
        _time += dt;

        float halfW = SpawnAreaSize.Width * 0.5f;
        float halfH = SpawnAreaSize.Height * 0.5f;
        float bottomLocal = -halfH; // local-space bounds (centered on Origin)
        float topLocal    = +halfH;

        for (int i = 0; i < _pool.Count; i++)
        {
            var leaf = _pool[i];

            // Per-leaf gust (de-phased by seed so they don’t all sway the same)
            float gust = (float)Math.Sin((_time + leaf.Seed * 3.17f) * Mathx.TAU * Math.Max(0f, GustFrequencyHz)) * GustStrength;

            // Noise-based sway
            float n = ValueNoise2D(
                leaf.Pos.X * NoiseSpaceScale + leaf.Seed,
                leaf.Pos.Y * NoiseSpaceScale + leaf.Seed + 19.87f,
                _time * Math.Max(0.0001f, NoiseTimeScale)
            );
            float sway = (n - 0.5f) * 2f * Math.Max(0f, NoiseSwayAmplitude);

            float vx = leaf.BaseDriftX + gust + sway;
            float vy = -Math.Max(0.001f, leaf.VY); // NEGATIVE: fall downward

            // integrate local position
            leaf.Pos.X += vx * dt;
            leaf.Pos.Y += vy * dt;
            leaf.RotDeg += leaf.AngularDegS * dt;


            // recycle if below bottom
            if (leaf.Pos.Y < bottomLocal - RecycleMargin)
            {
                RecycleLeafToTop(ref leaf, halfW, topLocal);
            }

            // WRITE BACK the modified struct!
            _pool[i] = leaf;

            //leaf.Entity.Update(RotationFromZDeg(leaf.RotDeg));
            //leaf.Entity.Update(new Dimension(16.0f * leaf.Scale, 16.0f * leaf.Scale));
            //leaf.Entity.Update(new Color(1.0f, 1.0f, 1.0f, leaf.alpha));
            // leaf.Entity.Update(new Color(
            // push to ECS entity (map local → world)
            UpdateEntityPosition(leaf);
            UpdateEntityRotation(leaf, leaf.RotDeg);
        }
    }

    // ---------- internals ----------

    void CreateLeaf(bool initialRandomY)
    {
        float halfW = SpawnAreaSize.Width * 0.5f;
        float halfH = SpawnAreaSize.Height * 0.5f;

        var leaf = new Leaf();

        // initial spawn in LOCAL space
        leaf.Pos.X = RandRange(-halfW, +halfW);
        leaf.Pos.Y = initialRandomY ? RandRange(-halfH, +halfH) : (halfH + RecycleMargin);
        leaf.Pos.Z = _zCursor; _zCursor += 0.01f;
        leaf.alpha = RandRange(0.5f, 1.0f);

        RandomizeLeafKinematics(ref leaf);

        List<string> textures = new List<string>()
        {
            "assets/sprites/leaf1.png",
            "assets/sprites/leaf2.png",
            "assets/sprites/leaf3.png",
            "assets/sprites/leaf4.png",
        };

        // Create ECS entity once
        var entity = new Entity();
        entity.Set(new Position(0, 0, leaf.Pos.Z));  // will be set right below
        entity.Set(new Dimension(32.0f, 32.0f));
        entity.Set(new Color(1.0f, 1.0f, 1.0f, 1.0f));
        entity.Set(new Rotation(0.0f, 1.0f, 0.0f, 0.0f));
        entity.Set(new Texture(textures[_rng.Next(textures.Count)]));
        entity.Set(new Type2D(12));
        leaf.Entity = entity;

        // set world position now
        UpdateEntityPosition(leaf);

        _pool.Add(leaf);
        // Console.WriteLine("Created leaf entity");
    }

    void RecycleLeafToTop(ref Leaf leaf, float halfW, float topLocal)
    {
        // Reuse the SAME entity; just move & randomize
        leaf.Pos.X = RandRange(-halfW, +halfW);
        leaf.Pos.Y = topLocal + RecycleMargin;
        // keep leaf.Pos.z (draw order)
        RandomizeLeafKinematics(ref leaf);
    }

    void RandomizeLeafKinematics(ref Leaf leaf)
    {
        leaf.VY = RandRange(FallSpeed.Min, FallSpeed.Max);
        leaf.BaseDriftX = RandRange(BaseDriftX.Min, BaseDriftX.Max);
        float tumble = RandRange(TumbleDegPerSec.Min, TumbleDegPerSec.Max);
        leaf.AngularDegS = (_rng.NextDouble() < 0.5) ? tumble : -tumble;
        leaf.Scale = RandRange(LeafScale.Min, LeafScale.Max);
        leaf.Seed = (float)_rng.NextDouble() * 1000f;
        // start at a random rotation
        leaf.RotDeg = RandRange(0f, 360f);
    }

    static void UpdateEntityRotation(in Leaf leaf, float deg)
    {
        float r = deg * (float)Math.PI / 180f;
        float half = r * 0.5f;
        float s = MathF.Sin(half);
        float c = MathF.Cos(half);
        // quaternion (x, y, z, w) for rotation about Z:
        leaf.Entity.Mutate<Rotation>((ref Rotation r) =>
        {
            r.X = 0f;
            r.Y = 0f;
            r.Z = s;
            r.W = c;
        });
    }

    void UpdateEntityPosition(in Leaf leaf)
    {
        // map local -> world using Origin and WorldScale
        float worldX = Origin.X + leaf.Pos.X * WorldScale;
        float worldY = Origin.Y + leaf.Pos.Y * WorldScale;
        var l = leaf;
        leaf.Entity.Mutate<Position>((ref Position p) =>
        {
            p.X = worldX;
            p.Y = worldY;
            p.Z = l.Pos.Z;
        });
        //leaf.Entity.Update(new Position(worldX, worldY, leaf.Pos.z));
    }

    float RandRange(float a, float b)
    {
        if (a > b) (a, b) = (b, a);
        return (float)(a + (_rng.NextDouble() * (b - a)));
    }

    // noise
    static float ValueNoise2D(float x, float y, float t)
    {
        float n1 = SmoothNoise(x + t * 0.71f, y);
        float n2 = SmoothNoise(x * 0.33f, y + t * 1.17f);
        return 0.5f * (n1 + n2);
    }

    static float SmoothNoise(float x, float y)
    {
        int x0 = (int)MathF.Floor(x);
        int y0 = (int)MathF.Floor(y);
        int x1 = x0 + 1;
        int y1 = y0 + 1;

        float tx = x - x0;
        float ty = y - y0;

        float a = Hash01(x0, y0);
        float b = Hash01(x1, y0);
        float c = Hash01(x0, y1);
        float d = Hash01(x1, y1);

        // quintic
        tx = tx * tx * tx * (tx * (tx * 6 - 15) + 10);
        ty = ty * ty * ty * (ty * (ty * 6 - 15) + 10);

        float u = Lerp(a, b, tx);
        float v = Lerp(c, d, tx);
        return Lerp(u, v, ty);
    }

    static float Lerp(float a, float b, float t) => a + (b - a) * t;

    static float Hash01(int x, int y)
    {
        unchecked
        {
            uint h = 2166136261u;
            h = (h ^ (uint)x) * 16777619u;
            h = (h ^ (uint)y) * 16777619u;
            h ^= (h >> 13);
            h *= 1274126177u;
            return (h & 0x00FFFFFF) / 16777216f;
        }
    }
}
