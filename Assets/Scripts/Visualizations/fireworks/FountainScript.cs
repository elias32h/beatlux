﻿using UnityEngine;
using System.Collections;

public class FountainScript : MonoBehaviour {

    Color col1;
    Color col2;
    Color col3;
    Color col5;
    Color col6;
    Color col7;
    float[] samples = new float[1024];
    Color[] col4;
    int count = 0;
    int colVal1;
    int colVal2;
    float comp = 0f;
    bool bol = false;
    float maxVol = 0;
    


	// Use this for initialization
	void Start () {
        this.GetComponent<ParticleSystem>().Play();
        if (Settings.Active.ColorScheme == null)
        {
            col1 = new Color32(243, 225, 125, 255);
            col2 = new Color32(125, 243, 156, 255);
            col3 = new Color32(243, 153, 149, 255);
        }
        else
        {
            col1 = Settings.Active.ColorScheme.Colors[0];
            col2 = Settings.Active.ColorScheme.Colors[1];
            col3 = Settings.Active.ColorScheme.Colors[2];
            col5 = Settings.Active.ColorScheme.Colors[3];
            col6 = Settings.Active.ColorScheme.Colors[4];
            col7 = Settings.Active.ColorScheme.Colors[5];
        }

        col4  = new Color[] { col1, col2, col3, col5, col6, col7 };
	}
	
	// Update is called once per frame
	void Update () {

        AudioListener.GetOutputData(samples, 0);
        float vol = 0;
        foreach (float sample in samples)
        {
            if (sample >= 0)
            {
                vol += sample;
            }
            else vol -= sample;
        }
        if (maxVol < vol) maxVol = vol;
        if (vol <= 0)
        {
            bol = true;
            this.GetComponent<ParticleSystem>().Stop();
            
        } else if (vol>0&&bol== true){
            this.GetComponent<ParticleSystem>().Play();
            bol = false;
        }
        

        //switches between selected colors
        if (count == 0)
        {
            colVal1 = Random.Range(0,col4.Length);
            colVal2 = Random.Range(0,col4.Length);
        }
        else if (count == 100)
        {
            colVal1 = Random.Range(0,col4.Length);
            colVal2 = Random.Range(0,col4.Length);
        }else if (count == 200)
        {
            colVal1 = Random.Range(0,col4.Length);
            colVal2 = Random.Range(0,col4.Length);
        }
        else if (count == 300)
        {
            count = 0;
        }
        
        // controlls color
        ParticleSystem ps = this.GetComponent<ParticleSystem>();
        ps.startLifetime = 3.5f * vol / maxVol;
        ps.startColor = Color.Lerp(col4[colVal1],col4[colVal2],Time.time);
        
        count++;

        // controls angle
        var sh = ps.shape;
        sh.enabled = true;
        if (comp < AudioPeer.freqBands[0])
        {
            comp = AudioPeer.freqBands[0];
            sh.angle = sh.angle+0.4f;
        }
        else if (comp > AudioPeer.freqBands[0]&&sh.angle>3.0)
        {
            comp = AudioPeer.freqBands[0];
            sh.angle = sh.angle - 0.4f;
        }
        
        

	}

}
