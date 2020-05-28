using System;
using System.Collections.Generic;
using System.Text;

namespace TProxy
{
    class CommandArgs : EventArgs
    {
        public Client Who;
    }

    class Command
    {
        public string Name;
        public string Permission;
        private Action<CommandArgs> _action;
        

        public Command(string name, string permission, Action<CommandArgs> action)
        {
            this.Name = name;
            this.Permission = permission;
            this._action = action;
        }

        public void Invoke(CommandArgs args) => _action.Invoke(args);
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
