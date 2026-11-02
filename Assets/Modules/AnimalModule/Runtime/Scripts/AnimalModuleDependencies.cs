using Modules.AnimalModule.Runtime.Scripts.Animal;
using Modules.AnimalModule.Runtime.Scripts.Behaviours;
using Modules.AnimalModule.Runtime.Scripts.Bounds;
using Modules.AnimalModule.Runtime.Scripts.Collisions;
using Modules.AnimalModule.Runtime.Scripts.Death;
using Modules.AnimalModule.Runtime.Scripts.FoodChain;
using Modules.AnimalModule.Runtime.Scripts.Movement.Jump;
using Modules.AnimalModule.Runtime.Scripts.Movement.Linear;
using Modules.AnimalModule.Runtime.Scripts.Movement.Wander;
using Modules.AnimalModule.Runtime.Scripts.Pool;
using Modules.AnimalModule.Runtime.Scripts.Spawn;
using VContainer;
using VContainer.Unity;

namespace Modules.AnimalModule.Runtime.Scripts
{
    /// <summary>
    /// Everything AnimalModule puts into the game scope. Entry points are registered in the order they start,
    /// so listeners subscribe before the first animal is spawned. Collision rules are applied in registration order.
    /// </summary>
    public static class AnimalModuleDependencies
    {
        public static void Register(IContainerBuilder builder)
        {
            builder.Register<AnimalsRepository>(Lifetime.Singleton);
            builder.Register<AnimalSerializableComponentsPool>(Lifetime.Singleton);
            builder.Register<AnimalFactory>(Lifetime.Singleton);
            builder.Register<IScreenBounds, ScreenBoundsService>(Lifetime.Singleton);
            builder.Register<SpawnPositionProvider>(Lifetime.Singleton);

            RegisterBehaviourFactories(builder);

            builder.Register<IFoodChainTieBreaker, RandomFoodChainTieBreaker>(Lifetime.Singleton);
            builder.Register<FoodChainEaterProvider>(Lifetime.Singleton);

            builder.Register<ICollisionRule, FoodChainCollisionRule>(Lifetime.Singleton);

            builder.RegisterEntryPoint<AnimalDeathsObserver>().AsSelf();
            builder.RegisterEntryPoint<AnimalCollisionsObserver>().AsSelf();
            builder.RegisterEntryPoint<CollisionRulesApplier>();
            builder.RegisterEntryPoint<AnimalDespawner>();

            builder.RegisterEntryPoint<AnimalsTicker>();
            builder.RegisterEntryPoint<AnimalSpawner>();
        }

        /// <summary>
        /// A new behaviour adds its factory here; AnimalBehavioursFactory finds it by the config type.
        /// </summary>
        public static void RegisterBehaviourFactories(IContainerBuilder builder)
        {
            builder.Register<AnimalBehavioursFactory>(Lifetime.Singleton);
            builder.Register<IAnimalBehaviourFactory, BoundsReturnBehaviourFactory>(Lifetime.Singleton);
            builder.Register<IAnimalBehaviourFactory, LinearMovementControllerFactory>(Lifetime.Singleton);
            builder.Register<IAnimalBehaviourFactory, JumpMovementControllerFactory>(Lifetime.Singleton);
            builder.Register<IAnimalBehaviourFactory, WanderBehaviourFactory>(Lifetime.Singleton);
        }
    }
}
