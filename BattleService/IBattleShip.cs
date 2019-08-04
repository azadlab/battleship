using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BattleShipControl
{
    interface IBattleShip
    {
        void StartNewGame(string pname);
        void EndGame();
        FireResult Fire(int x,int y);

    }
    public enum FireResult: byte
        {
            CarrierHit=1,
            DestroyerHit=2,
            BattleshipHit=3,
            SubmarineHit=4,
            BoatHit=5,
            Miss
        }
}
