using UnityEngine;
using System.Collections;

public class bl_CameraController : MonoBehaviour {

    [SerializeField]private Transform Target;
    [Header("Settings")]
    [Range(1,20)]public float LerpFollow = 7;
    [Range(1,20)]public float LerpRot = 10;
    [Range(0.01f,0.5f)]public float RateChange = 0.05f;
    [Range(10,170)]public float FoVTarget = 160;
    private bl_ChangerManager ChangerManager;

    private float distance;
    private float verticalDif;
    private float RateWaitFollow;
    private float RateWaitRot;
    private Camera m_Camera;
    private float DefaultFoV;
    private bool inZoom = false;
    private Vector3 defaultPosition;
    private Vector3 defaultRotation;
    private bool isPlay = false;
    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        ChangerManager = bl_ChangerManager.Instance;
        distance = transform.position.z - Target.position.z;
        verticalDif = transform.position.y - Target.position.y;
        m_Camera = GetComponentInChildren<Camera>();
        DefaultFoV = m_Camera.fieldOfView;
        defaultPosition = transform.position;
        defaultRotation = transform.eulerAngles;
    }

    /// <summary>
    /// 
    /// </summary>
    void OnEnable()
    {
        BlockBlast.bl_Event.Global.AddListener<BlockBlast.bl_GlobalEvents.OnChangeSide>(this.OnChange);
        BlockBlast.bl_Event.Global.AddListener<BlockBlast.bl_GlobalEvents.OnSlowMotion>(this.OnSlowMo);
        BlockBlast.bl_Event.Global.AddListener<BlockBlast.bl_GlobalEvents.OnStartPlay>(this.OnStartPlay);
        BlockBlast.bl_Event.Global.AddListener<BlockBlast.bl_GlobalEvents.OnFailGame>(this.OnFail);
    }

    /// <summary>
    /// 
    /// </summary>
    void OnDisable()
    {
        BlockBlast.bl_Event.Global.RemoveListener<BlockBlast.bl_GlobalEvents.OnChangeSide>(this.OnChange);
        BlockBlast.bl_Event.Global.RemoveListener<BlockBlast.bl_GlobalEvents.OnSlowMotion>(this.OnSlowMo);
        BlockBlast.bl_Event.Global.RemoveListener<BlockBlast.bl_GlobalEvents.OnStartPlay>(this.OnStartPlay);
        BlockBlast.bl_Event.Global.RemoveListener<BlockBlast.bl_GlobalEvents.OnFailGame>(this.OnFail);
    }

    void OnStartPlay(BlockBlast.bl_GlobalEvents.OnStartPlay e)
    {
        isPlay = true;
    }

    void OnFail(BlockBlast.bl_GlobalEvents.OnFailGame e)
    {
        isPlay = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    void OnSlowMo(BlockBlast.bl_GlobalEvents.OnSlowMotion e)
    {
        StopAllCoroutines();
        StartCoroutine(DoZoomEffect(e.Duration));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    void OnChange(BlockBlast.bl_GlobalEvents.OnChangeSide e)
    {
        RateWaitFollow = Time.time + RateChange / 2;
        RateWaitRot = Time.time + RateChange;
    }

    /// <summary>
    /// 
    /// </summary>
    void LateUpdate()
    {
        if (!Target)
            return;

        Follow();
        ZoomControl();
    }

    /// <summary>
    /// 
    /// </summary>
    void Follow()
    {
        if (RateWaitFollow < Time.time)
        {
            Vector3 p = (isPlay) ? Target.position : defaultPosition;
            if (isPlay)
            {
                p.z = p.z + distance;
                SetVerticalDif(ref p);
            }
            transform.position = Vector3.Lerp(transform.position, p, Time.deltaTime * BlockBlast.bl_Utils.MathfUtils.EaseLerpExtreme(LerpFollow));
        }

        if (RateWaitRot < Time.time)
        {
            Vector3 r = (isPlay) ? transform.eulerAngles : defaultRotation;

            if (isPlay)
            {
                r.z = GetRotAngle();
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(r), Time.deltaTime * BlockBlast.bl_Utils.MathfUtils.EaseLerpExtreme(LerpRot));
        }
    }

    
    void ZoomControl()
    {
        if (inZoom)
        {
            m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, FoVTarget, bl_GameManager.Instance.deltaTime);
        }
        else
        {
            m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, DefaultFoV, bl_GameManager.Instance.deltaTime * 4);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator DoZoomEffect(float duration)
    {
        inZoom = true;
        yield return StartCoroutine(BlockBlast.bl_Utils.CorrutinesUtils.WaitForRealSeconds(duration));
        inZoom = false;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    private void SetVerticalDif(ref Vector3 pos)
    {
        Vector3 p = pos;
        switch (ChangerManager.Side)
        {
            case LevelSides.Down:
                p.y = p.y + verticalDif;
                break;
            case LevelSides.Left:
                p.x = p.x + verticalDif;
                break;
            case LevelSides.Right:
                p.x = p.x - verticalDif;
                break;
            case LevelSides.Up:
                p.y = p.y - verticalDif;
                break;
        }
        pos = p;
    }

    private float GetRotAngle()
    {
        switch (ChangerManager.Side)
        {
            case LevelSides.Down:
                return 0;
            case LevelSides.Left:
                return -90;
            case LevelSides.Right:
                return 90;
            case LevelSides.Up:
                return 180;
        }
        return 0;
    }
}