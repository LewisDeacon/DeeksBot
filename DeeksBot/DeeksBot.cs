namespace DeeksBot
{
    using Discord;
    using Discord.Commands;
    using System;
    using System.IO;
    using System.Net;

    public class DeeksBot
    {
        /// <summary>
        /// The discord client
        /// </summary>
        DiscordClient client;

        /// <summary>
        /// The commands service
        /// </summary>
        CommandService commands;

        /// <summary>
        /// The token for connecting to the bot
        /// </summary>
        private const string botToken = "";

        /// <summary>
        /// Initializes and instance of the DeeksBot
        /// </summary>
        public DeeksBot()
        {
            client = new DiscordClient(input =>
            {
                input.LogLevel = LogSeverity.Info;
                input.LogHandler = Log;
            });

            client.UsingCommands(input => 
            {
                input.PrefixChar = '!';
                input.AllowMentionPrefix = true;
            });

            commands = client.GetService<CommandService>();

            RegisterSteamStatusCommand();
            OsrsHighScoreCheckerCommand();
            DisplaySourceCommand();
            WowCharLookupCommand();

            client.ExecuteAndWait(async () =>
            {
                await client.Connect(botToken, TokenType.Bot);
            });
        }

        /// <summary>
        /// Gets the highscore for the supplied user and writes it as a message on the channel
        /// </summary>
        private void OsrsHighScoreCheckerCommand()
        {
            commands.CreateCommand("osrs").Parameter("name", ParameterType.Multiple).Do(async (e) =>
            {
                var highscores = GetOsrsHighScore(e);
                await e.Channel.SendMessage($"Highscores for {e.Args[0]}: \n{highscores}");
            });
        }

        /// <summary>
        /// Retrieves the highscores of the user through http request and uses the RunescapePlayer to generate a formatted string
        /// </summary>
        /// <param name="e">The commandEventArgs from the calling command</param>
        /// <returns></returns>
        private string GetOsrsHighScore(CommandEventArgs e)
        {
            var username = e.Args[0];
            string result;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"http://services.runescape.com/m=hiscore_oldschool/index_lite.ws?player=" + username);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }

            RunescapePlayer rsPlayer = new RunescapePlayer(username);

            var splitResult = result.Split(new Char[] { ',', '\n' });

            int i = 0;
            string rank;
            string level;
            string xp;

            foreach (Skill x in Enum.GetValues(typeof(Skill)))
            {
                rank = splitResult.GetValue(i).ToString().Equals("-1") ? "N/A" : splitResult.GetValue(i).ToString();
                level = splitResult.GetValue(i+1).ToString().Equals("-1") ? "N/A" : splitResult.GetValue(i+1).ToString();
                xp = splitResult.GetValue(i+2).ToString().Equals("-1") ? "N/A" : splitResult.GetValue(i+2).ToString();


                rsPlayer.PlayerSkillRank.Add(x, rank);
                rsPlayer.PlayerSkillLevel.Add(x, level);
                rsPlayer.PlayerSkillXp.Add(x, xp);
                i+=3;
            }
            return rsPlayer.RunescapePlayerInfoFormatted();
        }
       
        /// <summary>
        /// A command that displays the github repo for the bots code
        /// </summary>
        private void DisplaySourceCommand()
        {
            commands.CreateCommand("Source").Do(async (e) =>
            {
                await e.Channel.SendMessage("https://github.com/LewisDeacon/DeeksBot");
            });
        }

        /// <summary>
        /// A command that links to the downdetector website for steam
        /// </summary>
        private void RegisterSteamStatusCommand()
        {
            commands.CreateCommand("Steam").Do(async (e) =>
            {
                await e.Channel.SendMessage("http://downdetector.com/status/steam");
            });
        }

        /// <summary>
        /// Create the command for the keyword wow which does a lookup on the provided name at battle.net armoury
        /// </summary>
        private void WowCharLookupCommand()
        {
            commands.CreateCommand("wow").Parameter("charname", ParameterType.Multiple).Do(async (e) =>
            {
                await e.Channel.SendMessage($"Character lookup for {e.Args[1]}\n https://worldofwarcraft.com/en-gb/character/{e.Args[0]}/{e.Args[1]}");
            });
        }

        /// <summary>
        /// Log publisher
        /// </summary>
        /// <param name="sender">The bot</param>
        /// <param name="e">The log message event args</param>
        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
