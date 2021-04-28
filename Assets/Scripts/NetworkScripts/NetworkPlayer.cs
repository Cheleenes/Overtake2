using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class NetworkPlayer : NetworkBehaviour
{
    public enum PlayerState
    {
        Empty,
        FromSelected,
        Cutting
    }
    int playerId_;
    GameObject fromCellGO_;
    GameObject toCellGO_;
    PlayerState state_;
    NetworkTeam myTeam_;

    public PlayerState State_ { get => state_; }

    // Start is called before the first frame update
    public void Initialize(int playerId, NetworkTeam myTeam)
    {
        playerId_ = playerId;
        state_ = PlayerState.Empty;
        myTeam_ = myTeam;
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        TouchControllers();
    }

    void TouchControllers()
    {
        switch (state_)
        {
            case PlayerState.Empty:
                if (Input.touchCount > 0)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Moved)
                    {
                        Vector3 ray = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                        RaycastHit2D hitInfo = Physics2D.Raycast(ray, Vector2.zero);
                        if (hitInfo)
                        {
                            if (hitInfo.collider.gameObject.tag == "Cell" && myTeam_.HasCell_(hitInfo.collider.gameObject.GetComponent<NetworkCell>()))
                            {
                                state_ = PlayerState.FromSelected;
                                fromCellGO_ = hitInfo.collider.gameObject;
                            }
                        }
                        else
                        {
                            state_ = PlayerState.Cutting;
                        }
                    }
                }
                break;

            case PlayerState.FromSelected:
                if (Input.touchCount > 0)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Ended)
                    {
                        Vector3 ray = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                        RaycastHit2D hitInfo = Physics2D.Raycast(ray, Vector2.zero);
                        if (hitInfo && hitInfo.collider.gameObject.tag == "Cell")
                        {
                            state_ = PlayerState.Empty;
                            toCellGO_ = hitInfo.collider.gameObject;
                            fromCellGO_.GetComponent<NetworkCell>().AttackCell(toCellGO_);
                            fromCellGO_ = null;
                            toCellGO_ = null;
                        }
                    }
                    else if (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled)
                    {
                        fromCellGO_ = null;
                        toCellGO_ = null;
                        state_ = PlayerState.Empty;
                    }
                }
                break;

            case PlayerState.Cutting:
                if (Input.touchCount > 0)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Stationary)
                    {
                        Vector3 ray = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                        RaycastHit2D hitInfo = Physics2D.Raycast(ray, Vector2.zero);
                        if (hitInfo)
                        {
                            if (hitInfo.collider.gameObject.tag == "BodyPart" && myTeam_.HasCell_(hitInfo.collider.gameObject.GetComponentInParent<NetworkTentacle>().ParentCell_))
                            {
                                hitInfo.collider.gameObject.GetComponentInParent<NetworkTentacle>().GetCut(hitInfo.collider.gameObject);
                            }
                        }
                    }
                    else if (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled)
                    {
                        state_ = PlayerState.Empty;
                    }
                }
                break;
        }
    }
}
