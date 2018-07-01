﻿/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [TextArea(6, 15)] public string Explanation;

    public int Order;

    // called on awake to add tutorials to tutorialmanager
    private void Awake()
    {
        TutorialManager.Instace.Tutorials.Add(this);
    }

    public virtual void CheckIfHappening()
    {
    }
}