using System.Collections.Generic;
using System.Reflection;
using FluentAssertions;
using Modules.AnimalModule.Runtime.Scripts.Animal;
using Modules.AnimalModule.Runtime.Scripts.Collisions;
using Modules.AnimalModule.Runtime.Scripts.FoodChain;
using Modules.AnimalModule.Runtime.Scripts.Health;
using Modules.AnimalModule.Runtime.Scripts.Movement;
using Modules.AnimalModule.Tests.Fakes;
using NUnit.Framework;
using UnityEngine;

namespace Modules.AnimalModule.Tests
{
    public class CollisionRulesApplierTests
    {
        private readonly List<Object> _assets = new List<Object>();

        private FoodChainTag _frogTag;
        private FoodChainTag _snakeTag;

        [SetUp]
        public void SetUp()
        {
            _frogTag = CreateAsset<FoodChainTag>();
            _snakeTag = CreateAsset<FoodChainTag>();
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var asset in _assets)
                Object.DestroyImmediate(asset);

            _assets.Clear();
        }

        [Test]
        public void PredatorMeetsPrey_PreyIsKilledByPredator()
        {
            var applier = CreateApplier(CreateFoodChainRule());
            var snake = CreateSnake();
            var frog = CreateFrog();

            applier.Apply(frog, snake);

            frog.HealthModel.IsDead.Should().BeTrue();
            snake.HealthModel.IsDead.Should().BeFalse();
            frog.Killer.Should().BeSameAs(snake);
        }

        [Test]
        public void PreyMeetsPrey_NobodyDies()
        {
            var applier = CreateApplier(CreateFoodChainRule());
            var first = CreateFrog();
            var second = CreateFrog();

            applier.Apply(first, second);

            first.HealthModel.IsDead.Should().BeFalse();
            second.HealthModel.IsDead.Should().BeFalse();
        }

        [Test]
        public void DeadAnimal_IsIgnored()
        {
            var applier = CreateApplier(CreateFoodChainRule());
            var firstSnake = CreateSnake();
            var secondSnake = CreateSnake();

            applier.Apply(firstSnake, secondSnake);
            applier.Apply(secondSnake, firstSnake);

            firstSnake.HealthModel.IsDead.Should().BeFalse(
                "the fake tie breaker lets the first snake win, so the dead snake would eat it if it were applied");
            secondSnake.HealthModel.IsDead.Should().BeTrue();
        }

        [Test]
        public void RuleThatStops_SkipsLaterRules()
        {
            var laterRule = new CountingRule(CollisionRuleResult.Continue);
            var applier = CreateApplier(CreateFoodChainRule(), laterRule);

            applier.Apply(CreateSnake(), CreateFrog());

            laterRule.CallsCount.Should().Be(0);
        }

        [Test]
        public void RuleThatContinues_LetsLaterRulesRun()
        {
            var laterRule = new CountingRule(CollisionRuleResult.Continue);
            var applier = CreateApplier(CreateFoodChainRule(), laterRule);

            applier.Apply(CreateFrog(), CreateFrog());

            laterRule.CallsCount.Should().Be(1);
        }

        [Test]
        public void EarlierRule_TakesPrecedence()
        {
            var shieldRule = new CountingRule(CollisionRuleResult.Stop);
            var applier = CreateApplier(shieldRule, CreateFoodChainRule());
            var frog = CreateFrog();

            applier.Apply(CreateSnake(), frog);

            shieldRule.CallsCount.Should().Be(1);
            frog.HealthModel.IsDead.Should().BeFalse("the rule before the food chain resolved the collision");
        }

        private CollisionRulesApplier CreateApplier(params ICollisionRule[] rules) =>
            new CollisionRulesApplier(new AnimalCollisionsObserver(new AnimalsRepository()), rules);

        private static FoodChainCollisionRule CreateFoodChainRule() =>
            new FoodChainCollisionRule(new FoodChainEaterProvider(new FakeFoodChainTieBreaker()));

        private AnimalModel CreateFrog() => CreateAnimal(new FoodChainConfig(_frogTag, FoodChainRole.Prey));

        private AnimalModel CreateSnake() =>
            CreateAnimal(new FoodChainConfig(_snakeTag, FoodChainRole.Predator, _frogTag, _snakeTag));

        private AnimalModel CreateAnimal(FoodChainConfig foodChain)
        {
            var config = CreateAsset<AnimalConfig>();
            var foodChainField = typeof(AnimalConfig)
                .GetField("<FoodChain>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);

            foodChainField.Should().NotBeNull("without the food chain every animal would be harmless");
            foodChainField.SetValue(config, foodChain);

            return new AnimalModel(config, new MovementModel(Vector3.forward), new HealthModel());
        }

        private T CreateAsset<T>() where T : ScriptableObject
        {
            var asset = ScriptableObject.CreateInstance<T>();
            _assets.Add(asset);
            return asset;
        }

        private class CountingRule : ICollisionRule
        {
            public int CallsCount { get; private set; }

            private readonly CollisionRuleResult _result;

            public CountingRule(CollisionRuleResult result)
            {
                _result = result;
            }

            public CollisionRuleResult Apply(AnimalModel first, AnimalModel second)
            {
                CallsCount++;
                return _result;
            }
        }
    }
}
