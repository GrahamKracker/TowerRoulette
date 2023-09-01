using System;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using TowerRoulette;

[assembly: MelonInfo(typeof(TowerRoulette.Main), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace TowerRoulette;

[HarmonyPatch]
public class Main : BloonsTD6Mod
{
    
    private static Random _random = new();
    public override void OnNewGameModel(GameModel result)
    {
        result.towers.ForEach(RandomizeStats);
    }

    private static void RandomizeStats(TowerModel originalModel)
    {
        if (_random.Next(0, 100) > 50)
        {
            BuffTower(originalModel);
        }
        else
        {
            NerfTower(originalModel);
        }
    }

    private static void BuffTower(TowerModel originalModel)
    {
        originalModel.range *= 2;
        originalModel.radius /= 2;
        originalModel.radiusSquared = originalModel.radius * originalModel.radius;
        originalModel.ignoreBlockers = true;
        
        foreach(var damageModel in originalModel.GetDescendants<DamageModel>().ToList())
        {
            damageModel.damage *= 2;
        }
        
        foreach (var projectile in originalModel.GetDescendants<ProjectileModel>().ToList())
        {
            projectile.pierce *= 2;
            projectile.ignoreBlockers = true;
            projectile.filters = null;
            projectile.canCollisionBeBlockedByMapLos = false;
        }

        foreach (var attack in originalModel.GetDescendants<AttackModel>().ToList())
        {
            attack.range *= 2;
            attack.attackThroughWalls = true;
        }

        foreach (var weapon in originalModel.GetDescendants<WeaponModel>().ToList())
        {
            weapon.rate /= 2;
            int? count = null;
            var countPropertyInfo = weapon.emission.GetIl2CppType().GetProperty("count");
            if (countPropertyInfo != null)
            {
                var value = countPropertyInfo.GetValue(weapon.emission);
                if (value != null)
                {
                    count = value.Unbox<int>();
                }
            }

            if (count != null)
                countPropertyInfo?.SetValue(weapon.emission, count * 2);
        }
    }

    private static void NerfTower(TowerModel originalModel)
    {
        originalModel.range /= 2;
        originalModel.radius *= 2;
        originalModel.radiusSquared = originalModel.radius * originalModel.radius;
        originalModel.ignoreBlockers = false;
        
        foreach(var damageModel in originalModel.GetDescendants<DamageModel>().ToList())
        {
            damageModel.damage /= 2;
        }
        
        foreach(var projectile in originalModel.GetDescendants<ProjectileModel>().ToList())
        {
            projectile.pierce /= 2;
        }
        
        foreach (var attack in originalModel.GetDescendants<AttackModel>().ToList())
        {
            attack.range /= 2;
        }
        
        foreach(var weapon in originalModel.GetDescendants<WeaponModel>().ToList())
        {
            weapon.rate *= 2;
            int? count = null;
            var countPropertyInfo = weapon.emission.GetIl2CppType().GetProperty("count");
            if (countPropertyInfo != null)
            {
                var value = countPropertyInfo.GetValue(weapon.emission);
                if (value != null)
                {
                    count = value.Unbox<int>();
                }
            }
            
            if(count != null)
                countPropertyInfo?.SetValue(weapon.emission, count / 2);
            
        }
    }
}