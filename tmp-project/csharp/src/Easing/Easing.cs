namespace Svampjakt;

class Easing 
{
    public static float EaseBounceIn(float t)
    {
        return 1.0f - EaseBounceOut(1.0f - t);
    }

    public static float EaseBounceOut(float t)
    {
        if (t < (1 / 2.75f))
        {
            return 7.5625f * t * t;
        }
        else if (t < (2 / 2.75f))
        {
            t -= (1.5f / 2.75f);
            return 7.5625f * t * t + 0.75f;
        }
        else if (t < (2.5f / 2.75f))
        {
            t -= (2.25f / 2.75f);
            return 7.5625f * t * t + 0.9375f;
        }
        else
        {
            t -= (2.625f / 2.75f);
            return 7.5625f * t * t + 0.984375f;
        }
    }
}
