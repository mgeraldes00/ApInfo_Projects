using System.Collections;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    // Default: 1.8, 0.5
    private const float sizeIncrease = 2.1f, sizeIncrement = 0.5f;
    private const float zoomStep = 1f;

    [SerializeField] private CameraSpace camSpace;

    [SerializeField] private Camera cam;

    [SerializeField] private Transform target;

    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector2 speed = Vector2.one;

    [SerializeField] private SpriteRenderer space;

    [SerializeField] private float startSize, camSize, increaseCounter;
    [SerializeField] private float minCamSize, maxCamSize;

    private Vector3 dragOrigin;

    private float spaceMinX, spaceMaxX, spaceMinY, spaceMaxY;

    private void Awake()
    {
        cam = gameObject.GetComponent<Camera>();
        camSpace = FindObjectOfType<CameraSpace>();

        startSize = cam.orthographicSize;
        camSize = startSize;

        spaceMinX = space.transform.position.x - space.bounds.size.x / 2;
        spaceMaxX = space.transform.position.x + space.bounds.size.x / 2;
        spaceMinY = space.transform.position.y - space.bounds.size.y / 2;
        spaceMaxY = space.transform.position.y + space.bounds.size.y / 2;
    }

    private void Update()
    {
        /*if (target != null)
        {
            Vector3 newPos;
            
            newPos.x = target.position.x + offset.x;
            newPos.y = target.position.y + (target.position.y * offset.y);    
            newPos.z = transform.position.z;

            Vector3 delta = newPos - transform.position;

            newPos.x = transform.position.x + delta.x * Time.deltaTime / speed.x;
            newPos.y = transform.position.y + delta.y * Time.deltaTime / speed.y;

            cam.transform.position = ClampCamera(newPos);
        }*/

        PanCamera();

        if (Input.GetAxis("Mouse ScrollWheel") > 0) ZoomIn();
        else if (Input.GetAxis("Mouse ScrollWheel") < 0) ZoomOut();
    }

    private void PanCamera()
    {
        if (Input.GetMouseButtonDown(0))
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButton(0))
        {
            Vector3 difference = 
                dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);

            cam.transform.position = 
                ClampCamera(cam.transform.position + difference);
        }
    }

    private void ZoomIn()
    {
        float newSize = cam.orthographicSize - zoomStep;

        cam.orthographicSize = 
            Mathf.Clamp(newSize, minCamSize, 
            Mathf.Min(maxCamSize, (space.bounds.size.x / 2f) / cam.aspect));
        
        cam.transform.position = ClampCamera(cam.transform.position);
    }

    private void ZoomOut()
    {
        float newSize = cam.orthographicSize + zoomStep;
        cam.orthographicSize = 
            Mathf.Clamp(newSize, minCamSize, 
            Mathf.Min(maxCamSize, (space.bounds.size.x / 2f) / cam.aspect));
        
        cam.transform.position = ClampCamera(cam.transform.position);
    }

    public IEnumerator IncreaseSize()
    {
        UpdateSpace();

        while(increaseCounter < sizeIncrease)
        {
            increaseCounter += sizeIncrement * Time.deltaTime;
            camSize = startSize + increaseCounter;
            cam.orthographicSize = camSize;
            cam.transform.position = ClampCamera(cam.transform.position);
            yield return null;
        }

        increaseCounter = 0;
        startSize = cam.orthographicSize;
    }

    private Vector3 ClampCamera(Vector3 targetPosition)
    {
        float camHeight = cam.orthographicSize;
        float camWidth = cam.orthographicSize * cam.aspect;

        float minX = spaceMinX + camWidth;
        float maxX = spaceMaxX - camWidth;
        float minY = spaceMinY + camHeight;
        float maxY = spaceMaxY - camHeight;

        float newX = Mathf.Clamp(targetPosition.x, minX, maxX);
        float newY = Mathf.Clamp(targetPosition.y, minY, maxY);

        return new Vector3(newX, newY, targetPosition.z);
    }

    public void UpdateSpace()
    {
        camSpace.ExpandSpace();

        spaceMinX = space.transform.position.x - space.bounds.size.x / 2;
        spaceMaxX = space.transform.position.x + space.bounds.size.x / 2;
        spaceMinY = space.transform.position.y - space.bounds.size.y / 2;
        spaceMaxY = space.transform.position.y + space.bounds.size.y / 2;
    }
}
