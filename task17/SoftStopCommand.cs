using System;
using System.Threading;

namespace task17
{
    public class SoftStopCommand : ICommand
    {
        private readonly ServerThread _target;

        public SoftStopCommand(ServerThread target)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
        }

        public void Execute()
        {
            if (Thread.CurrentThread != _target.Thread)
            {
                throw new InvalidOperationException("SoftStop может выполняться только в потоке, который должен быть остановлен.");
            }

            _target.SoftStop();
        }
    }
}