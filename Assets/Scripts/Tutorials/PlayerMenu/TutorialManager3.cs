﻿/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager3 : MonoBehaviour
{
    private static TutorialManager3 _instance;

    private Tutorial3 currentTutorial;
    public Text ExpText;

    public List<Tutorial3> Tutorials = new List<Tutorial3>();

    //creates an instance of the Tutorialmanager
    public static TutorialManager3 Instace
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<TutorialManager3>();

            if (_instance == null) Debug.Log("There is no TutorialManager");

            return _instance;
        }
    }

    // sets the tutorialstart to tutorial 0
    private void Start()
    {
        Init();
    }

    public void Init()
    {
        SetNextTutorial(0);
    }

    // if a tutorial is there, checks if sth is happening
    public void Update()
    {
        if (currentTutorial) currentTutorial.CheckIfHappening();
    }

    // called if a tutorial is finished
    public void CompletedTutorial()
    {
        SetNextTutorial(currentTutorial.Order + 1);
    }

    // sets next tutorial if possible, else finish
    private void SetNextTutorial(int currentOrder)
    {
        currentTutorial = GetTutorialByOrder(currentOrder);

        if (!currentTutorial)
        {
            CompletedAllTutorials();
            SetNextTutorial(0);
        }

        ExpText.text = Settings.MenuManager.LangManager.getString("PM0" + currentOrder + "");
    }

    // text if every tutorial is finished
    private void CompletedAllTutorials()
    {
        ExpText.text = "You completed all the tutorials";
    }

    // gets the tutorial order
    private Tutorial3 GetTutorialByOrder(int order)
    {
        return Tutorials.FirstOrDefault(t => t.Order == order);
    }
}