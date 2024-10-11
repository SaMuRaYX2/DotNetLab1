using MySql.Data.MySqlClient;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Library_for_bank
{
    public class CodeEvent : EventArgs
    {
        public delegate void del_to_send_code();
        public string request { get; private set; }
        public string choosen_service { get; private set; }
        public int id_user { get; private set; }
        public del_to_send_code del;
        public User user { get; private set; }
        public string email_recipient { get; private set; }
        
        public CodeEvent(string text, string service, int id_user)
        {
            user = new User();
            request = text;
            choosen_service = service;
            this.id_user = id_user;
            if (choosen_service == "EMAIL")
            {
                del = Send_Email;
            }
            Fill_class_user();
        }
        public void Fill_class_user()
        {
            DB db = new DB();
            db.openConnection();
            string query = "select * from users where id = @s1";
            using (MySqlCommand cmd = new MySqlCommand(query, db.getConnection()))
            {
                cmd.Parameters.Add("@s1", MySqlDbType.Int32).Value = id_user;
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
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
                    }
                }
            }
            db.closeConnection();
        }
        public void Send_Email()
        {
            //string modifiedNumber = user.number_telephone.Substring(3);
            //List<string> emailGateway = new List<string>();
            //emailGateway.Add("@gmail.com");
            //emailGateway.Add("@outlook.com");
            //emailGateway.Add("@yahoo.com");
            List<string> toEmail = new List<string>();
            int i = 0;

            toEmail.Add(user.email);
            email_recipient = toEmail[i];
            var fromAddress = new MailAddress("mybankcorporation100@gmail.com", "MyBank");
            var toAddress = new MailAddress(toEmail[i]);
            const string fromPassword = "czwj heju tagh xgbf";
            const string subject = "Ваш код підтвердження від MyBank";
            string body = request;
            var smtp = new SmtpClient()
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                From = new MailAddress(fromAddress.Address),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            })
            {
                message.To.Add(toEmail[i]);
                smtp.Send(message);
            }
            i++;


        }
        
        
    }
    
}
