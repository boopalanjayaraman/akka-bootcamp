using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTail
{
    public class ValidationActor : UntypedActor
    {
        private readonly IActorRef _consoleWriterActor;

        public ValidationActor(IActorRef consoleWriterActor)
        {
            _consoleWriterActor = consoleWriterActor;
        }

        protected override void OnReceive(object message)
        {
            var msg = message as string;

            if (string.IsNullOrWhiteSpace(msg))
            {
                _consoleWriterActor.Tell(new Messages.InputError("No input received."));
            }
            else
            {
                bool valid = IsValid(msg);
                if (valid)
                {
                    _consoleWriterActor.Tell(new Messages.InputSuccess("Thank you. Message was success."));
                }
                else
                {
                    _consoleWriterActor.Tell(new Messages.ValidationError("Invalid: input had odd number of characters."));
                }
            }

            Sender.Tell(new Messages.ContinueProcessing());
        }

        private bool IsValid(string message)
        {
            var even = message.Length % 2 == 0;
            return even;
        }
    }
}
