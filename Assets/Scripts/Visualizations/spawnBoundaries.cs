﻿/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;


public class spawnBoundaries : MonoBehaviour {
    public GameObject boundaries;
    public GameSettings gameSettings;
    public GameObject mirrors;
    public bool mirrorsOnOff = false;
   

    /**
     * Spawns invisible walls around the middle of the gameobject it is put on.
     **/
    void Start()
    {
        Instantiate(boundaries, Vector3.zero, Quaternion.identity, Settings.MenuManager.vizContents);
        
        if (File.Exists(Application.persistentDataPath + "/gamesettings.json") == true && mirrorsOnOff)
        {
            gameSettings = JsonUtility.FromJson<GameSettings>(File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));
            switch (gameSettings.mirrors)
            {
                case 0:
                    break;
                case 1:
                    Instantiate(mirrors, new Vector3(0,-10,0), Quaternion.identity, Settings.MenuManager.vizContents);
                    mirrors.GetComponent<MirrorReflection>().m_TextureSize = 256;
                    break;
                case 2:
                    Instantiate(mirrors, mirrors.transform.position, Quaternion.identity, Settings.MenuManager.vizContents);
                    mirrors.GetComponent<MirrorReflection>().m_TextureSize = 512;
                    break;
                default:
                    break;
            }

        }
    }
	
}
