using System;
using Akka.Actor;

namespace WinTail
{
    /// <summary>
    /// Actor responsible for reading FROM the console. 
    /// Also responsible for calling <see cref="ActorSystem.Terminate"/>.
    /// </summary>
    class ConsoleReaderActor : UntypedActor
    {
        public const string ExitCommand = "exit";
        public const string StartCommand = "start";
        private IActorRef _consoleWriterActor;

        public ConsoleReaderActor(IActorRef consoleWriterActor)
        {
            _consoleWriterActor = consoleWriterActor;
        }

        protected override void OnReceive(object message)
        {
            if (message.Equals(StartCommand))
            {
                DoPrintInstructions();
            }
            else if (message is Messages.InputError)
            {
                _consoleWriterActor.Tell(message as Messages.InputError);
            }

            GetAndValidateInput();
        }

        private void GetAndValidateInput()
        {
            var message = Console.ReadLine();
            if(string.IsNullOrWhiteSpace(message))
            {
                Self.Tell(new Messages.InputError("No input received."));
            }
            else if (!string.IsNullOrEmpty(message) && String.Equals(message, ExitCommand, StringComparison.OrdinalIgnoreCase))
            {
                // shut down the system (acquire handle to system via
                // this actors context)
                Context.System.Terminate();
                return;
            }
            else
            {
                bool valid = IsValid(message);
                if(valid)
                {
                    _consoleWriterActor.Tell(new Messages.InputSuccess("Thank you. Message was success."));
                    Self.Tell(new Messages.ContinueProcessing());
                }
                else
                {
                    Self.Tell(new Messages.ValidationError("Invalid: input had odd number of characters."));
                }
            }

            //// send input to the console writer to process and print
            //_consoleWriterActor.Tell(read);

            //// continue reading messages from the console
            //Self.Tell("continue");
        }

        private bool IsValid(string message)
        {
            var even = message.Length % 2 == 0;
            return even;
        }

        private void DoPrintInstructions()
        {
            Console.WriteLine("Write whatever you want into the console.");
            Console.WriteLine("Some entries will pass validation, some wont..\n\n");
            Console.WriteLine("Type 'exit' to quit this app any time. \n");
        }
    }
}