﻿// TabScore - TabScore, a wireless bridge scoring program.  Copyright(C) 2022 by Peter Flippant
// Licensed under the Apache License, Version 2.0; you may not use this file except in compliance with the License

using System;
namespace TabScore.Models
{
    public class EnterDirection
    {
        public int SectionID { get; set; }
        public int TableNumber { get; set; }
        public Direction Direction { get; set; }
        public string DisplayDirection { get; set; }
        public int RoundNumber { get; set; }
        public bool NorthSouthMissing { get; set; }
        public bool EastWestMissing { get; set; }
        public bool Confirm { get; set; }

        public EnterDirection(Section section, int tableNumber, Direction direction, int roundNumber, bool confirm)
        {
            SectionID = section.SectionID;
            TableNumber = tableNumber;
            Direction = direction;
            DisplayDirection = Enum.GetName(typeof(Direction), direction);
            if (section.TabletDevicesPerTable == 2)
            {
                if (direction == Direction.North) DisplayDirection = "North/South";
                else if (direction == Direction.East) DisplayDirection = "East/West";
            }
            RoundNumber = roundNumber;
            TableStatus tableStatus = AppData.TableStatusList.Find(x => x.SectionID == SectionID && x.TableNumber == tableNumber);
            NorthSouthMissing = tableStatus.RoundData.NumberNorth == 0 || tableStatus.RoundData.NumberNorth == section.MissingPair;
            EastWestMissing = tableStatus.RoundData.NumberEast == 0 || tableStatus.RoundData.NumberEast == section.MissingPair;
            Confirm = confirm;
        }
    }
}