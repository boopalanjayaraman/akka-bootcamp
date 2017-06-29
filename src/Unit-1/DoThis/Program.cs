using System;
﻿using Akka.Actor;

namespace WinTail
{
    #region Program
    class Program
    {
        public static ActorSystem MyActorSystem;

        static void Main(string[] args)
        {
            // initialize MyActorSystem
            MyActorSystem = ActorSystem.Create("MyActorSystem");

            PrintInstructions();

            // time to make your actors!
            Props consoleWriterActorProps = Props.Create(() => new ConsoleWriterActor());
            IActorRef consoleWriterActor = MyActorSystem.ActorOf(consoleWriterActorProps, "consoleWriterActor");

            Props tailCoordinatorActorProps = Props.Create(() => new TailCoordinatorActor());
            IActorRef tailCoordinatorActor = MyActorSystem.ActorOf(tailCoordinatorActorProps, "tailCoordinatorActor");

            Props validationActorProps = Props.Create(() => new FileValidatorActor(consoleWriterActor, tailCoordinatorActor));
            IActorRef validationActor = MyActorSystem.ActorOf(validationActorProps, "FileValidatorActor");

            Props consoleReaderActorProps = Props.Create(() => new ConsoleReaderActor(validationActor));
            IActorRef consoleReaderActor = MyActorSystem.ActorOf(consoleReaderActorProps, "consoleReaderActor");
             

            // tell console reader to begin
            consoleReaderActor.Tell(ConsoleReaderActor.StartCommand);

            // blocks the main thread from exiting until the actor system is shut down
            MyActorSystem.WhenTerminated.Wait();
        }

        private static void PrintInstructions()
        {
            Console.WriteLine("Please provide the URI of a log file on disk.\n");
        }
    }
    #endregion
}
