using System.Collections.Generic;
using System.Linq;

namespace TKLight
{
    public class ParticipantUtils
    {
        public static int LCN(params IParticipant[] P)
        {
            int G = 1;

            var L = P.ToList();
            L.Sort((a, b) => b.ActionsPerRound().Numerator.CompareTo(a.ActionsPerRound().Numerator));
            foreach (var x in L)
            {
                if (G % x.ActionsPerRound().Numerator != 0 || G == 1)
                    G *= x.ActionsPerRound().Numerator;
            }
            return G;
        }

        public static List<TurnScheduleEntry> PrepareSchedule(out int TicksPerRound, List<IParticipant> p)
        {

            foreach (var x in p)
                x.ActionsPerRound().Simplify();

            int lcn = ParticipantUtils.LCN(p.ToArray());

            foreach (var x in p)
            {
                var F = x.ActionsPerRound();
                int n = F.Numerator;
                int d = F.Denominator;

                int r = lcn / n;
                d *= r;
                F.Set(lcn, d);
            }

            p.Sort((a, b) => Compare(a, b));

            List<TurnScheduleEntry> SCH = new List<TurnScheduleEntry>();

            TicksPerRound = p[0].ActionsPerRound().Numerator * 2;

            foreach (var f in p)
            {
                int d = f.ActionsPerRound().Denominator;
                SCH.Add(new TurnScheduleEntry(f, -d, d * 2));
            }
            foreach (var x in p)
                x.ActionsPerRound().Simplify();

            return SCH;
        }

        private static int Compare(IParticipant a, IParticipant b)
        {
            int da = a.ActionsPerRound().Denominator;
            int db = b.ActionsPerRound().Denominator;

            if (da == db)
            {
                int pa = a.StartingPosition();
                int pb = b.StartingPosition();
                // try to resolve conflicts, but if the numbers are hard coded, 
                // avoid an infinite loop.

                // try a few times.
                int maxtries = 10;
                while(pa == pb && maxtries>0)
                {
                    pa = a.StartingPosition();
                    pb = b.StartingPosition();
                    maxtries -= 1;
                }

                
                return pa.CompareTo(pb);
            }
            else return da.CompareTo(db);
        }
    }
}