using Microsoft.Xna.Framework;


public static class Geom
{
    public static float line(float X, float Input_o, float Output_o, float Input_f, float Output_f)
    {
        if (X < Input_o) return Output_o;
        else if (X > Input_f) return Output_f;
        else
        {
            float M = (Output_f - Output_o) / (Input_f - Input_o);
            return Output_o + (X - Input_o) * M;
        }
    }
}
