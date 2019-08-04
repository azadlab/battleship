
namespace BattleBoat
{
    using System;

    /// <summary>
    /// Manages ship for player
    /// </summary>
//[Serializable]
    public class BattleShip : MarshalByRefObject
{

 private int type = 0;
 private int size = 0;
 private int[]location;
 private bool sunk = false;
 private bool Wreck = false;
 private int hits = 0;
 private int setSize = 0;
 	
  // Creates a ship
  public BattleShip(int type, int size)
  {
  	this.type = type;
  	this.size = size;
  	this.sunk = false;
  	this.location = new int[size];
  }

  // makes the ship know where it is.
  public void setLocation(int x, int y)
  {  
    this.location[setSize] = ((x*10) + y);
    this.setSize++;  	
  }
  
  // get the location
  public int[] getLocation()
  {  
    return location;  	
  }
  
  // set hit
  public void setHit()
  {
  	this.hits = hits + 1;
  	setSunk();
  }
  
  //set sunk indicates the ship is sunk
  public void setSunk()
  {
  	if(hits == size)
  	{ 
        //System.Windows.Forms.MessageBox.Show("I'm Sunk");  	  		
  		this.sunk = true;
  	}
  }

  //is a wreck returns true if ship has sunk.
  public bool isAWreck()
  { 	
  	if(!Wreck)
  	{
  		
  	Wreck = true;
  	return this.Wreck;
  	}
  	
  	return false;
  }

  //is sunk returns true if ship is sunk.
  public bool isSunk()
  { 	
  	return this.sunk;
  }
  
  // get size
  public int getSize()
  {
  	return this.size;
  }
  
  // get type
  public int getType()
  {
   // 0 Ocean 
   // 1 aircraft size 6
   // 2 destroyer size 5;
   // 3 cruiser size 4;
   // 4 submarine size 3
   // 5 Patrol ship size 2
   // 8 missle hit
   // 9 missle no hit

  return this.type;
  }
 }
}
