using System.Collections.Generic;

namespace TabletArtco
{
        class Block
        {
                public static List<List<Block>> blocks { get; set; } = new List<List<Block>>();
                public string name { get; set; }
                public int category { get; set; }
                public int inputState { get; set; }
                public int idx { get; set; }
                public string remotePath { get; set; }                
        }
}