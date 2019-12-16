using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using Rocket.Unturned.Skills;
using System.Collections.Generic;
using System.Linq;

namespace BuffSystem
{
    public class Elexir
    {
        public ushort itemId;
        public UnturnedSkill[] skills;
        public byte[] levelup;
        public int time;

        public Elexir(ushort id, int time, string[] skls, ushort[] lvlup)
        {
            itemId = id;
            this.time = time;
            skills = ConvertSkill(skls);
            levelup = new byte[lvlup.Length];
            for (int i = 0; i < lvlup.Length; i++)
                levelup[i] = (byte)lvlup[i];
        }

        public void CheckItem(UnturnedPlayer player)
        {
            var equip = player.Player.equipment;
            if (equip.itemID != itemId) return;
            ulong steamId = player.CSteamID.m_SteamID;
            var boostlevel = new byte[skills.Length];
            for (int i = 0; i < skills.Length; i++)
            {
                var skill = player.GetSkill(skills[i]);
                if (skill.level + levelup[i] <= skill.max)
                {
                    boostlevel[i] = levelup[i];
                    player.SetSkillLevel(skills[i], (byte)(skill.level + levelup[i]));
                }
                else
                {
                    boostlevel[i] = (byte)(skill.max - skill.level);
                    player.SetSkillLevel(skills[i], skill.max);
                }
            }
            BuffSystem.Manager.AddBuff(time, steamId, boostlevel, skills);
        }

        public static UnturnedSkill[] ConvertSkill(string[] skills)
        {
            UnturnedSkill[] result = new UnturnedSkill[skills.Length];
            for (int i = 0; i < skills.Length; i++)
            {
                switch (skills[i])
                {
                    case "Overkill":
                        result[i] = UnturnedSkill.Overkill;
                        break;
                    case "Agriculture":
                        result[i] = UnturnedSkill.Agriculture;
                        break;
                    case "Fishing":
                        result[i] = UnturnedSkill.Fishing;
                        break;
                    case "Cooking":
                        result[i] = UnturnedSkill.Cooking;
                        break;
                    case "Outdoors":
                        result[i] = UnturnedSkill.Outdoors;
                        break;
                    case "Crafting":
                        result[i] = UnturnedSkill.Crafting;
                        break;
                    case "Healing":
                        result[i] = UnturnedSkill.Healing;
                        break;
                    case "Survival":
                        result[i] = UnturnedSkill.Survival;
                        break;
                    case "Warmblooded":
                        result[i] = UnturnedSkill.Warmblooded;
                        break;
                    case "Strength":
                        result[i] = UnturnedSkill.Strength;
                        break;
                    case "Toughness":
                        result[i] = UnturnedSkill.Toughness;
                        break;
                    case "Immunity":
                        result[i] = UnturnedSkill.Immunity;
                        break;
                    case "Vitality":
                        result[i] = UnturnedSkill.Vitality;
                        break;
                    case "Sneakybeaky":
                        result[i] = UnturnedSkill.Sneakybeaky;
                        break;
                    case "Parkour":
                        result[i] = UnturnedSkill.Parkour;
                        break;
                    case "Diving":
                        result[i] = UnturnedSkill.Diving;
                        break;
                    case "Exercise":
                        result[i] = UnturnedSkill.Exercise;
                        break;
                    case "Cardio":
                        result[i] = UnturnedSkill.Cardio;
                        break;
                    case "Dexerity":
                        result[i] = UnturnedSkill.Dexerity;
                        break;
                    case "Sharpshooter":
                        result[i] = UnturnedSkill.Sharpshooter;
                        break;
                    case "Mechanic":
                        result[i] = UnturnedSkill.Mechanic;
                        break;
                    case "Engineer":
                        result[i] = UnturnedSkill.Engineer;
                        break;
                }
            }
            return result;
        }
        public static string[] ConvertSkill(UnturnedSkill[] skills)
        {
            string[] result = new string[skills.Length];
            for (int i = 0; i < skills.Length; i++)
                if (skills[i] == UnturnedSkill.Overkill)
                    result[i] = "Overkill";
                else if (skills[i] == UnturnedSkill.Agriculture)
                    result[i] = "Agriculture";
                else if (skills[i] == UnturnedSkill.Fishing)
                    result[i] = "Fishing";
                else if (skills[i] == UnturnedSkill.Cooking)
                    result[i] = "Cooking";
                else if (skills[i] == UnturnedSkill.Outdoors)
                    result[i] = "Outdoors";
                else if (skills[i] == UnturnedSkill.Crafting)
                    result[i] = "Crafting";
                else if (skills[i] == UnturnedSkill.Healing)
                    result[i] = "Healing";
                else if (skills[i] == UnturnedSkill.Survival)
                    result[i] = "Survival";
                else if (skills[i] == UnturnedSkill.Warmblooded)
                    result[i] = "Warmblooded";
                else if (skills[i] == UnturnedSkill.Strength)
                    result[i] = "Strength";
                else if (skills[i] == UnturnedSkill.Toughness)
                    result[i] = "Toughness";
                else if (skills[i] == UnturnedSkill.Immunity)
                    result[i] = "Immunity";
                else if (skills[i] == UnturnedSkill.Vitality)
                    result[i] = "Vitality";
                else if (skills[i] == UnturnedSkill.Sneakybeaky)
                    result[i] = "Sneakybeaky";
                else if (skills[i] == UnturnedSkill.Parkour)
                    result[i] = "Parkour";
                else if (skills[i] == UnturnedSkill.Diving)
                    result[i] = "Diving";
                else if (skills[i] == UnturnedSkill.Exercise)
                    result[i] = "Exercise";
                else if (skills[i] == UnturnedSkill.Cardio)
                    result[i] = "Cardio";
                else if (skills[i] == UnturnedSkill.Dexerity)
                    result[i] = "Dexerity";
                else if (skills[i] == UnturnedSkill.Sharpshooter)
                    result[i] = "Sharpshooter";
                else if (skills[i] == UnturnedSkill.Mechanic)
                    result[i] = "Mechanic";
                else if (skills[i] == UnturnedSkill.Engineer)
                    result[i] = "Engineer";
            return result;
        }
    }

    public class BuffManager
    {
        private List<Buff> buffs;

        public BuffManager()
        {
            buffs = new List<Buff>();
        }

        public void CheckEndBuff(UnturnedPlayer player)
        {
            if (buffs.Count == 0) return;
            foreach (Buff x in buffs)
            {
                if (x.SteamId == player.CSteamID.m_SteamID)
                {
                    if (!player.Dead && x.Time > 0)
                        x.Time--;
                    if (x is BuffSkill)
                    {
                        BuffSkill t = x as BuffSkill;
                        if (t.Time == 0 || player.Dead)
                        {
                            t.Time = -1;
                            for (int i = 0; i < t.skill.Length; i++)
                            {
                                var skilllevel = player.GetSkill(t.skill[i]).level;
                                if (skilllevel < t.boostLevel[i])
                                    player.SetSkillLevel(t.skill[i], 0);
                                else
                                    player.SetSkillLevel(t.skill[i], (byte)(skilllevel - t.boostLevel[i]));
                            }
                        }
                    }
                    else if (x is BuffHeal)
                    {
                        BuffHeal t = x as BuffHeal;
                        if (t.Time == 0 || player.Dead)
                            t.Time = -1;
                        if (t.ElapsedRate > 0) t.ElapsedRate--;
                        if (t.ElapsedRate == 0)
                        {
                            player.Player.life.askHeal((byte)t.Power, false, false);
                            t.ElapsedRate = t.Rate;
                        }
                    }
                }
            }
            buffs = buffs.Where(x => x.Time >= 0).ToList();
        }

        public void SaveBuffsToXML()
        {
            BuffSystem.Instance.Configuration.Load(BuffSystem.Instance.LoadItems);
            BuffSystem.Instance.Configuration.Instance.BuffHeal = new List<BuffHeal>();
            BuffSystem.Instance.Configuration.Instance.BuffSkills = new List<BuffSkillXML>();
            for (int i = 0; i < buffs.Count; i++)
                if (buffs[i] is BuffSkill)
                {
                    BuffSkill t = buffs[i] as BuffSkill;
                    BuffSystem.Instance.Configuration.Instance.BuffSkills.Add(new BuffSkillXML(t.boostLevel.Select(x => { return (ushort)x; }).ToArray(), t.Time, t.SteamId, Elexir.ConvertSkill(t.skill)));
                }
                else if (buffs[i] is BuffHeal)
                    BuffSystem.Instance.Configuration.Instance.BuffHeal.Add(buffs[i] as BuffHeal);
            BuffSystem.Instance.Configuration.Save();
        }

        public void AddBuff(int time, ulong SteamId, byte[] boostLevel, UnturnedSkill[] skill)
        {
            buffs.Add(new BuffSkill(boostLevel, time, SteamId, skill));
        }
        public void AddBuff(int time, ulong SteamId, int power, int rate)
        {
            buffs.Add(new BuffHeal(SteamId, time, power, rate));
        }
        public void AddBuff(List<BuffHeal> buff)
        {
            buffs.AddRange(buff.Select(x => x.Clone() as BuffHeal).ToArray());
        }
        public void ClearBuffs()
        {
            buffs.Clear();
        }
        public int Count()
        {
            return buffs.Count;
        }

        public class Buff
        {
            public int Time;
            public ulong SteamId;
        }

        private class BuffSkill: Buff
        {
            public byte[] boostLevel;
            public UnturnedSkill[] skill;

            public BuffSkill(byte[] boostLevel, int time, ulong SteamId, UnturnedSkill[] skill)
            {
                this.boostLevel = boostLevel;
                this.Time = time;
                this.SteamId = SteamId;
                this.skill = skill;
            }
        }
    }
}
