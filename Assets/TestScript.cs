using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public BeatNodes BN_Current;
    private void Awake()
    {
        Debug.Log("Total Beat Count:"+BN_Current.GetNodes().Count);
    }
}
