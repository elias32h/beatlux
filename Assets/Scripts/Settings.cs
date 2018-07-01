﻿/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using System.Collections.Generic;
using UnityEngine;

public abstract class Settings
{
    // Available visualizations
    public static readonly VisualizationObj[] Visualizations =
    {
        new VisualizationObj("Bubbles", 4, 6, "Watercolor_2"),
        new VisualizationObj("Hexagons", 2, 7, "Geo_3"),
        new VisualizationObj("Lichterketten", 5, 8),
        new VisualizationObj("Particle Fountains", 6, 9),
        new VisualizationObj("Spektrum", 3, 10, "Nightsky")
    };

    public static MenuFunctions MenuManager;


    //-- HELPER METHODS
    public static Color GetColorFromRgb(int r, int g, int b)
    {
        if (r >= 0 && r <= 255 &&
            g >= 0 && g <= 255 &&
            b >= 0 && b <= 255)
            return new Color(r / 255.0f, g / 255.0f, b / 255.0f);

        return Color.clear;
    }


    //-- ACTIVE ELEMENTS
    public abstract class Active
    {
        public static PlaylistObj Playlist;
        public static FileObj File;
        public static VisualizationObj Visualization;
        public static ColorSchemeObj ColorScheme;

        public static int Scene = 0;
    }

    //-- OPENED ELEMENTS
    public abstract class Selected
    {
        public static PlaylistObj Playlist;
        public static FileObj File;
        public static VisualizationObj Visualization;
        public static ColorSchemeObj ColorScheme;
    }

    //-- SOURCE FOLDER PATHS
    public abstract class Source
    {
        public static string Main;
        public static string Current;
    }

    //-- DEFAULT SETTINGS
    public abstract class Defaults
    {
        public static readonly VisualizationObj Visualization = new VisualizationObj("beatlux", 1, 5, "Nebula");


        //-- DEFAULT COLORS
        public static readonly Dictionary<string, Color[]> Colors = new Dictionary<string, Color[]>
        {
            // Default visualization (invisible)
            {
                Visualization.Name,
                new Color[] { }
            },


            // Bubbles
            {
                Visualizations[0].Name,
                new[]
                {
                    GetColorFromRgb(243, 233, 177),
                    GetColorFromRgb(172, 231, 243),
                    GetColorFromRgb(243, 205, 233),
                    GetColorFromRgb(190, 255, 252)
                }
            },

            // Hexagons
            {
                Visualizations[1].Name,
                new[]
                {
                    GetColorFromRgb(66, 70, 110),
                    GetColorFromRgb(58, 98, 171)
                }
            },

            // Lichterketten
            {
                Visualizations[2].Name,
                new[]
                {
                    GetColorFromRgb(242, 80, 166),
                    GetColorFromRgb(242, 209, 60),
                    GetColorFromRgb(84, 191, 60),
                    GetColorFromRgb(37, 176, 217),
                    GetColorFromRgb(242, 84, 44)
                }
            },

            // Particle Fountains
            {
                Visualizations[3].Name,
                new[]
                {
                    GetColorFromRgb(255, 26, 229),
                    GetColorFromRgb(23, 191, 255),
                    GetColorFromRgb(20, 255, 53),
                    GetColorFromRgb(232, 23, 83),
                    GetColorFromRgb(255, 147, 12),
                    GetColorFromRgb(255, 232, 51)
                }
            },

            // Spektrum
            {
                Visualizations[4].Name,
                new[]
                {
                    GetColorFromRgb(6, 104, 255),
                    GetColorFromRgb(0, 242, 255),
                    GetColorFromRgb(27, 240, 96)
                }
            }
        };
    }

    //-- INPUT SETTINGS
    public abstract class Input
    {
        public static int MaxLength = 30;
        public static Color InfoColor = GetColorFromRgb(250, 80, 80);
    }

    //-- PLAYER SETTINGS
    public abstract class Player
    {
        public const float Volume = 0.6f;
        public static bool Shuffle = false;
        public static int RepeatMode = -1; // -1=off, 0=all, 1=single
        public static bool ShuffleViz = false;

        public static bool TutorialTog = true;
    }
}