using System.Collections.Generic;
using FluentAssertions;
using Modules.AnimalModule.Runtime.Scripts.FoodChain;
using Modules.AnimalModule.Tests.Fakes;
using NUnit.Framework;
using UnityEngine;

namespace Modules.AnimalModule.Tests
{
    public class FoodChainEaterProviderTests
    {
        private readonly List<FoodChainTag> _tags = new List<FoodChainTag>();

        private FakeFoodChainTieBreaker _tieBreaker;
        private FoodChainEaterProvider _eaterProvider;

        private FoodChainTag _frogTag, _snakeTag, _flyTag, _spiderTag, _birdTag, _crabTag;

        [SetUp]
        public void SetUp()
        {
            _tieBreaker = new FakeFoodChainTieBreaker();
            _eaterProvider = new FoodChainEaterProvider(_tieBreaker);

            _frogTag = CreateTag();
            _snakeTag = CreateTag();
            _flyTag = CreateTag();
            _spiderTag = CreateTag();
            _birdTag = CreateTag();
            _crabTag = CreateTag();
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var tag in _tags)
                Object.DestroyImmediate(tag);

            _tags.Clear();
        }

        [Test]
        public void PreyAndPrey_NobodyEats()
        {
            _eaterProvider.GetEater(Frog(), Frog()).Should().Be(FoodChainEater.None);
        }

        [Test]
        public void PredatorAndPrey_PredatorEatsInAnyOrder()
        {
            _eaterProvider.GetEater(Snake(), Frog()).Should().Be(FoodChainEater.First);
            _eaterProvider.GetEater(Frog(), Snake()).Should().Be(FoodChainEater.Second);
        }

        [TestCase(true, FoodChainEater.First)]
        [TestCase(false, FoodChainEater.Second)]
        public void TwoAnimalsThatEatEachOther_TieBreakerDecides(bool isFirstWinner, FoodChainEater expected)
        {
            _tieBreaker.IsFirstWinner = isFirstWinner;

            _eaterProvider.GetEater(Snake(), Snake()).Should().Be(expected);
            _tieBreaker.CallsCount.Should().Be(1);
        }

        [Test]
        public void OneSidedChain_OnlyTheDietDecides()
        {
            var fly = new FoodChainConfig(_flyTag, FoodChainRole.Prey);
            var spider = new FoodChainConfig(_spiderTag, FoodChainRole.Predator, _flyTag);
            var bird = new FoodChainConfig(_birdTag, FoodChainRole.Predator, _spiderTag);

            _eaterProvider.GetEater(bird, spider).Should().Be(FoodChainEater.First);
            _eaterProvider.GetEater(fly, spider).Should().Be(FoodChainEater.Second);
            _eaterProvider.GetEater(bird, fly).Should().Be(FoodChainEater.None, "the bird doesn't eat flies");
            _tieBreaker.CallsCount.Should().Be(0);
        }

        [Test]
        public void AnimalNobodyEats_IsSafeFromPredators()
        {
            var crab = new FoodChainConfig(_crabTag, FoodChainRole.Prey);

            _eaterProvider.GetEater(Snake(), crab).Should().Be(FoodChainEater.None);
        }

        [TestCase(0, true)]
        [TestCase(1, false)]
        public void RandomTieBreaker_PicksWinnerByRandomService(int randomOffset, bool isFirstWinner)
        {
            var tieBreaker = new RandomFoodChainTieBreaker(new FakeRandomService { IntRangeOffset = randomOffset });

            tieBreaker.IsFirstWinner(Snake(), Snake()).Should().Be(isFirstWinner);
        }

        private FoodChainConfig Frog() => new FoodChainConfig(_frogTag, FoodChainRole.Prey);

        private FoodChainConfig Snake() => new FoodChainConfig(_snakeTag, FoodChainRole.Predator, _frogTag, _snakeTag);

        private FoodChainTag CreateTag()
        {
            var tag = ScriptableObject.CreateInstance<FoodChainTag>();
            _tags.Add(tag);
            return tag;
        }
    }
}
