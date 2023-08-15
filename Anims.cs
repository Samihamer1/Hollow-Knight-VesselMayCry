namespace VesselMayCry
{
    internal static class Anims
    {

        public static Dictionary<string, List<Sprite>> animationset = new Dictionary<string, List<Sprite>>();

        private static void LoadAnimation(string path, string name, int length)
        {
            List<Sprite> sprites = new List<Sprite>();
            for (int i = 0; i < length; i++)
            {
                //path takes you to the folder.
                sprites.Add(ResourceLoader.LoadSprite(path + "." + (i+1).ToString() + ".png"));
            }

            animationset.Add(name, sprites);
        }

        private static void LoadKnightAnimation(string path, string name, int fps, tk2dSpriteAnimationClip.WrapMode wrapmode, int length, int xbound, int ybound, bool loop, int frameloop)
        {
            tk2dSpriteAnimator animator = HeroController.instance.gameObject.GetComponent<tk2dSpriteAnimator>();
            List<tk2dSpriteAnimationClip> list = animator.Library.clips.ToList<tk2dSpriteAnimationClip>();

            Texture2D texture1 = ResourceLoader.LoadTexture2D(path);

            string[] names = new string[length];
            Rect[] rects = new Rect[length];
            Vector2[] anchors = new Vector2[length];

            for (int i = 0; i < length;i++)
            {
                names[i] = name + (i+1).ToString();
                rects[i] = new Rect(i * xbound, i * ybound, xbound, ybound);
                anchors[i] = new Vector2(0, 0);
            }

            GameObject knight = HeroController.instance.gameObject;

            tk2dSpriteCollectionData spriteCollectiondata = FrogCore.Utils.CreateTk2dSpriteCollection(texture1, names, rects, anchors, new GameObject());

            tk2dSpriteAnimationFrame[] list1 = new tk2dSpriteAnimationFrame[length];

            for (int i = 0; i < length; i++)
            {
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame();
                frame.spriteCollection = spriteCollectiondata;
                frame.spriteId = i;
                list1[i] = frame;
            }

            tk2dSpriteAnimationClip clip = new tk2dSpriteAnimationClip();
            clip.name = name;
            clip.fps = fps;
            clip.frames = list1;
            clip.wrapMode = wrapmode;

            if (loop)
            {
                clip.loopStart = frameloop;
                clip.wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            }

            clip.SetCollection(spriteCollectiondata);

            foreach (tk2dSpriteDefinition sprite in spriteCollectiondata.spriteDefinitions)
            {
                if (SpritePositions.spritepositions.ContainsKey(sprite.name))
                {
                    sprite.positions = SpritePositions.spritepositions[sprite.name];
                }
            }

            list.Add(clip);
            animator.Library.clips = list.ToArray();
        }

        public static IEnumerator PlayAnimation(string name, SpriteRenderer renderer, float length)
        {
            if (animationset.ContainsKey(name))
            {
                List<Sprite> sprites = animationset[name];
                float numofframes = sprites.Count;
                float fps = (1 / numofframes) * length; //for now,at least.
                for (int i = 0; i < sprites.Count; i++)
                {
                    if (renderer != null)
                    {
                        renderer.sprite = sprites[i];
                    }
                    yield return new WaitForSeconds(fps);
                }
            }
        }

        //I don't know if any of you noticed, but I can't actually animate.
        public static void AnimationInit()
        {
            //attack anims
            LoadAnimation("VesselMayCry.Resources.YamatoAnims.JudgementCut", "JudgementCut", 12);
            LoadAnimation("VesselMayCry.Resources.YamatoAnims.ComboC", "ComboC", 25);
            LoadAnimation("VesselMayCry.Resources.YamatoAnims.UpperSlash", "UpperSlash", 3);
            LoadAnimation("VesselMayCry.Resources.YamatoAnims.JudgementCutEnd", "JudgementCutEnd", 36);

            //knight anims
            LoadKnightAnimation("VesselMayCry.Resources.YamatoAnims.Knight.JudgementCut.set.png", "JC", 24, tk2dSpriteAnimationClip.WrapMode.Loop, 3, 109, 128, false, 0);
            LoadKnightAnimation("VesselMayCry.Resources.YamatoAnims.Knight.ComboC.set.png", "ComboC", 24, tk2dSpriteAnimationClip.WrapMode.Loop, 3, 109, 128,true,1);
            LoadKnightAnimation("VesselMayCry.Resources.YamatoAnims.Knight.UpperSlash.set.png", "UpperSlash", 60, tk2dSpriteAnimationClip.WrapMode.Once, 4, 130, 128, false, 0);
        }
    }
}
