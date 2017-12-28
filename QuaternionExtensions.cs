using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Relentless {

    public static class QuaternionExtensions {
        public static Quaternion Normalized(this Quaternion source) {
            var euler = source.eulerAngles;
            var x = (euler.x + 36000) % 360;
            var y = (euler.y + 36000) % 360;
            var z = (euler.z + 36000) % 360;
            return Quaternion.Euler(x, y, z);
        }

    }

}
