﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CreatePlaylistTutorial : Tutorial {

    public Sprite graphic = new Sprite();

    public override void CheckIfHappening()
    {
        GameObject.Find("FocusShape").GetComponent<Image>().sprite = graphic;
        if (EventSystem.current.currentSelectedGameObject == GameObject.Find("Add"))
        {
            TutorialManager.Instace.CompletedTutorial();
        }
    }
	
}
