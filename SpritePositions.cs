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
        }
    }
}
