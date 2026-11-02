# Animals Simulation

The project is a **top-down 3D simulation** where animals appear, wander, collide and eat each other, made with **Unity 6000.3.16f1** as a test task. DI runs on *VContainer*, assets are loaded through *Addressables*, the HUD is on *uGUI*, no ECS

The focus is the animal architecture: an animal is assembled from data, so a new kind of animal is a config, and a new behaviour is a few small classes that plug into a registry

---

### Demo
**Gameplay**: animals spawn, wander, bounce off each other and turn back at the screen edge; predators eat preys, "Tasty!" and the counters update

https://github.com/user-attachments/assets/727de4b3-6127-44e2-b646-139e962bc1c6

**A new animal without code**: the mouse is made in the editor from a tag, a config with ready-made behaviours and an entry in the spawn list

https://github.com/user-attachments/assets/789437b4-71df-4caa-9034-8bbe0e6cbdb5

---

### What happens in the game
1. Every 1–2 seconds a random animal appears at a free spot on the screen and moves in a random direction, turning by a random angle every 2 seconds
2. An animal that comes close to the screen edge (8% of the screen by default) turns back towards its center with a small random deviation, so it doesn't leave the screen
3. **Frog** (prey) jumps every 1.5 s for a fixed distance of 3 units. **Snake** (predator) crawls at 2 units per second
4. **Mouse** (prey) is a third animal added on top of the task without a line of code, to show how a new kind is made: it runs fast and turns often, and snakes eat it
5. Animals collide by physics. Preys bounce off each other, a predator eats a prey, and when two predators meet a random one survives
6. **"Tasty!"** appears under the animal that has just eaten
7. The top right corner counts dead preys and predators

---

### Architecture
1. **Modules** live in `Assets/Modules`, each with its own assembly; dependencies go one way, `Hud → Animal → Shared`, and `App` composes them all:
   - `SharedModule` - asset loading through *Addressables*, configs, repository, observers, tools
   - `AppModule` - DI scopes, game state machine, scenes
   - `AnimalModule` - configs, models, behaviours, food chain, collisions, death, spawning
   - `HudModule` - death counters, "Tasty!" labels
2. **DI on VContainer** (a fork from my template, where scopes are built asynchronously), with scopes split by lifetime:
   - `PersistentServicesScope` - for the whole app, scene `Initial`: shared services, game states, state machine
   - `GameServicesScope` - child scope, built when the `Game` scene is loaded and disposed on exit: everything of the game. It spawns the camera from the `GameCamera` prefab while configuring, so the game services get it through the constructor
3. Each module lists its services explicitly in a static `XModuleDependencies.Register(builder)`. In a large project I would register by marker interfaces, so nobody forgets a registration and nobody gets merge conflicts in one list; here an explicit list reads better, since the contents of a scope are visible in one place
4. **Game state machine**: `Bootstrap → Gameplay`
5. Patterns: `XSerializableComponents` (a MonoBehaviour that only holds references), models, controllers, factories, repositories with `Added` / `Removed` events, observers and mediators, object pools. No event bus: dependencies are visible in constructors, and subscriptions live exactly as long as the scope. Async code on *UniTask*

---

### An animal is data
1. `AnimalConfig` holds a reference to the prefab, the place in the food chain and an ordered list of behaviour configs picked by type in the inspector (*Odin* serialization):
   - **Frog**: `Wander` (2 s, up to 45°), `BoundsReturn`, `JumpMovement` (1.5 s, 3 units, 45°, turns at 180°/s on the ground); tag `Frog`, eats nothing, prey
   - **Snake**: `Wander` (2 s, up to 45°), `BoundsReturn`, `LinearMovement` (2 u/s, turns at 150°/s); tag `Snake`, eats `Frog`, `Snake` and `Mouse`, predator
   - **Mouse**: `Wander` (0.8 s, up to 70°), `BoundsReturn`, `LinearMovement` (4 u/s, turns at 300°/s); tag `Mouse`, eats nothing, prey. Only assets, no code
2. `AnimalFactory` takes `AnimalSerializableComponents` from the pool, creates `AnimalModel` (config, `MovementModel`, `HealthModel`) and builds the behaviours through `AnimalBehavioursFactory` - a registry *behaviour config type → factory*. The order of the list is the tick order, and `AnimalsTicker` ticks every animal from one `FixedTick`
3. **Collisions**: `AnimalCollisionsObserver` reports that two animals touched, once per contact, `CollisionRulesApplier` applies the collision rules to them (see *Food chain and collision rules* below)
4. **Death**: `AnimalModel.Die(killer)` → `HealthModel.Died` → `AnimalDeathsObserver.Died(victim, killer)`, listened to by `AnimalDespawner` (back to the pool), `DeathCountersMediator` and `TastyLabelsMediator`
5. **Loading**: `AnimalsSpawnConfig` references animal configs and `AnimalConfig` references its prefab through `AssetReference`, so a kind of animal is loaded on its first spawn. Animal configs and prefabs are in the `Animals` group, packed separately, so each one is its own bundle

---

### Food chain and collision rules
The task has two roles, prey and predator. Here the food chain is wider: who eats whom is set per kind of animal in the configs, so new kinds and new relations need no code

1. **Kinds are assets, not an enum**: a kind of food is a `FoodChainTag` ScriptableObject. A new kind is a new asset, and no switch over an enum has to be extended
2. **`FoodChainConfig`** inside every `AnimalConfig`:
   - `Tag` - what kind of food this animal is
   - `Eats` - the list of tags it eats
   - `Role` - prey or predator, only for the death counters; it doesn't affect who eats whom
3. An animal eats another one when the other's tag is in its `Eats`. So any chain can be configured, not only *predator eats prey*:
   - one-sided chains: a spider eats flies, a bird eats spiders, and a bird and a fly just bounce off each other
   - an animal nobody eats: give it a tag that is in nobody's `Eats`
   - cannibals: a snake has its own tag in `Eats`
   - when both animals can eat each other, `IFoodChainTieBreaker` decides; by default it is random, and it is replaceable through DI
4. **Collision rules**: `CollisionRulesApplier` applies the `ICollisionRule` list in registration order. A rule returns `Stop` when it has resolved the collision, so the rules after it are skipped, or `Continue` when it doesn't apply
5. The only rule now is `FoodChainCollisionRule`. A new interaction is a new rule registered in `AnimalModuleDependencies`, without changes in the applier: a poisonous prey that kills whoever bites it goes before the food chain rule and returns `Stop`; a push between preys goes after it and runs when nobody was eaten

---

### How to add an animal
1. Create a `FoodChainTag` for the new kind and add it to the `Eats` of the animals that eat it
2. Create an `AnimalConfig`: prefab, tag, diet (`Eats`), role, behaviours
3. Put the config and the prefab into the `Animals` Addressables group and add it to `AnimalsSpawnConfig`

No code is needed while the existing behaviours fit. A new behaviour, say flying, is three classes and one line:

```csharp
[Serializable] public class FlyConfig : IAnimalBehaviourConfig { ... }
public class FlyBehaviour : IAnimalBehaviour { ... }
public class FlyBehaviourFactory : AnimalBehaviourFactory<FlyConfig> { ... }
// AnimalModuleDependencies.RegisterBehaviourFactories:
builder.Register<IAnimalBehaviourFactory, FlyBehaviourFactory>(Lifetime.Singleton);
```

- A behaviour that holds subscriptions or other resources also implements `IDisposable`: it is disposed when the animal despawns, before its view goes back to the pool
- State that only one behaviour needs lives inside it. State that several behaviours share is an explicit model in `AnimalModel`, like `MovementModel` and `HealthModel`, so what an animal consists of is visible in one place

---

### Decisions and deviations from the task
1. **Camera** is tilted by 70° instead of looking straight down: the scene reads better with perspective
2. **At most 30 animals** are alive at once (`AnimalsSpawnConfig.MaxAnimalsCount`): only snakes eat snakes, so without a limit they pile up. While the field is full, the spawn is skipped
3. **Random movement** is a random direction at spawn, a random turn at an interval (`Wander`) and a random deviation when returning to the screen. Behaviours change the wanted direction, and the movement turns towards it with a limited turn speed, so animals move along arcs
4. **Predator vs predator**: the survivor is chosen randomly by a separate replaceable tie breaker
5. **Role for the counters** is an explicit field: the diet decides who eats whom, the role only decides how the death is counted

---

### Tests
1. EditMode tests on *NUnit* and *FluentAssertions* for movement, wandering, returning to the screen, the behaviour registry and disposal, the food chain, collision rules, health, deaths and HUD counters
2. `AnimalConfigsValidationTests` check the real assets: every `AnimalConfig` has a prefab with its components, a food chain tag and a factory for every behaviour, and `AnimalsSpawnConfig` is valid. A broken animal is found by the tests, not on its first spawn
3. Fakes for random, screen bounds and the tie breaker live in `AnimalModule/Tests/Fakes`. The fake random can return any point of a range, so both outcomes of random decisions are checked
4. Run them from *Window → General → Test Runner → EditMode* (`AnimalModule.Tests`, `HudModule.Tests`)

---

### How to run
1. Open the project in **Unity 6000.3.16f1**
2. Press Play in any scene: the game always starts from `Assets/Modules/AppModule/Runtime/Scenes/Initial.unity`, which loads `Game`
3. In the Editor, *Addressables* run in the *Use Asset Database* mode, so nothing has to be built. **Before building a player**, build the content: *Window → Asset Management → Addressables → Groups → Build → New Build → Default Build Script*

Build scenes: `Initial`, `Game`

---

### Stack
*Unity 6*, *C#*, *URP*, *VContainer* (fork), *UniTask*, *Addressables*, *Odin Inspector*, *uGUI*, *TextMesh Pro*, *NUnit*, *FluentAssertions 8* (free for non-commercial use)

---

*Happy to hear any feedback or questions! :)*
