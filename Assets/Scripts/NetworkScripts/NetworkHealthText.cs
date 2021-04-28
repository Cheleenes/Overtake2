using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using UnityEngine.UI;

public class NetworkHealthText : NetworkBehaviour
{
    NetworkCell cell_;
    Transform cellTransoform_;
    Text text_;
    RectTransform Rect_;
    public void Initialize(NetworkCell cell, Transform cellTransform)
    {
        cell_ = cell;
        cellTransoform_ = cellTransform;
        text_ = GetComponent<Text>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (cell_)
            text_.text = cell_.CurrentHealth_.ToString();
    }
}
