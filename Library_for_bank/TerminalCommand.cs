using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using FluentTextTable;
using Org.BouncyCastle.Utilities.Encoders;
using System.Net.NetworkInformation;
using System.Xml.Linq;
using System.Security.Cryptography;

namespace Library_for_bank
{
    public class CommandHelp
    {
        public string command { get; set; }
        public string description { get; set; }
    }
    public class TerminalCommand
    {
        public event EventHandler<CommandEvent> StartSearchCommand;
        public string _command { get; private set; }
        public List<string> list_of_command { get; private set; }
        public List<string> list_of_description { get; private set; }
        public string query { get; private set; }
        public string name_of_user { get; private set; }
        
        
        public TerminalCommand(string command)
        {
            _command = command;
            list_of_command = new List<string>();
            list_of_description = new List<string>();
            list_of_command.Add("show all users");
            list_of_command.Add("show user");
            list_of_command.Add("add user");
            list_of_command.Add("exit");
            list_of_command.Add("show bank");
            list_of_command.Add("show all banks");
            list_of_command.Add("add bank");
            list_of_command.Add("--help");
            list_of_command.Add("-h");
            list_of_description.Add("Дана команда виводить всіх користувачів банку MyBank");
            list_of_description.Add("Дана команда виводить користувача банку MyBank, для користувача ви повинні ввести ім'я, фамілію чи по-батькові");
            list_of_description.Add("Дана команда надає можливість добавити користувача в банк MyBank");
            list_of_description.Add("Дана команда надає можливість вийти з терміналу");
            list_of_description.Add("Дана команда виводить дані про банкомат, ви повинні ввести адресу банкомату");
            list_of_description.Add("Дана команда виводить всіх банкомати для банку MyBank");
            list_of_description.Add("Дана команда надає можливість добавити банкомат в базу даних для адміністратора");
            list_of_description.Add("Дана команда виводить інформацію про команди, які встроєні в термінал");
            list_of_description.Add("Дана команда виводить інформацію про команди, які встроєні в термінал");

            if (_command == "show all users")
            {
                query = "select * from users";
            }
            else if (_command == "show user")
            {
                query = "select * from users where name like @name or surname like @surname or fatherly like @fatherly";
            }
            else if (_command == "add user")
            {
                query = "insert into users (name,surname,fatherly,number_telephone,pin,telephone,age,sex,number_card,balance,address_email) values (@s1,@s2,@s3,@s4,@s5,@s6,@s7,@s8,@s9,@s10,@s11)";
            }
            else if(_command == "show bank")
            {
                query = "select * from bank where adress like @adress";
            }
            else if(_command == "show all banks")
            {
                query = "select * from bank";
            }
            else  if(_command == "add bank")
            {
                query = "insert into bank (name,balance,adress) values (@s1,@s2,@s3)";
            }
            
        }
        public void StartSeaching()
        {
            if (StartSearchCommand != null)
            {
                StartSearchCommand(this, new CommandEvent(_command));
            }

        }
        public void OutputAllBank()
        {
            List<Bank> banks = new List<Bank>();
            string adress_of_bank;
            
            DB db = new DB();
            db.openConnection();
            using (MySqlCommand command = new MySqlCommand(query, db.getConnection()))
            {
                if (_command == "show bank")
                {
                    Console.Write("\nadmin@My_Bank:~# Введіть адресу банку : ");
                    adress_of_bank = Console.ReadLine();
                    command.Parameters.Add("@adress",MySqlDbType.VarChar).Value = "%" + adress_of_bank + "%";
                }
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Bank bank = new Bank();
                        bank.id = reader.GetInt32("id");
                        bank.name = reader.GetString("name");
                        bank.adress = reader.GetString("adress");
                        bank.balance = reader.GetDecimal("balance");
                        banks.Add(bank);
                    }
                }
            }
            db.closeConnection();
            var table = Build.TextTable<Bank>(builder =>
            {
                builder
                .Columns.Add(x => x.id).NameAs("ID").HorizontalAlignmentAs(HorizontalAlignment.Center)
                .Columns.Add(x => x.name).NameAs("NAME").HorizontalAlignmentAs(HorizontalAlignment.Center)
                .Columns.Add(x => x.adress).NameAs("Address").HorizontalAlignmentAs(HorizontalAlignment.Center)
                .Columns.Add(x => x.balance).NameAs("BALANCE").HorizontalAlignmentAs(HorizontalAlignment.Center);
            });
            table.WriteLine(banks);
            
        }
        public void AddBank()
        {
            Bank bank = new Bank();
            Console.Write("\nadmin@My_Bank:~# Введіть ім'я банку : ");
            bank.name = Console.ReadLine();
            Console.Write("\nadmin@My_Bank:~# Введіть адресу банку : ");
            bank.adress = Console.ReadLine();
            Console.Write("\nadmin@My_Bank:~# Введіть баланс банку : ");
            decimal temp_balance;
            while(!decimal.TryParse(Console.ReadLine(),out temp_balance))
            {
                Console.Write("\nadmin@My_Bank:~# Введіть правильне значення балансу банку : ");
            }
            bank.balance = temp_balance;
            DB db = new DB();
            db.openConnection();
            using (MySqlCommand cmd = new MySqlCommand(query, db.getConnection()))
            {
                cmd.Parameters.Add("@s1", MySqlDbType.VarChar).Value = bank.name;
                cmd.Parameters.Add("@s2", MySqlDbType.Decimal).Value = bank.balance;
                cmd.Parameters.Add("@s3", MySqlDbType.VarChar).Value = bank.adress;
                int result = cmd.ExecuteNonQuery();
                db.closeConnection();
                if(result > 0)
                {
                    void Result()
                    {
                        Console.WriteLine("\t\t\t\tДані були завантаженні на сервер, Все добре, без помилок!!!");
                    }
                    db.Result_of_INSERT(Result);
                }
                else
                {
                    void Result()
                    {
                        Console.WriteLine("\t\t\t\tДані не були завантаженні на сервер, сталися якісь проблеми!!!");
                    }
                    db.Result_of_INSERT(Result);
                }
            }
            
        }
        public void OutputAllUsers()
        {
            List<User> users = new List<User>();
            if (_command == "show user")
            {
                Console.Write("\nadmin@My_Bank:~# Enter name,surname,fatherly of user : ");
                name_of_user = Console.ReadLine().ToString();
            }
            DB db = new DB();
            db.openConnection();
            MySqlCommand cmd = new MySqlCommand(query, db.getConnection());
            if (_command == "show user")
            {
                cmd.Parameters.AddWithValue("@name", "%" + name_of_user + "%");
                cmd.Parameters.AddWithValue("@surname", "%" + name_of_user + "%");
                cmd.Parameters.AddWithValue("@fatherly", "%" + name_of_user + "%");
            }
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    User user = new User();
                    user.id = reader.GetInt32("id");
                    user.name = reader.GetString("name");
                    user.surname = reader.GetString("surname");
                    user.fatherly = reader.GetString("fatherly");
                    user.number_telephone = reader.GetString("number_telephone");
                    user.pin = reader.GetInt32("pin");
                    user.telephone = reader.GetString("telephone");
                    user.age = reader.GetInt32("age");
                    user.sex = reader.GetString("sex");
                    user.number_card = reader.GetString("number_card");
                    user.balance = reader.GetDecimal("balance");
                    user.email = reader.GetString("address_email");
                    users.Add(user);

                }
            }
            db.closeConnection();
            var table = Build.TextTable<User>(builder =>
            {
                builder
                    .Columns.Add(x => x.id).NameAs("ID").HorizontalAlignmentAs(HorizontalAlignment.Center)
                    .Columns.Add(x => x.name).NameAs("NAME").HorizontalAlignmentAs(HorizontalAlignment.Center)
                    .Columns.Add(x => x.surname).NameAs("SURNAME").HorizontalAlignmentAs(HorizontalAlignment.Center)
                    .Columns.Add(x => x.fatherly).NameAs("FATHERLY").HorizontalAlignmentAs(HorizontalAlignment.Center)
                    .Columns.Add(x => x.number_telephone).NameAs("NUMBER_TEL").HorizontalAlignmentAs(HorizontalAlignment.Center)
                    .Columns.Add(x => x.pin).NameAs("PIN").HorizontalAlignmentAs(HorizontalAlignment.Center)
                    .Columns.Add(x => x.telephone).NameAs("TELEPHONE").HorizontalAlignmentAs(HorizontalAlignment.Center)
                    .Columns.Add(x => x.age).NameAs("AGE").HorizontalAlignmentAs(HorizontalAlignment.Center)
                    .Columns.Add(x => x.sex).NameAs("SEX").HorizontalAlignmentAs(HorizontalAlignment.Center)
                    .Columns.Add(x => x.number_card).NameAs("NUMBER_CARD").HorizontalAlignmentAs(HorizontalAlignment.Center)
                    .Columns.Add(x => x.balance).NameAs("BALANCE").HorizontalAlignmentAs(HorizontalAlignment.Center)
                    .Columns.Add(x => x.email).NameAs("EMAIL").HorizontalAlignmentAs(HorizontalAlignment.Center);
            });
            table.WriteLine(users);
        }
        public void InputUser()
        {
            User user = new User();
            Console.Write("\nadmin@My_Bank:~# Введіть ім'я користувача банку : ");
            user.name = Console.ReadLine();
            Console.Write("\nadmin@My_Bank:~# Введіть фамілію користувача банку : ");
            user.surname = Console.ReadLine();
            Console.Write("\nadmin@My_Bank:~# Введіть по-батькові користувача банку : ");
            user.fatherly = Console.ReadLine();
            Console.Write("\nadmin@My_Bank:~# Введіть номер телефону користувача : ");
            user.number_telephone = Console.ReadLine();
            Console.Write("\nadmin@My_Bank:~# Введіть пін-код карти користувача : ");
            int temp_pin;
            while (!int.TryParse(Console.ReadLine(), out temp_pin))
            {
                Console.Write("\nadmin@My_Bank:~# Невірний ввід, введіть правильне значення для пінкода : ");
            }
            user.pin = temp_pin;
            Console.Write("\nadmin@My_Bank:~# Введіть назву телефону користувача : ");
            user.telephone = Console.ReadLine();
            Console.Write("\nadmin@My_Bank:~# Введіть скільки років користувачеві : ");
            int age_temp;
            while (!int.TryParse(Console.ReadLine(), out age_temp))
            {
                Console.Write("\nadmin@My_Bank:~# Невірний ввід, введіть правильне значення : ");
            }
            user.age = age_temp;
            Console.Write("\nadmin@My_Bank:~# Введіть пол користувача : ");
            user.sex = Console.ReadLine();
            Console.Write("\nadmin@My_Bank:~# Введіть електрону пошту користувача : ");
            user.email = Console.ReadLine();
            Random rand = new Random();
            DB db = new DB();
            user.number_card = GenerateRandomStringNumber(16);
            bool test;
            while(test = Test_to_unique_number_card(user.number_card, db))
            {
                user.number_card = GenerateRandomStringNumber(16);
            }
            user.balance = 0.000m;
            using (MySqlCommand cmd = new MySqlCommand(query, db.getConnection()))
            {
                cmd.Parameters.Add("@s1", MySqlDbType.VarChar).Value = user.name;
                cmd.Parameters.Add("@s2", MySqlDbType.VarChar).Value = user.surname;
                cmd.Parameters.Add("@s3", MySqlDbType.VarChar).Value = user.fatherly;
                cmd.Parameters.Add("@s4", MySqlDbType.VarChar).Value = user.number_telephone;
                cmd.Parameters.Add("@s5", MySqlDbType.Int32).Value = user.pin;
                cmd.Parameters.Add("@s6", MySqlDbType.VarChar).Value = user.telephone;
                cmd.Parameters.Add("@s7", MySqlDbType.Int32).Value = user.age;
                cmd.Parameters.Add("@s8", MySqlDbType.VarChar).Value = user.sex;
                cmd.Parameters.Add("@s9", MySqlDbType.VarChar).Value = user.number_card;
                cmd.Parameters.Add("@s10", MySqlDbType.Decimal).Value = user.balance;
                cmd.Parameters.Add("@s11", MySqlDbType.VarChar).Value = user.email;
                db.openConnection();
                int result = cmd.ExecuteNonQuery();
                db.closeConnection();
                if (result > 0)
                {
                    void Result()
                    {
                        Console.WriteLine("\t\t\t\tДані були завантаженні на сервер, Все добре, без помилок!!!");
                    }
                    db.Result_of_INSERT(Result);
                }
                else
                {
                    void Result()
                    {
                        Console.WriteLine("\t\t\t\tДані не були завантаженні на сервер, Є якась помилка!!!");
                    }
                    db.Result_of_INSERT(Result);
                }
            }
        }
        public string GenerateRandomStringNumber(int length)
        {
            Random rnd = new Random();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append(rnd.Next(0, 10));
            }
            return sb.ToString();
        }
        private bool Test_to_unique_number_card(string number, DB db)
        {
            string query = "SELECT (number_card) from users";
            List<string> existingNumbers = new List<string>();
            using (MySqlCommand cmd = new MySqlCommand(query, db.getConnection()))
            {
                db.openConnection();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        existingNumbers.Add(reader.GetString("number_card"));
                    }
                }
            }
            db.closeConnection();
            return existingNumbers.Contains(number);
        }
    }

    public class User
    {
        public int id { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string fatherly { get; set; }
        public string number_telephone { get; set; }
        public int pin { get; set; }
        public string telephone { get; set; }
        public int age { get; set; }
        public string sex { get; set; }
        public string number_card { get; set; }
        public decimal balance { get; set; }
        public string email { get; set; }
    }
    public class Bank
    {
        public int id { get; set; }
        public string name { get; set; }
        public decimal balance { get; set; }
        public string adress { get; set; }
    }
}
