namespace TKLight
{
    public delegate void RoundChangedEvent(int round);

    public delegate void SwitchParticipantEvent(IParticipant p);

    public delegate void NewParticipantEvent(IParticipant p);
    
    public delegate void ParticipantStatusChangedEvent(IParticipant p);

    public delegate void MatchStartEvent();

    public delegate void MatchEndEvent();

}