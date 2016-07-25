using System;
using System.Threading;
using TKLight;

namespace Demo
{
    public class DemoUtils
    {
        public static Random R = new Random();
    }

    public class P : IParticipant
    {
        private PositiveFraction _frac;
        private int teamid;

        public P(string fraction, int pid, int team_id)
        {
            _frac = PositiveFraction.Parse(fraction);
            teamid = team_id;
            _id = pid;
            alive = true;
        }

        public PositiveFraction ActionsPerRound()
        {
            return _frac;
        }

        public int TeamID()
        {
            return teamid;
        }

        private bool alive;

        public bool Active()
        {
            return alive;
        }

        public int StartingPosition()
        {
            // simulate a 10 sided dice
            return DemoUtils.R.Next(1,11);
        }

        public void Die()
        {
            alive = false;
        }

        public int delay = 0;

     
        public bool TakeTurn()
        {
            if (delay == 2000)
            {
                Console.WriteLine("ID: " + ID() +  " TID: " + TeamID() + " @ " + ActionsPerRound());
            }
            delay -= 100;
            Thread.Sleep(100);
            Console.Write(".");

            if (DemoUtils.R.Next(20) == 0)
                alive = false;
       

            if (delay > 0)

                return true;

            Console.WriteLine(" done. ");
            return false;
        }

        int _id;
        public int ID()
        {
            return _id;
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            string[] sf = new string[] {
                "1/1", "2/1", "3/2","1/2"
            };

            int pid = 1;

            TimeKeeper T = new TimeKeeper();

            foreach (var x in sf)
            {
                T.AddParticipant(new P(x,pid, pid++));
            }

            T.OnRoundChanged += T_OnRoundChanged;
            T.OnSwitchParticipant += T_OnSwitchParticipant;
            T.OnNewParticipant += T_OnNewParticipant;
            T.OnParticipantStatusChanged += T_OnParticipantStatusChanged;
            T.OnMatchStart += T_OnMatchStart;
            T.OnMatchEnd += T_OnMatchEnd;

            T.BeginMatch();

            while (T.Update() && !Stop)
            {
                //  Console.Write(".");
            }

            Console.WriteLine();
            Console.WriteLine("-----------------");
            Console.ReadLine();
        }

        static void T_OnMatchEnd()
        {
            Console.WriteLine("Match ended.");
        }

        static void T_OnMatchStart()
        {
            Console.WriteLine("Match started.");
        }

        static void T_OnParticipantStatusChanged(IParticipant p)
        {
            Console.WriteLine("Participant : " + p.ID() + " Active: " + p.Active());
        }

        private static void T_OnNewParticipant(IParticipant p)
        {
            Console.WriteLine("New Particpant:  " + p.ID());
        }

        private static void T_OnSwitchParticipant(IParticipant p)
        {
            Console.WriteLine("P " + p.ID() + " now taking turn.");
            (p as P).delay = 2000;
        }

        private static bool Stop = false;

        private static void T_OnRoundChanged(int round)
        {
            Console.WriteLine("\r\nRound: " + round);
            if (round >= 1000)
            {
                Console.WriteLine("Time Up!");
                Stop = true;
            }
        }
    }
}