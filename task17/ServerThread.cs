using System;
using System.Collections.Concurrent;
using System.Threading;

namespace task17
{
    public class ServerThread
    {
        private readonly BlockingCollection<ICommand> _queue = new BlockingCollection<ICommand>();
        private readonly IScheduler _scheduler;
        private readonly Thread _thread;
        private Action _behavior;
        private volatile bool _stop = false;
        private bool _addingCompleted = false;

        public Thread Thread => _thread;

        public ServerThread() : this(new RoundRobinScheduler())
        {
        }

        public ServerThread(IScheduler scheduler)
        {
            _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
            _behavior = DefaultBehavior;
            _thread = new Thread(Run);
        }

        public void Start()
        {
            _thread.Start();
        }

        public void Join()
        {
            _thread.Join();
        }

        public void Add(ICommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            try
            {
                _queue.Add(command);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("ServerThread остановлен и больше не принимает команды.", ex);
            }
        }

        public void HardStop()
        {
            _stop = true;
            CompleteAddingOnce();
        }

        public void SoftStop()
        {
            CompleteAddingOnce();

            _behavior = SoftStopBehavior;
        }

        private void CompleteAddingOnce()
        {
            if (_addingCompleted)
                return;

            _addingCompleted = true;
            _queue.CompleteAdding();
        }

        private void DefaultBehavior()
        {
            while (_queue.TryTake(out var incoming))
            {
                _scheduler.Add(incoming);
            }

            if (_scheduler.HasCommand())
            {
                RunStep(_scheduler.Select());
                return;
            }

            try
            {
                var command = _queue.Take();
                RunStep(command);
            }
            catch (InvalidOperationException)
            {
                _stop = true;
            }
        }

        private void SoftStopBehavior()
        {
            if (!_scheduler.HasCommand() && _queue.IsCompleted)
            {
                _stop = true;
                return;
            }

            DefaultBehavior();
        }

        private void RunStep(ICommand command)
        {
            try
            {
                command.Execute();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(command, ex);
                return;
            }

            if (command is ILongCommand longCommand && !longCommand.IsCompleted)
            {
                _scheduler.Add(command);
            }
        }

        private void Run()
        {
            while (!_stop)
            {
                _behavior();
            }
        }
    }
}
