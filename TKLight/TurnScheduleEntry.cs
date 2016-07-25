namespace TKLight
{
    public class TurnScheduleEntry
    {
        public TurnScheduleEntry(IParticipant P, int C, int I)
        {
            this.Participant = P;
            this.CurrentRoundFraction = C;
            this.RoundFractionInterval = I;
        }

        public IParticipant Participant;
        public int CurrentRoundFraction;
        public int RoundFractionInterval;
    }
}