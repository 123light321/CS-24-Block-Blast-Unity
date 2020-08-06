using UnityEngine;
using System.Collections.Generic;

public class bl_ChangerManager : Singleton<bl_ChangerManager> {

    [Header("Settings")]
    public List<bl_SidesInfo> Sides = new List<bl_SidesInfo>();
    [Header("References")]
    [SerializeField]private Transform LevelRoot;

    private bl_SidesInfo CurrentSide;

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        CurrentSide = Sides[3];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="detector"></param>
    public void Change(bl_ChangeDetector detector)
    {
        CurrentSide = GetSide(detector);
        new BlockBlast.bl_GlobalEvents.OnChangeSide(CurrentSide.Side,false);
        bl_PlayerController.Instance.DoChanger();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="detector"></param>
    public void Change(LevelSides side, bool isFlip, bool smooth = false)
    {
        CurrentSide = GetSide(side);
        new BlockBlast.bl_GlobalEvents.OnChangeSide(side,isFlip);
        bl_PlayerController.Instance.DoChanger(smooth);
    }

    /// <summary>
    /// 
    /// </summary>
    public void Flip()
    {
        Change(GetSideInfo.Oposite,true,true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="de"></param>
    /// <returns></returns>
    private bl_SidesInfo GetSide(bl_ChangeDetector de)
    {
        for(int i = 0; i < Sides.Count; i++)
        {
            if(Sides[i].Side == de.Side)
            {
                return Sides[i];
            }
        }
        return Sides[0];
    }

    private bl_SidesInfo GetSide(LevelSides sid)
    {
        for (int i = 0; i < Sides.Count; i++)
        {
            if (Sides[i].Side == sid)
            {
                return Sides[i];
            }
        }
        return Sides[0];
    }

    /// <summary>
    /// 
    /// </summary>
    public LevelSides Side
    {
        get
        {
            return CurrentSide.Side;
        }
    }

    public bl_SidesInfo GetSideInfo
    {
        get
        {
            return CurrentSide;
        }
    }

    public bool IsVertical
    {
        get
        {
            return (Side == LevelSides.Down || Side == LevelSides.Up);
        }
    }

    public static bl_ChangerManager Instance
    {
        get
        {
            return ((bl_ChangerManager)mInstance);
        }
        set
        {
            mInstance = value;
        }
    }
}