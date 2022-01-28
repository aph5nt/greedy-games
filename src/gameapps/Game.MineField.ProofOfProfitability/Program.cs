using System;
using Autofac;
using GreedyGames.Domain;
using GreedyGames.Game.Minefield.Commands;
using GreedyGames.Game.Minefield.Domain;
using GreedyGames.Infrastructure;
using GreedyGames.Shared.Model;
using GreedyGames.Types;

namespace Game.MineField.ProofOfProfitability
{
    internal class Program
    {
        private static IContainer Container { get; set; }

        private static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new ProofModule());
            Container = builder.Build();


            using (var context = new AppDataContext())
            {
                context.Accounts.RemoveRange(context.Accounts);
                //context.Locks.RemoveRange(context.Locks);
                context.SaveChanges();

                context.Accounts.Add(new BankAccount
                {
                    Network = Network.FREE,
                    Address = Guid.NewGuid().ToString(),
                    //Balance = 1000,
                    UserName = GameTypes.Minefield.ToString(),
                    UpdatedAt = DateTime.UtcNow,
                    IsDepositAddress = false
                });

                context.Accounts.Add(new UserAccount
                {
                    Network = Network.FREE,
                    Address = Guid.NewGuid().ToString(),
                    //Balance = 100,
                    UserName = "aph5nt",
                    UpdatedAt = DateTime.UtcNow
                });
                context.SaveChanges();
            }

            // Start game handler
            var settings = new PlayHelper().CreateSettings();
            var starthandler = Container.Resolve<ICommandHandler<Start, UserState>>();
            var userState1 = starthandler.Execute(new Start
            {
                Settings = settings
            });

            var moveHandler = Container.Resolve<ICommandHandler<Move, UserState>>();
            var userState2 = moveHandler.Execute(new Move
            {
                Network = settings.Network,
                UserName = "aph5nt",
                Position = new Position {X = 0, Y = 0},
                GameId = settings.Id
            });

            if (userState2.Status == Status.Alive)
            {
                var takeAwayHandler = Container.Resolve<ICommandHandler<TakeAway, UserState>>();
                var userState3 = takeAwayHandler.Execute(new TakeAway
                {
                    Network = settings.Network,
                    UserName = "aph5nt",
                    GameId = settings.Id
                });
            }
        }
    }
}

/*
 module Domain 

open System
open CryptoGames.Game.Mines.Types
open Akka.Actor
open Akka.FSharp.Actors
open CryptoGames.Game.Mines
open CryptoGames.Types
open CryptoGames

type Play = IActorRef -> GameSettings -> Turns -> UserState
and Turns = int

type Strategy = 
    | FixedTurns of int 
    | DoubleBetOnLossDecreaseTurns of int

let database = CryptoGames.Data.Database.Create()

let Settings() =
    let newId = database.Games.NewId()
    { Id = newId; Type = Mines; Network = FREE; UserName = "aph5nt"; Bet = 1m; Dimension = { Dimension.X = 6; Dimension.Y = 3 }; Seed = new Seed(Guid.NewGuid()) }
   
/// iteration - starts from 1
/// turns - starts from 1
let Play : Play =
    fun actor settings turns ->

        let moveY() = 
             let seed = Guid.NewGuid().GetHashCode()
             let rndMove = new System.Random(seed)
             rndMove.Next(0, settings.Dimension.Y)
            
        let rec run iteration turns =
            match iteration with
            | iteration when iteration = turns -> actor <! new Commands.TakeAway()
            | iteration when iteration <> turns && iteration < settings.Dimension.X -> // if we reach max dimesntion then game takes win, no need to go futher
                let result = 
                    (actor.Ask({ Commands.Move.Position = { X = iteration; Y = moveY() } } ) 
                    |> Async.AwaitTask
                    |> Async.RunSynchronously)
                match result with
                | :? UserState as userState ->
                    if userState.Status = Alive then
                        run (iteration + 1) turns
                | _ -> ()
            | _ -> ()

        actor <! { Commands.Start.GameSettings = settings }
        run 0 turns
        actor.Ask<UserState>(new Commands.Get()) |> Async.AwaitTask |> Async.RunSynchronously
 
 
let Strategy1 actor maxTurns =
    let rec run settings turns =
        if turns  > 0 then
            let result =
                Play 
                <| actor 
                <| settings
                <| turns
            if result.Status = Dead && (turns-1) > 0 then
                run { Settings() with Bet = settings.Bet * 2m } (turns-1)
    
    run 
    <| Settings()   
    <| maxTurns
        
let Strategy2 actor maxTurns baseBet balance maxBalance =
    let rec run settings turns balance =
        if balance > 0m && balance <= maxBalance then
            if turns  > 0 then
                let result =
                    Play 
                    <| actor 
                    <| settings
                    <| turns
                match result.Status with
                | Alive
                | Escaped -> run { Settings() with Bet = baseBet } maxTurns (balance + result.Win)
                | Dead ->  run { Settings() with Bet = settings.Bet * 1m } (turns-1) (balance - result.Loss)   
            else 
               run { Settings() with Bet = baseBet } maxTurns balance
                
    run 
    <| Settings()   
    <| maxTurns
    <| balance

    
        
 
     */