using DeeksBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeksBot
{
    public enum Skill
    {
        Overall,
        Attack,
        Defence,
        Strength,
        Hitpoints,
        Ranged,
        Prayer,
        Magic,
        Cooking,
        Woodcutting,
        Fletching,
        Fishing,
        Firemaking,
        Crafting,
        Smithing,
        Mining,
        Herblore,
        Agility,
        Thieving,
        Slayer,
        Farming,
        Runecraft,
        Hunter,
        Construction
    }


    public class RunescapePlayer
    {
        public RunescapePlayer(string username)
        {
            this.Username = username;
            this.PlayerSkillRank = new Dictionary<Skill, string>();
            this.PlayerSkillLevel = new Dictionary<Skill, string>();
            this.PlayerSkillXp = new Dictionary<Skill, string>();
        }

        public string Username { get; set; }

        public IDictionary<Skill, string> PlayerSkillRank { get; set; }
        public IDictionary<Skill, string> PlayerSkillLevel { get; set; }
        public IDictionary<Skill, string> PlayerSkillXp { get; set; }

        public string RunescapePlayerInfoFormatted()
        {
            string format = "{0,-15} {1,-10} {2,-8} {3,-15}";
            StringBuilder formattedString = new StringBuilder();
            formattedString.AppendFormat(format, "```Skill", "    Rank", "  Level", "     XP");
            formattedString.AppendLine();
            foreach (Skill x in Enum.GetValues(typeof(Skill)))
            {
                formattedString.AppendFormat(format, x.ToString(), PlayerSkillRank[x], PlayerSkillLevel[x], PlayerSkillXp[x]);
                formattedString.AppendLine();
            }

            formattedString.Append("```");
            return formattedString.ToString();
        }
    }
}
