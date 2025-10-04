using Shared;

public class Move : IGameScript
{
    public float Speed = 5f;
    public float DirX = 1, DirY = 0;

    private Entity _e = null!;
    private Shared.ILogger _log = null!; 

    public void OnAttach(Entity e) => _e = e;

    public void OnUpdate(float dt)
    {
        _e.X += DirX * Speed * dt;
        _e.Y += DirY * Speed * dt;
    }

    public void OnDetach() {
        // When entity is removed..
    }

    public object? CaptureState() => new State { Speed = Speed, DirX = DirX, DirY = DirY };

    public void RestoreState(object? s)
    {
        if (s is State st) { Speed = st.Speed; DirX = st.DirX; DirY = st.DirY; }
    }

    private record State { public float Speed; public float DirX; public float DirY; }
}