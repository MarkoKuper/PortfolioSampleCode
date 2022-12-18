using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Attach : MonoBehaviour
{
    public Transform attachedObject;
    public Transform levelBoarder;
    Vector3 mousePosition = new Vector3();

    public Transform newObject;

    public float maxXPosition;
    public float minXPosition;
    public float maxZPosition;
    public float minZPosition;
    public float maxOffsetFromPlayer;
    public float offsetDeadZone;
    public float followSpeed = 5;

    bool zoomIn;
    bool shakeCamera;
    Vector3 initialPosition;

    Camera mainCamera;

    CameraShaker cameraShaker;

    void Start()
    {
        mainCamera = Camera.main;

        cameraShaker = new CameraShaker(mainCamera.transform);

        initialPosition = attachedObject.position - mainCamera.transform.position;
        //maxXPosition = levelBoarder.transform.localScale.x * 0.5f;
        //maxZPosition = levelBoarder.transform.localScale.z * 0.5f;
    }

    void Update()
    {
        if (zoomIn)
        {
            ZoomInOnLocation(newObject);
        }
        else
        {
            MousePosition();
            FollowObject();
        }
    }

    void FollowObject()
    {
        if (attachedObject != null)
        {
            mousePosition.x = Mathf.Clamp(mousePosition.x, attachedObject.position.x - maxOffsetFromPlayer, attachedObject.position.x + maxOffsetFromPlayer);
            mousePosition.z = Mathf.Clamp(mousePosition.z, attachedObject.position.z - (maxOffsetFromPlayer + 4), attachedObject.position.z + maxOffsetFromPlayer);
            Vector3 newPosition = new();
            if (Mathf.Abs((attachedObject.position - mousePosition).magnitude) < offsetDeadZone)
            {
                mousePosition = Vector3.zero;
                newPosition = attachedObject.position - initialPosition;
            }
            else
            {
                newPosition = ((attachedObject.position + mousePosition) * 0.5f) - initialPosition;
            }
            newPosition = new Vector3(Mathf.Clamp(newPosition.x, minXPosition, maxXPosition), newPosition.y, Mathf.Clamp(newPosition.z, minZPosition, maxZPosition));
            if (shakeCamera && attachedObject != null)
            {
                shakeCamera = cameraShaker.ShakeCamera(mainCamera.transform.position, newPosition);
            }
            else
            {
                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, newPosition, Time.deltaTime * followSpeed);
            }
                    
        }
    }

    void MousePosition()
    {
        Camera myCamera = Camera.main;
        Vector3 direction = Input.mousePosition;
        direction = myCamera.ScreenToWorldPoint(new Vector3(direction.x, direction.y, myCamera.nearClipPlane));
        float t = (0.5f - myCamera.transform.position.y) / (direction.y - myCamera.transform.position.y);
        float xValue = myCamera.transform.position.x + (t * (direction.x - myCamera.transform.position.x));
        float zValue = myCamera.transform.position.z + (t * (direction.z - myCamera.transform.position.z));
        mousePosition = new Vector3(xValue, 0.5f, zValue);
    }

    void ZoomInOnLocation(Transform newObject)
    {
        Vector3 newPosition = newObject.position - initialPosition;
        newPosition = new Vector3(newPosition.x, newPosition.y - 2, newPosition.z + 3);
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, newPosition, Time.deltaTime * followSpeed);
    }

    void ToggleZooming()
    {
        zoomIn = !zoomIn;
    }

    [ContextMenu("ShakeCamera")]
    public void ToggleShake()
    {
        shakeCamera = !shakeCamera;
    }
}
