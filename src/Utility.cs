using UnityEngine;

public static class Utility {

    public static Quaternion XLookRotation2D(Vector3 right) {
        Quaternion rightToUp = Quaternion.Euler(0f, 0f, 90f);
        Quaternion upToTarget = Quaternion.LookRotation(Vector3.forward, right);
        return upToTarget * rightToUp;
    }

    public static Vector2 GetDirection2D(Vector3 from, Vector3 to) {
        Vector3 dir = to - from;
        dir.z = 0f;
        return dir.normalized;
    }
}
