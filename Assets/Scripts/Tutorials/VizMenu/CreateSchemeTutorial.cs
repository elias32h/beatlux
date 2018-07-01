﻿/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using UnityEngine;
using UnityEngine.EventSystems;

public class CreateSchemeTutorial : Tutorial2
{
    public Sprite Graphic = new Sprite();

    public override void CheckIfHappening()
    {
        //GameObject.Find("FocusShape").GetComponent<Image>().sprite = graphic;
        if (EventSystem.current.currentSelectedGameObject == GameObject.Find("Add"))
            TutorialManager2.Instace.CompletedTutorial();
    }
}