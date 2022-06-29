using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BlackFoxBot
{
    interface IUserState
    {
        public string StateName { get; }
        void AwaitingFeedback(User user);
        void AwaitingContacts(User user);
        void AwaitingMain(User user);
        void AwaitingMessage(User user);
    }
    class User
    {
        public long id;
        public string username;
        public string feedbackMessage;
        public string role;
        public string contactWithMe;
        public (string instagram, string vk, string whatsapp) contacts;

        public IUserState State { get; set; }
        public User()
        {
        }
        public User(long id)
        {
            this.id = id;
        }
        public User(IUserState state)
        {
            State = state;
        }

        public User(long id, string username, string feedbackMessage, string role, string state, string instagram, string vk, string whatsapp)
        {
            switch (state)
            {
                case "mainState":
                    State = new MainState();
                    break;
                case "feedbackState":
                    State = new FeedbackState();
                    break;
                case "contactsState":
                    State = new ContactsState();
                    break;
                case "messageState":
                    State = new MessageState();
                    break;
            }
            this.id = id;
            this.username = username;
            this.feedbackMessage = feedbackMessage;
            this.role = role;
            contacts = (instagram, vk, whatsapp);
        }

        public static User UserHandler(long id, string username)
        {
            User user = new User();
            Task userCheck = Database.CheckUser(id, username);
            userCheck.Wait();
            Task getUserInfo = Database.GetUserInfo(id, user);
            getUserInfo.Wait();
            return user;
        }
        public static void CheckUser(bool exists, long id, string username)
        {
            if (!exists)
            {
                Task userAdd = Database.AddUser(id, username);
                userAdd.Wait();
            }
        }   

        public void GetInfo(string[] info)
        {
            id = Convert.ToInt64(info[0]);
            username = info[1];
            feedbackMessage = info[2];
            contacts = (info[5], info[6], info[7]);
            role = info[3];
            contactWithMe = info[8];
            switch (info[4])
            {
                case "mainState":
                    State = new MainState();
                    break;
                case "feedbackState":
                    State = new FeedbackState();
                    break;
                case "contactsState":
                    State = new ContactsState();
                    break;
                case "messageState":
                    State = new MessageState();
                    break;

            }
        }
        public void updateInfo(string column, string value)
        {
           Task userUpdate = Database.UpdateUser(id, column, value);
           userUpdate.Wait();
           Task getUserInfo = Database.GetUserInfo(id, this);
           getUserInfo.Wait();
        }
        public void AwaitingFeedback()
        {
            State.AwaitingFeedback(this);
        }
        public void AwaitingContacts()
        {
            State.AwaitingContacts(this);
        }
        public void AwaitingMain()
        {
            State.AwaitingMain(this);
        }
        public void AwaitingMessage()
        {
            State.AwaitingMessage(this);
        }


    }


    class MainState : IUserState
    {
        public string StateName { get; } = "mainState";
        public void AwaitingFeedback(User user)
        {
            Console.WriteLine("Переход пользователя в состояние отзыва");
            user.State = new FeedbackState();
            user.updateInfo("state", user.State.StateName);
        }
        public void AwaitingContacts(User user)
        {
            Console.WriteLine("Переход пользователя в состояние контактов");
            user.State = new ContactsState();
            user.updateInfo("state", user.State.StateName);
        }
        public void AwaitingMain(User user)
        {
        }  
        public void AwaitingMessage(User user)
        {
            Console.WriteLine("Переход пользователя в состояние отправки сообщения");
            user.State = new MessageState();
            user.updateInfo("state", user.State.StateName);
        }       


    }
    class FeedbackState : IUserState
    {
        public string StateName { get; } = "feedbackState";
        public void AwaitingFeedback(User user)
        {
        }
        public void AwaitingContacts(User user)
        {
            Console.WriteLine("Переход пользователя в состояние контактов");
            user.State = new ContactsState();
            user.updateInfo("state", user.State.StateName);
        }
        public void AwaitingMain(User user)
        {
            Console.WriteLine("Переход пользователя в главное состояние");
            user.State = new MainState();
            user.updateInfo("state", user.State.StateName);
        }
        public void AwaitingMessage(User user)
        {
        }

    }
    class ContactsState : IUserState
    {
        public string StateName { get; } = "contactsState";
        public void AwaitingFeedback(User user)
        {
            Console.WriteLine("Переход пользователя в состояние отзыва");
            user.State = new FeedbackState();
            user.updateInfo("state", user.State.StateName);
        }
        public void AwaitingContacts(User user)
        {
        }
        public void AwaitingMain(User user)
        {
            Console.WriteLine("Переход пользователя в главное состояние");
            user.State = new MainState();
            user.updateInfo("state", user.State.StateName);
        }
        public void AwaitingMessage(User user)
        {
        }

    }
    class MessageState : IUserState
    {
        public string StateName { get; } = "messageState";
        public void AwaitingFeedback(User user)
        {
        }
        public void AwaitingContacts(User user)
        {
        }
        public void AwaitingMain(User user)
        {
            Console.WriteLine("Переход пользователя в главное состояние");
            user.State = new MainState();
            user.updateInfo("state", user.State.StateName);
        }
        public void AwaitingMessage(User user)
        {
        }
    }
   
}
