using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

internal class Program
{
    private static void Main(string[] args)
    {
        Start();
        Console.ReadLine();
    }
    private static async void Start()
    {
        var botClient = new TelegramBotClient("7147395978:AAHhU2ASmjSOWFcYTMW7WrUYpKvik_mNyvQ");

        using CancellationTokenSource cts = new();

        // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
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
    }
    private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        await HandleMessageAsync(botClient, update, cancellationToken);
        await HandleCallbackAsync(botClient, update, cancellationToken);

    }
    private static async Task HandleCallbackAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
     
        if (update == null || update.CallbackQuery == null)
        {
            return;//выход
        }
        string answer = update.CallbackQuery.Data!;
        //if (answer == "11")
        //{
        //    await botClient.SendTextMessageAsync(
        //    chatId: update.CallbackQuery.Message.Chat.Id,
        //    text: answer+" first button",
        //    cancellationToken: cancellationToken);
        //}
        switch (answer)
        {
            case "11":
                await botClient.SendTextMessageAsync(
           chatId: update.CallbackQuery.Message.Chat.Id,
           text: answer + " first button",
           cancellationToken: cancellationToken);
                break;
            case "12":
                await botClient.SendTextMessageAsync(
           chatId: update.CallbackQuery.Message.Chat.Id,
           text: answer,
           cancellationToken: cancellationToken);
                break;
            case "21":
                await botClient.SendTextMessageAsync(
           chatId: update.CallbackQuery.Message.Chat.Id,
           text: " Hi",
           cancellationToken: cancellationToken);
                break;
            case "22":
                await botClient.SendTextMessageAsync(
           chatId: update.CallbackQuery.Message.Chat.Id,
           text: answer ,
           cancellationToken: cancellationToken);
                break;
        }

    }
    private static async Task HandleMessageAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // Only process Message updates: https://core.telegram.org/bots/api#message
        if (update.Message is not { } message)
            return;
        // Only process text messages
        if (message.Text is not { } messageText)
            return;

        var chatId = message.Chat.Id;
        if (message.Text == "/start")
        {
            InlineKeyboardMarkup inlineKeyboard = new(new[]
            {
                // first row
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "1.1", callbackData: "11"),
                    InlineKeyboardButton.WithCallbackData(text: "1.2", callbackData: "12"),
                },
                // second row
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "2.1", callbackData: "21"),
                    InlineKeyboardButton.WithCallbackData(text: "2.2", callbackData: "22"),
                },
            });

            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "A message with an inline keyboard markup",
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken);
        }
    }


    private static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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