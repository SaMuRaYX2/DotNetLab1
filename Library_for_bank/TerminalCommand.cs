using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using FluentTextTable;

namespace Library_for_bank
{
    public class TerminalCommand
    {
        public event EventHandler<CommandEvent> StartSearchCommand;
        public string _command { get; private set; }
        public List<string> list_of_command { get; private set; }
        public string query { get; private set; }
        public string name_of_user { get; private set; }
        public TerminalCommand(string command)
        {
            _command = command;
            list_of_command = new List<string>();
            list_of_command.Add("show all users");
            list_of_command.Add("show user");
            list_of_command.Add("add user");
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
                query = "insert into users (name,surname,fatherly,number_telephone,pin,telephone,age,sex,number_card) values (@s1,@s2,@s3,@s4,@s5,@s6,@s7,@s8,@s9)";
            }
        }
        public void StartSeaching()
        {
            if (StartSearchCommand != null)
            {
                StartSearchCommand(this, new CommandEvent(_command));
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
                    user.number_card = reader.GetInt32("number_card");
                    users.Add(user);

                }
            }
            db.closeConnection();
            var table = Build.TextTable<User>();
            Build.TextTable<User>(builder =>
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
                    .Columns.Add(x => x.number_card).NameAs("NUMBER_CARD").HorizontalAlignmentAs(HorizontalAlignment.Center);
            });
            table.WriteLine(users);
        }
        public void InputUser()
        {

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
        public int number_card { get; set; }
    }
}
