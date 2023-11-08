using UnityEngine;

namespace Zenvin.Util {
	[System.Serializable]
	public class Frustum {

		private Vector3 position;
		private Vector3 forward;
		private Vector3 up;
		private Vector3 right;
		private float fieldOfView;
		private float ratio;
		private float nearClipPlane;
		private float farClipPlane;

		private Vector3[] innerFrustum;
		private Vector3[] outerFrustum;

		private Plane upPlane;
		private Plane downPlane;
		private Plane rightPlane;
		private Plane leftPlane;
		private Plane forwardPlane;
		private Plane backPlane;



		private float FovRad => fieldOfView * Mathf.Deg2Rad;
		private float FovRatio => Mathf.Abs (ratio);

		public float FieldOfView {
			set {
				fieldOfView = Mathf.Clamp (value, 1f, 179f);
				UpdateFrustumCorners ();
			}
			get {
				return fieldOfView;
			}
		}

		public float AspectRatio {
			set {
				ratio = Mathf.Max (Mathf.Abs (value), 0.1f);
				UpdateFrustumCorners ();
			}
			get {
				return ratio;
			}
		}

		public float NearClipPlane {
			set {
				nearClipPlane = Mathf.Max (value, 0f);
				UpdateFrustumCorners ();
			}
			get {
				return nearClipPlane;
			}
		}

		public float FarClipPlane {
			set {
				farClipPlane = Mathf.Max (value, nearClipPlane + 0.1f);
				UpdateFrustumCorners ();
			}
			get {
				return farClipPlane;
			}
		}


		public Frustum () { }

		public Frustum (Camera cam) {
			Update (cam);
		}

		public Frustum (Transform transform, float fieldOfView, float ratio, float nearClipPlane, float farClipPlane) {
			UpdateFrustumWithValues (
				transform.position,
				transform.forward,
				transform.up,
				transform.right,
				fieldOfView,
				ratio,
				nearClipPlane,
				farClipPlane
			);
		}

		public Frustum (Vector3 position, Vector3 forward, Vector3 up, Vector3 right, float fieldOfView, float ratio, float nearClipPlane, float farClipPlane) {
			this.position = position;
			this.forward = forward;
			this.up = up;
			this.right = right;

			this.fieldOfView = Mathf.Clamp (fieldOfView, 1f, 179f);
			this.ratio = ratio;

			this.nearClipPlane = Mathf.Max (nearClipPlane, 0f);
			this.farClipPlane = Mathf.Max (farClipPlane, nearClipPlane + 0.1f);

			UpdateFrustumCorners ();
		}


		/// <summary>
		/// Updates the <see cref="Frustum"/>'s values to those of the given <see cref="Camera"/>.<br></br>
		/// If no camera is given, the frustum will not change.
		/// </summary>
		/// <param name="camera"> The camera to source new frustum values from. </param>
		public void Update (Camera camera) {
			if (camera == null) {
				return;
			}

			var transform = camera.transform;
			UpdateFrustumWithValues (
				transform.position,
				transform.forward,
				transform.up,
				transform.right,
				camera.fieldOfView,
				camera.aspect,
				camera.nearClipPlane,
				camera.farClipPlane
			);
		}

		/// <summary>
		/// Sets the Frustum's position and orientation values to those of the given <see cref="Transform"/> and updates the frustum corners.<br></br>
		/// If no transform is given, the frustum will not change.
		/// </summary>
		public void Update (Transform value) {
			if (value == null) {
				return;
			}

			position = value.position;
			forward = value.forward;
			up = value.up;
			right = value.right;

			UpdateFrustumCorners ();
		}

		/// <summary>
		/// Checks if a given <paramref name="point"/> is within the view frustum.
		/// </summary>
		public bool PointInFrustum (Vector3 point) {
			return !upPlane.GetSide (point) && !downPlane.GetSide (point) && !rightPlane.GetSide (point) &&
				   !leftPlane.GetSide (point) && !forwardPlane.GetSide (point) && !backPlane.GetSide (point);
		}

		/// <summary>
		/// Checks if given <paramref name="bounds"/> are within the frustum. <b>UNTESTED</b>
		/// </summary>
		public bool BoundsInFrustum (Bounds bounds) {
			return GeometryUtility.TestPlanesAABB (new Plane[] { leftPlane, rightPlane, downPlane, upPlane, backPlane, forwardPlane }, bounds);
		}

		/// <summary>
		/// Uses a <see cref="Physics.Linecast"/> to check if a given <paramref name="point"/> is obstructed.
		/// </summary>
		public bool HasLineOfSight (Vector3 point, LayerMask? layers = null, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore) {
			return !Physics.Linecast (position + forward * nearClipPlane, point, layers ?? Physics.AllLayers, triggerInteraction);
		}

		/// <summary>
		/// Checks if a given <paramref name="point"/> is both within the frustum and unobstructed.
		/// </summary>
		public bool PointVisible (Vector3 point, LayerMask? layers = null, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore) {
			if (!PointInFrustum (point)) {
				return false;
			}
			return HasLineOfSight (point, layers, triggerInteraction);
		}


		private void UpdateFrustumWithValues (Vector3 position, Vector3 forward, Vector3 up, Vector3 right, float fieldOfView, float ratio, float nearClipPlane, float farClipPlane) {
			this.position = position;
			this.forward = forward;
			this.up = up;
			this.right = right;

			this.fieldOfView = Mathf.Clamp (fieldOfView, 1f, 179f);
			this.ratio = ratio;

			this.nearClipPlane = Mathf.Max (nearClipPlane, 0f);
			this.farClipPlane = Mathf.Max (farClipPlane, nearClipPlane + 0.1f);

			UpdateFrustumCorners ();
		}

		private void UpdateFrustumCorners () {
			innerFrustum = GetFrustumCornersAtDistance (nearClipPlane);
			outerFrustum = GetFrustumCornersAtDistance (farClipPlane);

			UpdateFrustumPlanes ();
		}

		private void UpdateFrustumPlanes () {
			Vector3 upNormal = Vector3.Cross (innerFrustum[1] - innerFrustum[0], outerFrustum[0] - innerFrustum[0]).normalized;
			Vector3 upCenter = Vector3.Lerp (Vector3.Lerp (innerFrustum[0], innerFrustum[1], 0.5f), Vector3.Lerp (outerFrustum[0], outerFrustum[1], 0.5f), 0.5f);
			upPlane = new Plane (upNormal, upCenter);

			Vector3 downNormal = Vector3.Cross (innerFrustum[3] - innerFrustum[2], outerFrustum[3] - innerFrustum[3]).normalized;
			Vector3 downCenter = Vector3.Lerp (Vector3.Lerp (innerFrustum[3], innerFrustum[2], 0.5f), Vector3.Lerp (outerFrustum[3], outerFrustum[2], 0.5f), 0.5f);
			downPlane = new Plane (downNormal, downCenter);

			Vector3 rightNormal = Vector3.Cross (outerFrustum[0] - innerFrustum[0], innerFrustum[3] - innerFrustum[0]).normalized;
			Vector3 rightCenter = Vector3.Lerp (Vector3.Lerp (innerFrustum[0], innerFrustum[3], 0.5f), Vector3.Lerp (outerFrustum[0], outerFrustum[3], 0.5f), 0.5f);
			rightPlane = new Plane (rightNormal, rightCenter);

			Vector3 leftNormal = Vector3.Cross (innerFrustum[2] - innerFrustum[1], outerFrustum[1] - innerFrustum[1]).normalized;
			Vector3 leftCenter = Vector3.Lerp (Vector3.Lerp (innerFrustum[1], innerFrustum[2], 0.5f), Vector3.Lerp (outerFrustum[1], outerFrustum[2], 0.5f), 0.5f);
			leftPlane = new Plane (leftNormal, leftCenter);

			Vector3 backNormal = Vector3.Cross (innerFrustum[3] - innerFrustum[0], innerFrustum[1] - innerFrustum[0]).normalized;
			Vector3 backCenter = Vector3.Lerp (innerFrustum[0], innerFrustum[2], 0.5f);
			backPlane = new Plane (backNormal, backCenter);

			Vector3 forwardNormal = Vector3.Cross (outerFrustum[1] - outerFrustum[0], outerFrustum[3] - outerFrustum[0]).normalized;
			Vector3 forwardCenter = Vector3.Lerp (outerFrustum[0], outerFrustum[2], 0.5f);
			forwardPlane = new Plane (forwardNormal, forwardCenter);
		}

		private void GetFrustumValues (float dist, out float height, out float width) {
			height = 2 * Mathf.Tan (FovRad / 2f) * dist;
			width = height * FovRatio;
		}

		private Vector3[] GetFrustumCornersAtDistance (float dist) {

			GetFrustumValues (dist, out float height, out float width);
			Vector3 pos = position;
			Vector3 fwd = forward;

			Vector3[] ret = new Vector3[4];

			ret[0] = pos + fwd * dist + up * (height * 0.5f) + right * (width * 0.5f);    //top right
			ret[1] = pos + fwd * dist + up * (height * 0.5f) - right * (width * 0.5f);    //top left
			ret[2] = pos + fwd * dist - up * (height * 0.5f) - right * (width * 0.5f);    //bottom left
			ret[3] = pos + fwd * dist - up * (height * 0.5f) + right * (width * 0.5f);    //bottom right

			return ret;
		}



		public void DrawFrustum () {
#if UNITY_EDITOR
			if (innerFrustum == null || outerFrustum == null || innerFrustum.Length < 4 || outerFrustum.Length < 4) {
				return;
			}

			Gizmos.DrawLine (innerFrustum[0], innerFrustum[1]);
			Gizmos.DrawLine (innerFrustum[1], innerFrustum[2]);
			Gizmos.DrawLine (innerFrustum[2], innerFrustum[3]);
			Gizmos.DrawLine (innerFrustum[3], innerFrustum[0]);

			Gizmos.DrawLine (outerFrustum[0], outerFrustum[1]);
			Gizmos.DrawLine (outerFrustum[1], outerFrustum[2]);
			Gizmos.DrawLine (outerFrustum[2], outerFrustum[3]);
			Gizmos.DrawLine (outerFrustum[3], outerFrustum[0]);

			Gizmos.DrawLine (innerFrustum[0], outerFrustum[0]);
			Gizmos.DrawLine (innerFrustum[1], outerFrustum[1]);
			Gizmos.DrawLine (innerFrustum[2], outerFrustum[2]);
			Gizmos.DrawLine (innerFrustum[3], outerFrustum[3]);
#endif
		}

		public void DrawPointCheck (Vector3 point, LayerMask? layers = null, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore) {
#if UNITY_EDITOR
			Gizmos.color = PointVisible (point, layers, triggerInteraction) ? Color.cyan : Color.yellow;
			Gizmos.DrawLine (position + forward * nearClipPlane, point);
#endif
		}

	}

	public static class FrustumUtility {
		public static bool IsRendererVisible (this Camera cam, Renderer renderer, Frustum frustum = null) {
			var f = frustum ?? new Frustum ();
			f.Update (cam);
			return f.BoundsInFrustum (renderer.bounds);
		}
	}
}