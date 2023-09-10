using FrogCore.Ext;
using VesselMayCry.Weapons;

namespace VesselMayCry
{
    internal class GeneralHelper : MonoBehaviour
    {

        public static FsmState CreateTemplateState(string name, PlayMakerFSM fsm, string animname)
        {
            FsmState state = fsm.CreateState(name);
            state.AddMethod(() =>
            {
                HeroController.instance.RelinquishControl();
                HeroController.instance.StopAnimationControl();
            });

            FsmOwnerDefault knightowner = HeroController.instance.spellControl.GetState("Quake Antic").GetAction<Tk2dPlayAnimationWithEvents>(7).gameObject;

            if (animname != null)
            {
                Tk2dPlayAnimationWithEvents eventaction = new Tk2dPlayAnimationWithEvents();
                eventaction.gameObject = knightowner;
                eventaction.clipName = animname;

                state.AddAction(eventaction);
            }

            SetVelocity2d velocityaction = new SetVelocity2d();
            velocityaction.gameObject = knightowner;
            velocityaction.x = 0;
            velocityaction.y = 0;
            velocityaction.vector = new Vector2(0, 0);
            velocityaction.everyFrame = true;

            state.AddAction(velocityaction);

            return state;
        }


        //Regular one
        public static GameObject CreateAttackTemplate(string name, float scalex, float scaley, Vector3 position)
        {
            GameObject cut = new GameObject(name);
            cut.transform.SetScaleX(scalex * HeroController.instance.transform.GetScaleX());
            cut.transform.SetScaleY(scaley);
            cut.transform.position = position;
            PolygonCollider2D collider = cut.AddComponent<PolygonCollider2D>();
            collider.isTrigger = true;
            SpriteRenderer render = cut.AddComponent<SpriteRenderer>();
            cut.layer = (int)PhysLayers.HERO_ATTACK;
            Rigidbody2D rigidbody = cut.AddComponent<Rigidbody2D>();
            rigidbody.gravityScale = 0f;

            return cut;
        }

        public static IEnumerator DestroyAfter(GameObject obj, float time)
        {
            string nailartstate = HeroController.instance.gameObject.LocateMyFSM("Nail Arts").ActiveStateName;
            string spellstate = HeroController.instance.spellControl.ActiveStateName;

            //add a check for if ur in the same state as before the wait so you dont send next on a random state
            yield return new WaitForSeconds(time);
            Destroy(obj);
            if (nailartstate == "Inactive" && spellstate == HeroController.instance.spellControl.ActiveStateName)
            {
                HeroController.instance.spellControl.SendEvent("NEXT");
            }
            else if (spellstate == "Inactive" &&  nailartstate == HeroController.instance.gameObject.LocateMyFSM("Nail Arts").ActiveStateName)
            {
                HeroController.instance.gameObject.LocateMyFSM("Nail Arts").SendEvent("NEXT");
            }
        }

        public static IEnumerator EarlyControl(float time)
        {
            string nailartstate = HeroController.instance.gameObject.LocateMyFSM("Nail Arts").ActiveStateName;
            yield return new WaitForSeconds(time);
            if (nailartstate == HeroController.instance.gameObject.LocateMyFSM("Nail Arts").ActiveStateName)
            {
                HeroController.instance.gameObject.LocateMyFSM("Nail Arts").SendEvent("NEXT");
            }
        }


        //Regular attatching to hero
        public static GameObject CreateAttackTemplate(string name, float scalex, float scaley, Transform transform, Vector3 localpos)
        {
            GameObject cut = CreateAttackTemplate(name, scalex, scaley, new Vector3(0, 0, 0));
            cut.transform.parent = transform;
            cut.transform.localPosition = localpos;
            return cut;
        }

        //Regular with custom points
        public static GameObject CreateAttackTemplate(string name, float scalex, float scaley, Vector3 position, Vector2[] points)
        {
            GameObject cut = CreateAttackTemplate(name, scalex, scaley, position);
            cut.GetComponent<PolygonCollider2D>().points = points;
            return cut;
        }

        //Regular with a box collider
        public static GameObject CreateAttackTemplate(string name, float scalex, float scaley, Vector3 position, Vector2 size, Vector2 offset)
        {
            GameObject cut = CreateAttackTemplate(name, scalex, scaley, position);
            cut.GetComponent<PolygonCollider2D>().Destroy();
            BoxCollider2D collider = cut.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = size;
            collider.offset = offset;
            return cut;
        }

        //Regular with a box collider attatched to hero
        public static GameObject CreateAttackTemplate(string name, float scalex, float scaley, Transform transform, Vector3 localpos, Vector2 hitsize, Vector2 hitoffset)
        {
            GameObject cut = CreateAttackTemplate(name, scalex, scaley, new Vector3(0, 0, 0), hitsize, hitoffset);
            cut.transform.parent = transform;
            cut.transform.localPosition = localpos;

            return cut;
        }

        //For spiral swords, regular with a box collider attached to hero but without a rigidbody.
        public static GameObject CreateAttackTemplate(string name, float scalex, float scaley, Transform transform, Vector3 localpos, Vector2 hitsize, Vector2 hitoffset, bool noRigid)
        {
            GameObject cut = CreateAttackTemplate(name, scalex, scaley, transform, localpos, hitsize, hitoffset);
            cut.GetComponent<Rigidbody2D>().Destroy();

            return cut;
        }

        //Regular but coloured for yamato
        public static GameObject CreateAttackTemplate(string name, float scalex, float scaley, Vector3 position, bool coloured)
        {
            GameObject cut = CreateAttackTemplate(name, scalex, scaley, position);
            SpriteRenderer render = cut.GetComponent<SpriteRenderer>();
            if (coloured)
            {
                render.color = new Color(YamatoBase.colorR, YamatoBase.colorG, YamatoBase.colorB);
            }

            return cut;
        }

    }
}
