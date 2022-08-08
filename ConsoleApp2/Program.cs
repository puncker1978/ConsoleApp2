using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Net;
using System.Net.Http;

var botClient = new TelegramBotClient("TOKEN");

int year;
int month;
int day;
int hour;
int minute;
int second;

long chatId = 0;
string messageText;
int messageId;
string firstName;
string lastName;
long id;
Message sentMessage;

year = int.Parse(DateTime.UtcNow.Year.ToString());
month = int.Parse(DateTime.UtcNow.Month.ToString());
day = int.Parse(DateTime.UtcNow.Day.ToString());
hour = int.Parse(DateTime.UtcNow.Hour.ToString());
minute = int.Parse(DateTime.UtcNow.Minute.ToString());
second = int.Parse(DateTime.UtcNow.Second.ToString());
Console.WriteLine("Data: " + year + "/" + month + "/" + day);
Console.WriteLine("Time: " + hour + ":" + minute + ":" + second);

using var cts = new CancellationTokenSource();

// Старт бота
var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = { } // получение всех обновлений
};
botClient.StartReceiving(
    HandleUpdateAsync,
    HandleErrorAsync,
    receiverOptions,
    cancellationToken: cts.Token);

var me = await botClient.GetMeAsync();

Console.ReadKey();
cts.Cancel();


//Ответ бота на запросы пользователя
async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    if (update.Type != UpdateType.Message)
        return;
    if (update.Message!.Type != MessageType.Text)
        return;

    chatId = update.Message.Chat.Id;
    messageText = update.Message.Text;
    messageId = update.Message.MessageId;
    firstName = update.Message.From.FirstName;
    lastName = update.Message.From.LastName;
    id = update.Message.From.Id;
    year = update.Message.Date.Year;
    month = update.Message.Date.Month;
    day = update.Message.Date.Day;
    hour = update.Message.Date.Hour;
    minute = update.Message.Date.Minute;
    second = update.Message.Date.Second;

    messageText = messageText.ToLower();

    if (messageText == "/start")
    {
        // Echo received message text
        sentMessage = await botClient.SendTextMessageAsync(
        chatId: chatId,
        text: "Hello " + firstName + " " + lastName + "\n" +
        "Вот что я умею:\n" +
        "/meme -- мем\n" +
        "/sound -- музыка\n" +
        "/countdown -- обратный отсчёт\n" +
        "/album -- альбом с изображениями\n" +
        "/doc -- jpeg-изображение\n" +
        "/gif -- гифка\n",
        //replyMarkup: replyKeyboardMarkup,
        cancellationToken: cancellationToken);
    }

    //if message is "meme" .. bot answer with a meme image.
    if (messageText == "/meme")
    {
        sentMessage = await botClient.SendPhotoAsync(
        chatId: chatId,
        photo: "https://i.redd.it/uhkj4abc96r61.jpg",
        caption: "<b>MEME</b>",
        parseMode: ParseMode.Html,
        cancellationToken: cancellationToken);
    }

    //if message is "sound" .. bot answer with a Audio.
    if (messageText == "/sound")
    {
        Message message = await botClient.SendAudioAsync(
         chatId: chatId,
         audio: "https://github.com/TelegramBots/book/raw/master/src/docs/audio-guitar.mp3",
         cancellationToken: cancellationToken);
    }

    //if message is "countdown" .. bot answer with a countdown video.
    if (messageText == "/countdown")
    {
        Message message = await botClient.SendVideoAsync(
        chatId: chatId,
        video: "https://raw.githubusercontent.com/TelegramBots/book/master/src/docs/video-countdown.mp4",
        thumb: "https://raw.githubusercontent.com/TelegramBots/book/master/src/2/docs/thumb-clock.jpg",
        supportsStreaming: true,
        cancellationToken: cancellationToken);
    }

    //if message is "album" .. bot answer with multiple images.
    if (messageText == "/album")
    {
        Message[] messages = await botClient.SendMediaGroupAsync(
        chatId: chatId,
        media: new IAlbumInputMedia[]
        {
                new InputMediaPhoto("https://cdn.pixabay.com/photo/2017/06/20/19/22/fuchs-2424369_640.jpg"),
                new InputMediaPhoto("https://cdn.pixabay.com/photo/2017/04/11/21/34/giraffe-2222908_640.jpg"),
        },
        cancellationToken: cancellationToken);
    }

    //if message is "doc" .. bot answer with a doc.
    if (messageText == "/doc")
    {
        //Use sendDocument method to send general files.
        Message message = await botClient.SendDocumentAsync(
        chatId: chatId,
        document: "https://github.com/TelegramBots/book/raw/master/src/docs/photo-ara.jpg",
        caption: "<b>Ara bird</b>. <i>Source</i>: <a href=\"https://pixabay.com\">Pixabay</a>",
        parseMode: ParseMode.Html,
        cancellationToken: cancellationToken);
    }

    //if message is "album" .. bot answer with multiple images.
    if (messageText == "/gif")
    {
        //Use sendAnimation method to send animation files(GIF or H.264 / MPEG - 4 AVC video without sound).
        Message message = await botClient.SendAnimationAsync(
        chatId: chatId,
        animation: "https://raw.githubusercontent.com/TelegramBots/book/master/src/docs/video-waves.mp4",
        caption: "Waves",
        cancellationToken: cancellationToken);
    }
}

Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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

