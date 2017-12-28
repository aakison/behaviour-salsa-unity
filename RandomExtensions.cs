using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Relentless {
    public static class RandomExtensions {

        public static float Next(this Random random, float minimum, float maximum) {
            return (float)(random.NextDouble() * (maximum - minimum) + minimum);
        }

        public static float NextFloat(this Random random) {
            return (float)random.NextDouble();
        }

        public static UnityEngine.Vector3 NextVector3(this Random random, UnityEngine.Vector3 minimum, UnityEngine.Vector3 maximum) {
            var x = random.Next(minimum.x, maximum.x);
            var y = random.Next(minimum.y, maximum.y);
            var z = random.Next(minimum.z, maximum.z);
            return new UnityEngine.Vector3(x, y, z);
        }

        /// <summary>
        /// From: http://math.stackexchange.com/questions/114135/how-can-i-pick-a-random-point-on-the-surface-of-a-sphere-with-equal-distribution
        /// The area element on the sphere depends on latitude. If u, v are uniformly distributed in [0,1] then
        ///     ϕ = 2πu
        ///     θ = cos−1(2v−1)
        /// are uniformly distributed over the sphere. Here θ represents latitude and ϕ longitude, both in radians.
        /// For further details, more methods and some Mathematica code, see Sphere Point Picking at MathWorld.
        /// </summary>
        public static UnityEngine.Vector3 NextOnUnitSphere(this Random random) {
            var u = random.NextFloat();
            var v = random.NextFloat();
            var longitude = UnityEngine.Mathf.Rad2Deg * 2 * UnityEngine.Mathf.PI * u;
            var latitude = UnityEngine.Mathf.Rad2Deg * UnityEngine.Mathf.Acos(2 * v - 1);
            return PolarToCartesian(longitude, latitude);
        }

        private static UnityEngine.Vector3 PolarToCartesian(float longitude, float latitude) {
            var x = UnityEngine.Mathf.Sin(latitude) * UnityEngine.Mathf.Cos(longitude);
            var y = UnityEngine.Mathf.Sin(latitude) * UnityEngine.Mathf.Sin(longitude);
            var z = UnityEngine.Mathf.Cos(latitude);
            return new UnityEngine.Vector3(x, y, z);
        }

    }
}
