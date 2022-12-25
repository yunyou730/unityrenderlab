using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
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
    }

    string GetPathContent()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("[persist]{0}\n[stream]{1}",
                            Application.persistentDataPath,
                            Application.streamingAssetsPath);
        return sb.ToString();
    }

}
