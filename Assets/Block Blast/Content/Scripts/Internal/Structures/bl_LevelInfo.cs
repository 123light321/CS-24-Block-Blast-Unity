using UnityEngine;
using System;

[Serializable]
public class bl_LevelInfo  {
    public string Name;
    public string SubName;
    public int Speed;
    public int PointsNeeded;
    public int ProgresiveNeeded;
    public int ObstaclesLevel;
    public bool isNewLevel = false;
    [Header("References")]
    public AudioClip m_AudioClip;
}