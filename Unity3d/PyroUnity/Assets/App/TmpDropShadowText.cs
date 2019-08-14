using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmpDropShadowText : MonoBehaviour
{
    public string Text
    {
        get { return _text; }
        set { Top.text = Bottom.text = _text = value; }
    }
    public TMPro.TMP_Text Top;
    public TMPro.TMP_Text Bottom;

    private string _text;
}
