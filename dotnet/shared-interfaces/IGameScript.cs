using Entropy.ECS;

namespace Shared
{
    public interface IHost
    {
        void RegisterInstance(ulong entityId, IGameScript script);
        void UnregisterInstance(ulong entityId);
    }

    public static class Runtime
    {
        public static IHost Host { get; private set; } = null!;
        public static void Init(IHost host) =>
            Host = host ?? throw new ArgumentNullException(nameof(host));
    }

    public interface IGameScript
    {
        void OnAttach(int entityId);
        void OnDetach();
        void OnUpdate(float dt);
    }

    // Optional base to make self-registration one-liners
    public abstract class GameScriptBase : IGameScript
    {
        protected ulong EntityId { get; private set; }

        public virtual void OnAttach(int entityId)
        {
            EntityId = (ulong)entityId;
            Runtime.Host.RegisterInstance(EntityId, this);
        }

        public virtual void OnDetach()
        {
            if (EntityId != 0)
                Runtime.Host.UnregisterInstance(EntityId);
        }

        public abstract void OnUpdate(float dt);
    }
}