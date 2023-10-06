using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Zenvin.Util {
	[Serializable]
	public struct UnityGuid : IEquatable<UnityGuid>, IEquatable<Guid> {

		[SerializeField] private byte[] id;


		public bool IsEmpty => id == null || id.Length != 16;


		public UnityGuid (Guid guid) {
			id = guid.ToByteArray ();
		}

		public UnityGuid (byte[] id) {
			this.id = id;
		}


		public void GenerateID (bool force = false) {
			if (IsEmpty || force) {
				id = Guid.NewGuid ().ToByteArray ();
			}
		}

		public void Write (BinaryWriter writer) {
			if (writer == null) {
				throw new ArgumentNullException (nameof (writer));
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
			if (!reader.ReadBoolean ()) {
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
			if (IsEmpty && other.IsEmpty) {
				return true;
			}
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


		public override string ToString () {
			if (IsEmpty) {
				return "Empty";
			}
			StringBuilder sb = new StringBuilder ();

			sb.AppendFormat ("{0:x2}", id[03]);
			sb.AppendFormat ("{0:x2}", id[02]);
			sb.AppendFormat ("{0:x2}", id[01]);
			sb.AppendFormat ("{0:x2}", id[00]);
			sb.Append ("-");
			sb.AppendFormat ("{0:x2}", id[04]);
			sb.AppendFormat ("{0:x2}", id[05]);
			sb.Append ("-");
			sb.AppendFormat ("{0:x2}", id[06]);
			sb.AppendFormat ("{0:x2}", id[07]);
			sb.Append ("-");
			sb.AppendFormat ("{0:x2}", id[08]);
			sb.AppendFormat ("{0:x2}", id[09]);
			sb.Append ("-");
			sb.AppendFormat ("{0:x2}", id[15]);
			sb.AppendFormat ("{0:x2}", id[14]);
			sb.AppendFormat ("{0:x2}", id[13]);
			sb.AppendFormat ("{0:x2}", id[12]);
			sb.AppendFormat ("{0:x2}", id[11]);
			sb.AppendFormat ("{0:x2}", id[10]);

			return sb.ToString ();
		}


		public static UnityGuid NewGuid () {
			return new UnityGuid (Guid.NewGuid ());
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