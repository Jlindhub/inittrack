
int roundNumber = 0;
List<Unit> deadUnits = new List<Unit>(); //todo: dead unit printout
List<Unit> initiativeTrack = new List<Unit>();
Dictionary<int, Unit> unitNumberDictionary = new Dictionary<int, Unit>(); //lookup dictionary for unit numbers
while (true)
{
    if (unitNumberDictionary.Count == 0)
    {
        Console.WriteLine("unit list is empty, running a ton of adding cycles, manual stop will be required.");
        UnitAdder(1000);
        continue;
    }
    
    //combat loop
    roundNumber++;
    Console.WriteLine("round number: " + roundNumber);
    InitiativeSort();//round start
    foreach (var u in initiativeTrack)
    {
        u.HasActedThisTurn = false;
    }
    InitiativePrint();
    

    for (var i = 0; i < initiativeTrack.Count; i++)
    {
        var unit = initiativeTrack[i];
        if (unit.HasActedThisTurn) continue;
        Console.WriteLine($"\n \t currently:\n{unit.InitiativePlace}\t{unit.Init}\t{unit.Number}\t{unit.Name} 's turn.\n ");
        Console.WriteLine($"str: {unit.Str}/{unit.Ostr}, dex: {unit.Dex}/{unit.ODex}, end: {unit.End}/{unit.OEnd}");
        Console.WriteLine("what do you wish to do? [O]pen input menu, or any key for next unit.");
        var key = Console.ReadKey();
        Console.WriteLine("");
        if (key.Key == ConsoleKey.O){MainInput();}

        Console.WriteLine("turn over. any single-turn modifiers will be reset.");
        unit.HasActedThisTurn = true;
        unit.Init += unit.TimesDodged * 2;
        unit.RollMod = 0;
        InitiativeSort();
        InitiativePrint();
    }

    Console.WriteLine("\n \n \n \t Round over. next round begins.\n \n \n");
    
}


void MainInput()
{
    bool inputLoop = true;
    while (inputLoop)
    {
        Console.WriteLine("\t commands:");
        Console.WriteLine("[R]ESET: new combat (resets to empty lists!)");
        Console.WriteLine("[D]ODGE #: quickly applies dodge penalties to unit specified.");
        Console.WriteLine("[E]DIT #: edit unit number (init, str, etc)");
        Console.WriteLine("[K]ILL #: kill unit number (removes from list, adds to dead list");
        Console.WriteLine("RE[M]OVE #: remove unit number (removes, no dead list)");
        Console.WriteLine("[A]DD #: starts unit-adding, loops # times");
        Console.WriteLine("[N]EXT: goes to next unit in turn, or to next turn if no more units.");
        Console.WriteLine("[P]RINT: prints current initiative and dead lists");
        var input = Console.ReadKey();
        Console.WriteLine("");
        Unit unit;
            switch (input.Key) 
            {
                case ConsoleKey.R:
                    ResetLists();
                    break;
                case ConsoleKey.D:
                    UnitSelector().QuickDodge();
                    break;
                case ConsoleKey.E:
                    Edit(UnitSelector());
                    break;
                case ConsoleKey.K:
                    unit = UnitSelector();
                    deadUnits.Add(unit);
                    unitNumberDictionary.Remove(unit.Number);
                    initiativeTrack.Remove(unit);
                    Console.WriteLine($"unit {unit.Number}, {unit.Name} is dead and is no longer in the initiative count.");
                    break;
                case ConsoleKey.M:
                    unit = UnitSelector();
                    unitNumberDictionary.Remove(unit.Number);
                    Console.WriteLine($"unit {unit.Number}, {unit.Name} has been removed.");
                    break;
                case ConsoleKey.A:
                    int num = NumIn();
                    Console.WriteLine($"adding {num} units.");
                    UnitAdder(num);
                    break;
                case ConsoleKey.N:
                    inputLoop = false;
                    break;
                case ConsoleKey.P:
                    InitiativePrint();
                    DeadPrint();
                    break;
                default:
                    BadPut();
                    break;
            }
    }
}


//command methods



void Edit(Unit u)
{
    Console.WriteLine("editing unit {0}, #{1}. \n \t commands: \n INIT \n OINIT \n STR \n DEX \n END, \n MOD", u.Name, u.Number);
    switch (CNorm())
    {
        case "INIT":
            Prompt("initiative", ref u.OInit, ref u.Init);
            break;
        case "OINIT":
            Prompt("base initiative", ref u.OInit, ref u.OInit);
            break;
        case "STR":
            Prompt("strength", ref u.Ostr, ref u.Str);
            break;
        case "DEX":
            Prompt("Dexterity", ref u.ODex, ref u.Dex);
            break;
        case "END":
            Prompt("Endurance", ref u.OEnd, ref u.End);
            break;
        case "MOD":
            Prompt("roll modifier", ref u.RollMod, ref u.RollMod);
            break;
        default:
            BadPut();
            break;
    }

    void Prompt(string label, ref int orig, ref int cur)
    {
        bool hasOriginal = label != "roll modifier";
        Console.WriteLine($"editing {label}. \n{(hasOriginal ? $"Original value: {orig}, " : "")}current value: {cur}, please input modification to be added on.");
        cur = NumIn();
    }
    
}

void UnitAdder(int repeats){
        
    for (int i = 0; i < repeats; i++)
    {
        Console.WriteLine("\nunit number: " + (i+1) + ", name?(STOP to stop early)");
        string tempName = Console.ReadLine() ?? "unnamed";
        if (tempName.ToUpperInvariant() == "STOP"){Console.WriteLine("stopping additions.");break;}
        int tempIni = StatPrompt("initiative");
        int tempStr = StatPrompt("str");
        int tempDex = StatPrompt("dex");
        int tempEnd = StatPrompt("end");
        Unit unit = new Unit(tempName, tempIni, tempStr, tempDex, tempEnd, 0, i+1);
        unitNumberDictionary.Add(unit.Number,unit);
        initiativeTrack.Add(unit);
    }

    int StatPrompt(string label)
    {
        Console.WriteLine($"{label}?");
        return NumIn();
    }
}

//utility methods

void ResetLists()
{
    unitNumberDictionary.Clear();
    deadUnits.Clear();
    Console.WriteLine("active and dead lists cleared.");
}

void InitiativeSort() 
{
    int initIndex = 1;
    initiativeTrack.Sort((a,b) => b.Init.CompareTo(a.Init));
    foreach (var unit in initiativeTrack)
    {
        unit.InitiativePlace = initIndex;
        initIndex++;
    }

    Console.WriteLine("initiative updated. new initiative:");
}

void InitiativePrint() 
{
    Console.WriteLine("\n \n \t round order:");
    Console.WriteLine("#\tINIT\tUNIT#\tNAME\tDONE?");
    foreach (var unit in initiativeTrack)
    {
        Console.WriteLine($"{unit.InitiativePlace}\t{unit.Init}\t{unit.Number}\t{unit.Name}\t{(unit.HasActedThisTurn ? "DONE" : "NOT DONE")}");
    }

}

void DeadPrint()
{
    Console.WriteLine("current dead units:");
    foreach (var unit in deadUnits)
    {
        Console.Write($"{unit.Number}-{unit.Name}  ");
    }
}

string CNorm() //check for null and normalize to upper invariant
{
    while (true)
    {
        var inp = Console.ReadLine();
        if(!string.IsNullOrEmpty(inp))return inp.ToUpperInvariant();
        Console.WriteLine("empty input, try again");
    }
}

int NumIn() //validate number input
{
    while (true)
    {
        if(int.TryParse(Console.ReadLine(), out int num)){return num;}
        Console.WriteLine("input error, numbers only.");
    }
}

Unit UnitSelector()
{
    while (true)
    {
        Console.WriteLine("please select a unit.");
        foreach (var unit in unitNumberDictionary)
        {
            Console.WriteLine($"#{unit.Key}, {unit.Value.Name}");
        }
        int num = NumIn();
        if (num <= unitNumberDictionary.Count && num > 0) return unitNumberDictionary[num];
        Console.WriteLine("invalid number.");
    }
}

void BadPut() {Console.WriteLine("Unknown Command. Please try again."); } //minimizes repetition.
class Unit
{
    public int InitiativePlace;
    public int TimesDodged;
    public readonly int Number;
    public readonly string Name;
    public int OInit;
    public int Init;
    public int Ostr;
    public int Str;
    public int ODex;
    public int Dex;
    public int OEnd;
    public int End;
    public int RollMod;
    public bool HasActedThisTurn;
    public Unit(string name, int init, int str, int dex, int end, int rollMod, int unitNumber)
    {
        InitiativePlace = 0;
        TimesDodged = 0;
        Number = unitNumber; 
        Name = name; 
        OInit = init; 
        Init = OInit;
        Str = str; 
        Dex = dex; 
        End = end;
        Ostr = str;
        ODex = dex;
        OEnd = end;
        RollMod = rollMod;
        HasActedThisTurn = false;
    }

    public void QuickDodge()
    {
        Init -= 2; 
        RollMod -= 1;
        TimesDodged++;
    }
}