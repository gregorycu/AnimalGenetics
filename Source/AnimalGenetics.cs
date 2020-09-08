using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace AnimalGenetics
{
    public class AnimalGenetics : WorldComponent
    {
        public AnimalGenetics(World world) : base(world)
        {
            /*var affectedStats = new List<StatDef> {
                StatDefOf.MaxHitPoints,
                StatDefOf.MarketValue,
                StatDefOf.MarketValueIgnoreHp,
                StatDefOf.RoyalFavorValue,
                StatDefOf.SellPriceFactor,
                StatDefOf.Beauty,
                StatDefOf.Cleanliness,
                StatDefOf.Flammability,
                StatDefOf.DeteriorationRate,
                StatDefOf.WorkToMake,
                StatDefOf.WorkToBuild,
                StatDefOf.Mass,
                StatDefOf.ConstructionSpeedFactor,
                StatDefOf.Nutrition,
                StatDefOf.FoodPoisonChanceFixedHuman,
                StatDefOf.MoveSpeed,
                StatDefOf.GlobalLearningFactor,
                StatDefOf.HungerRateMultiplier,
                StatDefOf.RestRateMultiplier,
                StatDefOf.PsychicSensitivity,
                StatDefOf.ToxicSensitivity,
                StatDefOf.MentalBreakThreshold,
                StatDefOf.EatingSpeed,
                StatDefOf.ComfyTemperatureMin,
                StatDefOf.ComfyTemperatureMax,
                StatDefOf.Comfort,
                StatDefOf.MeatAmount,
                StatDefOf.LeatherAmount,
                StatDefOf.MinimumHandlingSkill,
                StatDefOf.MeleeDPS,
                StatDefOf.PainShockThreshold,
                StatDefOf.ForagedNutritionPerDay,
                StatDefOf.PsychicEntropyMax,
                StatDefOf.PsychicEntropyRecoveryRate,
                StatDefOf.PsychicEntropyGain,
                StatDefOf.MeditationFocusGain,
                StatDefOf.WorkSpeedGlobal,
                StatDefOf.MiningSpeed,
                StatDefOf.DeepDrillingSpeed,
                StatDefOf.MiningYield,
                StatDefOf.ResearchSpeed,
                StatDefOf.ConstructionSpeed,
                StatDefOf.HuntingStealth,
                StatDefOf.PlantWorkSpeed,
                StatDefOf.SmoothingSpeed,
                StatDefOf.FoodPoisonChance,
                StatDefOf.CarryingCapacity,
                StatDefOf.PlantHarvestYield,
                StatDefOf.FixBrokenDownBuildingSuccessChance,
                StatDefOf.ConstructSuccessChance,
                StatDefOf.GeneralLaborSpeed,
                StatDefOf.UnskilledLaborSpeed,
                StatDefOf.MedicalTendSpeed,
                StatDefOf.MedicalTendQuality,
                StatDefOf.MedicalSurgerySuccessChance,
                StatDefOf.NegotiationAbility,
                StatDefOf.ArrestSuccessChance,
                StatDefOf.TradePriceImprovement,
                StatDefOf.SocialImpact,
                StatDefOf.PawnBeauty,
                StatDefOf.AnimalGatherSpeed,
                StatDefOf.AnimalGatherYield,
                StatDefOf.TameAnimalChance,
                StatDefOf.TrainAnimalChance,
                StatDefOf.ShootingAccuracyPawn,
                StatDefOf.ShootingAccuracyTurret,
                StatDefOf.AimingDelayFactor,
                StatDefOf.MeleeHitChance,
                StatDefOf.MeleeDodgeChance,
                StatDefOf.PawnTrapSpringChance,
                StatDefOf.IncomingDamageFactor,
                StatDefOf.MeleeWeapon_AverageDPS,
                StatDefOf.MeleeWeapon_DamageMultiplier,
                StatDefOf.MeleeWeapon_CooldownMultiplier,
                StatDefOf.MeleeWeapon_AverageArmorPenetration,
                StatDefOf.SharpDamageMultiplier,
                StatDefOf.BluntDamageMultiplier,
                StatDefOf.StuffPower_Armor_Sharp,
                StatDefOf.StuffPower_Armor_Blunt,
                StatDefOf.StuffPower_Armor_Heat,
                StatDefOf.StuffPower_Insulation_Cold,
                StatDefOf.StuffPower_Insulation_Heat,
                StatDefOf.RangedWeapon_Cooldown,
                StatDefOf.RangedWeapon_DamageMultiplier,
                StatDefOf.AccuracyTouch,
                StatDefOf.AccuracyShort,
                StatDefOf.AccuracyMedium,
                StatDefOf.AccuracyLong,
                StatDefOf.StuffEffectMultiplierArmor,
                StatDefOf.StuffEffectMultiplierInsulation_Cold,
                StatDefOf.StuffEffectMultiplierInsulation_Heat,
                StatDefOf.ArmorRating_Sharp,
                StatDefOf.ArmorRating_Blunt,
                StatDefOf.ArmorRating_Heat,
                StatDefOf.Insulation_Cold,
                StatDefOf.Insulation_Heat,
                StatDefOf.EnergyShieldRechargeRate,
                StatDefOf.EnergyShieldEnergyMax,
                StatDefOf.SmokepopBeltRadius,
                StatDefOf.JumpRange,
                StatDefOf.EquipDelay,
                StatDefOf.MedicalPotency,
                StatDefOf.MedicalQualityMax,
                StatDefOf.ImmunityGainSpeed,
                StatDefOf.ImmunityGainSpeedFactor,
                StatDefOf.DoorOpenSpeed,
                StatDefOf.BedRestEffectiveness,
                StatDefOf.TrapMeleeDamage,
                StatDefOf.TrapSpringChance,
                StatDefOf.ResearchSpeedFactor,
                StatDefOf.MedicalTendQualityOffset,
                StatDefOf.WorkTableWorkSpeedFactor,
                StatDefOf.WorkTableEfficiencyFactor,
                StatDefOf.JoyGainFactor,
                StatDefOf.SurgerySuccessChanceFactor,
                StatDefOf.Ability_CastingTime,
                StatDefOf.Ability_EntropyGain,
                StatDefOf.Ability_PsyfocusCost,
                StatDefOf.Ability_Duration,
                StatDefOf.Ability_Range,
                StatDefOf.Ability_EffectRadius,
                StatDefOf.Ability_RequiredPsylink,
                StatDefOf.Ability_GoodwillImpact,
                StatDefOf.Ability_DetectChancePerEntropy,
                StatDefOf.Bladelink_DetectionChance,
                StatDefOf.MeditationFocusStrength
            };*/

            var affectedStats = new List<StatDef>
            {
                 StatDefOf.MoveSpeed
            };

            foreach (var stat in affectedStats)
            {
                //Log.Warning("Inserting " + stat.ToString());
                try
                {
                    stat.parts.Insert(0, new StatPart(stat));
                } catch
                {
                    Log.Error(stat.ToString() + " is broken");
                }
            }
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref _Data, "data", LookMode.Reference, LookMode.Deep, ref _Things, ref _StatGroups);
        }

        Dictionary<Thing, StatGroup> _Data = new Dictionary<Thing, StatGroup>();
        List<Thing> _Things = new List<Thing>();
        List<StatGroup> _StatGroups = new List<StatGroup>();

        private StatGroup GetData(Pawn pawn)
        {
            if (!_Data.ContainsKey(pawn))
                _Data[pawn] = new StatGroup(pawn);
            return _Data[pawn];
        }

        public float GetFactor(Pawn pawn, StatDef stat)
        {
            return GetData(pawn).GetFactor(stat);
        }
    }
}
