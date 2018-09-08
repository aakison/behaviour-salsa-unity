using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Salsa {

    public class SpawnOnTimeout : MonoBehaviour {

        [Tooltip("The number of seconds to wait before the gameObject is automatically destroyed.")]
        public float timeout;

        [Tooltip("The object to spawn after the amount of time.")]
        public GameObject prefab;

        internal IEnumerator Start()
        {
            yield return WaitFor.Seconds(timeout);
            Instantiate(prefab, transform.position, transform.rotation);
        }

    }

}
