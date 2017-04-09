using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum State
{
    hasPathToIron, hasPathToWood, hasPathToGrass, hasPathToSheep, hasPathToWind, hasPathToStone,
    axeAtBase, pickAxeAtBase, //Mtnclmibkit? bridge?
    hasSpace, hasAxe, hasPickAxe //mtn, bridge?
}
