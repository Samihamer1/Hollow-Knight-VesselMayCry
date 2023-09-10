namespace VesselMayCry
{
    internal static class Anims
    {

        private static bool init = false;
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
            if (!init)
            {
                init = true;

                //attack anims
                LoadAnimation("VesselMayCry.Resources.YamatoAnims.JudgementCut", "JudgementCut", 12);
                LoadAnimation("VesselMayCry.Resources.YamatoAnims.ComboC", "ComboC", 25);
                LoadAnimation("VesselMayCry.Resources.YamatoAnims.UpperSlash", "UpperSlash", 3);
                LoadAnimation("VesselMayCry.Resources.YamatoAnims.JudgementCutEnd", "JudgementCutEnd", 36);
                LoadAnimation("VesselMayCry.Resources.BeowulfAnims.Shockwave1", "Shockwave1", 4);
                LoadAnimation("VesselMayCry.Resources.BeowulfAnims.LunarPhase", "LunarPhase", 3);
                LoadAnimation("VesselMayCry.Resources.MirageEdgeAnims.SpiralSwords", "SpiralSwords", 1);
                LoadAnimation("VesselMayCry.Resources.MirageEdgeAnims.BlisteringSwords", "BlisteringSwords", 1);
                LoadAnimation("VesselMayCry.Resources.MirageEdgeAnims.Overdrive", "Overdrive", 1);
                LoadAnimation("VesselMayCry.Resources.MirageEdgeAnims.OverdriveDissipate", "OverdriveDissipate", 6);

                //knight anims
                LoadKnightAnimation("VesselMayCry.Resources.YamatoAnims.Knight.JudgementCut.set.png", "JC", 24, tk2dSpriteAnimationClip.WrapMode.Loop, 3, 109, 128, false, 0);
                LoadKnightAnimation("VesselMayCry.Resources.YamatoAnims.Knight.ComboC.set.png", "ComboC", 24, tk2dSpriteAnimationClip.WrapMode.Loop, 3, 109, 128, true, 1);
                LoadKnightAnimation("VesselMayCry.Resources.YamatoAnims.Knight.UpperSlash.set.png", "UpperSlash", 60, tk2dSpriteAnimationClip.WrapMode.Once, 4, 130, 128, false, 0);
                LoadKnightAnimation("VesselMayCry.Resources.YamatoAnims.Knight.JCEDash.set.png", "JCEDash", 25000, tk2dSpriteAnimationClip.WrapMode.Once, 7, 192, 117, false, 0);
                LoadKnightAnimation("VesselMayCry.Resources.YamatoAnims.Knight.JCEFinish.set.png", "JCEFinish", 24000, tk2dSpriteAnimationClip.WrapMode.Once, 25, 140, 128, false, 0);
                LoadKnightAnimation("VesselMayCry.Resources.YamatoAnims.Knight.AerialCleaveAntic.set.png", "AerialCleaveAntic", 8, tk2dSpriteAnimationClip.WrapMode.Once, 3, 105, 136, false, 0);
                LoadKnightAnimation("VesselMayCry.Resources.YamatoAnims.Knight.AerialCleaveFall.set.png", "AerialCleaveFall", 24, tk2dSpriteAnimationClip.WrapMode.Once, 1, 135, 136, false, 0);
                LoadKnightAnimation("VesselMayCry.Resources.YamatoAnims.Knight.AerialCleaveSlash.set.png", "AerialCleaveSlash", 24, tk2dSpriteAnimationClip.WrapMode.Once, 3, 135, 136, false, 0);
                LoadKnightAnimation("VesselMayCry.Resources.YamatoAnims.Knight.YamatoSlash.set.png", "YamatoSlash", 20, tk2dSpriteAnimationClip.WrapMode.Once, 15, 109, 128, false, 0);
                LoadKnightAnimation("VesselMayCry.Resources.YamatoAnims.Knight.YamatoSlashAlt.set.png", "YamatoSlashAlt", 20, tk2dSpriteAnimationClip.WrapMode.Once, 15, 136, 133, false, 0);
                LoadKnightAnimation("VesselMayCry.Resources.BeowulfAnims.Knight.StrongPunch.set.png", "StrongPunch", 20, tk2dSpriteAnimationClip.WrapMode.Once, 4, 109, 128, false, 0);
                LoadKnightAnimation("VesselMayCry.Resources.BeowulfAnims.Knight.LunarPhase.set.png", "LunarPhase", 60, tk2dSpriteAnimationClip.WrapMode.Loop, 6, 130, 128, false, 0);
                LoadKnightAnimation("VesselMayCry.Resources.BeowulfAnims.Knight.DashPunch.set.png", "DashPunch", 20, tk2dSpriteAnimationClip.WrapMode.Once, 6, 130, 128, false, 0);
                LoadKnightAnimation("VesselMayCry.Resources.BeowulfAnims.Knight.BeastUppercut.set.png", "BeastUppercut", 50, tk2dSpriteAnimationClip.WrapMode.Once, 7, 109, 128, false, 0);
                LoadKnightAnimation("VesselMayCry.Resources.BeowulfAnims.Knight.StarfallAntic.set.png", "StarfallAntic", 12, tk2dSpriteAnimationClip.WrapMode.Once, 3, 109, 128, false, 0);
                LoadKnightAnimation("VesselMayCry.Resources.BeowulfAnims.Knight.StarfallFall.set.png", "StarfallFall", 12, tk2dSpriteAnimationClip.WrapMode.Loop, 1, 109, 128, false, 0);
                LoadKnightAnimation("VesselMayCry.Resources.BeowulfAnims.Knight.HellOnEarthAntic.set.png", "HellOnEarthAntic", 12, tk2dSpriteAnimationClip.WrapMode.Once, 10, 109, 128, false, 0);
                LoadKnightAnimation("VesselMayCry.Resources.BeowulfAnims.Knight.HellOnEarthPunch.set.png", "HellOnEarthPunch", 24, tk2dSpriteAnimationClip.WrapMode.Once, 10, 109, 128, false, 0);
                LoadKnightAnimation("VesselMayCry.Resources.MirageEdgeAnims.Knight.DeepStinger.set.png", "DeepStinger", 24, tk2dSpriteAnimationClip.WrapMode.Loop, 5, 160, 208, false, 0);
            }
            
        }
    }
}
