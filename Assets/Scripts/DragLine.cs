using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragLine : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer_;
    [SerializeField] Vector3 linePoint0_;
    [SerializeField] Vector3 linePoint1_;
    [SerializeField] NetworkPlayer player_;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer_ = GetComponent<LineRenderer>();
        linePoint0_ = Vector3.zero;
        linePoint1_ = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (player_ != null)
        {
            switch (player_.state_)
            {
                case NetworkPlayer.PlayerState.Empty:
                    linePoint0_ = Vector3.zero;
                    linePoint1_ = Vector3.zero;
                    break;

                case NetworkPlayer.PlayerState.FromSelected:
                    if (player_.toCellGO_ != null)
                    {
                        linePoint0_ = player_.fromCellGO_.transform.position;
                        linePoint1_ = player_.toCellGO_.transform.position;
                    }
                    else
                    {
                        linePoint0_ = player_.fromCellGO_.transform.position;
                        if (Application.isEditor)
                            linePoint1_ = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);
                        else
                            linePoint1_ = new Vector3(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position).x, Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position).y, 0);
                    }
                    break;
            }
        }
        lineRenderer_.SetPosition(0, linePoint0_);
        lineRenderer_.SetPosition(1, linePoint1_);
    }

    public void SetPlayer(NetworkPlayer player)
    {
        player_ = player;
    }
}
