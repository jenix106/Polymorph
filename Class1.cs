using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

namespace Polymorph
{
    public class PolymorphSpell : SpellCastProjectile
    {
        public override void Throw(Vector3 velocity)
        {
            base.Throw(velocity);
            guidedProjectile.OnProjectileCollisionEvent += GuidedProjectile_OnProjectileCollisionEvent;
        }

        private void GuidedProjectile_OnProjectileCollisionEvent(ItemMagicProjectile projectile, CollisionInstance collisionInstance)
        {
            if(collisionInstance.targetColliderGroup?.collisionHandler?.ragdollPart?.ragdoll?.creature is Creature creature)
            {
                List<ItemData> itemData = new List<ItemData>();
                foreach(ItemData data in Catalog.GetDataList<ItemData>())
                {
                    if (data.category == "Breakable" && !itemData.Contains(data)) itemData.Add(data);
                }
                if (!itemData.IsNullOrEmpty())
                    itemData[Random.Range(0, itemData.Count)].SpawnAsync(item =>
                    {
                        creature.Kill();
                        creature.Despawn();
                    }, creature.ragdoll.targetPart.transform.position);
            }
            projectile.OnProjectileCollisionEvent -= GuidedProjectile_OnProjectileCollisionEvent;
        }
        public override void UpdateCaster()
        {
            base.UpdateCaster();
            if(spellCaster?.ragdollHand?.grabbedHandle is HandleRagdoll grabbedHandle && spellCaster.ragdollHand.playerHand.controlHand.castPressed)
            {
                spellCaster.ragdollHand.TryRelease();
                Creature creature = grabbedHandle?.ragdollPart?.ragdoll?.creature;
                List<ItemData> itemData = new List<ItemData>();
                foreach (ItemData data in Catalog.GetDataList<ItemData>())
                {
                    if (data.category == "Breakable" && !itemData.Contains(data)) itemData.Add(data);
                }
                if (!itemData.IsNullOrEmpty())
                    itemData[Random.Range(0, itemData.Count)].SpawnAsync(item =>
                    {
                        creature.Kill();
                        creature.Despawn();
                        if (!item.handles.IsNullOrEmpty())
                        {
                            spellCaster.ragdollHand.Grab(item.GetMainHandle(spellCaster.ragdollHand.side) ?? item.handles[0]);
                        }
                    }, creature.ragdoll.targetPart.transform.position);
            }
        }
    }
}
