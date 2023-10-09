using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInput : MonoBehaviour
{
    #region Singleton
    private static MouseInput _instance;
    public static MouseInput Instance { get { return _instance; } }

    private Camera cam;
    public Ray cameraRay;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion
    private void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        cameraRay = cam.ScreenPointToRay(Input.mousePosition);        
    }
}
