using System;
using System.IO;
using UnityEngine;

namespace Zenvin.Util {
	[Serializable]
	public struct UnityGuid : IEquatable<UnityGuid>, IEquatable<Guid> {

		[SerializeField, HideInInspector] private byte[] id;


		public bool IsEmpty => id == null || id.Length != 16;


		public void GenerateID (bool force = false) {
			if (IsEmpty || force) {
				id = Guid.NewGuid ().ToByteArray ();
			}
		}

		public void Write (BinaryWriter writer) {
			if (writer == null) {
				throw new ArgumentNullException (nameof(writer));
			}
			writer.Write (IsEmpty);
			if (IsEmpty) {
				return;
			}
			for (int i = 0; i < id.Length; i++) {
				writer.Write (id[i]);
			}
		}

		public void Read (BinaryReader reader) {
			if (reader == null) {
				throw new ArgumentNullException (nameof (reader));
			}
			if (!reader.ReadBoolean()) {
				id = null;
				return;
			}
			if (IsEmpty) {
				id = new byte[16];
			}
			for (int i = 0; i < 16; i++) {
				id[i] = reader.ReadByte ();
			}
		}

		public bool Equals (UnityGuid other) {
			return CompareByteIDs (id, other.id);
		}

		public bool Equals (Guid other) {
			return CompareByteIDs (id, other.ToByteArray ());
		}


		private bool CompareByteIDs (byte[] a, byte[] b) {
			if (a == null && b == null) {
				return true;
			}
			if (a?.Length != b?.Length) {
				return false;
			}
			for (int i = 0; i < a.Length; i++) {
				if (a[i] != b[i]) {
					return false;
				}
			}
			return true;
		}


		public static explicit operator Guid (UnityGuid guid) {
			return new Guid (guid.id);
		}

		public static explicit operator UnityGuid (Guid guid) {
			var uGuid = new UnityGuid ();
			uGuid.id = guid.ToByteArray ();
			return uGuid;
		}

	}
}