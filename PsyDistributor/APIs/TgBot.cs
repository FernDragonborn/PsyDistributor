using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PsyDistributor.APIs
{
    internal static class TgBot
    {
        private static readonly TelegramBotClient botClient = new TelegramBotClient("5233647141:AAE8RtgZfUTh1Nxl8NF9e-uXFbwtpoikPvE");
        internal static async Task TgInit()
        {
            using var cts = new CancellationTokenSource();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
            };
            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            var me = await botClient.GetMeAsync();

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            // Send cancellation request to stop bot
            cts.Cancel();

            async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
            {
                // Only process Message updates: https://core.telegram.org/bots/api#message
                if (update.Type != UpdateType.Message)
                    return;
                // Only process text messages
                if (update.Message!.Type != MessageType.Text)
                    return;

                var chatId = update.Message.Chat.Id;
                var messageText = update.Message.Text;

                if (messageText.ToLower() == "слава україні")
                {
                    Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

                    Message sentMessagePthPnh = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Путін - хуйло!",
                        cancellationToken: cancellationToken);
                }
                else
                {
                    Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");
                    // Echo received message text
                    Message sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "You said:\n" + messageText,
                        cancellationToken: cancellationToken);
                }

            }

            Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
            {
                var ErrorMessage = exception switch
                {
                    ApiRequestException apiRequestException
                        => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                    _ => exception.ToString()
                };

                Console.WriteLine(ErrorMessage);
                return Task.CompletedTask;
            }

        }
    }
}
