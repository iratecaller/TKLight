namespace TKLight
{
    public interface IParticipant
    {
        PositiveFraction ActionsPerRound();

        int TeamID();

        int ID();

        bool Active();

        int StartingPosition();

        bool TakeTurn();
    }
}