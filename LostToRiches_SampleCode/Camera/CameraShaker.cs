using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker
{
    Transform cameraTransform;

    Vector3 myOriginalPostion;
    Vector3 shakePosition;
    Vector3 startOfLerp;

    float shakeLength = 0;
    float shakeDuration = 0.5f;
    float timeLeft = 0.5f;
    float shakeDefault = 0.75f;
    float shakeAmount;
    float shakeSpeed = 8f;

    bool firstTime = true;

    public CameraShaker(Transform camera)
    {
        cameraTransform = camera;
    }

    public bool ShakeCamera(Vector3 originalPosition, Vector3 targetPosition)
    {    
        if(timeLeft > 0)
        {
            if (firstTime)
            {
                shakeAmount = shakeDefault;
                Vector3 randPos = Random.insideUnitCircle * shakeAmount;
                Vector3 up = cameraTransform.up * randPos.y;
                Vector3 right = cameraTransform.right * randPos.x;
                randPos = up + right;
                shakePosition = new Vector3();
                myOriginalPostion = new Vector3();
                myOriginalPostion = originalPosition;
                shakePosition = myOriginalPostion + randPos;
                startOfLerp = myOriginalPostion;
                firstTime = false;
            }
            else if(1 < shakeLength)
            {
                shakeAmount *= 0.8f;
                Vector3 randPos = Random.insideUnitCircle * shakeAmount;
                Vector3 up = cameraTransform.up * randPos.y;
                Vector3 right = cameraTransform.right * randPos.x;
                randPos = up + right;
                startOfLerp = shakePosition;
                shakePosition = myOriginalPostion + randPos;
                shakeLength = 0;
            }
            Vector3 posDifference = targetPosition - myOriginalPostion;
            cameraTransform.localPosition = Vector3.Lerp(startOfLerp, shakePosition, shakeLength) + posDifference;
            shakeLength += Time.deltaTime * shakeSpeed;
            timeLeft -= Time.deltaTime;
            return true;
        }
        else
        {
            Reset();
            return false;
        }
    }

    void Reset()
    {
        timeLeft = shakeDuration;
        firstTime = true;
        shakeAmount = shakeDefault;
    }
}
