using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.InputFiles;
using System.Diagnostics;
using Telegram.Bot.Requests;
using System.Linq.Expressions;
using System.Text;

namespace TikTokLoaderTgBot
{
    public class Program
    {
        public static ITelegramBotClient bot;
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Some events
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                if (message.Text.ToLower() == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "To get the video file, send the bot a link to the video in tiktok.");
                    return;
                }
                if (message.Text.ToLower() == "/id")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Your id: " + message.Chat.Id.ToString());
                    return;
                }

                if (message.Text.StartsWith("https://") && message.Text.Contains("tiktok"))
                {
                    var videoLoader = new TiktokLoader();
                    var stream = videoLoader.DownloadFile(message.Text);

                    if(stream == null)
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "Error: " + videoLoader.Error, replyToMessageId: message.MessageId);
                        return;
                    }

                    Message msg = botClient.SendVideoAsync(message.Chat, new InputOnlineFile(stream), replyToMessageId: message.MessageId).GetAwaiter().GetResult();
                    return;
                }
            }
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }


        static void Main(string[] args)
        {
            string tgToken = Environment.GetEnvironmentVariable("botToken");
            
            if(tgToken == null || tgToken == "")
            {
                Console.WriteLine("Error: invalid token for bot. Get a token in a telegram bot: @BotFather\n" +
                    "And run the this bot with arguments: -e \"botToken=your:token\"\n");
                return;
            }

            bot = new TelegramBotClient(tgToken);

            try
            {
                Console.WriteLine("Bot launched " + bot.GetMeAsync().Result.FirstName);
            }
            catch (Exception e)
            {
                Console.WriteLine("The exception was caused by an attempt to start the bot. Info: " + e.Message
                    + "Most often this is associated with a token for a bot. Recheck the validity of the token.");
                return;
            }

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            //Console.ReadLine();

            Thread.Sleep(Timeout.Infinite);

            return;
        }
    }
}