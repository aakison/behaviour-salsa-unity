using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class WaitFor {

    public static WaitForSeconds Seconds(float seconds) {
        WaitForSeconds ret;
        if(!cachedWaiters.TryGetValue(seconds, out ret)) {
            ret = new WaitForSeconds(seconds);
            cachedWaiters.Add(seconds, ret);
        }
        return ret;
    }

    private static Dictionary<float, WaitForSeconds> cachedWaiters = new Dictionary<float, WaitForSeconds>(new FloatComparer());

    public const object NextFrame = null;

    public static readonly object EndOfFrame = new WaitForEndOfFrame();

    private class FloatComparer : IEqualityComparer<float> {
        public bool Equals(float x, float y) {
            return x == y;
        }

        public int GetHashCode(float obj) {
            return obj.GetHashCode();
        }
    }

}
