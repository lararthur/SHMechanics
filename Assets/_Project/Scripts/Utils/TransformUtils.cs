using UnityEngine;
using System.Collections;

// "static" means this class exists globally. You don't attach it to GameObjects.
public static class TransformUtils
{
    // This is our reusable Coroutine. It requires the Transform it needs to rotate,
    // the angles to rotate by (as a Vector3), and how long it should take.
    public static IEnumerator RotationAnimation(Transform transformToRotate, Vector3 rotationOffset, float animationDuration)
    {
        Quaternion startRotation = transformToRotate.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(rotationOffset);

        float timeElapsed = 0f;

        // Loop this block of code until the duration is reached
        while (timeElapsed < animationDuration)
        {
            // Slerp (Spherical Linear Interpolation) smoothly blends between two rotations based on time
            transformToRotate.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / animationDuration);
            timeElapsed += Time.deltaTime;

            // "yield return null" tells Unity to pause this function here, render the frame, and come back next frame
            yield return null;
        }

        // Hard snap at the end
        transformToRotate.rotation = targetRotation;
    }
}
