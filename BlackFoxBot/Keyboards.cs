using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace BlackFoxBot
{
    class Keyboards
    {
        public static InlineKeyboardMarkup generalMenu = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Оставить отзыв", "cbReveiw")
            }
        });
        public static InlineKeyboardMarkup contactsMenu = new InlineKeyboardMarkup(new[]
{
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Telegram","cbContactTelegram")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Instagram","cbContactInstagram")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Vk","cbContactVk")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Whatsapp","cbContactWhatsapp")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Не связываться","cbContactNo")
            }
        });
        public static InlineKeyboardMarkup backMenuFeedbackState = new InlineKeyboardMarkup(new[]
{
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Изменить отзыв","cbReveiw")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Продолжить","cbContacts")
            }
        });
        public static InlineKeyboardMarkup backMenuContactsState = new InlineKeyboardMarkup(new[]
{
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Изменить/добавить контакт","cbContacts")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Продолжить","cbEnd")
            }
        });
        public static InlineKeyboardMarkup endMenu = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Вернуться на главное меню", "cbEnd")
            }
        });

        public static ReplyKeyboardMarkup replyKeyboard = new ReplyKeyboardMarkup(new[]
        {
            new[]
            {
                new KeyboardButton("Комплимент")
            },
            new[]
            {
                new KeyboardButton("Оскорбление")
            }
        });
    }
}
