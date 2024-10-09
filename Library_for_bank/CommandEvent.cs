using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentTextTable;

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
                    else if(_command == "--help" || _command == "-h")
                    {
                        del = () =>
                        {
                            List<CommandHelp> commands_list = new List<CommandHelp>();

                            for (int j = 0; j < terminalCommand.list_of_command.Count; j++)
                            {
                                CommandHelp help = new CommandHelp();
                                help.command = (terminalCommand.list_of_command[j]);
                                help.description = (terminalCommand.list_of_description[j]);
                                commands_list.Add(help);
                            }
                            var table = Build.TextTable<CommandHelp>(builder =>
                            {
                                builder
                                    .Borders.Top
                                        .LeftStyleAs("+")
                                        .IntersectionStyleAs("-")
                                        .RightStyleAs("+")
                                    .Borders.HeaderHorizontal
                                        .LineStyleAs("=")
                                    .Borders.InsideHorizontal
                                        .LineStyleAs("=")
                                    .Borders.Bottom
                                        .LeftStyleAs("+")
                                        .IntersectionStyleAs("-")
                                        .RightStyleAs("+");
                            });
                            table.WriteLine(commands_list);
                        };
                    }
                    else if(_command == "show all banks" || _command == "show bank")
                    {
                        del = terminalCommand.OutputAllBank;
                    }
                    else if(_command == "add bank")
                    {
                        del = terminalCommand.AddBank;
                    }
                    break;
                }
            }
            return test;
        }
    }
}
