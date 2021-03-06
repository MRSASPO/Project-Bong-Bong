﻿using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
    public Controller2D target;
    public float verticalOffset;
    public float lookAheadDistX;
    public float lookSmoothTimeX;
    public float verticalSmoothTime;
    public Vector2 focusAreaSize;

    //min and max_x for the camera's size, to set the boundaries of the camera, offset is to the center of the camera
    public float min_x = 0;
    public float max_x = 50;
    float prev_min_x;
    float prev_max_x;

    FocusArea focusArea;

    float currentLookAheadX;
    float targetLookAheadX;
    float lookAheadDirX;
    float smoothLookVelocityX;
    float smoothVelocityY;

    bool lookAheadStopped;
    Camera camera;

    void Start() {
        focusArea = new FocusArea(target.collider.bounds, focusAreaSize);
        camera = GetComponent<Camera>();
    }

    void LateUpdate() {
        focusArea.Update(target.collider.bounds);

        Vector2 focusPosition = focusArea.centre + Vector2.up * verticalOffset;

        if(focusArea.velocity.x != 0) {
            lookAheadDirX = Mathf.Sign(focusArea.velocity.x);
            if(Mathf.Sign(target.playerInput.x)==Mathf.Sign(focusArea.velocity.x) && target.playerInput.x != 0) {
                lookAheadStopped = false;
                targetLookAheadX = lookAheadDirX * lookAheadDistX;
            } else {
                if (!lookAheadStopped) {
                    lookAheadStopped = true;
                    targetLookAheadX = currentLookAheadX * (lookAheadDirX * lookAheadDistX - currentLookAheadX) / 4f;
                }
            } 
        }

        targetLookAheadX = lookAheadDirX * lookAheadDistX;
        currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

        focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
        focusPosition += Vector2.right * currentLookAheadX;

        Vector3 goalTransformedPosition = (Vector3)focusPosition + Vector3.forward * -10;

        Bounds camBounds = CameraExtensions.OrthographicBounds(camera);
        float height = camBounds.max.y - camBounds.min.y;
        float width = camBounds.max.x - camBounds.min.x;

        //Change the clamp value if needed
        transform.position = new Vector3(Mathf.Clamp(goalTransformedPosition.x, min_x+width/2, max_x-width/2), 
            goalTransformedPosition.y, 
            goalTransformedPosition.z);
    }

    /*void OnDrawGizmos() {
        Gizmos.color = new Color(1, 0, 0, .5f);
        Gizmos.DrawCube(focusArea.centre, focusAreaSize);
    }*/

    struct FocusArea {
        public Vector2 centre;
        public Vector2 velocity;
        float left, right;
        float top, bottom;

        public FocusArea(Bounds targetBounds, Vector2 size) {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;

            velocity = Vector2.zero;
            centre = new Vector2((left + right) / 2, (top + bottom) / 2);
        }

        public void Update(Bounds targetBounds) {
            float shiftX = 0;
            if(targetBounds.min.x < left) {
                shiftX = targetBounds.min.x - left;
            } else if (targetBounds.max.x > right) {
                shiftX = targetBounds.max.x - right;
            }
            left += shiftX;
            right += shiftX;

            float shiftY = 0;
            if (targetBounds.min.y < bottom) {
                shiftY = targetBounds.min.y - bottom;
            } else if (targetBounds.max.y > top) {
                shiftY = targetBounds.max.y - top;
            }
            top += shiftY;
            bottom += shiftY;
            centre = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = new Vector2(shiftX, shiftY);
        }
    }
}
