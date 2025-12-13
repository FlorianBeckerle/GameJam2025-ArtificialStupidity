using Player;
using UnityEngine;

public class CameraRig : MonoBehaviour, ICameraRig
{
    [SerializeField]
    private Camera camera;
    
    [SerializeField]
    private float camDistance = 5f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (camera == null)
        {
            Debug.LogError($"[CameraRig] Could not find camera");
            Exit();
        }
    }

    // Update is called once per frame
    void Update()
    {
        camera.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -camDistance);
    }

    public void Enter()
    {
        this.enabled = true;
    }

    public void Exit()
    {
        this.enabled = false;
    }
}
