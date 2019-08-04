using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels.Http;
using BattleShipControl;
using System.Net;
using System.Threading;
using System.Runtime.InteropServices;

namespace BattleShipGame
{
    public partial class MainEntry : Form
    {
        private int hitCount = 0;
        private int missCount = 0;
        private int attemptCount = 0;
        private Thread oThread = null;
        BattleShipService battle;
        BattleShipService battleClient;
        string Opponent = null;
        public Wait w;
        TcpChannel chnl;
        TcpChannel ClientChannel;
        
        [DllImport("winmm.dll")]
        public static extern long PlaySound(String lpszName, long hModule, long dwFlags);

        private string m_strCurrentSoundFile = "miss.wav";

        public MainEntry()
        {
            InitializeComponent();
            dg1.CellClick+=new DataGridViewCellEventHandler(dg1_CellClick);
            dg2.CellClick += new DataGridViewCellEventHandler(dg2_CellClick);
            initializeGrids(dg1);
            initializeGrids(dg2);
        }
        public void initializeGrids(DataGridView dg)
        {
            for (int i = 0; i < 10; i++)
            {
                dg.Columns.Add("Column-" + i + 1, "Column" + i + 1);
                dg.Columns[i].Width = 70;
            }
            //for (int i = 0; i < 10; i++)
            dg.Rows.Add(10);
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                { dg.Rows[i].Cells[j].Tag = 0; dg.Rows[i].Cells[j].Style.BackColor = Color.White; dg.Rows[i].Cells[j].Value = ""; }
        }
        
        public void LoadData(DataGridView dg,BattleShipService bship)
        {
            string[] row=new string[10];
            
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    //row[j] = bship.battlegrid.battleGrid[i, j].ToString();
                    if (bship.battlegrid.battleGrid[i, j] == 0)
                        row[j] = "";

                    else
                    // row[j] = "¤";
                    {

                        //dg.Rows[i].Cells[j].Value = bship.battlegrid.battleGrid[i, j].ToString();
                        dg.Rows[i].Cells[j].Value = "¤";
                        dg.Rows[i].Cells[j].Style.BackColor = Color.SeaGreen;
                    }

                }
                 
            }

        }
        private void RefreshGrid(DataGridView dgv)
        {
            dgv.Rows.Clear();
            dgv.Columns.Clear();
            initializeGrids(dgv);
        }
        public void dg1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (battleClient.IsEndeded)
            return;
            
            DataGridView dgv = (DataGridView)sender;
            if (dgv.CurrentCell.RowIndex > 9)
                return;
            //dgv.CurrentCell.Style.BackColor = Color.Red;
            //MessageBox.Show("Clicked");
            attemptCount++;
            battle.Attempts = attemptCount;
            
            // an attempt has been made so check the button
            string but = (string)dgv.CurrentCell.Value;
            if (but != "Splosh!")
            {
                if (but != "Boom!")
                {
                    
                    try
                    {
                        ChangeCellState(dgv); //pressed button   

                    }
                    catch (Exception ex) { MessageBox.Show("" + ex.Message); }
                }
            }


                battle.NextTurn = Opponent;
                //if (attemptCount == 1)
                    battleClient.NextTurn = Opponent;
                w.WaitMessage.Text = "Waiting for " + Opponent+ " to play his/her turn";
                w.ShowDialog();
                
            
        }

        private void ChangeCellState(DataGridView dgv)
        {
            int x = dgv.CurrentCell.RowIndex;
            int y = dgv.CurrentCell.ColumnIndex;
            //if (playerGrid.fireMissle(x,y))
            
            if (battleClient.Fire(x, y) != FireResult.Miss)
            {
                
                dgv.CurrentCell.Style.BackColor = Color.Red;
                dgv.CurrentCell.Value = "Boom!";
                PlaySoundInThread("hit.wav");
                //grid[i].Enabled = false;
                dgv.CurrentCell.Tag = -1;
                //battleClient.battlegrid.battleGrid[dgv.CurrentCell.RowIndex, dgv.CurrentCell.ColumnIndex] = -1;
                battle.Scores++;
                hitCount++;
            }
            else
            {
                dgv.CurrentCell.Style.BackColor = Color.Blue;
                dgv.CurrentCell.Value = "Splosh!";
                PlaySoundInThread("miss.wav");
                //grid[i].Enabled = false;
                //battleClient.battlegrid.battleGrid[dgv.CurrentCell.RowIndex, dgv.CurrentCell.ColumnIndex] = -2;
                missCount++;
                dgv.CurrentCell.Tag = -2;
            }
            
            // check to see if the ship is sunk or has already been sunk.
            // play a ship's bell to indicate sinking.
            for (int count = 0; count < 5; count++)
            {
                //if (playerGrid.getBattleShip(count).isSunk()) 
                
                if (battleClient.GetShip(count).isSunk())
                {
                    statusStrip1.Items[count].BackColor = Color.Red;
                    //if(playerGrid.getBattleShip(count).isAWreck()) 	
                    if (battleClient.GetShip(count).isAWreck())
                    {
                        PlaySoundInThread("ship.wav");
                    }
                }
            }
            // Check for winning or for running out of attempts
            if (CheckForWin(dgv) == true) WonTheGame();
            if ((CheckForWin(dgv) == false) && CheckForAttempt()) LostTheGame(dgv);

        }

        public void dg2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if (ShipsList.SelectedItems.Count < 1)
            {
                MessageBox.Show("Please Select ship Type from the List", "Problem in size", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            /*if (dgv.CurrentCell.Style.BackColor!=Color.White)
            {
                //MessageBox.Show(dgv.CurrentCell.Style.BackColor.ToString());
                dgv.CurrentCell.Style.BackColor = Color.White;
                dgv.DefaultCellStyle.SelectionBackColor = Color.CornflowerBlue;
                dgv.CurrentCell.Style.SelectionBackColor = Color.CornflowerBlue;
                dgv.CurrentCell.Tag = 0;
                return;
            }*/
            
            switch (Convert.ToInt32((string)ShipsList.SelectedItems[0].Tag))
            {
                case 1:
                    dgv.CurrentCell.Style.SelectionBackColor = Color.Goldenrod;
                    dgv.CurrentCell.Style.BackColor = Color.Goldenrod;
                    dgv.CurrentCell.Tag = 1;
                    break;
                case 2:
                    dgv.CurrentCell.Style.SelectionBackColor= Color.Beige;
                    dgv.CurrentCell.Style.BackColor = Color.Beige;
                    dgv.CurrentCell.Tag = 2;
                    break;
                case 3:
                    dgv.CurrentCell.Style.SelectionBackColor = Color.PaleTurquoise;
                    dgv.CurrentCell.Style.BackColor = Color.PaleTurquoise;
                    dgv.CurrentCell.Tag = 3;
                    break;
                case 4:
                    dgv.CurrentCell.Style.SelectionBackColor = Color.LightYellow;
                    dgv.CurrentCell.Style.BackColor = Color.LightYellow;
                    dgv.CurrentCell.Tag = 4;
                    break;
                case 5:
                    dgv.CurrentCell.Style.SelectionBackColor = Color.YellowGreen;
                    dgv.CurrentCell.Style.BackColor = Color.YellowGreen;
                    dgv.CurrentCell.Tag = 5;
                    break;
                case 6:
                    dgv.CurrentCell.Style.BackColor = Color.White;
                    dgv.DefaultCellStyle.SelectionBackColor = Color.Wheat;
                    dgv.CurrentCell.Style.SelectionBackColor = Color.Wheat;
                    dgv.CurrentCell.Tag = 0;
                    break;

            }
            
            
        }

        private void dg1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        public void PlaySoundInThread(string wavefile)
        {
            m_strCurrentSoundFile = wavefile;
            oThread = new Thread(new ThreadStart(PlayASound));
            oThread.Start();
        }
        public void PlayASound()
        {
            if (m_strCurrentSoundFile.Length > 0)
            {
                PlaySound(Application.StartupPath + "\\" + m_strCurrentSoundFile, 0, 0);
            }
            m_strCurrentSoundFile = "";
            oThread.Abort();
        }

        private bool CheckForWin(DataGridView dgv)
        {
            /*int count = 0;
            for (int i = 0; i < 100; i++)
            {
                if (dgv.CurrentCell.Style.BackColor == Color.Red)
                {
                    count = count + 1;
                }
            }*/
            //if (battle.Scores == 20)
            if ((battle.Scores == 20&&(battle.Attempts<battleClient.Attempts))||(battle.Scores > battleClient.Scores&&attemptCount==35))
            {
              
                return true;
            }
            lblAim.Text = "You have " + (35 - attemptCount) + " goes left - hit " + hitCount + " missed " + missCount;
            return false;
        }

        // Check if the user has tried to many times.
        private bool CheckForAttempt()
        {
            if (attemptCount == 35)
            {
                return true;
            }
            return false;
        }

        // Lost The Game.....
        // the user has lost game therefore show all the missed squares and idicate to the user hard luck
        private void LostTheGame(DataGridView dgv)
        {
            showShips(dgv);
            lblAim.Text = "Hard Luck you lost you tried " + (attemptCount) + " - hit " + hitCount + " missed " + missCount;
            //for (int i = 0; i < 100; i++)
            
            //    dgv.Enabled = false;
            TurnTimer.Enabled = false;
            w.Hide();
            System.Windows.Forms.MessageBox.Show("Hard luck you lost!!!!");
            PlaySoundInThread("END.wav");
        }

        private void WonTheGame()
        {
            w.Hide();
            lblAim.Text = "Congratulations - You have hit " + hitCount + " missed " + missCount;
            System.Windows.Forms.MessageBox.Show("Congratulations you Won!!!!");
            PlaySoundInThread("tada.wav");
            TurnTimer.Enabled = false;
        }
        private void showShips(DataGridView dgv)
        {

            for (int i = 0; i < 10; i++)
            {

                //int x = dgv.CurrentCell.RowIndex;
                //int y = dgv.CurrentCell.ColumnIndex;
                for (int j = 0; j < 10;j++ )
                    if (dgv.Rows[i].Cells[j].Style.BackColor != Color.Red)
                    {
                        //if (playerGrid.fireMissle(x,y))
                        if (battleClient.Fire(i, j) != FireResult.Miss)
                        {
                            dgv.Rows[i].Cells[j].Style.BackColor = Color.Yellow;
                            dgv.Rows[i].Cells[j].Value = "Missed!";
                            //dgv.CurrentCell.Enabled = false;
                        }
                    }
            }
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        int Clickcount = 0;
        private void StartGameButton_Click(object sender, EventArgs e)
        {
            int[,] shiplocs;
            shiplocs=LoadGrids();
            try
            {
                if (Clickcount == 0)
                    EstablishConnection(shiplocs);
                else
                    StartNewGame();
                groupBox2.Enabled = false;
                Clickcount++;
            }
            catch (Exception) { }
            /*int[,] shiplocs;
            try
            {
                shiplocs = GetShipsGrid();
                if (shiplocs != null)
                {
                    
                    groupBox2.Enabled = false;
                    dg2.ReadOnly = true;
                    
                    
                    dg1.Enabled = true;
                }
                else
                    MessageBox.Show("Please Check ships for sizes", "Problem in size", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex) { MessageBox.Show("Please Check whether ships are correctly placed\n"+ex.Message, "Problem in Ship Initialization", MessageBoxButtons.OK, MessageBoxIcon.Warning); }        */
        }
        private void EstablishConnection(int[,] shipslocs)
        {
            
            try
            {
                if (shipslocs == null)
                {
                    groupBox2.Enabled = true;
                    return;
                }
                battle = new BattleShipService();
                
                
                IDictionary prop = new Hashtable();
                BinaryServerFormatterSinkProvider serverProv = new BinaryServerFormatterSinkProvider();
                BinaryClientFormatterSinkProvider clientProv = new BinaryClientFormatterSinkProvider();
                serverProv.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
                prop["name"] = "tcpserver";
                prop["port"] = Convert.ToInt32(ServerPort.Text);
                
                //chnl = new TcpChannel(Convert.ToInt32(ServerPort.Text));
                chnl = new TcpChannel(prop, clientProv, serverProv);

                ChannelServices.RegisterChannel(chnl);

                RemotingConfiguration.RegisterWellKnownServiceType(
                    typeof(BattleShipService), "BattleShipService", WellKnownObjectMode.Singleton);

                


                prop = new Hashtable();
                prop["name"] = "tcp1";
                prop["port"] = (Convert.ToInt32(ClientPort.Text) + 10).ToString();
                //ClientChannel = new TcpChannel(prop, null, null);
                serverProv = new BinaryServerFormatterSinkProvider();
                clientProv = new BinaryClientFormatterSinkProvider();
                ClientChannel = new TcpChannel(prop, clientProv, serverProv);

                ChannelServices.RegisterChannel(ClientChannel);

                //TcpChannel Clientchnl = new TcpChannel(prop,null,null);
                //ChannelServices.RegisterChannel(Clientchnl);

                battle = (BattleShipService)Activator.GetObject(typeof(BattleShipService),
                                        "tcp://" + Environment.MachineName + ":" + ServerPort.Text + "/BattleShipService");
                battle.CreateGrid(shipslocs);
                battle.Player = PName.Text;
                //battle.NextTurn = PName.Text;
                TurnTimer.Enabled = true;

                w = new Wait();
                //w.ShowDialog();
                


                TurnTimer.Interval = 1000;
                groupBox1.Enabled = false;
                startNewGameToolStripMenuItem.Enabled = true;
                endGameToolStripMenuItem.Enabled = true;

                try
                {
                    battleClient = (BattleShipService)Activator.GetObject(typeof(BattleShipService),
                                            "tcp://" + ClientLoc.Text + ":" + ClientPort.Text + "/BattleShipService");
                    Opponent = battleClient.Player;
                    battle.NextTurn = Opponent;

                    battleClient.ConnectPlayer();

                    //LoadData(dg1, battleClient);
                    w.WaitMessage.Text = "Waiting for " + Opponent + " to play his/her turn";
                    w.ShowDialog();

                }
                catch (Exception) { PlayerTimer.Enabled = true; }
            }
            catch (Exception error)
            {

                MessageBox.Show("Error Connecting, please check connection information\n" + error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                groupBox1.Enabled = true;
                groupBox2.Enabled = true;
                try
                {
                    ChannelServices.UnregisterChannel(chnl);
                    ChannelServices.UnregisterChannel(ClientChannel);
                    Clickcount = 0;
                }
                catch (Exception ex) { }
            }
        }
        private void PlayerTimer_Tick(object sender, EventArgs e)
        {

            try
            {
                
                if (battle.playerconnected)
                {
                    battleClient = (BattleShipService)Activator.GetObject(typeof(BattleShipService),
                                                  "tcp://" + ClientLoc.Text + ":" + ClientPort.Text + "/BattleShipService");
                
                    PlayerTimer.Enabled = false;
                    Opponent = battleClient.Player;
                    battle.NextTurn = PName.Text;
                   // LoadData(dg1, battleClient);
                }
                
            }
            catch (Exception ex) { MessageBox.Show(""+ex.Message); }
        }
        

        public bool EndGame()
        {
            try
            {
                if (MessageBox.Show("Do you really want to end the game!", "End Game", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    TurnTimer.Enabled = false;
                    battle.EndGame();
                    battleClient.EndGame();
                    dg1.Enabled = false;
                    return true;
                }
                else
                    return false;
            }
            catch (Exception) { return true; }
            
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new About().ShowDialog();
        }

        private void startNewGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshGrid(dg1);
            RefreshGrid(dg2);
            StartNewGame();
        }
        private void StartNewGame()
        {
            battle.StartNewGame(PName.Text);
            
            RefreshCalcs();
            //LoadData(dg2, battle);
            groupBox2.Enabled = true;
            
            battleClient.IsNewGame = true;
            battle.NextTurn = PName.Text;
            TurnTimer.Enabled = true;
            if (w.Visible)
                w.Hide();
            battle.IsEndeded = false;
            
            
        }
        private int[,] LoadGrids()
        {
            int[,] shiplocs=null;
            try
            {
                
                dg1.Enabled = false;
                shiplocs = GetShipsGrid();
                if (shiplocs != null)
                {

                    groupBox2.Enabled = false;
                    dg2.ReadOnly = true;
                    dg1.Enabled = true;
                }
                else
                    MessageBox.Show("Please Check ships for sizes", "Problem in size", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex) { MessageBox.Show("Please Check whether ships are correctly placed\n" + ex.Message, "Problem in Ship Initialization", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            return shiplocs;
        }

        private void endGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EndGame();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Application.Exit();
        }
        Thread th = null;
        private void TurnTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (battle.NextTurn == PName.Text && w.Visible&&!battle.IsEndeded)
                    w.Hide();
                else if (battle.NextTurn == Opponent && !w.Visible && !battle.IsEndeded)
                    w.ShowDialog();
                
            
            th = new Thread(new ThreadStart(UpdateVision));
            
            th.Start();
            }
            catch (Exception) { }
        }
        public void UpdateVision()
        {
            
            if (battle.LastHitX > -1 && battle.LastHitY > -1)
            {
                if (battle.LastResult != FireResult.Miss)
                {
                    dg2.Rows[battle.LastHitX].Cells[battle.LastHitY].Style.BackColor = Color.Red;
                    dg2.Rows[battle.LastHitX].Cells[battle.LastHitY].Value = "Boom!";
                    dg2.Rows[battle.LastHitX].Cells[battle.LastHitY].Tag = 1;

                }
                else
                {
                    dg2.Rows[battle.LastHitX].Cells[battle.LastHitY].Style.BackColor = Color.Blue;
                    dg2.Rows[battle.LastHitX].Cells[battle.LastHitY].Value = "Splosh!";
                    dg2.Rows[battle.LastHitX].Cells[battle.LastHitY].Tag = 2;
                }
            }

            if (battle.IsNewGame)
            {
                battle.IsNewGame = false;
                battle.StartNewGame(PName.Text);
                RefreshGrid(dg1);
                RefreshGrid(dg2);
                //LoadData(dg2, battle);
                battle.NextTurn = Opponent;
                w.Hide();
                battle.IsEndeded = false;
                RefreshCalcs();
                groupBox2.Enabled = true;
                //MessageBox.Show("Please Select your Ships\nGame has been reset by the other user", "Select Your Ships", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if (battle.IsEndeded)
            {
                //TurnTimer.Enabled = false;
                w.Hide();
                initializeGrids(dg1);
                initializeGrids(dg2);
                groupBox2.Enabled = true;
            }
            /*if (!battle.playerconnected&&Opponent!=battle.Player)
            {
                TurnTimer.Enabled = false;   
                groupBox1.Enabled = true;

                ChannelServices.UnregisterChannel(chnl);
                ChannelServices.UnregisterChannel(ClientChannel);
            }*/
            th.Abort();
        }
        private void RefreshCalcs()
        {
            attemptCount = 0;
            hitCount = 0;
            missCount = 0;

        }
        private int[,] GetShipsGrid()
        {
            int AC = 0, DT = 0, CR = 0, SB = 0, PB = 0;
            int[,] shiplocs=new int[10,10];
            
            for (int i = 0; i < 10;i++ )

                for (int j = 0; j < 10; j++)
                {
                    
                    switch(Convert.ToInt32(dg2.Rows[i].Cells[j].Tag.ToString()))
                    {
                        
                        case 1:
                        
                        shiplocs[i, j] = Convert.ToInt32(ShipsList.Items[0].Tag.ToString());
                        //dg2.Rows[i].Cells[j].Value = shiplocs[i, j];
                        AC++;
                        break;
                        case 2:
                        shiplocs[i, j] = Convert.ToInt32(ShipsList.Items[1].Tag.ToString());
                        //dg2.Rows[i].Cells[j].Value = shiplocs[i, j];
                        DT++;
                        break;
                        case 3:
                        shiplocs[i, j] = Convert.ToInt32(ShipsList.Items[2].Tag.ToString());
                        //dg2.Rows[i].Cells[j].Value = shiplocs[i, j];
                        CR++;
                        break;
                        case 4:
                        shiplocs[i, j] = Convert.ToInt32(ShipsList.Items[3].Tag.ToString());
                        //dg2.Rows[i].Cells[j].Value = shiplocs[i, j];
                        SB++;
                        break;
                        case 5:
                        shiplocs[i, j] = Convert.ToInt32(ShipsList.Items[4].Tag.ToString());
                        //dg2.Rows[i].Cells[j].Value = shiplocs[i, j];
                        PB++;
                        break;
                        default:
                        shiplocs[i, j] = 0;
                        //dg2.Rows[i].Cells[j].Value = shiplocs[i, j];
                        break;
                    }
                }

            //MessageBox.Show("AC="+AC+"\nDT="+DT+"\nCR="+CR+"\nSB="+SB+"\nPB="+PB);
            if (AC == 6 && DT == 5 && CR == 4 && SB == 3 && PB == 2)
                return shiplocs;
            return null;
        }

        
        
    }
}
