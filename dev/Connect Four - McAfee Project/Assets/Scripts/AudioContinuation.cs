using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioContinuation : MonoBehaviour
{
    private static AudioContinuation _instance ;

    void Awake()
    {
        if (!_instance)
            _instance = this ;
        else
            Destroy(this.gameObject) ;

        DontDestroyOnLoad(transform.gameObject);
    }
}
