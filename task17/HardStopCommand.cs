using System;
using System.Threading;

namespace task17
{
    public class HardStopCommand : ICommand
    {
        private readonly ServerThread _target;

        public HardStopCommand(ServerThread target)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
        }

        public void Execute()
        {
            if (Thread.CurrentThread != _target.Thread)
            {
                throw new InvalidOperationException(
                    "HardStop может выполняться только в потоке, который должен быть остановлен.");
            }

            _target.HardStop();
        }
    }
}