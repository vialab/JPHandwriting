using UnityEngine;

public static class Utils {
    public static void SmartDestroy(Object obj, bool allowDestroyingAssets = false) {
        if (obj == null) return;
#if UNITY_EDITOR
        if (Application.isEditor) {
            Object.DestroyImmediate(obj, allowDestroyingAssets);
        }
        else {
            Object.Destroy(obj);
        }
#else
        Object.Destroy(obj, allowDestroyingAssets);
#endif
    }

    public static Vector2 Bezier(Vector2 a, Vector2 b, float t) {
        return Vector2.Lerp(a, b, t);
    }

    public static Vector2 Bezier(Vector2 a, Vector2 b, Vector2 c, float t) {
        return Vector2.Lerp(Bezier(a, b, t), Bezier(b, c, t), t);
    }

    public static Vector2 Bezier(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float t) {
        return Vector2.Lerp(Bezier(a, b, c, t), Bezier(b, c, d, t), t);
    }

    public static Vector2 Bezier(Vector2 a, Vector2 b, Vector2 c, Vector2 d, Vector2 e, float t) {
        return Vector2.Lerp(Bezier(a, b, c, d, t), Bezier(b, c, d, e, t), t);
    }
}
