using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_for_bank
{
    public class CommandEvent : EventArgs 
    {
        public string _command { get; private set; }
        public bool right_command { get; private set; }
        public delegate void ExecuteCommand();
        public ExecuteCommand del;
        public CommandEvent(string command)
        {
            _command = command;
            right_command = SearchCommand();
        }
        public bool SearchCommand()
        {
            bool test = false;
            TerminalCommand terminalCommand = new TerminalCommand(_command);
            
            for(int i = 0; i < terminalCommand.list_of_command.Count; i++)
            {
                if (_command == terminalCommand.list_of_command[i])
                {
                    test = true;
                    if (_command == "show all users" || _command == "show user")
                    {
                        del = terminalCommand.OutputAllUsers;
                    }
                    else if (_command == "add user")
                    {
                        del = terminalCommand.InputUser;
                    }
                    else if(_command == "exit")
                    {
                        del = () =>
                        {
                            Environment.Exit(0);
                        };
                    }
                    break;
                }
            }
            return test;
        }
    }
}
