using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Npgsql;

namespace BlackFoxBot
{
    class Bot
    {
        static TelegramBotClient bot;
        static CancellationTokenSource cts = new CancellationTokenSource();
        static CancellationToken cancellationToken = cts.Token;

        static public void SendMenu(long id, string text, InlineKeyboardMarkup menu)
        {
            bot.SendTextMessageAsync(id, text, null, null, null, null, null, null, menu, cancellationToken);
        }
        public async Task MessageListenerAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is Message message)
            {
                await Task.Run(() => MessageListener(message));
            }
            if (update.CallbackQuery is CallbackQuery callbackQuery)
            {
                await Task.Run(() => CallbackListener(callbackQuery));
            }
        }
        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Debug.WriteLine($"Ебучая ошибка такова: {exception.Message}");
            if (exception is ApiRequestException apiRequestException)
            {
                await botClient.SendTextMessageAsync(123, apiRequestException.ToString());
            }
        }
        public void commandHandler(CallbackQuery callbackQuery, User user)
        {
            switch (callbackQuery.Data)
            {
                case "cbReveiw":
                    bot.SendTextMessageAsync(callbackQuery.From.Id, "Подробно опишите что случилось или ваши пожелания", null, null, null, null, null, null, null, cancellationToken);
                    user.AwaitingFeedback();
                    break;
                case "cbContacts":
                    user.updateInfo("contactwithme", "Да");
                    SendMenu(callbackQuery.From.Id, "Укажите мессенджер для связи с вами", Keyboards.contactsMenu);
                    break;
                case "cbContactTelegram":
                    //user.AwaitingMain();
                    //receivingMessage(user);
                    SendMenu(callbackQuery.From.Id, "Ваш telegram был добавлен", Keyboards.backMenuContactsState);
                    break;
                case "cbContactInstagram":
                    bot.SendTextMessageAsync(callbackQuery.From.Id, "Укажите ваш instagram", null, null, null, null, null, null, null, cancellationToken);
                    user.AwaitingContacts();
                    user.updateInfo("instagram", "Ожидание");
                    break;
                case "cbContactVk":
                    bot.SendTextMessageAsync(callbackQuery.From.Id, "Укажите ваш vk", null, null, null, null, null, null, null, cancellationToken);
                    user.AwaitingContacts();
                    user.updateInfo("vk", "Ожидание");
                    break;
                case "cbContactWhatsapp":
                    bot.SendTextMessageAsync(callbackQuery.From.Id, "Укажите ваш whatsapp", null, null, null, null, null, null, null, cancellationToken);
                    user.AwaitingContacts();
                    user.updateInfo("whatsapp", "Ожидание");
                    break;
                case "cbContactNo":
                    user.updateInfo("contactwithme", "Нет");
                    receivingMessage(user);
                    SendMenu(callbackQuery.From.Id, "Спасибо за ваш отзыв, что вы хотите сделать?", Keyboards.generalMenu);
                    //user.updateInfo("whatsapp", "Ожидание"); указание в таблице чтобы не связывались с человеком
                    break;
                case "cbEnd":
                    receivingMessage(user);
                    SendMenu(callbackQuery.From.Id, "Спасибо за ваш отзыв, мы обязательно свяжемся с вами, что вы хотите сделать?", Keyboards.generalMenu);
                    break;

            }
        }
        public void menuHandler(Message message, User user)
        {
            if (message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
            {
                switch (message.Text)
                {
                    case "/buttonOn":
                        bot.SendTextMessageAsync(message.Chat.Id, "Ваши кнопки сэр", null, null, null, null, null, null, Keyboards.replyKeyboard, cancellationToken);
                        break;
                    case "/start":
                        Console.WriteLine("Вызов главного меню");
                        SendMenu(user.id, "Здравствуйте\nЧто вы хотите сделать?", Keyboards.generalMenu);
                        break;
                    case "/sendMessage":
                        if (user.role == "admin")
                        {
                            bot.SendTextMessageAsync(message.Chat.Id, "Напишите id пользователя, после чего знак $ и ваше сообщение. Пример: 1111111$Ваше сообщение", null, null, null, null, null, null, null, cancellationToken);
                            user.AwaitingMessage();
                        }
                        else
                        {
                            bot.SendTextMessageAsync(message.Chat.Id, "У вас недостаточно прав", null, null, null, null, null, null, null, cancellationToken);
                        }
                        break;
                    case "Комплимент":
                        bot.SendTextMessageAsync(message.Chat.Id, "Вы прекрасны ", null, null, null, null, null, null, null, cancellationToken);
                        break;
                    case "Оскорбление":
                        bot.SendTextMessageAsync(message.Chat.Id, "Вы отвратительны", null, null, null, null, null, null, null, cancellationToken);
                        break;
                }
            }
        }
        private void CallbackListener(CallbackQuery callbackQuery)
        {
            commandHandler(callbackQuery, User.UserHandler(callbackQuery.From.Id, callbackQuery.From.Username));
        }
        private void receivingMessage(User user)
        {
            bot.SendTextMessageAsync(Config.receivingId,
                                "Пришел новый отзыв" +
                                $"\nОтзыв: {user.feedbackMessage}" +
                                $"\nНужно связываться: {user.contactWithMe}" +
                                "\nКонтакты:" +
                                $"\nTelegram: {user.username}" +
                                $"\nInstagram: {user.contacts.instagram}" +
                                $"\nVk: {user.contacts.vk}" +
                                $"\nWhatsapp: {user.contacts.whatsapp}", null, null, null, null, null, null, null, cancellationToken);
        }
        private void userStateHandler(User user, Message message)
        {
            switch (user.State.StateName)
            {
                case "mainState":
                    menuHandler(message, user);
                    break;
                case "feedbackState":
                    Console.WriteLine("ПРИШОЛ ОТЗЫВ");
                    if (message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
                    {
                        user.updateInfo("feedbackmessage", message.Text);
                    }
                    user.AwaitingMain();
                    //SendMenu(message.Chat.Id, "Укажите в каком мессенджере вы хотите чтобы мы связались с вами", Keyboards.contactsMenu);
                    SendMenu(message.Chat.Id, "Ваш отзыв был добавлен", Keyboards.backMenuFeedbackState);
                    break;
                case "contactsState":
                    if (user.contacts.instagram == "Ожидание")
                    {
                        user.updateInfo("instagram", message.Text);
                        user.AwaitingMain();
                        //receivingMessage(user);
                        SendMenu(message.From.Id, "Ваш instagram был добавлен", Keyboards.backMenuContactsState);
                    }
                    else if (user.contacts.vk == "Ожидание")
                    {
                        user.updateInfo("vk", message.Text);
                        user.AwaitingMain();
                        //receivingMessage(user);
                        SendMenu(message.From.Id, "Ваши vk был добавлен", Keyboards.backMenuContactsState);
                    }
                    else if (user.contacts.whatsapp == "Ожидание")
                    {
                        user.updateInfo("whatsapp", message.Text);
                        user.AwaitingMain();
                        //receivingMessage(user);
                        SendMenu(message.From.Id, "Ваш whatsapp был добавлен", Keyboards.backMenuContactsState);
                    }
                    break;
                case "messageState":
                    string[] str = message.Text.Split('$');
                    int index = message.Text.IndexOf('$');
                    try
                    {
                        bot.SendTextMessageAsync(Convert.ToInt64(message.Text.Substring(0, index)), message.Text.Substring(index + 1), null, null, null, null, null, null, null, cancellationToken);
                    }
                    catch
                    {
                        Console.WriteLine("Не получилось отправить сообщение");
                    }
                    user.AwaitingMain();
                    break;
            }
        }
        private void MessageListener(Message message)
        {
            User user = User.UserHandler(message.From.Id, message.From.Username);
            userStateHandler(user, message);
        }
        public void StartBot()
        {
            bot = new TelegramBotClient(Config.Token);
            bot.StartReceiving(new DefaultUpdateHandler(MessageListenerAsync, HandleErrorAsync), null, cancellationToken);
            Console.ReadLine();
        }
    }
}
