using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     目前只支持Camera的Render Mode = Screen Space - Overly
/// </summary>
public class ListenTuio : MonoBehaviour
{
    /// <summary>
    ///     触摸是否进入
    /// </summary>
    private bool _isEnter;

    /// <summary>
    ///     触摸出现的时候，是否是首先进入该监听区域
    /// </summary>
    private bool _isFirstEnter;

    private Vector2 _leftDown;
    private Vector2 _leftUp;
    private MaskableGraphic _maskableGraphic;

    private Vector2 _pos;
    private Vector2 _rightDown;
    private Vector2 _rightUp;
    private Vector2 _sizeData;

    public event Action OnClick;

    public event Action PointEnter;

    public event Action PointExit;

    public event Action Pointing;

    private void EnterEvent(EventInfo info)
    {
        var pos = info.Position;

        var isContains = GlobSetting.ContainsQuadrangle(_leftDown, _leftUp, _rightDown, _rightUp, pos);

        if (isContains)
        {
            Debug.Log("EnterEvent  触摸进入了该组件" + name);
            _isEnter = true;
            if (PointEnter != null) PointEnter();
            _isFirstEnter = true;
        }
        else
        {
            _isEnter = false;
        }
    }

    private void EnteringEvent(EventInfo info)
    {
        var pos = info.Position;

        var isContains = GlobSetting.ContainsQuadrangle(_leftDown, _leftUp, _rightDown, _rightUp, pos);

        if (isContains)
        {
            if (!_isEnter)
                //Debug.Log("EnteringEvent 进入了该组件" + this.name);
                if (PointEnter != null)
                    PointEnter();

            _isEnter = true;
        }
        else
        {
            if (_isEnter)
                //Debug.Log(_leftDown + " " + _leftUp + " " + _rightDown + " " + _rightUp + " "+ pos);
                // Debug.Log("EnteringEvent  触摸离开了该组件 " +this.name);
                if (PointExit != null)
                    PointExit();
            _isEnter = false;
        }
    }

    private void ExitEvent(EventInfo info)
    {
        var pos = info.Position;

        var isContains = GlobSetting.ContainsQuadrangle(_leftDown, _leftUp, _rightDown, _rightUp, pos);

        if (isContains)
        {
            if (_isFirstEnter)
            {
                if (OnClick != null)
                    OnClick();
                Debug.Log("触发了点击事件 " + name);
                _isFirstEnter = false;
            }
        }
        else
        {
            if (_isEnter)
                //Debug.Log("ExitEvent 触摸离开了该组件" + this.name);
                if (PointExit != null)
                    PointExit();
            _isEnter = false;
        }
    }

    private void Start()
    {
        _maskableGraphic = GetComponent<MaskableGraphic>();
        TuioManager.Instance.EnterEvent += EnterEvent;
        TuioManager.Instance.EnteringEvent += EnteringEvent;
        TuioManager.Instance.ExitEvent += ExitEvent;

        _sizeData = _maskableGraphic.rectTransform.sizeDelta;

        _pos = _maskableGraphic.rectTransform.position;

        _leftDown = _pos + new Vector2(-_sizeData.x / 2, -_sizeData.y / 2);

        _leftUp = _pos + new Vector2(-_sizeData.x / 2, _sizeData.y / 2);

        _rightDown = _pos + new Vector2(_sizeData.x / 2, -_sizeData.y / 2);

        _rightUp = _pos + new Vector2(_sizeData.x / 2, _sizeData.y / 2);

        // Debug.Log(_sizeData);
    }

    // Update is called once per frame
    private void Update()
    {
    }
}