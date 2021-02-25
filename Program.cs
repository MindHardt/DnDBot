using System;
using DSharpPlus;
using System.Threading.Tasks;

namespace DnDBot
{
    class Program
    {
        static void Main(string[] args)
        {
            MainTask(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }
        static async Task MainTask(string[] args)
        {
            var discord = new DiscordClient(new DiscordConfiguration
            {
                Token = "ODEzMjk5MzY3ODA5MTIyMzY0.YDNSDw.fEdrSk_Ful-B1ISHIDH_fOb0dwU",
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug
            });
            discord.MessageCreated += async e =>
            {
                string message = e.Message.Content.ToLower();
                if (message == "=magicshop")
                {
                    await e.Message.RespondAsync(DnD.MagicShop());
                    await e.Message.DeleteAsync();
                }
                if (message.StartsWith("=r"))
                {
                    await e.Message.CreateReactionAsync(DSharpPlus.Entities.DiscordEmoji.FromName(discord, ":ok_hand:"));
                    await e.Message.RespondAsync(Roll(message));
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
        static string Roll(string s)
        {
            if (!CheckRollString(s, out int count, out int die, out int constant)) return ("**Введенные данные неверны, проверьте правильность написания.**\nПример верного ввода: *=r 2d8+3*, *=r 1d20*.");
            else
            {
                string result = constant < 0 ? $"**Бросок {count}d{die}{constant}**\n\n" : $"**Бросок {count}d{die}+{constant}**\n";
                int sum = constant;
                Random rnd = new Random();
                if (count > 10) result += "Вы бросаете слишком много кубов! Результаты бросков не будут приведены.";
                for (int i = 1; i <= count; i++)
                {
                    int currentDie = rnd.Next(1, die+1);
                    if (count <= 10) result += $"\nКуб №{i} = **{currentDie}**";
                    if (count == 1 && die == 20 && currentDie == 1) result += ":red_circle:";
                    if (count == 1 && die == 20 && currentDie == 20) result += ":green_circle:";
                    sum += currentDie;
                }
                result += $"\nКонстанта = **{constant}**\nРЕЗУЛЬТАТ = **{sum}**:white_check_mark:";
                return (result);
            };
        }
        static bool CheckRollString(string s, out int count, out int die, out int constant)
        {
            Console.WriteLine(s);
            s = s.Replace("=r", "");
            s = s.Replace("d", " ");
            s = s.Replace("+", " ");
            s = s.Replace("-", " -");
            Console.WriteLine(s);
            constant = 0;
            die = 1;
            string[] parts = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            bool result = parts.Length < 3 ? int.TryParse(parts[0], out count) && int.TryParse(parts[1], out die) && count > 0 && die > 0 : int.TryParse(parts[0], out count) && int.TryParse(parts[1], out die) && int.TryParse(parts[2], out constant) && count > 0 && die > 0;
            return (result);
        }

    }
}
