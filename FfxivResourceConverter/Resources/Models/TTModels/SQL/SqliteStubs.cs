// © XIV-Tools.
// Licensed under the MIT license.

#pragma warning disable

namespace FfxivResourceConverter.Resources.Models.TTModels.SQL
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	public class SqliteDataReader
	{
		public int FieldCount => throw new NotImplementedException();
		public bool IsClosed => throw new NotImplementedException();
		public string GetName(int idx) => throw new NotImplementedException();
		public int this[int index] => throw new NotImplementedException();
		public bool Read() => throw new NotImplementedException();
		public string GetString(int idx) => throw new NotImplementedException();
		public int GetInt32(int idx) => throw new NotImplementedException();
		public float GetFloat(int idx) => throw new NotImplementedException();
		public byte GetByte(int idx) => throw new NotImplementedException();
		public bool GetBoolean(int idx) => throw new NotImplementedException();
		public void Close() => throw new NotImplementedException();
	}

	public class SqliteTransaction : IDisposable
	{
		public void Dispose() => throw new NotImplementedException();
		public void Commit() => throw new NotImplementedException();
	}

	public class SqliteConnection : IDisposable
	{
		public SqliteConnection(string connectionstring) => throw new NotImplementedException();
		public void Open() => throw new NotImplementedException();
		public void Dispose() => throw new NotImplementedException();
		public SqliteTransaction BeginTransaction() => throw new NotImplementedException();
	}

	public class SqliteCommand : IDisposable
	{
		public SqliteCommand(string str, SqliteConnection db) => throw new NotImplementedException();
		public Params Parameters => throw new NotImplementedException();
		public void Dispose() => throw new NotImplementedException();
		public long ExecuteScalar() => throw new NotImplementedException();
		public SqliteDataReader ExecuteReader() => throw new NotImplementedException();

		public class Params
		{
			public void AddWithValue(string key, object value) => throw new NotImplementedException();
		}
	}
}
