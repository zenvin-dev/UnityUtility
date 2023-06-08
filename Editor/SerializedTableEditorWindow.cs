using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zenvin.EditorUtil;
using Zenvin.EditorUtil.Table;

namespace Zenvin.Util {
	public class SerializedTableEditorWindow : EditorWindow, ITableEditorCallbacksExtended {

		private static readonly HashSet<object> rowKeys = new HashSet<object> ();
		private static readonly HashSet<object> colKeys = new HashSet<object> ();
		private readonly Rect[] headerButtonPositions = new Rect[2];

		private TableEditor editor;
		private SerializedProperty target;
		private SerializedTable table;


		int ITableEditorCallbacks.ColumnCount => table.ColumnCount;
		int ITableEditorCallbacks.RowCount => table.RowCount;


		internal static void Open (SerializedProperty prop, GUIContent label = null) {
			if (prop == null) {
				return;
			}
			var table = prop.GetTarget<SerializedTable> ();
			if (table == null) {
				return;
			}

			var win = CreateWindow<SerializedTableEditorWindow> ();

			win.target = prop;
			win.table = table;

			win.titleContent = label == null ? new GUIContent (prop.name) : new GUIContent (label);
			win.minSize = new Vector2 (500f, 350f);
			win.Show ();
		}


		private void OnGUI () {
			if (target == null || target.serializedObject == null) {
				target = null;
				Close ();
				return;
			}
			if (editor == null) {
				editor = new TableEditor (this);
			}

			rowKeys.Clear ();
			colKeys.Clear ();
			var repaint = editor.DrawTable (position);
			if (!repaint.HasValue) {
				return;
			}
			if (repaint.HasValue && repaint.Value) {
				Repaint ();
			}

			target.serializedObject.ApplyModifiedProperties ();
		}

		private void GetHeaderControlRects (Rect cell, out Rect button, out Rect control) {
			control = cell
				.CenteredByHeight (EditorGUIUtility.singleLineHeight)
				.Inset (RectTransform.Edge.Left, EditorGUIUtility.singleLineHeight, out button)
				.Inset (EditorGUIUtility.standardVerticalSpacing, 0f, 0f, 0f);
		}

		private bool TryGetCellProperty (Vector2Int cell, out SerializedProperty prop) {
			prop = null;

			if (cell.x == -1) {
				if (target.FindPropertyRelative ("rows").TryGetArrayElementAtIndex (cell.y, out SerializedProperty element)) {
					prop = element.FindPropertyRelative ("Key");
					return true;
				}
				return false;
			}
			if (cell.y == -1) {
				return target.FindPropertyRelative ("columns").TryGetArrayElementAtIndex (cell.x, out prop);
			}

			if (target.FindPropertyRelative ("rows").TryGetArrayElementAtIndex (cell.y, out SerializedProperty row)) {
				var valuesProp = row.FindPropertyRelative ("Values");
				if (valuesProp.TryGetArrayElementAtIndex (cell.x, out SerializedProperty column)) {
					prop = column;
					return true;
				}
			}
			return false;
		}

		bool ITableEditorCallbacks.HasCellError (Vector2Int cell) {
			if (cell.x == -1 && cell.y >= 0) {
				if (TryGetCellProperty (cell, out SerializedProperty prop) && !rowKeys.Add (prop.GetRawValue ())) {
					return true;
				}
			}
			if (cell.y == -1 && cell.x >= 0) {
				if (TryGetCellProperty (cell, out SerializedProperty prop) && !colKeys.Add (prop.GetRawValue ())) {
					return true;
				}
			}
			return false;
		}

		void ITableEditorCallbacks.OnDrawCell (Vector2Int cell, Rect position) {
			if (TryGetCellProperty (cell, out SerializedProperty prop)) {
				position = position.CenteredByHeight (EditorGUIUtility.singleLineHeight);
				EditorGUI.PropertyField (position, prop, GUIContent.none);
			}
		}

		void ITableEditorCallbacks.OnDrawColumnHeader (int columnIndex, Rect position) {
			EditorGUI.BeginDisabledGroup (Application.isPlaying);
			if (target.FindPropertyRelative ("columns").TryGetArrayElementAtIndex (columnIndex, out SerializedProperty element)) {
				GetHeaderControlRects (position, out Rect btn, out position);
				position = position.CenteredByHeight (EditorGUIUtility.singleLineHeight);
				EditorGUI.PropertyField (position, element, GUIContent.none);
				if (GUI.Button (btn, "X")) {
					if (table.TryRemoveColumn (columnIndex)) {
						EditorUtility.SetDirty (target.serializedObject.targetObject);
						target.serializedObject.ApplyModifiedProperties ();
						Repaint ();
					}
				}
			}
			EditorGUI.EndDisabledGroup ();
		}

		void ITableEditorCallbacks.OnDrawRowHeader (int rowIndex, Rect position) {
			EditorGUI.BeginDisabledGroup (Application.isPlaying);
			if (target.FindPropertyRelative ("rows").TryGetArrayElementAtIndex (rowIndex, out SerializedProperty element)) {
				GetHeaderControlRects (position, out Rect btn, out position);
				EditorGUI.PropertyField (position, element.FindPropertyRelative ("Key"), GUIContent.none);
				if (GUI.Button (btn, "X")) {
					if (table.TryRemoveRow (rowIndex)) {
						EditorUtility.SetDirty (target.serializedObject.targetObject);
						target.serializedObject.ApplyModifiedProperties ();
						Repaint ();
					}
				}
			}
			EditorGUI.EndDisabledGroup ();
		}

		void ITableEditorCallbacksExtended.OnDrawCorner (Rect position) {
			EditorGUI.BeginDisabledGroup (Application.isPlaying);
			position.SplitNonAlloc (RectTransform.Axis.Horizontal, headerButtonPositions);
			if (GUI.Button (headerButtonPositions[0].Inset (6, 3, 6, 6), "Add Row")) {
				if (table.TryAddRow ()) {
					EditorUtility.SetDirty (target.serializedObject.targetObject);
					target.serializedObject.ApplyModifiedProperties ();
					Repaint ();
				}
			}
			if (GUI.Button (headerButtonPositions[1].Inset (3, 6, 6, 6), "Add Column")) {
				if (table.TryAddColumn ()) {
					EditorUtility.SetDirty (target.serializedObject.targetObject);
					target.serializedObject.ApplyModifiedProperties ();
					Repaint ();
				}
			}
			EditorGUI.EndDisabledGroup ();
		}

	}
}