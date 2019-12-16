using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using Rocket.Unturned.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BuffSystem
{
    public class Elexir
    {
        public ushort itemId;
        public UnturnedSkill[] skills;
        public byte[] levelup;
        public int time;
        List<ulong> drinkingPlayers;

        public Elexir(ushort id, int time, string[] skls, ushort[] lvlup)
        {
            itemId = id;
            this.time = time;
            skills = ConvertSkill(skls);
            levelup = new byte[lvlup.Length];
            for (int i = 0; i < lvlup.Length; i++)
                levelup[i] = (byte)lvlup[i];
            drinkingPlayers = new List<ulong>();
        }

        public void CheckItem(UnturnedPlayer player)
        {
            var equip = player.Player.equipment;
            if (equip.itemID != itemId) return;
            ulong steamId = player.CSteamID.m_SteamID;
            if (!drinkingPlayers.Contains(steamId) && equip.itemID == itemId && equip.isSelected && equip.isBusy)
            {
                drinkingPlayers.Add(steamId);
                UnturnedChat.Say("Start");
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
                BuffSystem.Manager.AddBuff(boostlevel, time, steamId, skills);
            }
            else if (drinkingPlayers.Contains(steamId) && !equip.isBusy)
            {
                drinkingPlayers.Remove(steamId);
                UnturnedChat.Say("Stop");
            }
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
            {
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
            }
            return result;
        }
    }
}
