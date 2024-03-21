namespace SDG;

public static class Mathf
{
    public static float Modf(float x, float m) {
        return (x % m + m) % m;
    }
}