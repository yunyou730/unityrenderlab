using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using lan.game;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MenuDebugInfo : MonoBehaviour
{
    private TextMeshProUGUI _txtPathShowing = null;

    private void Awake()
    {
        GameObject go = transform.Find("Text_PathShowing").gameObject;
        _txtPathShowing = go.GetComponent<TextMeshProUGUI>();
        //var a = go.GetComponent<Text>();
        //Debug.Log("xxx");

        //var comps = go.GetComponents();
        
    }

    void Start()
    {
        _txtPathShowing.text = GetPathContent();
        Application.logMessageReceived += LogCallback;
    }
    
    string GetPathContent()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("[persist]{0}\n[stream]{1}",
                            Application.persistentDataPath,
                            Application.streamingAssetsPath);
        return sb.ToString();
    }
    
    public void LogCallback(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Assert || type == LogType.Exception)
        {
            _txtPathShowing.text += condition + "\n" + stackTrace;
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.W))
        {
            _txtPathShowing.text += "w press";
            //GameObject go = null;//new GameObject();
            //go.name = "sss";
        }
    }
}
