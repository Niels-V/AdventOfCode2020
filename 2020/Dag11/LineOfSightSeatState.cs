using System;

namespace Dag11
{
    public class LineOfSightSeatState
    {
        private readonly SeatStatus[,] _initialState;
        private readonly SeatStatus[,] _nextState;
        private readonly int _m;
        private readonly int _n;
        private bool _stateChanged = false;
        private bool _newStateCalculated = false;
        private int _totalSeatsOccupied = 0;

        public LineOfSightSeatState(SeatStatus[,] initialState)
        {
            _initialState = initialState;
            _nextState = (SeatStatus[,])initialState.Clone();
            _m = initialState.GetLength(1);
            _n = initialState.GetLength(0);
        }

        public SeatStatus[,] CalculateNewState()
        {
            _totalSeatsOccupied = 0;
            for (int i = 0; i < _n; i++)
            {
                for (int j = 0; j < _m; j++)
                {
                    _nextState[i, j] = CalculateNewStatus(i, j);
                }
            }
            _newStateCalculated = true;
            return _nextState;
        }

        internal bool NextStateChanged
        {
            get
            {
                if (!_newStateCalculated) throw new InvalidOperationException("Next state is not calculated yet.");
                return _stateChanged;
            }
        }
        internal int NextStateSeatOccupied
        {
            get
            {
                if (!_newStateCalculated) throw new InvalidOperationException("Next state is not calculated yet.");
                return _totalSeatsOccupied;
            }
        }

        private SeatStatus CalculateNewStatus(int posx, int posy)
        {
            if (_initialState[posx, posy] == SeatStatus.Floor)
            {
                return SeatStatus.Floor;
            }
            var lineOfSightOccupiedSeats = 0;


            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) { continue; } //Skip with no movement
                    bool seatOccupied = IsOccupiedInDirection(posx, posy, dx, dy);
                    if (seatOccupied)
                    {
                        lineOfSightOccupiedSeats++;
                    }
                }
            }
            if (_initialState[posx, posy] == SeatStatus.Occupied && lineOfSightOccupiedSeats >= 5)
            {
                _stateChanged = true;
                return SeatStatus.Empty;
            }
            if (_initialState[posx, posy] == SeatStatus.Empty && lineOfSightOccupiedSeats == 0)
            {
                _totalSeatsOccupied++;
                _stateChanged = true;
                return SeatStatus.Occupied;
            }
            if (_initialState[posx, posy] == SeatStatus.Occupied)
            {
                _totalSeatsOccupied++;
            }
            return _initialState[posx, posy];
        }

        private bool IsOccupiedInDirection(int posx, int posy, int dx, int dy)
        {
            if (dx == 0 && dy == 0) { throw new InvalidOperationException("No direction specified"); }
            bool seatOccupied = false;
            int i = posx + dx;
            int j = posy + dy;
            while (i >= 0 && i < _n && j >= 0 && j < _m)
            {
                var seatStatus = _initialState[i, j];
                if (seatStatus == SeatStatus.Empty) { return false; }
                if (seatStatus == SeatStatus.Occupied) { return true; }
                i += dx;
                j += dy;
            }
            return seatOccupied;
        }
    }
}
