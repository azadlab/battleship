using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BattleBoat;
using System.Runtime;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Windows.Forms;

namespace BattleShipControl
{
    
    public class BattleShipService :MarshalByRefObject,IBattleShip
    {
        public BattleGridManager battlegrid;
        private string m_player=null;
        private int m_scores = 0;
        private int m_attempts = 0;
        private string m_turn;
        public bool playerconnected=false;
        private int m_lasthitX=-1;
        private int m_lasthitY=-1;
        private FireResult m_lastresult = FireResult.Miss;
        private bool m_isnew = false;
        private bool m_isended = false;
        private int[,] shiplocs = null;

        public void StartNewGame(string s)
        {
            battlegrid = new BattleGridManager(ShipLocs);
            Scores = 0;
            Attempts = 0;
            LastHitX = -1;
            LastHitY = -1;
        }
        public void YourTurn()
        {
        }
        public FireResult Fire(int x,int y)
        {
            FireResult fr=FireResult.Miss;
            
            if (battlegrid.fireMissle(x, y))
            {
                
                switch (battlegrid.getBattleShip(battlegrid.getShip(x, y)-1).getType())
                {
                    case 1:
                        fr = FireResult.CarrierHit;
                    break;
                    case 2:
                    fr = FireResult.DestroyerHit;
                    break;
                    case 3:
                    fr = FireResult.BattleshipHit;
                    break;
                    case 4:
                    fr = FireResult.SubmarineHit;
                    break;
                    case 5:
                    fr = FireResult.BoatHit;
                    break;
                }
                

            }
            
            LastHitX = x;
            LastHitY = y;
            LastResult = fr;
            return fr;
            
        }
        public void EndGame()
        {
            IsEndeded = true;
        }

        public void ConnectPlayer()
        {
            playerconnected = true;
            
        }
        public void CreateGrid(int[,] shipslocs)
        {
            ShipLocs = shipslocs;
            battlegrid = new BattleGridManager(ShipLocs);
        }

        public BattleGridManager GetBattleGrid()
        {
            return battlegrid;
        }
        
        public string GetMachine()
        {
            return Environment.CurrentDirectory;
        }
        public BattleShip GetShip(int n)
        {
            return battlegrid.getBattleShip(n);
        }
        public string Player
        {
            get { return m_player; }
            set { m_player = value; }
        }
        public int Scores
        {
            get { return m_scores; }
            set { m_scores = value; }
        }
        public int Attempts
        {
            get { return m_attempts; }
            set { m_attempts = value; }
        }
        public string NextTurn
        {
            get { return m_turn; }
            set { m_turn = value; }
        }

        public int LastHitX
        {
            get { return m_lasthitX; }
            set { m_lasthitX = value; }
        }
        public int LastHitY
        {
            get { return m_lasthitY; }
            set { m_lasthitY = value; }
        }
        public FireResult LastResult
        {
            get { return m_lastresult; }
            set { m_lastresult = value; }
        }
        public bool IsNewGame
        {
            get { return m_isnew; }
            set { m_isnew = value; }
        }
        public bool IsEndeded
        {
            get { return m_isended; }
            set { m_isended = value; }
        }
        public int[,] ShipLocs
        {
            get { return shiplocs; }
            set { shiplocs = value; }
        }
        
    }
}
