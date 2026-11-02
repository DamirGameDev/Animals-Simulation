namespace Modules.AnimalModule.Runtime.Scripts.Behaviours
{
    /// <summary>
    /// Anything an animal does every physics tick: moving, returning to the screen, and so on.
    /// A behaviour that holds subscriptions or other resources implements IDisposable too:
    /// it is disposed when the animal despawns, before its view goes back to the pool.
    /// </summary>
    public interface IAnimalBehaviour
    {
        void FixedTick(float deltaTime);
    }
}
