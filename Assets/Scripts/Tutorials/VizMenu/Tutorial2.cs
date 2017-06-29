﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tutorial2 : MonoBehaviour {

    public int Order;
    [TextArea(6, 15)]
    public string Explanation;

    void Awake()
    {
        TutorialManager2.Instace.Tutorials.Add(this);
    }

    public virtual void CheckIfHappening() { }
}
