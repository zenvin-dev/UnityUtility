using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.LowLevel;

namespace Zenvin.Util {
	/// <summary>
	/// Utilities to allow interfacing with Unity's low-level <see cref="PlayerLoopSystem"/>, 
	/// based on an <see href="https://github.com/adammyhre/Unity-Improved-Timers/blob/master/Runtime/PlayerLoopUtils.cs">implementation by Adam Mhyre</see>.
	/// </summary>
	public static class PlayerLoopUtility {
		/// <summary>
		/// Inserts a new <see cref="PlayerLoopSystem"/> into the current PlayerLoop.
		/// </summary>
		public static bool InsertSystem<T> (in PlayerLoopSystem systemToInsert, int index) {
			var current = PlayerLoop.GetCurrentPlayerLoop ();
			return InsertSystem<T> (ref current, in systemToInsert, index);
		}

		/// <summary>
		/// Inserts a new <see cref="PlayerLoopSystem"/> into an arbitrary existing one.<br></br>
		/// </summary>
		/// <remarks>
		/// Usually, the target loop system would be acquired using <see cref="PlayerLoop.GetCurrentPlayerLoop"/>.<br></br>
		/// The <see cref="InsertSystem{T}(in PlayerLoopSystem, int)"/> method provides a shortcut for this.
		/// </remarks>
		public static bool InsertSystem<T> (ref PlayerLoopSystem loop, in PlayerLoopSystem systemToInsert, int index) {
			if (loop.type != typeof (T))
				return HandleSubSystemLoop<T> (ref loop, systemToInsert, index);

			var playerLoopSystemList = new List<PlayerLoopSystem> ();
			if (loop.subSystemList != null)
				playerLoopSystemList.AddRange (loop.subSystemList);

			playerLoopSystemList.Insert (index, systemToInsert);
			loop.subSystemList = playerLoopSystemList.ToArray ();
			return true;
		}

		/// <summary>
		/// Removes an existing <see cref="PlayerLoopSystem"/> from another existing one.
		/// </summary>
		/// /// <remarks>
		/// Usually, the target loop system would be acquired using <see cref="PlayerLoop.GetCurrentPlayerLoop"/>.<br></br>
		/// The <see cref="InsertSystem{T}(in PlayerLoopSystem, int)"/> method provides a shortcut for this.
		/// </remarks>
		public static void RemoveSystem<T> (ref PlayerLoopSystem loop, in PlayerLoopSystem systemToRemove) {
			if (loop.subSystemList == null)
				return;

			var playerLoopSystemList = new List<PlayerLoopSystem> (loop.subSystemList);
			for (int i = 0; i < playerLoopSystemList.Count; ++i) {
				if (playerLoopSystemList[i].type == systemToRemove.type && playerLoopSystemList[i].updateDelegate == systemToRemove.updateDelegate) {
					playerLoopSystemList.RemoveAt (i);
					loop.subSystemList = playerLoopSystemList.ToArray ();
				}
			}

			HandleSubSystemLoopForRemoval<T> (ref loop, systemToRemove);
		}

		/// <summary>
		/// Writes all <see cref="PlayerLoopSystem"/>s that exist in the given one to the console.<br></br>
		/// If <see langword="null"/> is passed, Unity's current loop is used.
		/// </summary>
		public static void LogPlayerLoop (PlayerLoopSystem? loop = null) {
			StringBuilder sb = new StringBuilder ();

			if (!loop.HasValue) {
				loop = PlayerLoop.GetCurrentPlayerLoop ();
				sb.AppendLine ("Unity Player Loop");
			}

			foreach (PlayerLoopSystem subSystem in loop.Value.subSystemList) {
				PrintSubsystem (subSystem, sb, 0);
			}
			Debug.Log (sb.ToString ());
		}


		private static void HandleSubSystemLoopForRemoval<T> (ref PlayerLoopSystem loop, PlayerLoopSystem systemToRemove) {
			if (loop.subSystemList == null)
				return;

			for (int i = 0; i < loop.subSystemList.Length; ++i) {
				RemoveSystem<T> (ref loop.subSystemList[i], systemToRemove);
			}
		}

		private static bool HandleSubSystemLoop<T> (ref PlayerLoopSystem loop, in PlayerLoopSystem systemToInsert, int index) {
			if (loop.subSystemList == null)
				return false;

			for (int i = 0; i < loop.subSystemList.Length; ++i) {
				if (!InsertSystem<T> (ref loop.subSystemList[i], in systemToInsert, index))
					continue;
				return true;
			}

			return false;
		}


		private static void PrintSubsystem (PlayerLoopSystem system, StringBuilder sb, int level) {
			sb.Append (' ', level * 2).AppendLine (system.type.ToString ());
			if (system.subSystemList == null || system.subSystemList.Length == 0)
				return;

			foreach (PlayerLoopSystem subSystem in system.subSystemList) {
				PrintSubsystem (subSystem, sb, level + 1);
			}
		}
	}
}
