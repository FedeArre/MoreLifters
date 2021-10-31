public class Lift
{
    public float percentage { get; set; }
    public float posX { get; set; }
    public float posY { get; set; }
    public float posZ { get; set; }
    public float rotY { get; set; }
    public float rotW { get; set; }
    public bool isSmall { get; set; }
    public float percentageJackH { get; set; }
    public float percentageJackP { get; set; }
    public bool wasInTrailer { get; set; }
    public float percentageSecondJackH { get; set; }
    public float percentageSecondJackP { get; set; }
    public Lift(float percentage, float posX, float posY, float posZ, float rotY, float rotW, bool isSmall, float percentageJackH, float percentageJackP, bool wot, float sjHeight, float sjPos)
    {
        this.percentage = percentage;
        this.posX = posX;
        this.posY = posY;
        this.posZ = posZ;
        this.rotY = rotY;
        this.rotW = rotW;
        this.isSmall = isSmall;
        this.percentageJackH = percentageJackH;
        this.percentageJackP = percentageJackP;
        this.wasInTrailer = wot;
        this.percentageSecondJackH = sjHeight;
        this.percentageSecondJackP = sjPos;
    }
}