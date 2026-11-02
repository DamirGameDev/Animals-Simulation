using FluentAssertions;
using Modules.AnimalModule.Runtime.Scripts.FoodChain;
using Modules.HudModule.Runtime.Scripts.DeathCounters;
using NUnit.Framework;

namespace Modules.HudModule.Tests
{
    public class DeathCountersModelTests
    {
        [Test]
        public void NewModel_HasZeroForEveryRole()
        {
            var model = new DeathCountersModel();

            model.GetCount(FoodChainRole.Prey).Should().Be(0);
            model.GetCount(FoodChainRole.Predator).Should().Be(0);
        }

        [Test]
        public void Increment_CountsRolesSeparatelyAndReportsNewCount()
        {
            var model = new DeathCountersModel();
            FoodChainRole? changedRole = null;
            var changedCount = 0;
            model.Changed += (role, count) =>
            {
                changedRole = role;
                changedCount = count;
            };

            model.Increment(FoodChainRole.Prey);
            model.Increment(FoodChainRole.Prey);

            model.GetCount(FoodChainRole.Prey).Should().Be(2);
            model.GetCount(FoodChainRole.Predator).Should().Be(0);
            changedRole.Should().Be(FoodChainRole.Prey);
            changedCount.Should().Be(2);
        }
    }
}
