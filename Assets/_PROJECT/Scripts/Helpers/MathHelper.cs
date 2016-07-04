using UnityEngine;
using System.Collections;

public class MathHelper : MonoBehaviour
{
    [Range(0, 1)]
    public float RoundaboutRange;

    public Transform trans;

    public static Vector2 Roundabout(float value)
    {
        value = Mathf.Clamp(value, 0f, 1f);

        return new Vector2(Mathf.Lerp(-1, 1, value), Mathf.Lerp(-1, 1, 1 - value));


    }

    public static float Percentage(float total, float desiredPercentage)
    {
        return total * desiredPercentage / 100;
    }

    void Update()
    {
        trans.position = new Vector3(Roundabout(RoundaboutRange).x, Roundabout(RoundaboutRange).y, 0);
    }

}
