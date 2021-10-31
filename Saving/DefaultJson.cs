namespace LifterMod
{
    public class DefaultJson
    {
        public static LiftsObject DefaultLifters = Preload();

        public static LiftsObject Preload()
        {
            LiftsObject objects = new LiftsObject();

            objects.lifts.Add(new Lift(0.0f, 147.791458f, 62.132618f, 519.039246f, -0.7204912f, -0.69346416f, true, 0f, 0f, false, 0f, 0f));
            objects.lifts.Add(new Lift(0.0f, 152.973953f, 62.132618f, 518.845337f, 0.714504f, 0.6996314f, false, 0f, 0f, false, 0f, 100f));

            return objects;
        }
    }
}
