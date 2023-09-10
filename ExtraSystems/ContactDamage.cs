using VesselMayCry.Weapons;

namespace VesselMayCry.ExtraSystems
{
    internal class ContactDamage : MonoBehaviour
    {
        private int damagenumber = 40;
        private float magnitude = 0f;
        private int direction = (int)AttackDirection.normal;
        private bool active = true;
        private bool extra = false;
        private bool ignoreinvul = true;
        private AttackTypes dtype = AttackTypes.Nail;
        public void OnTriggerEnter2D(Collider2D collider)
        {
            if (active)
            {
                if (collider.gameObject.GetComponent<HealthManager>() != null || collider.gameObject.GetComponentInChildren<HealthManager>() != null || collider.GetComponentInParent<HealthManager>() != null)
                {
                    if (collider.gameObject.layer == (int)PhysLayers.ENEMIES)
                    {
                        Hit(collider.gameObject);
                    }
                }
                if (collider.gameObject.GetComponent<Breakable>() != null || collider.gameObject.GetComponentInChildren<Breakable>() != null || collider.GetComponentInParent<Breakable>() != null)
                {
                    collider.gameObject.GetComponent<Breakable>().BreakSelf();
                }


            }
        }

        private void Hit(GameObject obj)
        {

            HitInstance hitInstance = new HitInstance();
            hitInstance.AttackType = dtype;
            hitInstance.IgnoreInvulnerable = ignoreinvul;
            hitInstance.IsExtraDamage = extra;
            hitInstance.Multiplier = 1;
            hitInstance.MagnitudeMultiplier = magnitude;
            hitInstance.MoveDirection = false;
            hitInstance.CircleDirection = true;
            hitInstance.Direction = direction;
            hitInstance.MoveAngle = 180;
            hitInstance.Source = gameObject;
            hitInstance.SpecialType = SpecialTypes.None;


            hitInstance.DamageDealt = damagenumber;
            HitTaker.Hit(obj, hitInstance);

            if (gameObject.name == "Starfall")
            {
                HeroController.instance.GetComponent<BeowulfBase>().EndStarfall();
            }

            if (gameObject.name == "Blistering Swords")
            {
                Destroy(gameObject);
            }

            if (gameObject.name == "Round Trip")
            {
                HeroController.instance.GetComponent<MirageEdgeBase>().StopRoundTrip();
            }
        }

        public void HitAgain()
        {
            Collider2D thiscollider = gameObject.GetComponent<Collider2D>();
            //Is this a problem? probably not?
            Collider2D[] results = new Collider2D[1000];
            ContactFilter2D nofilter = new ContactFilter2D().NoFilter();
            thiscollider.OverlapCollider(nofilter, results);

            List<Collider2D> list = results.ToList();
            foreach (Collider2D collider in list)
            {
                if (collider != null)
                {
                    if (collider.gameObject.GetComponent<HealthManager>() != null || collider.gameObject.GetComponentInChildren<HealthManager>() != null || collider.GetComponentInParent<HealthManager>() != null)
                    {
                        if (collider.gameObject.layer == (int)PhysLayers.ENEMIES)
                        {
                            Hit(collider.gameObject);
                        }
                    }
                }
            }
        }

        public void SetKnockback(float val)
        {
            magnitude = val;
        }

        public void SetDamage(int damage)
        {
            damagenumber = damage;
        }

        public void SetPogo()
        {
            direction = (int)AttackDirection.downward;
        }

        public void DisableAuto()
        {
            active = false;
        }

        public void SetExtra()
        {
            extra = true;
            ignoreinvul = false;
        }

        public void SetExtraOnly()
        {
            extra = true;
        }
    }
}
