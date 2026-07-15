using System;
using System.Collections.Concurrent;
using System.Threading;

namespace task17
{
    public class ServerThread
    {
        private readonly BlockingCollection<ICommand> _queue = new BlockingCollection<ICommand>();
        private readonly Thread _thread;
        private Action _behavior;
        private volatile bool _stop = false;
        private bool _addingCompleted = false;

        public Thread Thread => _thread;

        public ServerThread()
        {
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

        internal void HardStop()
        {
            _stop = true;
            CompleteAddingOnce();
        }

        internal void SoftStop()
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
            try
            {
                ICommand command = _queue.Take();
                try
                {
                    command.Execute();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Handle(command, ex);
                }
            }
            catch (InvalidOperationException)
            {
                _stop = true;
            }
        }
        private void SoftStopBehavior()
        {
            if (_queue.IsCompleted)
            {
                _stop = true;
                return;
            }

            DefaultBehavior();
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