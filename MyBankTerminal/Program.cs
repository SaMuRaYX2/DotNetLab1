using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library_for_bank;
using Figgle;

namespace MyBankTerminal
{
   
    internal class Program
    {
        
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            string enter_command = "";
            MessageGreeting();
            do
            {
                Console.Write("admin@My_Bank:~# ");
                enter_command = Console.ReadLine().ToString();
                TerminalCommand command = new TerminalCommand(enter_command);
                command.StartSearchCommand += OutputCommand;
                command.StartSeaching();
                command.StartSearchCommand -= OutputCommand;
            } while (true);
        }
        
        public static void OutputCommand(object sender, CommandEvent e)
        {
            if(e.right_command == true)
            {
                e.del();
            }
            else
            {
                Console.WriteLine("                        ______");
                Console.WriteLine("Команду не знайдено!!! (  :)  )");
                Console.WriteLine("                        |____|");
            }
        }
       
        public static void MessageGreeting()
        {
            Console.WriteLine(FiggleFonts.Doom.Render(" _________ "));
            Console.WriteLine(FiggleFonts.Doom.Render("|_M_y_B_a_n_k__|"));
            
        }
    }
}
