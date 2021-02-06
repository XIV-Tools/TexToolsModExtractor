// © XIV-Tools.
// Licensed under the MIT license.

// xivModdingFramework
// Copyright © 2018 Rafael Gonzalez - All Rights Reserved
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
namespace FfxivResourceConverter.Resources.Models
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using FfxivResourceConverter.Resources.Models.TTModels.SQL;

	public static class TTModelDb
	{
		/// <summary>
		/// Saves the TTModel to a .DB file for use with external importers/exporters.
		/// </summary>
		public static void ToDb(TTModel model, FileInfo file, bool useAllBones, Action<bool, string> loggingFunction = null)
		{
			if (loggingFunction == null)
			{
				loggingFunction = ModelModifiers.NoOp;
			}

			file.Delete();
			string directory = file.DirectoryName;

			ModelModifiers.MakeExportReady(model, loggingFunction);

			string connectionString = "Data Source=" + file + ";";
			try
			{
				List<string> bones = useAllBones ? null : model.Bones;

				// TODO: Get bones for skeletons
				Console.WriteLine("WARNING: NO SKELETONS");
				Dictionary<string, SkeletonData> boneDict = new Dictionary<string, SkeletonData>(); ////model.ResolveBoneHeirarchy(null, TTModel.XivRace.All_Races, bones, loggingFunction);

				const string creationScript = "CreateImportDB.sql";

				// Spawn a DB connection to do the raw queries.
				// Using statements help ensure we don't accidentally leave any connections open and lock the file handle.
				using (SqliteConnection db = new SqliteConnection(connectionString))
				{
					db.Open();

					// Create the DB
					string[] lines = File.ReadAllLines("Resources\\SQL\\" + creationScript);
					string sqlCmd = string.Join("\n", lines);

					using (SqliteCommand cmd = new SqliteCommand(sqlCmd, db))
					{
						cmd.ExecuteScalar();
					}

					// Write the Data.
					using (SqliteTransaction transaction = db.BeginTransaction())
					{
						// Metadata.
						string query = @"insert into meta (key, value) values ($key, $value)";
						using (SqliteCommand cmd = new SqliteCommand(query, db))
						{
							// FFXIV stores stuff in Meters.
							cmd.Parameters.AddWithValue("key", "unit");
							cmd.Parameters.AddWithValue("value", "meter");
							cmd.ExecuteScalar();

							// Application that created the db.
							cmd.Parameters.AddWithValue("key", "application");
							cmd.Parameters.AddWithValue("value", "FfxivResourceConverter");
							cmd.ExecuteScalar();

							cmd.Parameters.AddWithValue("key", "version");
							cmd.Parameters.AddWithValue("value", typeof(TTModelDb).Assembly.GetName().Version);
							cmd.ExecuteScalar();

							// Axis information
							cmd.Parameters.AddWithValue("key", "up");
							cmd.Parameters.AddWithValue("value", "y");
							cmd.ExecuteScalar();

							cmd.Parameters.AddWithValue("key", "front");
							cmd.Parameters.AddWithValue("value", "z");
							cmd.ExecuteScalar();

							cmd.Parameters.AddWithValue("key", "handedness");
							cmd.Parameters.AddWithValue("value", "r");
							cmd.ExecuteScalar();

							// FFXIV stores stuff in Meters.
							cmd.Parameters.AddWithValue("key", "root_name");
							cmd.Parameters.AddWithValue("value", Path.GetFileNameWithoutExtension(model.Source));
							cmd.ExecuteScalar();
						}

						// Skeleton
						query = @"insert into skeleton (name, parent, matrix_0, matrix_1, matrix_2, matrix_3, matrix_4, matrix_5, matrix_6, matrix_7, matrix_8, matrix_9, matrix_10, matrix_11, matrix_12, matrix_13, matrix_14, matrix_15) 
											 values ($name, $parent, $matrix_0, $matrix_1, $matrix_2, $matrix_3, $matrix_4, $matrix_5, $matrix_6, $matrix_7, $matrix_8, $matrix_9, $matrix_10, $matrix_11, $matrix_12, $matrix_13, $matrix_14, $matrix_15);";

						using (SqliteCommand cmd = new SqliteCommand(query, db))
						{
							foreach (KeyValuePair<string, SkeletonData> b in boneDict)
							{
								KeyValuePair<string, SkeletonData> parent = boneDict.FirstOrDefault(x => x.Value.BoneNumber == b.Value.BoneParent);
								string parentName = parent.Key == null ? null : parent.Key;
								cmd.Parameters.AddWithValue("name", b.Value.BoneName);
								cmd.Parameters.AddWithValue("parent", parentName);

								for (int i = 0; i < 16; i++)
								{
									cmd.Parameters.AddWithValue("matrix_" + i.ToString(), b.Value.PoseMatrix[i]);
								}

								cmd.ExecuteScalar();
							}
						}

						int modelIdx = 0;
						List<string> models = new List<string>() { Path.GetFileNameWithoutExtension(model.Source) };
						foreach (string mdl in models)
						{
							query = @"insert into models (model, name) values ($model, $name);";
							using (SqliteCommand cmd = new SqliteCommand(query, db))
							{
								cmd.Parameters.AddWithValue("model", modelIdx);
								cmd.Parameters.AddWithValue("name", mdl);
								cmd.ExecuteScalar();
							}

							modelIdx++;
						}

						int matIdx = 0;
						foreach (string material in model.Materials)
						{
							// Materials
							query = @"insert into materials (material_id, name, diffuse, normal, specular, opacity, emissive) values ($material_id, $name, $diffuse, $normal, $specular, $opacity, $emissive);";
							using (SqliteCommand cmd = new SqliteCommand(query, db))
							{
								string mtrl_prefix = directory + "\\" + Path.GetFileNameWithoutExtension(material.Substring(1)) + "_";
								string mtrl_suffix = ".png";
								string name = material;
								try
								{
									name = Path.GetFileNameWithoutExtension(material);
								}
								catch
								{
								}

								cmd.Parameters.AddWithValue("material_id", matIdx);
								cmd.Parameters.AddWithValue("name", name);
								cmd.Parameters.AddWithValue("diffuse", mtrl_prefix + "d" + mtrl_suffix);
								cmd.Parameters.AddWithValue("normal", mtrl_prefix + "n" + mtrl_suffix);
								cmd.Parameters.AddWithValue("specular", mtrl_prefix + "s" + mtrl_suffix);
								cmd.Parameters.AddWithValue("emissive", mtrl_prefix + "e" + mtrl_suffix);
								cmd.Parameters.AddWithValue("opacity", mtrl_prefix + "o" + mtrl_suffix);
								cmd.ExecuteScalar();
							}

							matIdx++;
						}

						int meshIdx = 0;
						foreach (TTMeshGroup m in model.MeshGroups)
						{
							// Bones
							query = @"insert into bones (mesh, bone_id, name) values ($mesh, $bone_id, $name);";
							int bIdx = 0;
							foreach (string b in m.Bones)
							{
								using (SqliteCommand cmd = new SqliteCommand(query, db))
								{
									cmd.Parameters.AddWithValue("name", b);
									cmd.Parameters.AddWithValue("bone_id", bIdx);
									cmd.Parameters.AddWithValue("parent_id", null);
									cmd.Parameters.AddWithValue("mesh", meshIdx);
									cmd.ExecuteScalar();
								}

								bIdx++;
							}

							// Groups
							query = @"insert into meshes (mesh, name, material_id, model) values ($mesh, $name, $material_id, $model);";
							using (SqliteCommand cmd = new SqliteCommand(query, db))
							{
								cmd.Parameters.AddWithValue("name", m.Name);
								cmd.Parameters.AddWithValue("mesh", meshIdx);

								// This is always 0 for now.  Support element for Liinko's work on multi-model export.
								cmd.Parameters.AddWithValue("model", 0);
								cmd.Parameters.AddWithValue("material_id", model.GetMaterialIndex(meshIdx));
								cmd.ExecuteScalar();
							}

							// Parts
							int partIdx = 0;
							foreach (TTMeshPart p in m.Parts)
							{
								// Parts
								query = @"insert into parts (mesh, part, name) values ($mesh, $part, $name);";
								using (SqliteCommand cmd = new SqliteCommand(query, db))
								{
									cmd.Parameters.AddWithValue("name", p.Name);
									cmd.Parameters.AddWithValue("part", partIdx);
									cmd.Parameters.AddWithValue("mesh", meshIdx);
									cmd.ExecuteScalar();
								}

								// Vertices
								int vIdx = 0;
								foreach (TTVertex v in p.Vertices)
								{
									query = @"insert into vertices ( mesh,  part,  vertex_id,  position_x,  position_y,  position_z,  normal_x,  normal_y,  normal_z,  color_r,  color_g,  color_b,  color_a,  uv_1_u,  uv_1_v,  uv_2_u,  uv_2_v,  bone_1_id,  bone_1_weight,  bone_2_id,  bone_2_weight,  bone_3_id,  bone_3_weight,  bone_4_id,  bone_4_weight) 
														values ($mesh, $part, $vertex_id, $position_x, $position_y, $position_z, $normal_x, $normal_y, $normal_z, $color_r, $color_g, $color_b, $color_a, $uv_1_u, $uv_1_v, $uv_2_u, $uv_2_v, $bone_1_id, $bone_1_weight, $bone_2_id, $bone_2_weight, $bone_3_id, $bone_3_weight, $bone_4_id, $bone_4_weight);";
									using (SqliteCommand cmd = new SqliteCommand(query, db))
									{
										cmd.Parameters.AddWithValue("part", partIdx);
										cmd.Parameters.AddWithValue("mesh", meshIdx);
										cmd.Parameters.AddWithValue("vertex_id", vIdx);

										cmd.Parameters.AddWithValue("position_x", v.Position.X);
										cmd.Parameters.AddWithValue("position_y", v.Position.Y);
										cmd.Parameters.AddWithValue("position_z", v.Position.Z);

										cmd.Parameters.AddWithValue("normal_x", v.Normal.X);
										cmd.Parameters.AddWithValue("normal_y", v.Normal.Y);
										cmd.Parameters.AddWithValue("normal_z", v.Normal.Z);

										cmd.Parameters.AddWithValue("color_r", v.VertexColor[0] / 255f);
										cmd.Parameters.AddWithValue("color_g", v.VertexColor[1] / 255f);
										cmd.Parameters.AddWithValue("color_b", v.VertexColor[2] / 255f);
										cmd.Parameters.AddWithValue("color_a", v.VertexColor[3] / 255f);

										cmd.Parameters.AddWithValue("uv_1_u", v.UV1.X);
										cmd.Parameters.AddWithValue("uv_1_v", v.UV1.Y);
										cmd.Parameters.AddWithValue("uv_2_u", v.UV2.X);
										cmd.Parameters.AddWithValue("uv_2_v", v.UV2.Y);

										cmd.Parameters.AddWithValue("bone_1_id", v.BoneIds[0]);
										cmd.Parameters.AddWithValue("bone_1_weight", v.Weights[0] / 255f);

										cmd.Parameters.AddWithValue("bone_2_id", v.BoneIds[1]);
										cmd.Parameters.AddWithValue("bone_2_weight", v.Weights[1] / 255f);

										cmd.Parameters.AddWithValue("bone_3_id", v.BoneIds[2]);
										cmd.Parameters.AddWithValue("bone_3_weight", v.Weights[2] / 255f);

										cmd.Parameters.AddWithValue("bone_4_id", v.BoneIds[3]);
										cmd.Parameters.AddWithValue("bone_4_weight", v.Weights[3] / 255f);

										cmd.ExecuteScalar();
										vIdx++;
									}
								}

								// Indices
								for (int i = 0; i < p.TriangleIndices.Count; i++)
								{
									query = @"insert into indices (mesh, part, index_id, vertex_id) values ($mesh, $part, $index_id, $vertex_id);";
									using (SqliteCommand cmd = new SqliteCommand(query, db))
									{
										cmd.Parameters.AddWithValue("part", partIdx);
										cmd.Parameters.AddWithValue("mesh", meshIdx);
										cmd.Parameters.AddWithValue("index_id", i);
										cmd.Parameters.AddWithValue("vertex_id", p.TriangleIndices[i]);
										cmd.ExecuteScalar();
									}
								}

								// Shape Parts
								foreach (KeyValuePair<string, TTShapePart> shpKv in p.ShapeParts)
								{
									if (!shpKv.Key.StartsWith("shp_")) continue;
									TTShapePart shp = shpKv.Value;

									query = @"insert into shape_vertices ( mesh,  part,  shape,  vertex_id,  position_x,  position_y,  position_z) 
																   values($mesh, $part, $shape, $vertex_id, $position_x, $position_y, $position_z);";
									using (SqliteCommand cmd = new SqliteCommand(query, db))
									{
										foreach (KeyValuePair<int, int> vKv in shp.VertexReplacements)
										{
											TTVertex v = shp.Vertices[vKv.Value];
											cmd.Parameters.AddWithValue("part", partIdx);
											cmd.Parameters.AddWithValue("mesh", meshIdx);
											cmd.Parameters.AddWithValue("shape", shpKv.Key);
											cmd.Parameters.AddWithValue("vertex_id", vKv.Key);

											cmd.Parameters.AddWithValue("position_x", v.Position.X);
											cmd.Parameters.AddWithValue("position_y", v.Position.Y);
											cmd.Parameters.AddWithValue("position_z", v.Position.Z);

											cmd.ExecuteScalar();
											vIdx++;
										}
									}
								}

								partIdx++;
							}

							meshIdx++;
						}

						transaction.Commit();
					}
				}
			}
			catch (Exception)
			{
				ModelModifiers.MakeImportReady(model, loggingFunction);
				throw;
			}

			// Undo the export ready at the start.
			ModelModifiers.MakeImportReady(model, loggingFunction);
		}

		/// <summary>
		/// Loads a TTModel file from a given SQLite3 DB filepath.
		/// </summary>
		public static TTModel FromDb(FileInfo file, Action<bool, string> loggingFunction = null)
		{
			if (loggingFunction == null)
			{
				loggingFunction = ModelModifiers.NoOp;
			}

			string connectionString = "Data Source=" + file + ";";
			TTModel model = new TTModel();
			model.Source = file.FullName;

			// Spawn a DB connection to do the raw queries.
			using (SqliteConnection db = new SqliteConnection(connectionString))
			{
				db.Open();

				// Load Mesh Groups
				string query = "select * from meshes order by mesh asc;";
				using (SqliteCommand cmd = new SqliteCommand(query, db))
				{
					using (CacheReader reader = new CacheReader(cmd.ExecuteReader()))
					{
						while (reader.NextRow())
						{
							int meshNum = reader.GetInt32("mesh");

							// Spawn mesh groups as needed.
							while (model.MeshGroups.Count <= meshNum)
							{
								model.MeshGroups.Add(new TTMeshGroup());
							}

							model.MeshGroups[meshNum].Name = reader.GetString("name");
						}
					}
				}

				// Load Mesh Parts
				query = "select * from parts order by mesh asc, part asc;";
				using (SqliteCommand cmd = new SqliteCommand(query, db))
				{
					using (CacheReader reader = new CacheReader(cmd.ExecuteReader()))
					{
						while (reader.NextRow())
						{
							int meshNum = reader.GetInt32("mesh");
							int partNum = reader.GetInt32("part");

							// Spawn mesh groups if needed.
							while (model.MeshGroups.Count <= meshNum)
							{
								model.MeshGroups.Add(new TTMeshGroup());
							}

							// Spawn parts as needed.
							while (model.MeshGroups[meshNum].Parts.Count <= partNum)
							{
								model.MeshGroups[meshNum].Parts.Add(new TTMeshPart());
							}

							model.MeshGroups[meshNum].Parts[partNum].Name = reader.GetString("name");
						}
					}
				}

				// Load Bones
				query = "select * from bones where mesh >= 0 order by mesh asc, bone_id asc;";
				using (SqliteCommand cmd = new SqliteCommand(query, db))
				{
					using (CacheReader reader = new CacheReader(cmd.ExecuteReader()))
					{
						while (reader.NextRow())
						{
							int meshId = reader.GetInt32("mesh");
							model.MeshGroups[meshId].Bones.Add(reader.GetString("name"));
						}
					}
				}
			}

			// Loop for each part, to populate their internal data structures.
			for (int mId = 0; mId < model.MeshGroups.Count; mId++)
			{
				TTMeshGroup m = model.MeshGroups[mId];
				for (int pId = 0; pId < m.Parts.Count; pId++)
				{
					TTMeshPart p = m.Parts[pId];
					WhereClause where = new WhereClause();
					WhereClause mWhere = new WhereClause();
					mWhere.Column = "mesh";
					mWhere.Value = mId;
					WhereClause pWhere = new WhereClause();
					pWhere.Column = "part";
					pWhere.Value = pId;

					where.Inner.Add(mWhere);
					where.Inner.Add(pWhere);

					// Load Vertices
					// The reader handles coalescing the null types for us.
					p.Vertices = BuildListFromTable(connectionString, "vertices", where, (reader) =>
					{
						TTVertex vertex = new TTVertex();

						// Positions
						vertex.Position.X = reader.GetFloat("position_x");
						vertex.Position.Y = reader.GetFloat("position_y");
						vertex.Position.Z = reader.GetFloat("position_z");

						// Normals
						vertex.Normal.X = reader.GetFloat("normal_x");
						vertex.Normal.Y = reader.GetFloat("normal_y");
						vertex.Normal.Z = reader.GetFloat("normal_z");

						// Vertex Colors - Vertex color is RGBA
						vertex.VertexColor[0] = (byte)Math.Round(reader.GetFloat("color_r") * 255);
						vertex.VertexColor[1] = (byte)Math.Round(reader.GetFloat("color_g") * 255);
						vertex.VertexColor[2] = (byte)Math.Round(reader.GetFloat("color_b") * 255);
						vertex.VertexColor[3] = (byte)Math.Round(reader.GetFloat("color_a") * 255);

						// UV Coordinates
						vertex.UV1.X = reader.GetFloat("uv_1_u");
						vertex.UV1.Y = reader.GetFloat("uv_1_v");
						vertex.UV2.X = reader.GetFloat("uv_2_u");
						vertex.UV2.Y = reader.GetFloat("uv_2_v");

						// Bone Ids
						vertex.BoneIds[0] = (byte)reader.GetByte("bone_1_id");
						vertex.BoneIds[1] = (byte)reader.GetByte("bone_2_id");
						vertex.BoneIds[2] = (byte)reader.GetByte("bone_3_id");
						vertex.BoneIds[3] = (byte)reader.GetByte("bone_4_id");

						// Weights
						vertex.Weights[0] = (byte)Math.Round(reader.GetFloat("bone_1_weight") * 255);
						vertex.Weights[1] = (byte)Math.Round(reader.GetFloat("bone_2_weight") * 255);
						vertex.Weights[2] = (byte)Math.Round(reader.GetFloat("bone_3_weight") * 255);
						vertex.Weights[3] = (byte)Math.Round(reader.GetFloat("bone_4_weight") * 255);

						return vertex;
					});

					p.TriangleIndices = BuildListFromTable(connectionString, "indices", where, (reader) =>
					{
						try
						{
							return reader.GetInt32("vertex_id");
						}
						catch (Exception ex)
						{
							throw ex;
						}
					});
				}
			}

			// Spawn a DB connection to do the raw queries.
			using (SqliteConnection db = new SqliteConnection(connectionString))
			{
				db.Open();

				// Load Shape Verts
				string query = "select * from shape_vertices order by shape asc, mesh asc, part asc, vertex_id asc;";
				using (SqliteCommand cmd = new SqliteCommand(query, db))
				{
					using (CacheReader reader = new CacheReader(cmd.ExecuteReader()))
					{
						while (reader.NextRow())
						{
							string shapeName = reader.GetString("shape");
							int meshNum = reader.GetInt32("mesh");
							int partNum = reader.GetInt32("part");
							int vertexId = reader.GetInt32("vertex_id");

							TTMeshPart part = model.MeshGroups[meshNum].Parts[partNum];

							// Copy the original vertex and update position.
							TTVertex vertex = (TTVertex)part.Vertices[vertexId].Clone();
							vertex.Position.X = reader.GetFloat("position_x");
							vertex.Position.Y = reader.GetFloat("position_y");
							vertex.Position.Z = reader.GetFloat("position_z");

							TTVertex repVert = part.Vertices[vertexId];
							if (repVert.Position.Equals(vertex.Position))
							{
								// Skip morphology which doesn't actually change anything.
								continue;
							}

							if (!part.ShapeParts.ContainsKey(shapeName))
							{
								TTShapePart shpPt = new TTShapePart();
								shpPt.Name = shapeName;
								part.ShapeParts.Add(shapeName, shpPt);
							}

							part.ShapeParts[shapeName].VertexReplacements.Add(vertexId, part.ShapeParts[shapeName].Vertices.Count);
							part.ShapeParts[shapeName].Vertices.Add(vertex);
						}
					}
				}
			}

			// Convert the model to FFXIV's internal weirdness.
			ModelModifiers.MakeImportReady(model, loggingFunction);
			return model;
		}

		/// <summary>
		/// Creates a list from the data entries in a cache table, using the given where clause and predicate.
		/// </summary>
		public static List<T> BuildListFromTable<T>(string connectionString, string table, WhereClause where, Func<CacheReader, T> func)
		{
			List<T> list = new List<T>();
			using (SqliteConnection db = new SqliteConnection(connectionString))
			{
				db.Open();

				// Check how large the result set will be so we're not constantly
				// Reallocating the array.
				string query = "select count(*) from " + table + " ";
				if (where != null)
				{
					query += where.GetSql();
				}

				using (SqliteCommand cmd = new SqliteCommand(query, db))
				{
					if (where != null)
					{
						where.AddParameters(cmd);
					}

					int val = (int)(long)cmd.ExecuteScalar();
					list = new List<T>(val);
				}

				// Set up the actual full query.
				query = "select * from " + table;
				if (where != null)
				{
					query += where.GetSql();
				}

				using (SqliteCommand cmd = new SqliteCommand(query, db))
				{
					if (where != null)
					{
						where.AddParameters(cmd);
					}

					using (CacheReader reader = new CacheReader(cmd.ExecuteReader()))
					{
						while (reader.NextRow())
						{
							try
							{
								list.Add(func(reader));
							}
							catch (Exception)
							{
								throw;
							}
						}
					}
				}
			}

			return list;
		}

		/// <summary>
		/// Class for composing SQL Where clauses programatically.
		/// A [WhereClause] with any [Inner] entries is considered
		/// a parenthetical group and has its own Column/Value/Comparer ignored.
		/// </summary>
		public class WhereClause
		{
			public List<WhereClause> Inner;
			public string Column;
			public object Value;
			public ComparisonType Comparer = ComparisonType.Equal;
			public JoinType Join = JoinType.And;

			public WhereClause()
			{
				this.Inner = new List<WhereClause>();
			}

			public enum ComparisonType
			{
				Equal,
				NotEqual,
				Like,
			}

			public enum JoinType
			{
				And,
				Or,
			}

			/// <summary>
			/// Generates the body of the Where clause, without the starting ' where '.
			/// </summary>
			/// <param name="includeWhere">If literal word ' where ' should be included.</param>
			/// <param name="skipJoin">If the [and/or] should be skipped.</param>
			public string GetSql(bool includeWhere = true, bool skipJoin = true)
			{
				// No clause if no valid value.
				if ((this.Inner == null || this.Inner.Count == 0) && (this.Column == null || this.Column == string.Empty))
				{
					return string.Empty;
				}

				string result = string.Empty;
				if (includeWhere)
				{
					result += " where ";
				}

				// If we're a parenthetical group
				if (this.Inner != null && this.Inner.Count > 0)
				{
					bool first = true;

					if (skipJoin)
					{
						result += " ( ";
					}
					else
					{
						result += " " + this.Join.ToString().ToLower() + " ( ";
					}

					foreach (WhereClause where in this.Inner)
					{
						if (first)
						{
							// First item in a parenthetical group or top level has its
							// Join ignored - it is implicitly [AND] logically.
							result += where.GetSql(false, true);
							first = false;
						}
						else
						{
							result += where.GetSql(false, false);
						}
					}

					result += " ) ";
				}
				else
				{
					// We're a standard single term where clause
					if (!skipJoin)
					{
						// [AND/OR]
						result += " " + this.Join.ToString().ToLower() + " ";
					}

					// [AND/OR] [COLUMN]
					result += " " + this.Column + " ";

					// [AND/OR] [COLUMN] [=/LIKE]
					if (this.Comparer == ComparisonType.Equal)
					{
						result += " = ";
					}
					else if (this.Comparer == ComparisonType.NotEqual)
					{
						result += " != ";
					}
					else
					{
						result += " like ";
					}

					// [AND/OR] [COLUMN] [=/LIKE] [$COLUMN]
					result += " $" + this.Column + " ";
				}

				return result;
			}

			public void AddParameters(SqliteCommand cmd)
			{
				if (this.Inner != null && this.Inner.Count > 0)
				{
					foreach (WhereClause where in this.Inner)
					{
						where.AddParameters(cmd);
					}
				}
				else
				{
					if (this.Column != null && this.Column != string.Empty)
					{
						cmd.Parameters.AddWithValue(this.Column, this.Value);
					}
				}
			}
		}

		/// <summary>
		/// A thin wrapper for the SQLiteDataReader class that
		/// helps with string column accessors, NULL value coalescence, and
		/// ensures the underlying reader is properly closed and destroyed to help
		/// avoid lingering file handles.
		/// </summary>
		public class CacheReader : IDisposable
		{
			private static readonly Type NullType = typeof(DBNull);

			private readonly SqliteDataReader reader;
			private readonly Dictionary<string, int> headers;

			private bool disposed = false;

			public CacheReader(SqliteDataReader reader)
			{
				this.reader = reader;
				this.headers = new Dictionary<string, int>();

				// Immediately get and cache the headers.
				for (int idx = 0; idx < this.reader.FieldCount; idx++)
				{
					this.headers.Add(this.reader.GetName(idx), idx);
				}
			}

			~CacheReader()
			{
				this.Dispose(false);
			}

			/// <summary>
			/// Gets returns ther raw SQLiteDataReader object.
			/// </summary>
			public SqliteDataReader Raw => this.reader;

			/// <summary>
			/// Gets column names/keys.
			/// </summary>
			public Dictionary<string, int> Columns => this.headers;

			public byte GetByte(string fieldName)
			{
				if (this.reader[this.headers[fieldName]].GetType() == NullType)
				{
					return 0;
				}

				return this.reader.GetByte(this.headers[fieldName]);
			}

			public float GetFloat(string fieldName)
			{
				if (this.reader[this.headers[fieldName]].GetType() == NullType)
				{
					return 0f;
				}

				return this.reader.GetFloat(this.headers[fieldName]);
			}

			public int GetInt32(string fieldName)
			{
				if (this.reader[this.headers[fieldName]].GetType() == NullType)
				{
					return 0;
				}

				return this.reader.GetInt32(this.headers[fieldName]);
			}

			public int? GetNullableInt32(string fieldName)
			{
				if (this.reader[this.headers[fieldName]].GetType() == NullType)
				{
					return null;
				}

				return this.reader.GetInt32(this.headers[fieldName]);
			}

			public string GetString(string fieldName)
			{
				if (this.reader[this.headers[fieldName]].GetType() == NullType)
				{
					return null;
				}

				return this.reader.GetString(this.headers[fieldName]);
			}

			public bool GetBoolean(string fieldName)
			{
				if (this.reader[this.headers[fieldName]].GetType() == NullType)
				{
					return false;
				}

				return this.reader.GetBoolean(this.headers[fieldName]);
			}

			/// <summary>
			/// Moves forward to the next row.
			/// </summary>
			public bool NextRow()
			{
				return this.reader.Read();
			}

			public void Dispose()
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}

			// Dispose(bool disposing) executes in two distinct scenarios.
			// If disposing equals true, the method has been called directly
			// or indirectly by a user's code. Managed and unmanaged resources
			// can be disposed.
			// If disposing equals false, the method has been called by the
			// runtime from inside the finalizer and you should not reference
			// other objects. Only unmanaged resources can be disposed.
			protected virtual void Dispose(bool disposing)
			{
				// Check to see if Dispose has already been called.
				if (!this.disposed)
				{
					// If disposing equals true, dispose all managed
					// and unmanaged resources.
					if (disposing)
					{
						// Dispose managed resources.

						// Ensure the raw reader's file handle was closed.
						if (!this.reader.IsClosed)
						{
							this.reader.Close();
						}
					}

					// Note disposing has been done.
					this.disposed = true;
				}
			}
		}
	}
}
