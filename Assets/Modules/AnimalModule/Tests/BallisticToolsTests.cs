using FluentAssertions;
using Modules.SharedModule.Runtime.Scripts.Tools;
using NUnit.Framework;
using UnityEngine;

namespace Modules.AnimalModule.Tests
{
    public class BallisticToolsTests
    {
        private const float Gravity = 9.81f;
        private const float Precision = 0.001f;

        [TestCase(3, 45)]
        [TestCase(3, 30)]
        [TestCase(5, 60)]
        public void LaunchVelocity_LandsAtGivenDistance(float distance, float angle)
        {
            var velocity = BallisticTools.GetLaunchVelocity(Vector3.forward, distance, angle, Gravity);

            var flightTime = 2 * velocity.y / Gravity;
            var horizontalSpeed = new Vector3(velocity.x, 0, velocity.z).magnitude;

            (horizontalSpeed * flightTime).Should().BeApproximately(distance, Precision);
        }

        [Test]
        public void LaunchVelocity_FollowsDirectionOnGround()
        {
            var velocity = BallisticTools.GetLaunchVelocity(new Vector3(2, 5, 0), 3, 45, Gravity);

            velocity.z.Should().BeApproximately(0, Precision);
            velocity.x.Should().BePositive();
            velocity.x.Should().BeApproximately(velocity.y, Precision, "a 45 degree launch splits speed equally");
        }
    }
}
