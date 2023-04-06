namespace ArchitectureSandbox.Zen2
{
    public enum RotationState {
        NoPushing,
        PushingClockwise,
        PushingAntiClockwise
    }
    public interface IRotatable2D
    {
        void SwitchTo(RotationState state);
    }
    public interface IRotatable2DInternal : IRotatable2D
    {
        void Tick(float deltaTime);
    }
}