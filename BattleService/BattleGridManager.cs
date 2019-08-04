namespace BattleBoat
{
    using System;

    /// <summary>
    /// Manages Grid for player
    /// </summary>
//[Serializable]
    public class BattleGridManager : MarshalByRefObject
{

public int[,] battleGrid;
private int[,] missleGrid;
private BattleShip[] ships;

  // Fire Missle - this fires a missle at the grid and detects a hit.
  public bool fireMissle(int x, int y)
  {
      
  	bool hit = false;
  	missleGrid[x,y] = 9;
    
    if (getShip(x,y) > 0 && getShip(x,y) < 8)
  	{
  	  hit = true;
  	  missleGrid[x,y] = 8;
      
      if (getShip(x,y) > 0 && getShip(x,y) < 6)
      {
       	  	 		 		      
      ships[(getShip(x,y)-1)].setHit();
      }
  	}
  	return hit;
  }
    
  //getShip this gets what ship is hidden on the grid.
  public int getShip(int x, int y)
  {
  	
  if(x >9)
    {
    		return 9;    	
    }
  if (y > 9)
  	{
    		return 9;
   	}      
  //    System.Windows.Forms.MessageBox.Show("get ship x & y " + x + y);
  return battleGrid[x,y];
  }

 // get battleship
 public BattleShip getBattleShip(int ship)
 { 
 	return ships[ship];
 }
 
 // add ship to grid
 public void addShip(BattleShip ship)
 {
 	bool placed = false;
 	// continue until random setting is complete
    while(!placed)
 	{	
 	  // I have found that random does not return random numbers randomly.
 	  Random r = new Random();
 	  int x = r.Next(10); // 
 	      x = r.Next(10); // do this again to ensure random
      int y = r.Next(10); 
 	      y = r.Next(10); // do again to ensure random!
      //System.Windows.Forms.MessageBox.Show("x & y" + x + y);
 	  placed = true;
 	  int direction = checkPosition(x,y,ship.getSize());
      
 	  if (direction == 1) {addShipDown(x,y,ship);}
 	  if (direction == 2) {addShipAcross(x,y,ship);}
 	  if (direction == 3) {placed = false;}
 	}
 }
 
 // add the ship down the grid
 public void addShipDown(int x, int y, BattleShip ship)
 {
   int downX = x;
   // check down!
   for(int count = 0; count < ship.getSize(); count++)
   {
    setShip(downX,y, ship.getType());
   	ship.setLocation(downX,y);
    downX++;	 
   }
 }

 // add the ship across the grid
 public void addShipAcross(int x, int y, BattleShip ship)
 {
   int downX = x;
   // check across!
   for(int count = 0; count < ship.getSize(); count++)
   {
     setShip(downX,y, ship.getType());
   	 ship.setLocation(downX,y);
     y++;	    	
   }
 }

 // check the position on the grid
 public int checkPosition(int x, int y, int size)
 {
   int direction = 3; // not placed
   int downX = x;

   // check down!
   bool okay = true;
   for(int count = 0; count < size; count++)
   {
     if(getShip(downX,y) != 0) okay = false;
     downX++;	    	
   }
   //System.Windows.Forms.MessageBox.Show("Okay " + okay);  	  	 
   if(okay)
   {	
     direction = 1;		   	
   }
  
   // CHECK ACROSS
   if(!okay)
   {	
    okay = true;
    for(int count = 0; count < size; count++)
     {
       if(getShip(x,y) != 0) okay = false;
       y++;	      	
     }
    if(okay)
     {	
        direction = 2;		   	
     }
    }
   return direction;
 }
 
  // set ships automtically on grid.
  public void addShips()
  {
  	
    ships = new BattleShip[5];
    //System.Windows.Forms.MessageBox.Show("Building my ships");
	ships[0] = new BattleShip(1,6); // aircrft
  	ships[1] = new BattleShip(2,5); // cruiser  	
  	ships[2] = new BattleShip(3,4); // destroyer
    ships[3] = new BattleShip(4,3); // submarine    
  	ships[4] = new BattleShip(5,2); // patrol boat

    //System.Windows.Forms.MessageBox.Show("Deploying my ships");  	  	
  	for(int count =0; count < ships.Length; count++)
  	{
    addShip(ships[count]);
  	}
  }
  
  
  //setShip this sets what ship is hidden on the grid.
  public void setShip(int x, int y, int type)
  {
    //System.Windows.Forms.MessageBox.Show("Adding " + x +y + " type " + type);
    battleGrid[x,y] = type;
  }

  // initialise the grid
  public BattleGridManager()
  {
	  Initialize();
      addShips();
  }
  public BattleGridManager(int[,] shiplocs)
  {
      Initialize();
      ships = new BattleShip[5];
      ships[0] = new BattleShip(1, 6); // aircrft
      ships[1] = new BattleShip(2, 5); // cruiser  	
      ships[2] = new BattleShip(3, 4); // destroyer
      ships[3] = new BattleShip(4, 3); // submarine    
      ships[4] = new BattleShip(5, 2); // patrol boat
      
      for (int i = 0; i < 10;i++ )

          for (int j = 0; j < 10; j++)
          {
              battleGrid[i, j] = shiplocs[i, j];
              switch (shiplocs[i,j])
              {
                  case 1:
                  ships[0].setLocation(i, j);
                  setShip(i,j,ships[0].getType());
                  break;
                  case 2:
                  ships[1].setLocation(i, j);
                  setShip(i,j,ships[1].getType());
                  break;
                  case 3:
                  ships[2].setLocation(i, j);
                  setShip(i,j,ships[2].getType());
                  break;
                  case 4:
                  ships[3].setLocation(i, j);
                  setShip(i, j, ships[3].getType());
                  break;
                  case 5:
                  ships[4].setLocation(i, j);
                  setShip(i, j, ships[4].getType());
                  break;
              }
          }
      
      
  }
  // initialise the grid
  public void Initialize ()
  {
    battleGrid = new int[10,10] {
						{0,0,0,0,0,0,0,0,0,0},
						{0,0,0,0,0,0,0,0,0,0},
						{0,0,0,0,0,0,0,0,0,0},
						{0,0,0,0,0,0,0,0,0,0},
						{0,0,0,0,0,0,0,0,0,0},
						{0,0,0,0,0,0,0,0,0,0},
						{0,0,0,0,0,0,0,0,0,0},
						{0,0,0,0,0,0,0,0,0,0},
						{0,0,0,0,0,0,0,0,0,0},
						{0,0,0,0,0,0,0,0,0,0}};
  	
    missleGrid = new int[10,10] {
						{0,0,0,0,0,0,0,0,0,0},
						{0,0,0,0,0,0,0,0,0,0},
						{0,0,0,0,0,0,0,0,0,0},
						{0,0,0,0,0,0,0,0,0,0},
						{0,0,0,0,0,0,0,0,0,0},
						{0,0,0,0,0,0,0,0,0,0},
						{0,0,0,0,0,0,0,0,0,0},
						{0,0,0,0,0,0,0,0,0,0},
						{0,0,0,0,0,0,0,0,0,0},
						{0,0,0,0,0,0,0,0,0,0}};
  	
 // 0 Ocean 
 // 1 aircraft size 6
 // 2 destroyer size 5;
 // 3 cruiser size 4;
 // 4 submarine size 3
 // 5 Patrol ship size 2
 // 8 missle hit
 // 9 missle no hit
 
  }
}
}
