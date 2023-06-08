using System;
using System.Collections.Generic;
using UnityEngine;

namespace Zenvin.Util {
	public abstract class SerializedTable {
		public abstract int ColumnCount { get; }
		public abstract int RowCount { get; }

		internal abstract bool TryAddColumn ();
		internal abstract bool TryRemoveColumn (int columnIndex);
		internal abstract bool TryAddRow ();
		internal abstract bool TryRemoveRow (int rowIndex);
	}

	[Serializable]
	public sealed class SerializedTable<TRow, TColumn, TValue> : SerializedTable {

		private Dictionary<Key, Vector2Int> values = null;

		[SerializeField, HideInInspector] private List<TColumn> columns = new List<TColumn> ();
		[SerializeField, HideInInspector] private List<TableRow> rows = new List<TableRow> ();


		public sealed override int ColumnCount => columns?.Count ?? 0;
		public sealed override int RowCount => rows?.Count ?? 0;


		private void Initialize () {
			if (values != null) {
				return;
			}

			values = new Dictionary<Key, Vector2Int> ();
			if (rows == null) {
				return;
			}
			for (int y = 0; y < RowCount; y++) {
				var row = rows[y];
				for (int x = 0; x < row.ValueCount; x++) {
					var col = columns[x];
					var key = new Key (row.Key, col);
					if (!values.ContainsKey (key)) {
						values[key] = new Vector2Int (y, x);
					}
				}
			}
		}

		
		public bool SetValue (TRow row, TColumn column, TValue value) {
			if (!TryGetCell (row, column, out Vector2Int cell)) {
				return false;
			}
			return rows[cell.y].SetValue (cell.x, value);
		}

		public TValue GetValue (TRow row, TColumn column, TValue fallback = default) {
			if (TryGetValue (row, column, out TValue value)) {
				return value;
			}
			return fallback;
		}

		public TValue GetValue (Vector2Int cell) {
			return TryGetValue (cell, out TValue value) ? value : default;
		}

		public TColumn GetColumnKey (int index) {
			if (index < 0 || index >= ColumnCount) {
				return default;
			}
			return columns[index];
		}

		public TRow GetRowKey (int index) {
			if (index < 0 || index >= RowCount) {
				return default;
			}
			return rows[index].Key;
		}

		public bool TryGetCell (TRow row, TColumn column, out Vector2Int cell) {
			if (Application.isPlaying) {
				Initialize ();
				return values.TryGetValue (new Key (row, column), out cell);
			}

			cell = Vector2Int.one * -1;
			for (int y = 0; y < RowCount; y++) {
				var rowCell = rows[y];
				for (int x = 0; x < rowCell.ValueCount; x++) {
					var rKey = rows[y].Key;
					var cKey = columns[x];

					if ((rKey == null && row != null) || (rKey != null && row == null)) {
						continue;
					}
					if ((cKey == null && column != null) || (cKey != null && column == null)) {
						continue;
					}
					if (!rKey.Equals (row) || !cKey.Equals (column)) {
						continue;
					}

					cell = new Vector2Int (y, x);
					return true;
				}
			}
			return false;
		}

		public bool TryGetValue (TRow row, TColumn column, out TValue value) {
			value = default;
			return TryGetCell (row, column, out Vector2Int cell) && TryGetValue (cell, out value);
		}

		public bool TryGetValue (Vector2Int cell, out TValue value) {
			value = default;
			if (cell.x < 0 || cell.x >= ColumnCount) {
				return false;
			}
			if (cell.y < 0 || cell.y >= RowCount) {
				return false;
			}
			value = rows[cell.y].Values[cell.x];
			return true;
		}


		internal bool TryAddColumn (TColumn column, TValue @default = default) {
			if (Application.isPlaying || values != null) {
				return false;
			}
			foreach (var row in rows) {
				row.AddColumn (columns.Count - 1, columns.Count, @default);
			}
			columns.Add (column);
			return true;
		}

		internal override bool TryAddColumn () {
			return TryAddColumn (default, default);
		}

		internal override bool TryRemoveColumn (int columnIndex) {
			if (Application.isPlaying || values != null) {
				return false;
			}
			if (columnIndex < 0 || columnIndex >= columns.Count) {
				return false;
			}
			foreach (var row in rows) {
				row.RemoveColumn (columnIndex);
			}
			columns.RemoveAt (columnIndex);
			return true;
		}

		internal bool TryAddRow (TRow row, TValue @default = default) {
			if (Application.isPlaying || values != null) {
				return false;
			}
			rows.Add (new TableRow (ColumnCount));
			return true;
		}

		internal override bool TryRemoveRow (int rowIndex) {
			if (Application.isPlaying || values != null) {
				return false;
			}
			if (rowIndex < 0 || rowIndex >= rows.Count) {
				return false;
			}
			rows.RemoveAt (rowIndex);
			return true;
		}

		internal override bool TryAddRow () {
			return TryAddRow (default, default);
		}


		[Serializable]
		internal class TableRow {
			public TRow Key;
			public List<TValue> Values = new List<TValue> ();

			public int ValueCount => Values.Count;


			internal TableRow (int columns) {
				Values = new List<TValue> (new TValue[columns]);
			}


			internal void AddColumn (int index, int totalSizeBeforeAdd, TValue value) {
				Values.ForceListLength (totalSizeBeforeAdd);
				Values.Insert (index, value);
			}

			internal void RemoveColumn (int index) {
				Values.RemoveAt (index);
			}

			internal bool SetValue (int index, TValue value) {
				if (index < 0 || index >= ValueCount) {
					return false;
				}
				Values[index] = value;
				return true;
			}
		}

		public readonly struct Key {
			public readonly TRow Row;
			public readonly TColumn Column;

			public Key (TRow row, TColumn column) {
				Row = row;
				Column = column;
			}
		}

	}
}