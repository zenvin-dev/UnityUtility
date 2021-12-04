using UnityEngine;
using System;
using System.Collections;

namespace Zenvin.BaseClasses {
	/// <summary>
	/// A <see cref="MonoBehaviour"/> that caches its <see cref="Transform"/> reference to increase performance. <br></br>
	/// Also contains implicit conversions to <see cref="Transform"/> and <see cref="GameObject"/>, as well as improved implementations for "Invoke" methods.
	/// </summary>
	public abstract class MonoBehavior : MonoBehaviour {

		private Transform transformReference;

		/// <summary>
		/// More performant replacement for a <see cref="MonoBehaviour"/>'s inherited <see cref="Component.transform"/>.
		/// </summary>
		public new Transform transform { get => transformReference ?? (transformReference = base.transform); }


		public static implicit operator Transform (MonoBehavior mb) {
			return mb.transform;
		}

		public static implicit operator GameObject (MonoBehavior mb) {
			return mb.gameObject;
		}


		[Obsolete ("Use Debug.Log instead.", true)]
		public new void print (object obj) { }

		[Obsolete ("Methods should not be invoked by name.", true)]
		public new void Invoke (string methodName, float delay) { }

		[Obsolete ("Methods should not be invoked by name.", true)]
		public new void InvokeRepeating (string methodName, float delay, float rate) { }

		[Obsolete ("SendMessage might not always yield the desired result. Better use GetComponent and call the target method proper.", true)]
		public new void SendMessage (string methodName) { }
		
		[Obsolete ("SendMessage might not always yield the desired result. Better use GetComponent and call the target method proper.", true)]
		public new void SendMessage (string methodName, object parameter) { }

		[Obsolete ("SendMessage might not always yield the desired result. Better use GetComponent and call the target method proper.", true)]
		public new void SendMessage (string methodName, object parameter, SendMessageOptions options) { }


		/// <summary>
		/// Replacement for <see cref="MonoBehaviour.Invoke(string, float)"/>, which uses a <see cref="Action"/> instead.
		/// </summary>
		public Coroutine Invoke (Action method, float delay) {
			return StartCoroutine (InvokeDelayed (method, delay));
		}

		/// <summary>
		/// Replacement for <see cref="MonoBehaviour.InvokeRepeating(string, float, float)"/>, which uses a <see cref="Action"/> instead.
		/// </summary>
		public Coroutine InvokeRepeating (Action method, float delay, float rate) {
			return StartCoroutine (InvokeRepeated (method, delay, rate));
		}


		private IEnumerator InvokeDelayed (Action callback, float delay) {
			yield return new WaitForSeconds (delay);
			callback?.Invoke ();
		}

		private IEnumerator InvokeRepeated (Action callback, float delay, float rate) {
			yield return new WaitForSeconds (delay);
			WaitForSeconds wfs = new WaitForSeconds (rate);
			while (true) {
				callback?.Invoke ();
				yield return wfs;
			}
		}

	}
}