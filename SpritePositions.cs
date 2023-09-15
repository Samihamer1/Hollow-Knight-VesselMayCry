namespace VesselMayCry
{
    internal class SpritePositions : MonoBehaviour
    {
        
        public static Dictionary<string, Vector3[]> spritepositions = new Dictionary<string, Vector3[]>();

        private static void AddToPos(string name, double left, double right, double top, double bottom) 
        {
            //assumes knight facing the right.
            List<Vector3> list = new List<Vector3>
            {
                new Vector3((float)left, (float)bottom, 0),
                new Vector3((float) right, (float) bottom, 0),
                new Vector3((float) left, (float) top, 0),
                new Vector3((float) right, (float) top, 0),
            };

            spritepositions.Add(name, list.ToArray());
        }

        private static void AddToPosBulk(string name, double left, double right, double top, double bottom, int lowerinclusive, int upperinclusive)
        {
            for (int i = lowerinclusive; i < upperinclusive+1; i++)
            {
                AddToPos(name+i,left,right, top, bottom);
            }
        }

        public static void Initialise()
        {
            //I only figured out that this was a stupid horrible dumb stupid way to fix the issue, but it works. I'll just do better next mod.

            AddToPosBulk("JC", -0.7219, 1, 0.53, -1.4, 1, 3);

            AddToPosBulk("ComboC", -0.7219, 1, 0.53, -1.4, 1, 2);
            AddToPos("ComboC3", -0.65, 1.05, 0.53, -1.4);

            AddToPosBulk("UpperSlash", -0.8219, 1, 0.53, -1.4, 1, 4);

            AddToPos("JCEDash1",-0.6875,0.5625,0.4375,-1.4063);
            AddToPos("JCEDash2", -0.8969, 0.7281, 0.3125, -1.4219);
            AddToPos("JCEDash3", -1.0906, 1.3781, 0.3594, -1.375);
            AddToPos("JCEDash4", -1.2219, 1.7, 0.4063, -1.375);
            AddToPos("JCEDash5", -1.3219, 1.4594, 0.4063, -1.375);
            AddToPos("JCEDash6", -1.4219, 1.5781, 0.4063, -1.375);
            AddToPos("JCEFinish1", -1.2406, 0.7344, 8.625, 6.4219);
            AddToPos("JCEFinish2", -1.2406, 0.7344, 5.625, 3.4219);
            AddToPos("JCEFinish3", -1.2406, 0.7344, 2.625, 0.4219);           
            AddToPosBulk("JCEFinish", -1.2406, 0.7344, 0.625, -1.4219, 4, 25);

            AddToPosBulk("AerialCleaveAntic", -0.5937, 0.8906, 0.3594, -1.4844, 1, 3);
            AddToPosBulk("AerialCleaveSlash", -0.8594, 1.3094, 0.7719, -1.4219, 1, 3);
            AddToPos("AerialCleaveFall1", -0.5937, 1.1906, 0.4594, -1.4844);

            AddToPosBulk("StarfallAntic", -0.9844, 0.756, 0.6094, -1.3906, 1, 3);
            AddToPos("StarfallFall1", -0.9844, 0.756, 0.6094, -1.3906);

            AddToPosBulk("StrongPunch", -0.9844, 0.756, 0.6094, -1.3906, 1, 4);

            AddToPosBulk("DeepStinger", -1.5, 1.1, 1.2, -2, 1, 5);

            AddToPosBulk("LunarPhase", -0.8844, 1.056, 0.6094, -1.3906, 1, 6);

            AddToPosBulk("YamatoSlash", -0.9844, 0.756, 0.6094, -1.3906, 1, 15);
            AddToPosBulk("BeowulfSlash", -0.9844, 0.756, 0.6094, -1.3906, 1, 15);
            AddToPosBulk("MirageSlash", -0.9844, 0.756, 0.6094, -1.3906, 1, 15);

            AddToPosBulk("YamatoSlashAlt", -0.8844, 1.056, 0.6094, -1.3906, 1, 15);
            AddToPosBulk("BeowulfSlashAlt", -0.8844, 1.056, 0.6094, -1.3906, 1, 15);
            AddToPosBulk("MirageSlashAlt", -0.8844, 1.056, 0.6094, -1.3906, 1, 15);

            AddToPosBulk("DashPunch", -0.8844, 1.056, 0.6094, -1.3906, 1, 6);

            AddToPosBulk("BeastUppercut", -0.9844, 0.756, 0.6094, -1.3906, 1, 7);

            AddToPosBulk("HellOnEarthAntic", -0.9844, 0.756, 0.6094, -1.3906, 1, 10);
            AddToPosBulk("HellOnEarthPunch", -0.9844, 0.756, 0.6094, -1.3906, 1, 10);
        }
    }
}
