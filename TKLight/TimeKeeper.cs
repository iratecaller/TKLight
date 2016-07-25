using System.Collections.Generic;
using System.Linq;

namespace TKLight
{
    public class TimeKeeper
    {
        #region Private State / Variables
        private bool _informed_participant_changed;
        private int _next_to_process;
        private List<IParticipant> _participants;
        private List<IParticipant> _participants_to_add;
        private int _round;
        private List<TurnScheduleEntry> _schedule;
        private int _ticks_per_round;
        private TurnScheduleEntry[] _toprocess;
        private int curticks;
        private List<int> _active_participants;
        private List<int> _inactive_participants;
        #endregion


        public TimeKeeper()
        {
            _participants = new List<IParticipant>();
            _participants_to_add = new List<IParticipant>();
            _active_participants = new List<int>();
            _inactive_participants = new List<int>();
        }
        public event NewParticipantEvent OnNewParticipant;

        public event RoundChangedEvent OnRoundChanged;

        public event SwitchParticipantEvent OnSwitchParticipant;

        public event ParticipantStatusChangedEvent OnParticipantStatusChanged;

        public event MatchStartEvent OnMatchStart;

        public event MatchEndEvent OnMatchEnd;


        public IEnumerable<IParticipant> Participants
        {
            get
            {
                return _participants;
            }
        }

        public IEnumerable<TurnScheduleEntry> Schedule
        {
            get
            {
                return _schedule;
            }
        }
        public void AddParticipant(IParticipant p)
        {
            if (!_participants_to_add.Contains(p))
                _participants_to_add.Add(p);
        }

        public void BeginMatch()
        {
            _ticks_per_round = 0;
            curticks = 0;
            _round = 0;
            _next_to_process = 0;
            _informed_participant_changed = false;
            _active_participants.Clear();
            _inactive_participants.Clear();
            _participants.Clear();
            ReconfigureForNewParticipantConfiguration();
            _toprocess = null;

        }

        public bool IsMatchOver()
        {
            // must be more than one team with active members.

            bool ret = true;
            int countOfActiveTeams = 0;
            int lastActiveTeams = -1;

            if (_participants != null)
            {
                foreach (var x in _participants)
                {
                    if (x.Active())
                    {
                        int teamid = x.TeamID();

                        if (lastActiveTeams != teamid)
                        {
                            lastActiveTeams = teamid;
                            countOfActiveTeams += 1;
                        }
                        if (countOfActiveTeams > 1)
                        {
                            ret = false;
                            break;
                        }
                    }
                }
            }
            return ret;
        }

        public bool Update()
        {
            if(curticks==0)
            {
                if (OnMatchStart != null)
                    OnMatchStart();
            }
            if (IsMatchOver())
            {
                if (OnMatchEnd != null)
                    OnMatchEnd();
                return false;
            }
            if (_toprocess != null && _toprocess.Length > 0)
            {
                if (_next_to_process >= _toprocess.Length)
                {
                    _toprocess = null;
                }
                else
                {
                    if (!_informed_participant_changed)
                    {
                        _informed_participant_changed = true;
                        if (OnSwitchParticipant != null)
                            OnSwitchParticipant(_toprocess[_next_to_process].Participant);
                    }
                    if (!_toprocess[_next_to_process].Participant.TakeTurn())
                    {
                        _next_to_process += 1;
                        _informed_participant_changed = false;
                    }
                }
            }
            else
            {
                DetectStatusChanges();
                _toprocess = GetSchedulesToProcess().ToArray();
                _next_to_process = 0;
                AdvanceAllSchedules();
                _informed_participant_changed = false;
            }

            return true;
        }

        private void DetectStatusChanges()
        {
            if (OnParticipantStatusChanged != null)
            {
                foreach (var x in _schedule)
                {
                    bool active = x.Participant.Active();
                    int id = x.Participant.ID();

                    if (active && !_active_participants.Contains(id))
                    {
                        _active_participants.Add(id);
                        if (_inactive_participants.Contains(id))
                            _inactive_participants.Remove(id);
                        OnParticipantStatusChanged(x.Participant);
                    }
                    else if (!active && !_inactive_participants.Contains(id))
                    {
                        _inactive_participants.Add(id);
                        if (_active_participants.Contains(id))
                            _active_participants.Remove(id);

                        OnParticipantStatusChanged(x.Participant);
                    }
                }
            }
        }

        private void AdvanceAllSchedules()
        {
            foreach (var x in _schedule)
            {
                x.CurrentRoundFraction += 1;
                if (x.CurrentRoundFraction >= x.RoundFractionInterval)
                    x.CurrentRoundFraction = 0;
            }
            if (curticks % _ticks_per_round == 0)
            {
                _round += 1;
                ReconfigureForNewParticipantConfiguration();
                if (OnRoundChanged != null)
                    OnRoundChanged(_round);
            }
            curticks += 1;
        }
        private IEnumerable<TurnScheduleEntry> GetSchedulesToProcess()
        {
            foreach (var x in _schedule)
            {
                if (x.CurrentRoundFraction == 0 && x.Participant.Active())
                {
                    yield return x;
                }
            }
            yield break;
        }
        private void RecalculateSchedule()
        {
            _schedule = ParticipantUtils.PrepareSchedule(out _ticks_per_round, _participants);
        }
        private void ReconfigureForNewParticipantConfiguration()
        {
            if (_participants_to_add.Count() > 0)
            {

                foreach (var p in _participants_to_add)
                {
                    if (!_participants.Contains(p))
                    {
                        _participants.Add(p);
                        if (OnNewParticipant != null)
                            OnNewParticipant(p);
                    }
                }
                _participants_to_add.Clear();

                RecalculateSchedule();

            }
        }
    }
}