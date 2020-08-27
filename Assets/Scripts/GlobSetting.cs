using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobSetting  {

    static float GetCross(Vector2 p1, Vector2 p2, Vector2 p)
    {
        return (p2.x - p1.x) * (p.y - p1.y) - (p.x - p1.x) * (p2.y - p1.y);
    }


    public static bool ContainsQuadrangle(Vector2 leftDownP2, Vector2 leftUpP1, Vector2 rightDownP3, Vector2 rightUpP4, Vector2 point)
    {

        float value1 = GetCross(leftUpP1, leftDownP2, point);

        float value2 = GetCross(rightDownP3, rightUpP4, point);

        if (value1 * value2 < 0) return false;

        float value3 = GetCross(leftDownP2, rightDownP3, point);

        float value4 = GetCross(rightUpP4, leftUpP1, point);

        if (value3 * value4 < 0) return false;

        return true;
    }

}
