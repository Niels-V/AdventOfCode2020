using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Dag17
{
    public class CubeState
    {
        private readonly CubeStatus[,,] _initialState;
        private readonly CubeStatus[,,] _nextState;
        private readonly int _m;
        private readonly int _n;
        private readonly int _o;
        private bool _stateChanged = false;
        private bool _newStateCalculated = false;
        private int _totalCubesOccupied = 0;

        public CubeState(CubeStatus[,,] initialState)
        {
            _initialState = initialState;
            _nextState = (CubeStatus[,,])initialState.Clone();
            _m = initialState.GetLength(2);
            _n = initialState.GetLength(1);
            _o = initialState.GetLength(0);
        }

        public CubeStatus[,,] CalculateNewState()
        {
            _totalCubesOccupied = 0;
            for (int i = 0; i < _n; i++)
            {
                for (int j = 0; j < _m; j++)
                {
                    for (int k = 0; k < _o; k++)
                    {
                        _nextState[i, j, k] = CalculateNewStatus(i, j, k);
                    }
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
        internal int NextStateNumberCubesOccupied
        {
            get
            {
                if (!_newStateCalculated) throw new InvalidOperationException("Next state is not calculated yet.");
                return _totalCubesOccupied;
            }
        }

        private CubeStatus CalculateNewStatus(int posx, int posy, int posz)
        {
            var neigbouringActiveCubes = 0;
            for (int i = posx - 1; i <= posx + 1; i++)
            {
                if (i < 0 || i >= _n) { continue; } //skip around the edges
                for (int j = posy - 1; j <= posy + 1; j++)
                {
                    if (j < 0 || j >= _m) { continue; } //skip around the edges
                    for (int k = posz - 1; k <= posz + 1; k++)
                    {
                        if (k < 0 || k >= _o) { continue; } //skip around the edges

                        if (i == posx && j == posy && k == posz) { continue; } //Skip the center cube
                        if (_initialState[i, j, k] == CubeStatus.Active)
                        {
                            neigbouringActiveCubes++;
                        }
                    }
                }
            }
            if (_initialState[posx, posy, posz] == CubeStatus.Active && (neigbouringActiveCubes < 2 || neigbouringActiveCubes > 3))
            {
                _stateChanged = true;
                return CubeStatus.Inactive;
            }
            if (_initialState[posx, posy, posz] == CubeStatus.Inactive && neigbouringActiveCubes == 3)
            {
                _stateChanged = true;
                return CubeStatus.Active;
            }
            return _initialState[posx, posy,posz];
        }
    }
}
