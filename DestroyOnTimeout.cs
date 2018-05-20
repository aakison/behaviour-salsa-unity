using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Salsa {

    public class DestroyOnTimeout : MonoBehaviour {

        [Tooltip("The number of seconds to wait before the gameObject is automatically destroyed.")]
        public float timeout;

        private IEnumerator Start() {
            yield return WaitFor.Seconds(timeout);
            Destroy(gameObject);
        }

    }
}
