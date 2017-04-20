﻿using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.IO;

public class Playlist : MonoBehaviour {

	// Dialog reference
	public Dialog dialog;

	// Database connection
	public SqliteConnection db;

	// Playlists
	public List<PlaylistObj> playlists;

	// Playlist List GameObject
	public GameObject playlist;



	public void Start ()
	{
		// Connect to database
		DbConnect ();

		// Select playlists from database
		Load (true);

		// Close database connection
		DbClose ();
	}



	public void Display ()
	{
		// Remove all GameObjects
		for (int i=playlist.transform.childCount - 1; i >= 0; i--) {
			GameObject.DestroyImmediate (playlist.transform.GetChild (i).gameObject);
		}

		foreach (PlaylistObj p in playlists)
		{
			DisplayPlaylist (p);

			// Add files
			foreach (FileObj f in p.Files) {
				DisplayFile (p, f);
			}

			// Hide playlist files
			if (playlist.transform.Find ("#" + p.ID + "/Contents") != null)
				playlist.transform.Find ("#" + p.ID + "/Contents").gameObject.SetActive (false);
		}
	}

	public void DisplayPlaylist (PlaylistObj playlist)
	{
		// Create GameObject
		GameObject gameObject = DisplayPlaylistOrFile (playlist, null);
		Text text = gameObject.transform.Find ("Main/Text").gameObject.GetComponent<Text> ();

		// Text settings
		text.text = playlist.Name;

		// Get Event Trigger
		EventTrigger events = gameObject.GetComponent<EventTrigger> ();

		// Add Click Event
		EventTrigger.Entry evtClick = new EventTrigger.Entry ();
		evtClick.eventID = EventTriggerType.PointerClick;
		events.triggers.Add (evtClick);

		evtClick.callback.AddListener ((eventData) => {
			ToggleFiles (playlist);
		});

		// Add Event to Button
		gameObject.transform.Find ("Main/Text").gameObject.GetComponent<Button> ().onClick.AddListener (delegate {
			ToggleFiles (playlist);
		});
	}

	public void DisplayFile (PlaylistObj playlist, FileObj file)
	{
		// Create GameObject
		GameObject gameObject = DisplayPlaylistOrFile (playlist, file);
		Text text = gameObject.transform.Find ("Text").gameObject.GetComponent<Text> ();

		// Text settings
		text.text = Path.GetFileName (file.Path);
	}

	public GameObject DisplayPlaylistOrFile (PlaylistObj playlist, FileObj file)
	{
		if (playlist != null)
		{
			// Set name
			string name = "#" + playlist.ID + (file != null ? ("." + file.ID) : "");

			// Create main GameObject
			GameObject main = new GameObject ("Main");
			if (file != null) main.name = "Contents";

			// Check if Contents GameObject already exists
			Transform contents = this.playlist.transform.Find ("#" + playlist.ID + "/Contents");
			bool contentsExists = !ReferenceEquals (contents, null);

			if (contentsExists && file != null) {
				DestroyImmediate (main);
				main = contents.gameObject;
			}

			// Set parent of GameObject
			Transform parent = this.playlist.transform;
			if (file != null) parent = main.transform;

			// Create GameOject
			GameObject gameObject = new GameObject (name);
			gameObject.transform.SetParent (parent);

			// Add Vertical Layout Group
			if (!contentsExists || file == null) {
				VerticalLayoutGroup vlg = (file == null ? gameObject : main).AddComponent<VerticalLayoutGroup> ();
				vlg.spacing = 20;
				vlg.childForceExpandWidth = true;
				vlg.childForceExpandHeight = false;
			}


			// Set GameObject for return
			GameObject goReturn = gameObject;

			// Set parent of main GameObject
			parent = gameObject.transform;
			if (file != null) parent = this.playlist.transform.Find ("#" + playlist.ID);
			main.transform.SetParent (parent);


			// Change GameObjects if file is displayed to inherit from different GameObject
			if (file != null) main = gameObject;


			// Add Layout Element to GameObject
			LayoutElement mainLayout = main.AddComponent<LayoutElement> ();
			mainLayout.minHeight = 30;
			mainLayout.preferredHeight = mainLayout.minHeight;

			// Add image to GameObject
			Image mainImg = main.AddComponent<Image> ();
			mainImg.color = Color.clear;

			// Set transformations
			mainImg.rectTransform.pivot = new Vector2(0, 0.5f);

			// Add Horizontal Layout Group
			HorizontalLayoutGroup mainHlg = main.AddComponent<HorizontalLayoutGroup> ();
			mainHlg.spacing = 10;
			mainHlg.childForceExpandWidth = false;
			mainHlg.childForceExpandHeight = false;
			mainHlg.childAlignment = TextAnchor.MiddleLeft;

			// Set padding right of Horizontal Layout Group
			mainHlg.padding = new RectOffset (0, (file == null ? 65 : 30), 0, 0);

			// Add Drop Handler script
			if (file == null) gameObject.AddComponent <DropHandler> ();


			// Create arrow text GameObject
			GameObject mainArrow = new GameObject ("Arrow");
			mainArrow.transform.SetParent (main.transform);

			// Add text
			TextUnicode mainTextArrow = mainArrow.AddComponent<TextUnicode> ();
			if (playlist.Equals(Settings.playlist) && file == null) {
				mainTextArrow.text = IconFont.DROPDOWN_CLOSED;
			}

			// Set text alignment
			mainTextArrow.alignment = TextAnchor.MiddleLeft;

			// Font settings
			mainTextArrow.font = IconFont.font;
			mainTextArrow.fontSize = 20;

			// Add Layout Element
			LayoutElement mainLayoutElementArrow = mainArrow.AddComponent<LayoutElement> ();
			mainLayoutElementArrow.minWidth = 22;


			if (file != null)
			{
				// Create listening text GameObject
				GameObject mainListening = new GameObject ("Listening");
				mainListening.transform.SetParent (main.transform);

				// Add text
				TextUnicode mainTextListening = mainListening.AddComponent<TextUnicode> ();

				if (playlist.Equals (Settings.playlist) && file.Equals (Settings.file))
				{
					mainTextListening.text = IconFont.LISTENING;
					mainTextListening.fontSize = 30;
					mainTextListening.color = new Color (0.7f, 0.7f, 0.7f);
				}
				else
				{
					mainTextListening.text = IconFont.DROPDOWN_CLOSED;
					mainTextListening.fontSize = 20;
					mainTextListening.color = Color.gray;
				}

				// Set text alignment
				mainTextListening.alignment = TextAnchor.MiddleLeft;

				// Font settings
				mainTextListening.font = IconFont.font;

				// Add Layout Element
				LayoutElement mainLayoutElementListening = mainListening.AddComponent<LayoutElement> ();
				mainLayoutElementListening.minWidth = 32;
			}


			// Create text GameObject
			GameObject mainText = new GameObject ("Text");
			mainText.transform.SetParent (main.transform);

			// Add text
			Text text = mainText.AddComponent<Text> ();

			// Set text alignment
			text.alignment = TextAnchor.MiddleLeft;

			// Set text color
			if (file == null) {
				text.color = Color.white;
			} else if (playlist.Equals (Settings.playlist) && file.Equals (Settings.file)) {
				text.color = new Color (0.7f, 0.7f, 0.7f);
			} else {
				text.color = Color.gray;
			}

			// Font settings
			text.font = Resources.Load<Font> ("Fonts/FuturaStd-Book");
			text.fontSize = 30;

			// Set transformations
			text.rectTransform.pivot = new Vector2 (0.5f, 0.5f);

			// Add button
			Button buttonText = mainText.AddComponent<Button> ();
			buttonText.transition = Selectable.Transition.Animation;

			// Add animator
			Animator animatorText = mainText.AddComponent<Animator> ();
			animatorText.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController> ("Animations/MenuButtons");


			// Create edit icons GameObject
			GameObject editIcons = new GameObject ("Images");
			editIcons.transform.SetParent (main.transform);

			// Set transformations
			RectTransform editIconsTrans = editIcons.AddComponent<RectTransform> ();
			editIconsTrans.anchoredPosition = Vector2.zero;
			editIconsTrans.anchorMin = new Vector2 (1, 0.5f);
			editIconsTrans.anchorMax = new Vector2 (1, 0.5f);
			editIconsTrans.pivot = new Vector2 (1, 0.5f);

			// Add Layout Element
			LayoutElement editIconslayoutElement = editIcons.AddComponent<LayoutElement> ();
			editIconslayoutElement.ignoreLayout = true;

			// Add Content Size Fitter
			ContentSizeFitter editIconsCsf = editIcons.AddComponent<ContentSizeFitter> ();
			editIconsCsf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
			editIconsCsf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

			// Add Layout Group
			HorizontalLayoutGroup editIconsHlgImg = editIcons.AddComponent<HorizontalLayoutGroup> ();
			editIconsHlgImg.childAlignment = TextAnchor.MiddleRight;
			editIconsHlgImg.spacing = 5;
			editIconsHlgImg.childForceExpandWidth = false;
			editIconsHlgImg.childForceExpandHeight = false;

			// Disable edit icons GameObject
			editIcons.SetActive (false);


			if (file == null)
			{
				// Create edit text GameObject
				GameObject edit = new GameObject ("Edit");
				edit.transform.SetParent (editIcons.transform);

				// Add text
				TextUnicode editText = edit.AddComponent<TextUnicode> ();
				editText.text = IconFont.EDIT;

				// Set text alignment
				editText.alignment = TextAnchor.MiddleRight;

				// Set transformations
				editText.rectTransform.sizeDelta = new Vector2 (20, 30);

				// Font settings
				editText.font = IconFont.font;
				editText.fontSize = 30;

				// Add button
				Button buttonEditEvt = edit.AddComponent<Button> ();
				buttonEditEvt.transition = Selectable.Transition.Animation;

				// Add button onclick event
				buttonEditEvt.onClick.AddListener (delegate {
					ShowDialog ("PL_EDIT", gameObject);
				});

				// Add animator
				Animator animatorEditEvt = edit.AddComponent<Animator> ();
				animatorEditEvt.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController> ("Animations/MenuButtons");
			}


			// Create delete text GameObject
			GameObject delete = new GameObject ("Delete");
			delete.transform.SetParent (editIcons.transform);

			// Add text
			Text deleteText = delete.AddComponent<Text> ();
			deleteText.text = IconFont.TRASH;

			// Set text alignment
			deleteText.alignment = TextAnchor.MiddleRight;

			// Set transformations
			deleteText.rectTransform.sizeDelta = new Vector2 (20, 30);

			// Font settings
			deleteText.font = IconFont.font;
			deleteText.fontSize = 30;

			// Add button
			Button buttonDeleteEvt = delete.AddComponent<Button> ();
			buttonDeleteEvt.transition = Selectable.Transition.Animation;

			// Add button onclick event
			buttonDeleteEvt.onClick.AddListener (delegate {
				if (FindFile (gameObject) == null) {
					ShowDialog ("PL_DEL", gameObject);
				} else {
					Delete (gameObject);
				}
			});

			// Add animator
			Animator animatorDeleteEvt = delete.AddComponent<Animator> ();
			animatorDeleteEvt.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController> ("Animations/MenuButtons");


			// Create GameObject Event Triggers
			EventTrigger evtWrapper = gameObject.AddComponent<EventTrigger> ();

			// Add Hover Enter Event
			EventTrigger.Entry evtHover = new EventTrigger.Entry ();
			evtHover.eventID = EventTriggerType.PointerEnter;
			evtWrapper.triggers.Add (evtHover);

			evtHover.callback.AddListener ((eventData) => {
				editIcons.SetActive (true);
			});

			// Add Hover Exit Event
			EventTrigger.Entry evtExit = new EventTrigger.Entry ();
			evtExit.eventID = EventTriggerType.PointerExit;
			evtWrapper.triggers.Add (evtExit);

			evtExit.callback.AddListener ((eventData) => {
				editIcons.SetActive (false);
			});


			return goReturn;
		}

		return null;
	}

	public void ToggleFiles (PlaylistObj playlist)
	{
		ToggleFiles (playlist, false);
	}

	public void ToggleFiles (PlaylistObj playlist, bool forceOpen)
	{
		// Set playlist as active playlist
		if (!forceOpen) Settings.playlist = playlist;

		// Show or hide playlist files
		bool opened = false;
		foreach (PlaylistObj p in playlists)
		{
			// Get GameObject for current file
			Transform contents = this.playlist.transform.Find ("#" + p.ID + "/Contents");
			GameObject main = contents != null ? contents.gameObject : null;

			// Toggle files for GameObject
			if (main != null) {
				if (p == playlist) {
					main.SetActive (!forceOpen ? !main.activeSelf : true);
					opened = main.activeSelf;
				} else {
					main.SetActive (false);
				}
			}

			// Change arrows
			Text arr = this.playlist.transform.Find ("#" + p.ID).transform.Find ("Main/Arrow").GetComponent<Text>();
			if (!forceOpen && p != playlist) {
				arr.text = "";
			}
		}

		// Change arrow image
		Text arrow = this.playlist.transform.Find ("#" + playlist.ID + "/Main/Arrow").GetComponent<Text> ();
		if (!forceOpen && arrow != null) {
			arrow.text = opened ? IconFont.DROPDOWN_OPENED : IconFont.DROPDOWN_CLOSED;
		}
	}

	public long NewPlaylist (string name)
	{
		if (name.Length > 0)
		{
			// Create playlist object
			PlaylistObj playlist = new PlaylistObj (name);

			// Create object in database
			long id = Create (playlist);

			// Reload playlists
			Load (true);

			return id;
		}

		return (long) Database.Constants.QueryFailed;
	}

	public bool Delete (GameObject gameObject)
	{
		// Get file
		PlaylistObj playlist = FindPlaylist (gameObject);
		FileObj file = FindFile (gameObject);

		bool deleted = playlist != null ? (file != null ? DeleteFile (playlist, file) : DeletePlaylist (playlist)) : false;

		if (deleted) {
			// Remove from interface
			DestroyImmediate (gameObject);

			if (file == null) {
				// Remove from list
				playlists.Remove (playlist);

				// Unset active playlist
				if (playlist == Settings.playlist) Settings.playlist = null;
			} else {
				// Remove files from playlist
				playlist.Files.Remove (file);
			}
		}

		return deleted;
	}

	public PlaylistObj FindPlaylist (GameObject gameObject)
	{
		if (gameObject != null)
		{
			// Get playlist id
			string[] name = gameObject.name.Split ('.');
			long playlistID = Int64.Parse (name [0].Split ('#') [1]);

			// Get playlist
			return playlists.Find (x => x.ID == playlistID);
		}

		return null;
	}

	public FileObj FindFile (GameObject gameObject)
	{
		if (gameObject != null)
		{
			// Get playlist and file id
			string[] name = gameObject.name.Split ('.');
			long fileID = name.Length > 1 ? Int64.Parse (name [1]) : 0;

			// Get playlist
			PlaylistObj playlist = FindPlaylist (gameObject);

			// Get file
			if (playlist != null) {
				return playlist.Files.Find (x => x.ID == fileID);
			}
		}

		return null;
	}

	public void ShowDialog (string type) {
		ShowDialog (type, null);
	}

	public void ShowDialog (string type, GameObject obj)
	{
		if (dialog != null)
		{
			// Playlist object
			PlaylistObj playlist = FindPlaylist (obj);

			// Content elements
			Transform header = dialog.wrapper.transform.Find ("Header");
			Transform main = dialog.wrapper.transform.Find ("Main");
			Transform footer = dialog.wrapper.transform.Find ("Footer");

			// Header elements
			Text heading = header.Find ("Heading").GetComponent<Text> ();

			// Main elements
			Text text;
			InputField inputField;
			Text inputText;

			// Footer elements
			Button buttonOK = footer.Find ("Button_OK").GetComponent<Button> ();
			Text buttonOKText = footer.Find ("Button_OK").Find ("Text").GetComponent<Text> ();
			Button buttonCancel = footer.Find ("Button_Cancel").GetComponent<Button> ();
			Text buttonCancelText = footer.Find ("Button_Cancel").Find ("Text").GetComponent<Text> ();

			// Remove listener
			buttonOK.onClick.RemoveAllListeners ();


			switch (type) {

			// New playlist
			case "PL_ADD":
				
				// UI elements
				heading.text = "Neue Playlist erstellen";
				inputField = dialog.GetInputField ("", "Wie soll die neue Playlist heißen?");
				inputText = inputField.transform.Find ("Text").gameObject.GetComponent<Text> ();

				// Events
				buttonOK.onClick.AddListener (delegate {
					
					long id = NewPlaylist (inputText.text);

					switch (id) {

					case (long) Database.Constants.DuplicateFound:

						// TODO
						print ("Bereits vorhanden.");

						break;

					case (long) Database.Constants.QueryFailed:
						
						// TODO
						print ("Fehlgeschlagen.");

						break;

					default:
						
						HideDialog ();

						break;

					}
				});

				break;



			// Edit playlist
			case "PL_EDIT":
				
				if (playlist != null)
				{
					// UI elements
					heading.text = "Playlist bearbeiten";
					inputField = dialog.GetInputField (playlist.Name, playlist.Name);
					inputText = inputField.transform.Find ("Text").gameObject.GetComponent<Text> ();

					// Events
					buttonOK.onClick.AddListener (delegate {

						// Update playlist objects
						if (Settings.playlist != null && Settings.playlist.Equals (playlist)) {
							Settings.playlist.Name = inputText.text;
						}
						playlist.Name = inputText.text;

						// Update database
						bool edited = Edit (playlist);

						if (edited) {
							Load (true);
						} else {
							// TODO
							print ("Fehlgeschlagen.");
						}

						HideDialog ();
					});
				}
				else
				{
					return;
				}

				break;



			// Delete playlist
			case "PL_DEL":
				
				if (playlist != null)
				{
					// UI elements
					heading.text = "Playlist löschen";
					text = dialog.GetText ("Playlist \"" + playlist.Name + "\" endgültig löschen?");

					// Events
					buttonOK.onClick.AddListener (delegate {
						
						Delete (obj);
						Load (true);
						HideDialog ();
					});
				}
				else
				{
					return;
				}

				break;



			default:
				return;

			}

			// Show dialog
			dialog.dialog.SetActive (true);

			return;
		}

		return;
	}

	public void HideDialog ()
	{
		if (dialog != null) dialog.HideDialog ();
	}



	//-- DATABASE METHODS

	public void Load (bool displayPlaylists)
	{
		if (DbConnect ())
		{
			// Reset playlists list
			playlists = new List<PlaylistObj> ();

			// Database command
			SqliteCommand cmd = new SqliteCommand (db);

			// Query statement
			string sql = "SELECT id,name,files FROM playlist ORDER BY name ASC";
			cmd.CommandText = sql;

			// Get sql results
			SqliteDataReader reader = cmd.ExecuteReader ();

			// Read sql results
			while (reader.Read ())
			{
				// Create playlist object
				PlaylistObj obj = new PlaylistObj (reader.GetString (1));

				// Set ID
				obj.ID = reader.GetInt64 (0);

				// Get file IDs
				string[] fileIDs = !reader.IsDBNull (2) ? reader.GetString (2).Split (new Char[] { ',', ' ' }) : new string[0];

				// Select files
				List<FileObj> files = new List<FileObj> ();
				foreach (string id in fileIDs)
				{
					FileObj file = GetFile (Int64.Parse (id), false);
					if (file != null) {
						files.Add (file);
					}
				}

				// Set files
				obj.Files = files;

				// Add contents to playlists array
				playlists.Add (obj);
			}

			// Close reader
			reader.Close ();
			cmd.Dispose ();

			// Close database connection
			DbClose ();

			if (displayPlaylists) {
				Display ();
			}
		}
	}

	public long Create (PlaylistObj playlist)
	{
		if (DbConnect () && playlist != null && playlist.Name.Length > 0)
		{
			// SQL settings
			SqliteCommand cmd = null;
			SqliteDataReader reader = null;
			string sql = "";

			// Check if playlist files are already in database
			foreach (FileObj file in playlist.Files)
			{
				// Query statement
				sql = "SELECT id FROM file WHERE path = @Path";
				cmd = new SqliteCommand (sql, db);

				// Add Parameters to statement
				cmd.Parameters.Add (new SqliteParameter ("Path", file.Path));

				// Get sql results
				reader = cmd.ExecuteReader ();

				// Read and add file IDs
				int count = 0;
				while (reader.Read ()) {
					file.ID = reader.GetInt64 (0);
					count++;
				}

				// Close reader
				reader.Close ();
				cmd.Dispose ();

				// Add file to database if not already exists
				if (count == 0)
				{
					// Query statement
					sql = "INSERT INTO file (path) VALUES(@Path); " +
						"SELECT last_insert_rowid()";
					cmd = new SqliteCommand (sql, db);

					// Add Parameters to statement
					cmd.Parameters.Add (new SqliteParameter ("Path", file.Path));

					// Execute statement
					file.ID = (long) cmd.ExecuteScalar ();
					cmd.Dispose ();
				}
			}

			// Format file IDs
			string files = FormatFileIDs (playlist.Files);

			// Insert playlist into database
			try
			{
				sql = "INSERT INTO playlist (name,files) VALUES(@Name, @Files); " +
					"SELECT last_insert_rowid()";
				cmd = new SqliteCommand (sql, db);

				// Add Parameters to statement
				cmd.Parameters.Add (new SqliteParameter ("Name", playlist.Name));
				cmd.Parameters.Add (new SqliteParameter ("Files", files));

				// Execute insert statement and get ID
				long id = (long) cmd.ExecuteScalar ();

				// Close database connection
				cmd.Dispose ();
				DbClose ();

				return id;
			}
			catch (SqliteException)
			{
				// Close database connection
				DbClose ();

				return (long) Database.Constants.DuplicateFound;
			}
		}

		// Close database connection
		DbClose ();

		return (long) Database.Constants.QueryFailed;
	}

	public bool Edit (PlaylistObj playlist)
	{
		if (DbConnect () && playlist != null && playlist.Name.Length > 0)
		{
			try
			{
				// Query statement
				string sql = "UPDATE playlist SET name = @Name, files = @Files WHERE id = @ID";
				SqliteCommand cmd = new SqliteCommand (sql, db);

				// Add Parameters to statement
				cmd.Parameters.Add (new SqliteParameter ("Name", playlist.Name));
				cmd.Parameters.Add (new SqliteParameter ("Files", FormatFileIDs (playlist.Files)));
				cmd.Parameters.Add (new SqliteParameter ("ID", playlist.ID));

				// Result
				int result = cmd.ExecuteNonQuery ();

				// Close database connection
				cmd.Dispose ();
				DbClose ();

				return result > 0;
			}
			catch (SqliteException) {}
		}

		// Close database connection
		DbClose ();

		return false;
	}

	public bool AddFile (FileObj file, PlaylistObj playlist)
	{
		if (DbConnect () && file != null && playlist != null)
		{
			// Reset file ID
			file.ID = 0;

			// Update file ID: Query statement
			string sql = "SELECT id FROM file WHERE path = @Path";
			SqliteCommand cmd = new SqliteCommand (sql, db);

			// Add Parameters to statement
			cmd.Parameters.Add (new SqliteParameter ("Path", file.Path));

			// Get sql results
			SqliteDataReader reader = cmd.ExecuteReader ();

			// Read id
			while (reader.Read ()) {
				file.ID = reader.GetInt64 (0);
			}

			// Close reader
			reader.Close();
			cmd.Dispose ();

			// Add file if ID is still not valid
			if (!(file.ID > 0))
			{
				// Query statement
				sql = "INSERT INTO file (path) VALUES (@Path); " +
					"SELECT last_insert_rowid()";
				cmd = new SqliteCommand (sql, db);

				// Add Parameters to statement
				cmd.Parameters.Add (new SqliteParameter ("Path", file.Path));

				// Send query
				file.ID = (long) cmd.ExecuteScalar ();
				cmd.Dispose ();
			}

			if (!playlist.Files.Contains (file))
			{
				// Add file to playlist
				playlist.Files.Add (file);

				// Set file IDs
				string files = FormatFileIDs (playlist.Files);

				// Query statement
				sql = "UPDATE playlist SET files = @Files WHERE id = @ID";
				cmd = new SqliteCommand (sql, db);

				// Add Parameters to statement
				cmd.Parameters.Add (new SqliteParameter ("Files", files));
				cmd.Parameters.Add (new SqliteParameter ("ID", playlist.ID));

				// Result
				int result = cmd.ExecuteNonQuery ();

				// Close database connection
				cmd.Dispose ();
				DbClose ();

				// Add file to interface
				DisplayFile (playlist, file);

				return result > 0;
			}
		}

		// Close database connection
		DbClose ();

		return false;
	}

	public FileObj GetFile (long id, bool closeConnection)
	{
		if (DbConnect ())
		{
			// Send database query
			SqliteCommand cmd = new SqliteCommand (db);
			cmd.CommandText = "SELECT id,path FROM file WHERE id = @ID";

			// Add Parameters to statement
			cmd.Parameters.Add (new SqliteParameter ("ID", id));

			SqliteDataReader reader = cmd.ExecuteReader ();
			FileObj file = null;

			// Read and add file
			while (reader.Read ()) {
				file = new FileObj ();

				file.ID = reader.GetInt64 (0);
				file.Path = reader.GetString (1);
			}

			// Close reader
			reader.Close ();
			cmd.Dispose ();
			if (closeConnection) DbClose ();

			return file;
		}

		// Close database connection
		if (closeConnection) DbClose ();

		return null;
	}

	public FileObj GetFile (string path)
	{
		if (DbConnect ())
		{
			// Send database query
			SqliteCommand cmd = new SqliteCommand (db);
			cmd.CommandText = "SELECT id,path FROM file WHERE path = @File";

			// Add Parameters to statement
			cmd.Parameters.Add (new SqliteParameter ("File", path));

			SqliteDataReader reader = cmd.ExecuteReader ();
			FileObj file = null;

			// Read and add file
			while (reader.Read ()) {
				file = new FileObj ();

				file.ID = reader.GetInt64 (0);
				file.Path = reader.GetString (1);
			}

			// Close reader
			reader.Close ();
			cmd.Dispose ();
			DbClose ();

			return file;
		}

		// Close database connection
		DbClose ();

		return null;
	}

	public bool DeletePlaylist (PlaylistObj playlist)
	{
		if (DbConnect () && playlist != null)
		{
			// Query statement
			string sql = "DELETE FROM playlist WHERE id = @ID";
			SqliteCommand cmd = new SqliteCommand (sql, db);

			// Add Parameters to statement
			cmd.Parameters.Add (new SqliteParameter ("ID", playlist.ID));

			// Result
			int result = cmd.ExecuteNonQuery ();

			// Close database connection
			cmd.Dispose ();
			DbClose ();

			return result > 0;
		}

		// Close database connection
		DbClose ();

		return false;
	}

	public bool DeleteFile (PlaylistObj playlist, FileObj file)
	{
		if (DbConnect () && playlist != null && file != null && playlist.Files.Contains (file))
		{
			// Select files of playlist
			string sql = "SELECT files FROM playlist WHERE id = @ID";
			SqliteCommand cmd = new SqliteCommand (sql, db);

			// Add Parameters to statement
			cmd.Parameters.Add (new SqliteParameter ("ID", playlist.ID));

			// Get sql results
			SqliteDataReader reader = cmd.ExecuteReader ();

			// Read file IDs
			List<String> fileIDs = null;
			while (reader.Read ()) {
				if (!reader.IsDBNull (0)) {
					fileIDs = new List<String> (reader.GetString (0).Split (new Char[] { ',', ' ' }));
				}
			}

			// Close reader
			reader.Close();
			cmd.Dispose ();

			if (fileIDs != null && fileIDs.Contains (file.ID.ToString ()))
			{
				// Remove file
				fileIDs.Remove (file.ID.ToString ());

				// Update file IDs
				string files = FormatFileIDs (fileIDs);

				// Query statement
				sql = "UPDATE playlist SET files = @Files WHERE id = @ID";
				cmd = new SqliteCommand (sql, db);

				// Add Parameters to statement
				cmd.Parameters.Add (new SqliteParameter ("Files", files));
				cmd.Parameters.Add (new SqliteParameter ("ID", playlist.ID));

				// Result
				int result = cmd.ExecuteNonQuery ();

				// Close database connection
				cmd.Dispose ();
				DbClose ();

				return result > 0;
			}

			// TODO remove file if no other playlist contains it
		}

		// Close database connection
		DbClose ();

		return false;
	}



	//-- DATABASE CONNECTION

	public bool DbConnect ()
	{
		if (db == null) {
			db = Database.GetConnection ();
		}

		return db != null;
	}

	public void DbClose ()
	{
		// Close database
		Database.Close ();

		// Reset database instance
		db = null;
	}



	//-- HELPER METHODS

	public string FormatFileIDs (List<String> fileIDs)
	{
		// Create FileObj list
		List<FileObj> files = new List<FileObj> (fileIDs.Count);
		foreach (string id in fileIDs) {
			FileObj file = new FileObj ();
			file.ID = Int64.Parse (id);
			files.Add (file);
		}

		return FormatFileIDs (files);
	}

	public string FormatFileIDs (List<FileObj> files)
	{
		// Output
		List<string> IDs = new List<string> ();

		foreach (FileObj file in files) {
			if (file.ID != 0) {
				IDs.Add (file.ID.ToString ());
			}
		}

		return IDs.Count > 0 ? String.Join (",", IDs.ToArray ()) : null;
	}
}