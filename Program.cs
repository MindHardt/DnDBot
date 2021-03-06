using System;
using DSharpPlus;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

namespace DnDBot
{
    class Program
    {
        private static Dictionary<string, List<Macro>> Macros = new Dictionary<string, List<Macro>>();
        private static readonly string token = FileWorker.GetLine(FileWorker.CreateUnexsistent(Environment.CurrentDirectory + "\\token.txt"), 1);
        static void Main(string[] args)
        {
            MainTask(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }
        static async Task MainTask(string[] args)
        {
            var discord = new DiscordClient(new DiscordConfiguration
            {
                Token = token,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug
            }) ;
            discord.MessageCreated += async e =>
            {
                string message = e.Message.Content.ToLower();
                if (Macros.TryGetValue(e.Author.Mention, out List<Macro> macros))
                {
                    message = Macro.UseMacros(macros, message);
                }
                if (message.StartsWith("=m"))
                {
                    if (message.StartsWith("=mcreate"))
                    {

                    }
                    if (message.StartsWith("=mlist"))
                    {

                    }
                    if (message.StartsWith("=mstart"))
                    {
                        string filePath = Environment.CurrentDirectory + "\\Macros\\" + e.Author.Mention + ".Macros.txt";
                        string respond = "Запускаю создание словаря макросов...\n";
                        FileWorker.CreateUnexsistentMacrosFile(filePath, out string log);
                        respond += log;

                    }
                    if (message.StartsWith("=mdelete"))
                    {

                    }
                    if (message.StartsWith("=moverwrite"))
                    {
                        string filePath = Environment.CurrentDirectory + "\\Macros\\" + e.Author.Mention + ".Macros.txt";
                    }
                }
                if (message == "=open")
                {
                    System.Diagnostics.Process.Start("explorer", Environment.CurrentDirectory);
                }
                if (message.StartsWith("=r"))
                {
                    await e.Message.RespondAsync(e.Author.Mention + "\n" +DnD.Roll(message, out int xyu));
                }
                if (message == "=u gay")
                {
                    await e.Message.CreateReactionAsync(DSharpPlus.Entities.DiscordEmoji.FromName(discord, ":regional_indicator_n:"));
                    await e.Message.CreateReactionAsync(DSharpPlus.Entities.DiscordEmoji.FromName(discord, ":regional_indicator_o:"));
                    await e.Message.CreateReactionAsync(DSharpPlus.Entities.DiscordEmoji.FromName(discord, ":regional_indicator_u:"));
                    await e.Message.CreateReactionAsync(DSharpPlus.Entities.DiscordEmoji.FromName(discord, ":middle_finger:"));
                }
            };
            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
    class Macro
    {
        public string Key { get; }
        public string Value { get; }
        public static KeyValuePair<string, string> GetMacro(string pair)
        {
            string[] pairs = pair.Split('>');
            return new KeyValuePair<string, string>(pairs[0], pairs[1]);
        }
        public static string UseMacros(List<Macro> list, string line)
        {
            foreach (Macro macro in list)
            {
                if (line == macro.Key) return macro.Value;
            }
            return line;

        }
        public static void Overwrite(string path, List<Macro> list)
        {
            File.WriteAllText(path, MacrosList(list));
        }

        public static string MacrosList(List<Macro> list)
        {
            string result = String.Empty;
            foreach (Macro macro in list)
            {
                result += macro.Key + ">" + macro.Value + "\n";
            }
            return result;
        }
    }
    
}
