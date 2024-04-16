using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using YRB.Lib.Storage;
using YRB.Lib.Utils;

namespace YRB.Lib.Services
{
    public class BotWorker : IHostedService
    {
        private readonly ILogger<BotWorker> _logger;
        private readonly ChromePathSource _chromePathSource;
        private readonly UsersRepository _usersRepository;
        private readonly TelegramBotClient _telegramBotClient;
        private readonly Task _cromeFindingTask;
        private readonly CancellationTokenSource _cts = new();

        private readonly string _pwd;
        public BotWorker(IConfiguration configuration, ChromePathSource chromePathSource, UsersRepository usersRepository, ILogger<BotWorker> logger)
        {
            try
            {
                var token = configuration.GetSection("Bot:Token").Value;
                if (string.IsNullOrEmpty(token))
                {
                    throw new ApplicationException("bot token is null or empty!");
                }

                _telegramBotClient = new TelegramBotClient(token);
                _usersRepository = usersRepository;
                _chromePathSource = chromePathSource;
                _logger = logger;
                _logger.LogInformation("DbPath: " + YrbDbContext.GetDbPath());
                _cromeFindingTask = ChromeFinder(_cts.Token);
                _pwd = configuration.GetSection("Bot:UserPwd").Value ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in BotWorker constructor");
            }
        }

        private async Task ProcessUpdate(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                if (message != null)
                {
                    var from = message.From;
                    var chat = message.Chat;
                    if (from != null && chat != null && chat.Id == from.Id)
                    {
                        try
                        {
                            if (await _usersRepository.CheckUserExistance(from.Id))
                            {
                                if (message.Text != null)
                                {
                                    _logger.LogDebug(message.Text);
                                    if (message.Text.Contains("youtu"))
                                    {
                                        var dir = await _chromePathSource.GetChromeDirectory();
                                        _logger.LogDebug(dir ?? "chrome dir is null!");
                                        if (dir != null)
                                        {
                                            BrowserHelper.SendButtonPressToChrome("k");
                                            var task1 = Task.Delay(1000);
                                            BrowserHelper.OpenLink(message.Text, dir);
                                            await Task.Delay(5000);
                                            BrowserHelper.SendButtonPressToChrome("%+w");
                                            await Task.Delay(5000);
                                            BrowserHelper.SendButtonPressToChrome("f");
                                            BrowserHelper.SendButtonPressToChrome("%+w");
                                        }
                                    }
                                    else if (message.Text == "Во весь экран" || message.Text == "Уменьшить")
                                    {
                                        BrowserHelper.SendButtonPressToChrome("f");
                                    }
                                    else if (message.Text == "Пауза")
                                    {
                                        var processes = Process.GetProcesses();
                                        foreach (var proc in processes)
                                        {
                                            _logger.LogDebug(proc.ProcessName + "; " + proc.MainWindowTitle ?? "null" + ";");
                                        }
                                        BrowserHelper.SendButtonPressToChrome("k");
                                    }
                                    else if (message.Text == "Закрыть Chrome")
                                    {
                                        BrowserHelper.KillChromes();
                                    }
                                }
                            }
                            else if (message.Text == _pwd)
                            {
                                await _usersRepository.UpsertUser(from.Id, from.Username, from.FirstName);
                                var keyboard = new ReplyKeyboardMarkup(
                                    new[] {
                                    new KeyboardButton[] { "Во весь экран" },
                                    new KeyboardButton[] { "Пауза" },
                                    new KeyboardButton[] { "Закрыть Chrome" },
                                    });
                                keyboard.ResizeKeyboard = true;
                                await _telegramBotClient.SendTextMessageAsync(chat, "Авторизация успешна. Добро пожаловать! Для открытия в браузере видео с youtube-а просто отправьте мне ссылку на видео.", replyMarkup: keyboard);
                            }
                            else if (message.Text == "/start")
                            {
                                await _telegramBotClient.SendTextMessageAsync(chat, "Здравствуйте! Пожалуйста, отправьте пароль от бота.");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error while update processing!");
                        }
                    }
                }
            }
        }

        private Task ProcessError(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken) => Task.CompletedTask;

        private async Task ChromeFinder(CancellationToken cancellationToken)
        {
            try
            {
                var delay = 5000;
                while (!cancellationToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Chrome finding started!");
                    var path = BrowserHelper.GetChromePath();
                    if (path != null)
                    {
                        _logger.LogInformation("Path: " + path);
                        await _chromePathSource.UpsertPath(path);
                        delay = 60000;
                    }
                    else
                    {
                        _logger.LogInformation("Path == null!");
                    }
                    await Task.Delay(delay, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ChromeFinder");
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _telegramBotClient.StartReceiving(ProcessUpdate, ProcessError, new Telegram.Bot.Polling.ReceiverOptions()
                {
                    AllowedUpdates = new Telegram.Bot.Types.Enums.UpdateType[] { Telegram.Bot.Types.Enums.UpdateType.Message }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in StartAsync");
            }
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _telegramBotClient.CloseAsync();
        }
    }
}
