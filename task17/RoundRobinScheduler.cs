using System;
using System.Collections.Generic;

namespace task17
{
    public class RoundRobinScheduler : IScheduler
    {
        private readonly Queue<ICommand> _commands = new Queue<ICommand>();
        private readonly object _lock = new object();

        public bool HasCommand()
        {
            lock (_lock)
            {
                return _commands.Count > 0;
            }
        }

        public ICommand Select()
        {
            lock (_lock)
            {
                if (_commands.Count == 0)
                    throw new InvalidOperationException("Планировщик пуст.");

                return _commands.Dequeue();
            }
        }

        public void Add(ICommand cmd)
        {
            if (cmd == null)
                throw new ArgumentNullException(nameof(cmd));

            lock (_lock)
            {
                _commands.Enqueue(cmd);
            }
        }
    }
}