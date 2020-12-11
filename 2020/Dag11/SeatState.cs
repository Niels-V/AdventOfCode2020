using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Dag11
{
    public class SeatState
    {
        private readonly SeatStatus[,] _initialState;
        private readonly SeatStatus[,] _nextState;
        private readonly int _m;
        private readonly int _n;
        private bool _stateChanged = false;
        private bool _newStateCalculated = false;
        private int _totalSeatsOccupied = 0;

        public SeatState(SeatStatus[,] initialState)
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
            var neigbouringOccupiedSeats = 0;
            for (int i = posx - 1; i <= posx + 1; i++)
            {
                if (i < 0 || i >= _n) { continue; } //skip around the edges
                for (int j = posy - 1; j <= posy + 1; j++)
                {
                    if (j < 0 || j >= _m) { continue; } //skip around the edges
                    if (i == posx && j == posy) { continue; } //Skip the center seat
                    if (_initialState[i, j] == SeatStatus.Occupied)
                    {
                        neigbouringOccupiedSeats++;
                    }
                }
            }
            if (_initialState[posx, posy] == SeatStatus.Occupied && neigbouringOccupiedSeats >= 4)
            {
                _stateChanged = true;
                return SeatStatus.Empty;
            }
            if (_initialState[posx, posy] == SeatStatus.Empty && neigbouringOccupiedSeats == 0)
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
    }
}
