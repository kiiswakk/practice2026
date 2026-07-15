using System;
using Xunit;
using task17;

namespace ServerThreadTests
{
    public class ServerThreadTests : IDisposable
    {
        private class Command : ICommand
        {
            public bool Executed { get; private set; }
            public Action? OnExecute { get; set; }
            public void Execute()
            {
                Executed = true;
                OnExecute?.Invoke();
            }
        }

        public ServerThreadTests()
        {
            ExceptionHandler.Clear();
        }

        public void Dispose()
        {
            ExceptionHandler.Clear();
        }

        [Fact]
        public void SoftStop_ShouldDrainAllCommands_BeforeStopping()
        {
            var server = new ServerThread();
            var loadConfig = new Command();
            var openConnection = new Command();
            var softStop = new SoftStopCommand(server);
            var flushBuffer = new Command();
            var closeConnection = new Command();

            server.Add(loadConfig);
            server.Add(openConnection);
            server.Add(softStop);
            server.Add(flushBuffer);
            server.Add(closeConnection);
            server.Start();
            server.Join();

            Assert.True(loadConfig.Executed);
            Assert.True(openConnection.Executed);
            Assert.True(flushBuffer.Executed);
            Assert.True(closeConnection.Executed);
        }

        [Fact]
        public void HardStop_ShouldStopImmediately_AndIgnoreRemainingCommands()
        {
            var server = new ServerThread();
            var acceptRequest = new Command();
            var validateRequest = new Command();
            var hardStop = new HardStopCommand(server);
            var processRequest = new Command();
            var sendResponse = new Command();

            server.Add(acceptRequest);
            server.Add(validateRequest);
            server.Add(hardStop);
            server.Add(processRequest);
            server.Add(sendResponse);
            server.Start();
            server.Join();

            Assert.True(acceptRequest.Executed);
            Assert.True(validateRequest.Executed);
            Assert.False(processRequest.Executed);
            Assert.False(sendResponse.Executed);
        }

        [Fact]
        public void StopCommands_ExecutedManually_ShouldThrowInvalidOperationException()
        {
            var server = new ServerThread();
            var softStop = new SoftStopCommand(server);
            var hardStop = new HardStopCommand(server);

            Assert.Throws<InvalidOperationException>(() => softStop.Execute());
            Assert.Throws<InvalidOperationException>(() => hardStop.Execute());
        }

        [Fact]
        public void HardStop_ForAnotherServer_ShouldThrow()
        {
            var targetServer = new ServerThread();
            var callerServer = new ServerThread();
            var wrongHardStop = new HardStopCommand(targetServer);
            var stopCallerServer = new HardStopCommand(callerServer);
            var markerCommand = new Command();

            callerServer.Add(wrongHardStop);
            callerServer.Add(markerCommand);
            callerServer.Add(stopCallerServer);
            callerServer.Start();
            callerServer.Join();

            Assert.True(markerCommand.Executed);
            Assert.Same(wrongHardStop, ExceptionHandler.LastCommand);
            Assert.IsType<InvalidOperationException>(ExceptionHandler.LastException);

            targetServer.Add(new HardStopCommand(targetServer));
            targetServer.Start();
            targetServer.Join();
        }

        [Fact]
        public void SoftStop_TargetingAnotherRealServerThread_ShouldThrow_AndOwnServerKeepsWorking()
        {
            var targetServer = new ServerThread();
            var callerServer = new ServerThread();
            var wrongSoftStop = new SoftStopCommand(targetServer);
            var markerCommand = new Command();

            callerServer.Add(wrongSoftStop);
            callerServer.Add(markerCommand);
            callerServer.Add(new HardStopCommand(callerServer));
            callerServer.Start();
            callerServer.Join();

            Assert.True(markerCommand.Executed);
            Assert.Same(wrongSoftStop, ExceptionHandler.LastCommand);
            Assert.IsType<InvalidOperationException>(ExceptionHandler.LastException);

            targetServer.Add(new HardStopCommand(targetServer));
            targetServer.Start();
            targetServer.Join();
        }

        [Fact]
        public void Add_AfterSoftStop_ShouldThrow()
        {
            var server = new ServerThread();
            server.Add(new SoftStopCommand(server));
            server.Start();
            server.Join();

            Assert.Throws<InvalidOperationException>(() => server.Add(new Command()));
        }

        [Fact]
        public void Add_AfterHardStop_ShouldThrow()
        {
            var server = new ServerThread();
            server.Add(new HardStopCommand(server));
            server.Start();
            server.Join();

            Assert.Throws<InvalidOperationException>(() => server.Add(new Command()));
        }

        [Fact]
        public void FaultyCommand_ExceptionIsCaptured_AndQueueContinuesProcessing()
        {
            var server = new ServerThread();
            var brokenCommand = new Command { OnExecute = () => throw new InvalidOperationException("сбой обработки") };
            var recoveryCommand = new Command();

            server.Add(brokenCommand);
            server.Add(recoveryCommand);
            server.Add(new SoftStopCommand(server));
            server.Start();
            server.Join();

            Assert.True(recoveryCommand.Executed);
            Assert.Same(brokenCommand, ExceptionHandler.LastCommand);
            Assert.IsType<InvalidOperationException>(ExceptionHandler.LastException);
        }
    }
}