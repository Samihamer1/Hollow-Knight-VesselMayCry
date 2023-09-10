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

        public static void Initialise()
        {
            //this has to be some sort of crime, man
            AddToPos("JC1", -0.7219, 1, 0.53, -1.4);
            AddToPos("JC2", -0.7219, 1, 0.53, -1.4);
            AddToPos("JC3", -0.7219, 1, 0.53, -1.4);
            AddToPos("ComboC1", -0.7219, 1, 0.53, -1.4);
            AddToPos("ComboC2", -0.7219, 1, 0.53, -1.4);
            AddToPos("ComboC3", -0.65, 1.05, 0.53, -1.4);
            AddToPos("UpperSlash1", -0.8219, 1, 0.53, -1.4);
            AddToPos("UpperSlash2", -0.8219, 1, 0.53, -1.4);
            AddToPos("UpperSlash3", -0.8219, 1, 0.53, -1.4);
            AddToPos("UpperSlash4", -0.8219, 1, 0.53, -1.4);
            AddToPos("JCEDash1",-0.6875,0.5625,0.4375,-1.4063);
            AddToPos("JCEDash2", -0.8969, 0.7281, 0.3125, -1.4219);
            AddToPos("JCEDash3", -1.0906, 1.3781, 0.3594, -1.375);
            AddToPos("JCEDash4", -1.2219, 1.7, 0.4063, -1.375);
            AddToPos("JCEDash5", -1.3219, 1.4594, 0.4063, -1.375);
            AddToPos("JCEDash6", -1.4219, 1.5781, 0.4063, -1.375);
            AddToPos("JCEFinish1", -1.2406, 0.7344, 8.625, 6.4219);
            AddToPos("JCEFinish2", -1.2406, 0.7344, 5.625, 3.4219);
            AddToPos("JCEFinish3", -1.2406, 0.7344, 2.625, 0.4219);
            
            //jcefinish 4-19 have the same values
            for (int i = 4; i < 26; i++)
            {
                AddToPos("JCEFinish"+i, -1.2406, 0.7344, 0.625, -1.4219);
            }

            //aerial cleave antic shares
            for (int i = 1; i < 4; i++)
            {
                AddToPos("AerialCleaveAntic" + i, -0.5937, 0.8906, 0.3594, -1.4844);
                AddToPos("AerialCleaveSlash" + i, -0.8594, 1.3094, 0.7719, -1.4219);
                AddToPos("StarfallAntic" + i, -0.9844, 0.756, 0.6094, -1.3906);
            };

            AddToPos("StarfallFall1", -0.9844, 0.756, 0.6094, -1.3906);
            AddToPos("AerialCleaveFall1", -0.5937, 1.1906, 0.4594, -1.4844);

            for (int i = 1; i < 5; i++)
            {
                AddToPos("StrongPunch" + i, -0.9844, 0.756, 0.6094, -1.3906);
            }

            for (int i = 1; i < 6; i++)
            {
                AddToPos("DeepStinger" + i, -1.5, 1.1, 1.2, -2);
            }

            for (int i = 1; i < 7; i++)
            {
                AddToPos("LunarPhase" + i, -0.8844, 1.056, 0.6094, -1.3906);
            }

            for (int i = 1; i < 16; i++)
            {
                AddToPos("YamatoSlash" + i, -0.9844, 0.756, 0.6094, -1.3906);
                AddToPos("YamatoSlashAlt" + i, -0.8844, 1.056, 0.6094, -1.3906);
            }

            for (int i = 1; i < 7; i++)
            {
                AddToPos("DashPunch" + i, -0.8844, 1.056, 0.6094, -1.3906);
            }

            for (int i = 1; i < 8; i++)
            {
                AddToPos("BeastUppercut" + i, -0.9844, 0.756, 0.6094, -1.3906);
            }

            for (int i = 1; i < 11; i++)
            {
                AddToPos("HellOnEarthAntic" + i, -0.9844, 0.756, 0.6094, -1.3906);
                AddToPos("HellOnEarthPunch" + i, -0.9844, 0.756, 0.6094, -1.3906);
            }
        }
    }
}
