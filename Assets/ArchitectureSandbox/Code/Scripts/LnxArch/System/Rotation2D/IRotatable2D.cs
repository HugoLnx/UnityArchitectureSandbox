namespace ArchitectureSandbox.LnxArchSandbox
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
    public interface IRotatable2DWithCallbacks : IRotatable2D
    {
        void Tick(float deltaTime);
    }
}