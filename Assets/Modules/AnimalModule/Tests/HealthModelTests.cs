using FluentAssertions;
using Modules.AnimalModule.Runtime.Scripts.Animal;
using Modules.AnimalModule.Runtime.Scripts.Health;
using Modules.AnimalModule.Runtime.Scripts.Movement;
using NUnit.Framework;
using UnityEngine;

namespace Modules.AnimalModule.Tests
{
    public class HealthModelTests
    {
        [Test]
        public void NewAnimal_HasOneHealthPointAndIsAlive()
        {
            var animal = CreateAnimal();

            animal.HealthModel.HealthPoints.Should().Be(1);
            animal.HealthModel.IsDead.Should().BeFalse();
        }

        [Test]
        public void Die_KillsAndReportsOnce()
        {
            var animal = CreateAnimal();
            var diedCount = 0;
            animal.HealthModel.Died += () => diedCount++;

            animal.Die(null);
            animal.Die(null);

            animal.HealthModel.IsDead.Should().BeTrue();
            animal.HealthModel.HealthPoints.Should().Be(0);
            diedCount.Should().Be(1);
        }

        [Test]
        public void Die_KillerIsKnownWhenHealthReportsDeath()
        {
            var killer = CreateAnimal();
            var victim = CreateAnimal();
            AnimalModel killerOnDeath = null;
            victim.HealthModel.Died += () => killerOnDeath = victim.Killer;

            victim.Die(killer);
            victim.Die(CreateAnimal());

            killerOnDeath.Should().BeSameAs(killer);
            victim.Killer.Should().BeSameAs(killer, "the second death is ignored");
        }

        private static AnimalModel CreateAnimal() =>
            new AnimalModel(null, new MovementModel(Vector3.forward), new HealthModel());
    }
}
