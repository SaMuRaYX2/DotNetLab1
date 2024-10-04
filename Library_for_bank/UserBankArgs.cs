using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_for_bank
{
    public class UserBankArgs : EventArgs
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string Surname { get; private set; }
        public string Fatherly { get; private set; }
        public string Number_telephone { get; private set; }
        public int Pin { get; private set; }
        public string Telephone { get; private set; }
        public int Age { get; private set; }
        public string Sex { get; private set; }
        public string Number_card { get; private set; }
        public decimal balance { get; private set; }
        public UserBankArgs(string name,string surname, string fatherly,string number_telephone,int pin,string telephone,int age, string sex, string number_card, decimal balance)
        {
            Name = name;
            Surname = surname;
            Fatherly = fatherly;
            Number_telephone = number_telephone;
            Pin = pin;
            Telephone = telephone;
            Age = age;
            Sex = sex;
            Number_card = number_card;
            this.balance = balance;
        }

    }
}
