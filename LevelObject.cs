using System.Collections.Generic;
using System.Windows.Forms;

namespace sth1edwv
{
    public class LevelObject: IDataItem
    {
        public byte Type { get; set; }
        public byte X { get; set; }
        public byte Y { get; set; }

        public LevelObject(IReadOnlyList<byte> memory, int offset)
        {
            Offset = offset;
            LengthConsumed = 3;
            Type = memory[offset++];
            X = memory[offset++];
            Y = memory[offset];
        }

        public int Offset { get; }
        public int LengthConsumed { get; }
        public IList<byte> GetData()
        {
            return new List<byte> { Type, X, Y };
        }

        public TreeNode ToNode()
        {
            if (!Names.TryGetValue(Type, out var name))
            {
                name = "UNKNOWN";
            }

            return new TreeNode($"({X}, {Y}) 0x{Type:X2} = {name}") { Tag = this };
        }

        public static Dictionary<int, string> Names { get; } = new()
        {
            { 0x00, "NONE" },
            { 0x01, "Super Ring monitor" },
            { 0x02, "Power Sneakers monitor" },
            { 0x03, "One-Up monitor" },
            { 0x04, "Shield monitor" },
            { 0x05, "Invincibility monitor" },
            { 0x06, "Chaos Emerald" },
            { 0x07, "End sign" },
            { 0x08, "Badnik 'Crabmeat' (GH)" },
            { 0x09, "Wooden platform - Swinging (GH)" },
            { 0x0A, "Explosion" },
            { 0x0B, "Wooden platform (GH)" },
            { 0x0C, "Wooden platform - Falls when touched (GH)" },
            { 0x0E, "Badnik 'Buzz Bomber' (GH/B)" },
            { 0x0F, "Wooden platform - Sliding left-right (GH)" },
            { 0x10, "Badnik 'Moto Bug' (GH)" },
            { 0x11, "Badnik 'Newtron' (GH)" },
            { 0x12, "Robotnik - Green Hill Boss (GH)" },
            { 0x16, "Flame Thrower (SB)" },
            { 0x17, "Door - Opens from left only (SB)" },
            { 0x18, "Door - Opens from right only (SB)" },
            { 0x19, "Door - Two ways (SB)" },
            { 0x1A, "Electric sphere (SB)" },
            { 0x1B, "Badnik 'Ball Hog' (SB)" },
            { 0x1C, "Unknown - Ball from the 'Ball Hog' ?" },
            { 0x1D, "Switch (SB, L, others ?)" },
            { 0x1E, "Switch Activated Door (SB)" },
            { 0x1F, "Badnik 'Caterkiller' (SB)" },
            { 0x21, "Bumper - Sliding left-right (Bonus Stage ?)" },
            { 0x22, "Robotnik - Scrap Brain Boss (SB)" },
            { 0x23, "Free animal - Rabbit" },
            { 0x24, "Free animal - Bird" },
            { 0x25, "Animal Cell" },
            { 0x26, "Badnik 'Chopper' (J, B)" },
            { 0x27, "Vertical Step - Falling from a waterfall (J)" },
            { 0x28, "Horizontal Step - Falling from a waterfall (J)" },
            { 0x29, "Floating Step - Sonic can travel with it (J)" },
            { 0x2C, "Robotnik - Jungle Boss (J)" },
            { 0x2D, "Badnik 'Yadrin' (B)" },
            { 0x2E, "Falling Bridge (B)" },
            { 0x30, "Passing Clouds (SKYB)" },
            { 0x31, "Propeller (SKYB)" },
            { 0x32, "Badnik 'Bomb' (SKYB)" },
            { 0x33, "Cannon Ball (SKYB)" },
            { 0x35, "Badnik 'Unidus' (SKYB)" },
            { 0x37, "Rotating cannon (SKYB)" },
            { 0x38, "Flying platform (SKYB)" },
            { 0x39, "Spiked wall slowly moving right (SKYB)" },
            { 0x3A, "Small cannon in Sky Base Act 1 (SKYB)" },
            { 0x3B, "Flying platform moving up-down (SKYB)" },
            { 0x3C, "Badnik 'Jaws' (L)" },
            { 0x3D, "Rotating spiked ball (L)" },
            { 0x3E, "Spear, shifting up-down (L)" },
            { 0x3F, "Fire ball thrower (L)" },
            { 0x40, "Water Level Object (L)" },
            { 0x41, "Bubble Maker (L)" },
            { 0x44, "Badnik 'Burrobot' (L)" },
            { 0x45, "Platform, move up when touched (L)" },
            { 0x46, "Electrical Hazard for the Sky Base Boss (SKYB)" },
            { 0x48, "Robotnik - Bridge Boss (SB)" },
            { 0x49, "Robotnik - Labyrinth Boss (L)" },
            { 0x4A, "Robotnik - Sky Base Boss (SKYB)" },
            { 0x4B, "Zone that makes you fall (like in GH2)" },
            { 0x4C, "Flipper (Bonus Stage)" },
            { 0x4D, "RESET!" },
            { 0x4E, "Balance (B)" },
            { 0x4F, "RESET!" },
            { 0x50, "Flower (GH)" },
            { 0x51, "Box - Starpost" },
            { 0x52, "Box - Continue" },
            { 0x53, "Final animation in GH, when Sonic falls on Robotnik (then, goes to the next level)" },
            { 0x54, "Emeralds animation (on the map), when Sonic has them all (and goes to the next level)" },
            { 0x55, "Makes Sonic blink for a short time" },
            { 0xFF, "NONE" }
        };
    }
}