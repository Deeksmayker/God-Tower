using System;

namespace Source.Logic.Utils.DebugConsoleCommands
{
    public class DebugCommand
    {
        private string _command;
        private string _description;
        private Action _action;

        public DebugCommand(string command, string description, Action action)
        {
            _command = command;
            _description = description;
            _action = action;
        }

        public void Execute()
        {
            _action.Invoke();
        }

        public string GetCommandName()
        {
            return _command;
        }

        public string GetDescription()
        {
            return _description;
        }
    }
}
