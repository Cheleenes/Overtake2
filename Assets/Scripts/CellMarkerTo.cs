using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellMarkerTo : MonoBehaviour
{
    [SerializeField] NetworkPlayer player_;
    Vector3 outside;
    // Start is called before the first frame update
    void Start()
    {
        outside = new Vector3(0, -10, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (player_ != null)
        {
            switch (player_.state_)
            {
                case NetworkPlayer.PlayerState.Empty:
                    transform.position = outside;
                    break;

                case NetworkPlayer.PlayerState.FromSelected:
                    if (player_.toCellGO_ != null)
                    {
                        transform.position = player_.toCellGO_.transform.position;
                    }
                    break;
            }
        }
    }

    public void SetPlayer(NetworkPlayer player)
    {
        player_ = player;
    }
}
