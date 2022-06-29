using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlackFoxBot
{
    class Database
    {
        public delegate void CheckExist(bool exists);
        public delegate void CheckState(string str);
        public delegate void GetInfo(string[] info, int countColumns);
        public delegate void GetTestInfo(int numberColumns, string value);
        public static async Task AddUser(long id, string username)
        {
            await using var conn = new NpgsqlConnection(Config.connString);
            await conn.OpenAsync();
            await using (var cmd = new NpgsqlCommand($"INSERT INTO users (userid, username) VALUES ({id}, '{username}')", conn))
            {
                await cmd.ExecuteNonQueryAsync();
            }
            Console.WriteLine("Добавление пользователя " + id + " "+ username);
            await conn.CloseAsync();
        }
        public static async Task UpdateUser(long id, string column, string value)
        {
            await using var conn = new NpgsqlConnection(Config.connString);
            await conn.OpenAsync();
            await using (var cmd = new NpgsqlCommand($"UPDATE users SET {column} = '{value}' WHERE userid = '{id}'", conn))
            {

                await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
        }
        public static async Task CheckUser(long id, string username)
        {
            await using var conn = new NpgsqlConnection(Config.connString);
            await conn.OpenAsync();
            await using (var cmd = new NpgsqlCommand($"SELECT EXISTS(SELECT userid FROM users WHERE userid = {id})", conn))
            {
                cmd.Parameters.AddWithValue("userid", id);
                await using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        await Task.Factory.StartNew(() => User.CheckUser(reader.GetBoolean(0), id, username), TaskCreationOptions.AttachedToParent);

                    }
                }
            }
            await conn.CloseAsync();
        }
        public static async Task GetUserInfo(long id, User user)
        {

            await using var conn = new NpgsqlConnection(Config.connString);
            await conn.OpenAsync();
            await using (var cmd = new NpgsqlCommand($"SELECT * FROM users WHERE userid = {id}", conn))
            {
                await using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        string[] info = new string[reader.FieldCount];
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            info[i] = reader.GetValue(i).ToString();
                        }

                       await Task.Factory.StartNew(() => user.GetInfo(info), TaskCreationOptions.AttachedToParent);
                    }
                }
            }
            await conn.CloseAsync();
        }
        //public static async void CheckUserState(long id, CheckState check)
        //{
        //    await using var conn = new NpgsqlConnection(Config.connString);
        //    await conn.OpenAsync();
        //    await using (var cmd = new NpgsqlCommand($"SELECT state FROM users WHERE userid = {id}", conn))
        //    {
        //        await using (var reader = await cmd.ExecuteReaderAsync())
        //        {
        //            while (await reader.ReadAsync())
        //            {
        //                check(reader.GetString(0));
        //            }
        //        }
        //    }
        //    await conn.CloseAsync();
        //}
    }
}
