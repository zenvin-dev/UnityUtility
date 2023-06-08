using System;
using UnityEngine;

namespace Zenvin.Util {
	[Serializable]
	public struct Point {

		[SerializeField] private bool useObj;
		[SerializeField] private Transform obj;
		[SerializeField] private Vector3 pos;

		public Vector3 Position {
			get {
				if (!useObj) {
					return pos;
				}
				if (obj == null) {
					return Vector3.zero;
				}
				return obj.position;
			}
		}

		public float X => Position.x;
		public float Y => Position.y;
		public float Z => Position.z;

		public bool Valid => !useObj || obj != null;


		public static implicit operator Point (Vector3 position) {
			Point p = new Point ();
			p.useObj = false;
			p.pos = position;
			return p;
		}

		public static implicit operator Point (Transform transform) {
			Point p = new Point ();
			p.useObj = true;
			p.obj = transform;
			return p;
		}

		public static implicit operator Vector3 (Point point) {
			return point.Position;
		}

	}
}