using System;
using System.Collections.Generic;
using System.Text;

namespace TProxy
{
    class CommandArgs : EventArgs
    {
        public Client who;
    }

    class Command
    {
        public string name;
        public string permission;
        private Action<CommandArgs> action;
        

        public Command(string name, string permission, Action<CommandArgs> action)
        {
            this.name = name;
            this.permission = permission;
            this.action = action;
        }

        public void Invoke(CommandArgs args) => action.Invoke(args);
    }
    
    class Commands
    {
        public static List<Command> CommandsList = new List<Command>()
        {

        };

        public static void InitCommands()
        {

        }



    }
}
