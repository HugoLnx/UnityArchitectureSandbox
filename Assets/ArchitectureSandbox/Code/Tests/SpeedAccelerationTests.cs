using System.Collections;
using System.Collections.Generic;
using ArchitectureSandbox.Zen2;
using NUnit.Framework;
// using UnityEngine;
// using UnityEngine.TestTools;

public class SpeedAccelerationTests
{
    private const float Precision = 1e-3f;

    [Test]
    public void SpeedAccelerationSimplePasses()
    {
        SpeedAcceleration acceleratedSpeed = new(
            acceleration: 180f,
            deacceleration: 360f,
            maxSpeed: 720f
        );

        acceleratedSpeed.TickAccelerateForward(1f);
        Assert.That(acceleratedSpeed.Speed, Is.EqualTo(180f).Within(Precision));
    }
}
