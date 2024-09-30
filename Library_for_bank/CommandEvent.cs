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
            del = terminalCommand.OutputAllUsers;
            
            for(int i = 0; i < terminalCommand.list_of_command.Count; i++)
            {
                if (_command == terminalCommand.list_of_command[i])
                {
                    test = true;
                    break;
                }
            }
            return test;
        }
    }
}
