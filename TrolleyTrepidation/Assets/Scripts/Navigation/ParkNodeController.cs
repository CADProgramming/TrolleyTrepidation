using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkNodeController : MonoBehaviour
{
    private const float XOFFSETCLOSE = 4.6f;
    private const float ZOFFSETCLOSE = 5.0f;
    private const float XOFFSETFAR = -7.5f;
    private const float ZOFFSETFAR = 8.0f;

    public bool isEmpty;
    public Vector3 closeEntrance;
    public Vector3 farEntrance;

    // Start is called before the first frame update
    void Start()
    {
        isEmpty = true;

        closeEntrance  = transform.position + transform.rotation * new Vector3(XOFFSETCLOSE, 0, ZOFFSETCLOSE);
        farEntrance = transform.position + transform.rotation * new Vector3(XOFFSETFAR, 0, ZOFFSETFAR);
    }
}
