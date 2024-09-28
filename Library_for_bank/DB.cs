using MySql.Data.MySqlClient;
namespace Library_for_bank
{
    public class DB
    {
        public event EventHandler<UserBankArgs> Start_Fill_DataBase;
        public event EventHandler<UserBankArgs> Select_from_DataBase;
        public delegate void Result_Of_INSERT();
        MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;username=root;password=root;database=mybank;");
        public void openConnection()
        {
            if(connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();
            }
        }
        public void closeConnection()
        {
            if(connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
            }
        }
        public MySqlConnection getConnection()
        {
            return connection;
        }
        public void OnFill_DataBase(string name, string surname, string fatherly, string number_telephone, int pin, string telephone, int age, string sex, int number_card)
        {
            if(Start_Fill_DataBase != null)
            {
                Start_Fill_DataBase(this, new UserBankArgs(name, surname, fatherly, number_telephone, pin, telephone, age, sex, number_card));
            }
        }
        public void Start_Filling(string name, string surname, string fatherly, string number_telephone, int pin, string telephone, int age, string sex, int number_card)
        {
            OnFill_DataBase(name, surname, fatherly, number_telephone, pin, telephone, age, sex, number_card);
        }
        public void Result_of_INSERT(Result_Of_INSERT result)
        {
            result();
        }
        
    }
}
