using UnityEngine;
using System.Collections;

public class bl_SideLimitDetector : MonoBehaviour {

    public LevelSides RightChange;
    public LevelSides LeftChange;
    [Header("Gizmo")]
    public Color m_GizmoColor;
    public bool DrawGizmo = false;
    private bl_ChangerManager CM;

    void Awake()
    {
        CM = bl_ChangerManager.Instance;
    }

    void OnTriggerStay(Collider c)
    {
        if(c.tag == "Player")
        {
            if (BlockBlast.bl_Utils.IsMobile)
            {
                OnMobile();
            }
            else
            {
                OnNonMobile();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void OnNonMobile()
    {

        if (CM.Side == LeftChange)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (CM.Side == RightChange)
                    return;

                CM.Change(RightChange,false);
            }
        }

        if (CM.Side == RightChange)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (CM.Side == LeftChange)
                    return;

                CM.Change(LeftChange, false);
            }
        }

    }

    /// <summary>
    /// 
    /// </summary>
    void OnMobile()
    {
        for (var i = 0; i < Input.touchCount; ++i)
        {
            Touch touch = Input.GetTouch(i);
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
            {
                float buttonsize = Screen.width / 3;

                if (CM.Side == LeftChange)
                {
                    if (touch.position.x >= (buttonsize * 2))
                    {
                        if (CM.Side == RightChange)
                            return;

                        CM.Change(RightChange, false);
                    }
                }

                if (CM.Side == RightChange)
                {
                    if (touch.position.x >= 0 && touch.position.x < buttonsize)
                    {
                        if (CM.Side == LeftChange)
                            return;

                        CM.Change(LeftChange, false);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void OnDrawGizmos()
    {
        if (!DrawGizmo)
            return;

        Collider c = GetComponent<Collider>();
        Gizmos.color = m_GizmoColor;
        Gizmos.DrawCube(c.bounds.center, c.bounds.size);
    }
}