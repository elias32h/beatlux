﻿using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Collections.Generic;

/// <summary>
/// Database class offers connection to the local database. It should be
/// only one connection opened which is realized through the use of the
/// Singleton pattern.
/// 
/// == Usage ==
/// SqliteConnection db = Database.getConnection ();
/// SqliteCommand cmd = new SqliteCommand (db);
/// // and so on ...
/// </summary>
public class Database {

	// Database name
	private static string NAME = "beatlux.db";

	// Class instance
	private static Database Instance;

	// Database connection
	public static SqliteConnection Connection;



	// Enums for error handling
	public enum Constants : long
	{
		Successful = -1,

		QueryFailed = -10,
		DuplicateFound = -11,
		EmptyInputValue = -12,
	}



	// Object initialization
	private Database ()
	{
		// Path to database
		string uri = "Data Source=" + Application.dataPath + "/" + NAME;

		// Connect to database
		Connection = Connect (uri);
	}

	// Connect to dataabse
	private SqliteConnection Connect (string uri)
	{
		// Connect to database
		Connection = new SqliteConnection (uri);

		// Open database connection
		Connection.Open ();

		return Connection;
	}

	// Close database connection
	public static void Close ()
	{
		// Close connection to database
		if (Connection != null) {
			Connection.Close ();
		}

		// Reset instance
		Instance = null;

		// Reset database instance
		Connection = null;
	}

	public static void CreateTables ()
	{
		// SQL statements
		string[] stm = {
			"CREATE TABLE IF NOT EXISTS `file` ( `id` INTEGER PRIMARY KEY AUTOINCREMENT, `path` TEXT NOT NULL UNIQUE )",
			"CREATE TABLE IF NOT EXISTS `playlist` ( `id` INTEGER PRIMARY KEY AUTOINCREMENT, `name` TEXT NOT NULL UNIQUE, `files` TEXT DEFAULT NULL )",
			"CREATE TABLE IF NOT EXISTS `visualization` ( `id` INTEGER PRIMARY KEY AUTOINCREMENT, `name` TEXT NOT NULL UNIQUE, `colors` INTEGER NOT NULL DEFAULT 1, `buildNumber` INTEGER UNIQUE, `skybox` TEXT DEFAULT NULL )",
			"CREATE TABLE IF NOT EXISTS `color_scheme` ( `id` INTEGER PRIMARY KEY AUTOINCREMENT, `name` TEXT NOT NULL, `viz_id` INTEGER, `colors` TEXT NOT NULL, FOREIGN KEY(`viz_id`) REFERENCES `visualization`(`id`) )",
		};

		// Create tables
		if (GetConnection () != null)
		{
			foreach (string sql in stm)
			{
				SqliteCommand cmd = new SqliteCommand (sql, Connection);
				cmd.ExecuteNonQuery ();
				cmd.Dispose ();
			}
		}
	}

	public static void InsertDefaultViz ()
	{
		if (GetConnection () != null)
		{
			for (int i=0; i < Settings.Visualizations.Length; i++)
			{
				// Current visualization
				VisualizationObj viz = Settings.Visualizations [i];

				if (Application.CanStreamedLevelBeLoaded (viz.BuildNumber))
				{
					// Query statement
					string sql = "INSERT INTO visualization (name, colors, buildNumber, skybox) " +
						"VALUES (@Name, @Colors, @BuildNumber, @Skybox); " +
						"SELECT last_insert_rowid()";
					SqliteCommand cmd = new SqliteCommand (sql, Connection);

					// Add Parameters to statement
					cmd.Parameters.Add (new SqliteParameter ("Name", viz.Name));
					cmd.Parameters.Add (new SqliteParameter ("Colors", viz.Colors));
					cmd.Parameters.Add (new SqliteParameter ("BuildNumber", viz.BuildNumber));
					cmd.Parameters.Add (new SqliteParameter ("Skybox", viz.Skybox));

					try
					{
						// Execute insert statement
						long id = (long) cmd.ExecuteScalar ();

						// Update ID
						Settings.Visualizations [i].ID = id;

						// Dispose command
						cmd.Dispose ();
					}
					catch
					{
						// Dispose command
						cmd.Dispose ();

						// Select ID from database
						sql = "SELECT id FROM visualization WHERE name = @Name AND buildNumber = @BuildNumber";
						cmd = new SqliteCommand (sql, Connection);

						// Add Parameters to statement
						cmd.Parameters.Add (new SqliteParameter ("Name", viz.Name));
						cmd.Parameters.Add (new SqliteParameter ("BuildNumber", viz.BuildNumber));

						// Get sql results
						SqliteDataReader reader = cmd.ExecuteReader ();

						// Read id
						while (reader.Read ()) {
							Settings.Visualizations [i].ID = reader.GetInt64 (0);
						}

						// Close reader
						reader.Close();
						cmd.Dispose ();
					}
				}
			}
		}
	}

	public static void InsertDefaultCS ()
	{
		if (GetConnection () != null && Settings.Visualizations != null && Settings.Visualizations.Length > 0)
		{
			foreach (VisualizationObj viz in Settings.Visualizations)
			{
				if (!ColorScheme.Exists (new ColorSchemeObj (viz.Name, viz)))
				{
					// Insert default color scheme
					string sql = "INSERT INTO color_scheme (name, viz_id, colors) VALUES (@Name, @Viz_ID, @Colors)";
					SqliteCommand cmd = new SqliteCommand (sql, Connection);

					// Add Parameters to statement
					cmd.Parameters.Add (new SqliteParameter ("Name", viz.Name));
					cmd.Parameters.Add (new SqliteParameter ("Viz_ID", viz.ID));

					if (Settings.Defaults.Colors.ContainsKey (viz.Name))
					{
						// Set colors
						Color[] colors = Settings.Defaults.Colors [viz.Name];
						cmd.Parameters.Add (new SqliteParameter ("Colors", ColorScheme.FormatColors (colors)));

						// Execute insert statement
						cmd.ExecuteNonQuery ();

						// Dispose command
						cmd.Dispose ();
					}
				}
			}
		}
	}

	public static void InsertDefaultPlaylist ()
	{
		if (GetConnection () != null)
		{
			// Database command
			SqliteCommand cmd = new SqliteCommand (Connection);

			// Query statement
			string sql = "SELECT id FROM playlist LIMIT 1";
			cmd.CommandText = sql;

			// Get sql results
			SqliteDataReader reader = cmd.ExecuteReader ();


			if (!reader.HasRows)
			{
				// Dispose command
				cmd.Dispose ();

				// Create default playlist
				sql = "INSERT INTO playlist (name) VALUES(@Name)";
				cmd = new SqliteCommand (sql, Connection);

				// Add parameters
				cmd.Parameters.Add (new SqliteParameter ("Name", "Neue Playlist"));

				// Send query
				cmd.ExecuteNonQuery ();
			}


			// Close reader
			reader.Close ();
			cmd.Dispose ();
		}
	}



	// Get instance (Singleton)
	public static SqliteConnection GetConnection ()
	{
		// Create instance if not exists
		if (Instance == null)
		{
			Instance = new Database ();

			CreateTables ();
			InsertDefaultViz ();
			InsertDefaultCS ();
			InsertDefaultPlaylist ();
		}

		// Return database connection
		return Connection;
	}



	public static bool Connect ()
	{
		if (Connection == null) {
			Connection = Database.GetConnection ();
		}

		return Connection != null;
	}
}
