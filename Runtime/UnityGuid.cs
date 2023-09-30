using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Zenvin.Util {
	[Serializable]
	public struct UnityGuid : IEquatable<UnityGuid>, IEquatable<Guid> {

		[SerializeField, HideInInspector] private byte a;   // 1
		[SerializeField, HideInInspector] private byte b;   // 2
		[SerializeField, HideInInspector] private byte c;   // 3
		[SerializeField, HideInInspector] private byte d;   // 4

		[SerializeField, HideInInspector] private byte e;   // 5
		[SerializeField, HideInInspector] private byte f;   // 6
		[SerializeField, HideInInspector] private byte g;   // 7
		[SerializeField, HideInInspector] private byte h;   // 8

		[SerializeField, HideInInspector] private byte i;   // 9
		[SerializeField, HideInInspector] private byte j;   // 10
		[SerializeField, HideInInspector] private byte k;   // 11
		[SerializeField, HideInInspector] private byte l;   // 12

		[SerializeField, HideInInspector] private byte m;   // 13
		[SerializeField, HideInInspector] private byte n;   // 14
		[SerializeField, HideInInspector] private byte o;   // 15
		[SerializeField, HideInInspector] private byte p;   // 16


		public static UnityGuid Empty => new UnityGuid ();
		public bool IsEmpty => (a + b + c + d + e + f + g + h + i + j + k + l + m + n + o + p) == 0;


		public UnityGuid (UnityGuid guid) {
			a = guid.a;
			b = guid.b;
			c = guid.c;
			d = guid.d;

			e = guid.e;
			f = guid.f;
			g = guid.g;
			h = guid.h;

			i = guid.i;
			j = guid.j;
			k = guid.k;
			l = guid.l;

			m = guid.m;
			n = guid.n;
			o = guid.o;
			p = guid.p;
		}


		public void GenerateID (bool force = false) {
			if (IsEmpty || force) {
				Apply (Guid.NewGuid ().ToByteArray ());
			}
		}

		public void Write (BinaryWriter writer) {
			if (writer == null) {
				throw new ArgumentNullException (nameof (writer));
			}

			writer.Write (a);
			writer.Write (b);
			writer.Write (c);
			writer.Write (d);

			writer.Write (e);
			writer.Write (f);
			writer.Write (g);
			writer.Write (h);

			writer.Write (i);
			writer.Write (j);
			writer.Write (k);
			writer.Write (l);

			writer.Write (m);
			writer.Write (n);
			writer.Write (o);
			writer.Write (p);
		}

		public void Read (BinaryReader reader) {
			if (reader == null) {
				throw new ArgumentNullException (nameof (reader));
			}

			a = reader.ReadByte ();
			b = reader.ReadByte ();
			c = reader.ReadByte ();
			d = reader.ReadByte ();

			e = reader.ReadByte ();
			f = reader.ReadByte ();
			g = reader.ReadByte ();
			h = reader.ReadByte ();

			i = reader.ReadByte ();
			j = reader.ReadByte ();
			k = reader.ReadByte ();
			l = reader.ReadByte ();

			m = reader.ReadByte ();
			n = reader.ReadByte ();
			o = reader.ReadByte ();
			p = reader.ReadByte ();
		}

		public bool Equals (UnityGuid other) {
			if (IsEmpty && other.IsEmpty) {
				return true;
			}
			return a == other.a &&
				   b == other.b &&
				   c == other.c &&
				   d == other.d &&

				   e == other.e &&
				   f == other.f &&
				   g == other.g &&
				   h == other.h &&

				   i == other.i &&
				   j == other.j &&
				   k == other.k &&
				   l == other.l &&

				   m == other.m &&
				   n == other.n &&
				   o == other.o &&
				   p == other.p;
		}

		public bool Equals (Guid other) {
			var bytes = other.ToByteArray ();
			return bytes != null &&
				   bytes.Length == 16 &&
				   a == bytes[00] &&
				   b == bytes[01] &&
				   c == bytes[02] &&
				   d == bytes[03] &&

				   e == bytes[04] &&
				   f == bytes[05] &&
				   g == bytes[06] &&
				   h == bytes[07] &&

				   i == bytes[08] &&
				   j == bytes[09] &&
				   k == bytes[10] &&
				   l == bytes[11] &&

				   m == bytes[12] &&
				   n == bytes[13] &&
				   o == bytes[14] &&
				   p == bytes[15];
		}

		public byte[] ToByteArray () {
			return new byte[] {
				a,
				b,
				c,
				d,
				e,
				f,
				g,
				h,
				i,
				j,
				k,
				l,
				m,
				n,
				o,
				p
			};
		}

		public override string ToString () {
			if (IsEmpty) {
				return "Empty";
			}
			StringBuilder sb = new StringBuilder ();

			sb.AppendFormat ("{0:x2}", a);
			sb.AppendFormat ("{0:x2}", b);
			sb.AppendFormat ("{0:x2}", c);
			sb.AppendFormat ("{0:x2}", d);
			sb.Append ("-");
			sb.AppendFormat ("{0:x2}", e);
			sb.AppendFormat ("{0:x2}", f);
			sb.Append ("-");
			sb.AppendFormat ("{0:x2}", g);
			sb.AppendFormat ("{0:x2}", h);
			sb.Append ("-");
			sb.AppendFormat ("{0:x2}", i);
			sb.AppendFormat ("{0:x2}", j);
			sb.Append ("-");
			sb.AppendFormat ("{0:x2}", k);
			sb.AppendFormat ("{0:x2}", l);
			sb.AppendFormat ("{0:x2}", m);
			sb.AppendFormat ("{0:x2}", n);
			sb.AppendFormat ("{0:x2}", o);
			sb.AppendFormat ("{0:x2}", p);

			return sb.ToString ();
		}


		private void Apply (byte[] bytes) {
			if (bytes == null || bytes.Length != 16) {
				return;
			}
			a = bytes[00];
			b = bytes[01];
			c = bytes[02];
			d = bytes[03];

			e = bytes[04];
			f = bytes[05];
			g = bytes[06];
			h = bytes[07];

			i = bytes[08];
			j = bytes[09];
			k = bytes[10];
			l = bytes[11];

			m = bytes[12];
			n = bytes[13];
			o = bytes[14];
			p = bytes[15];
		}


		public static explicit operator Guid (UnityGuid guid) {
			return new Guid (guid.ToByteArray ());
		}

		public static explicit operator UnityGuid (Guid guid) {
			var uGuid = new UnityGuid ();
			uGuid.Apply (guid.ToByteArray ());
			return uGuid;
		}

	}
}