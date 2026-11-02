using Modules.AnimalModule.Runtime.Scripts.Animal;

namespace Modules.AnimalModule.Runtime.Scripts.Collisions
{
    /// <summary>
    /// What happens when two animals touch. A rule that has resolved the collision returns Stop, so the rules
    /// registered after it are skipped; a rule that doesn't apply returns Continue.
    /// </summary>
    public interface ICollisionRule
    {
        CollisionRuleResult Apply(AnimalModel first, AnimalModel second);
    }
}
