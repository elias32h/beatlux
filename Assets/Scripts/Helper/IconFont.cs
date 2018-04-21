﻿/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using UnityEngine;
using System.Collections;


/// <summary>
/// Provides variables to display icons using the icon font.
/// </summary>
public class IconFont : MonoBehaviour {

	/// <summary>
	/// The icon font resource.
	/// </summary>
	public static Font font = Resources.Load<Font> ("Fonts/beatlux");

	public const string VISUALIZATION = "\ue900";
	public const string DROPDOWN_OPENED = "\ue901";
	public const string DROPDOWN_CLOSED = "\ue902";
	public const string OPTIONS = "\ue903";
	public const string LISTENING = "\ue904";
	public const string SEARCH = "\ue905";
	public const string TRASH = "\ue906";
	public const string MUSIC = "\ue907";
	public const string FOLDER = "\ue908";
	public const string PAUSE = "\ue909";
	public const string PLAY = "\ue90a";
	public const string ADD = "\ue90b";
	public const string CLOSE_COMPRESSED = "\ue90c";
	public const string CLOSE = "\ue90d";
	public const string SHUFFLE = "\ue90e";
	public const string EDIT = "\ue90f";
	public const string FAST_FORWARD = "\ue910";
	public const string REWIND = "\ue911";
	public const string REPEAT = "\ue912";
	public const string ARROW_LEFT = "\ue913";
	public const string ARROW_BACK = "\ue914";
	public const string HOME = "\ue915";
	public const string VIZ_NEXT = "\ue916";
	public const string VIZ_PREV = "\ue917";
	public const string VIZ_NEXT_OLD = "\ue918";
	public const string VIZ_PREV_OLD = "\ue919";
	public const string LOCK = "\ue91a";
	public const string REPEAT_SINGLE = "\ue91b";

}