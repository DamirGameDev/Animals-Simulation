using FluentAssertions;
using Modules.AnimalModule.Runtime.Scripts.Movement;
using NUnit.Framework;
using UnityEngine;

namespace Modules.AnimalModule.Tests
{
    public class MovementModelTests
    {
        [Test]
        public void SetDirection_KeepsDirectionOnGroundAndNormalized()
        {
            var model = new MovementModel(new Vector3(3, 7, 4));

            model.Direction.y.Should().Be(0);
            model.Direction.magnitude.Should().BeApproximately(1, 0.0001f);
            model.Direction.x.Should().BeApproximately(0.6f, 0.0001f);
            model.Direction.z.Should().BeApproximately(0.8f, 0.0001f);
        }

        [Test]
        public void SetDirection_WithZeroVector_KeepsPreviousDirection()
        {
            var model = new MovementModel(Vector3.right);

            model.SetDirection(Vector3.up);

            model.Direction.Should().Be(Vector3.right);
        }
    }
}
