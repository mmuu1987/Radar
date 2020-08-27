using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{

    public Text DesText;

    public ScriptableObject ScriptableObject;

    private Button MyButton;
	// Use this for initialization
	void Start () {

        this.GetComponent<ListenTuio>().PointExit += Test_PointExit;
        this.GetComponent<ListenTuio>().PointEnter += Test_PointEnter;
        this.GetComponent<ListenTuio>().OnClick += Test_OnClick;
	    MyButton = this.GetComponent<Button>();
        MyButton.onClick.AddListener((() =>
        {
            Debug.LogError("手动点击按钮触发的事件"+this.name);
        }));
		
	}

    void Test_OnClick()
    {
        DesText.text = "Test_OnClick " + this.name;
    }

    void Test_PointEnter()
    {
        DesText.text = "Test_PointEnter" + this.name;
    }

    void Test_PointExit()
    {
        DesText.text = "Test_PointExit" + this.name;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
