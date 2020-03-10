using System.Collections.Generic;

namespace TabletArtco
{
    class Block
    {
        public static int[] blockTab0ResIds = {
            Resource.Drawable.Eventblock_Start, Resource.Drawable.Eventblock_RecvSignal, Resource.Drawable.Eventblock_InputKey,
            Resource.Drawable.Eventblock_Touch, Resource.Drawable.Eventblock_ClickSprite, Resource.Drawable.Eventblock_Clone
        };

        public static int[] blockTab1ResIds = {
            Resource.Drawable.Moveblock_Down1, Resource.Drawable.Moveblock_Down5, Resource.Drawable.Moveblock_Down10,
            Resource.Drawable.Moveblock_Left1, Resource.Drawable.Moveblock_Left5, Resource.Drawable.Moveblock_Left10,
            Resource.Drawable.Moveblock_Right1, Resource.Drawable.Moveblock_Right5, Resource.Drawable.Moveblock_Right10,
            Resource.Drawable.Moveblock_Up1, Resource.Drawable.Moveblock_Up5, Resource.Drawable.Moveblock_Up10,
            Resource.Drawable.Actblock_Slow, Resource.Drawable.Actblock_Fast, Resource.Drawable.Actblock_Flash,
            Resource.Drawable.Actblock_RRotate, Resource.Drawable.Actblock_LRotate, Resource.Drawable.Actblock_RotateLoop,
            Resource.Drawable.Actblock_Wave, Resource.Drawable.Actblock_TWave, Resource.Drawable.Actblock_RandomMove,
            Resource.Drawable.Actblock_Zigzag, Resource.Drawable.Actblock_TZigzag, Resource.Drawable.Actblock_Bounce,
            Resource.Drawable.Actblock_Jump, Resource.Drawable.Actblock_RLJump, Resource.Drawable.Actblock_Animate
        };
        public static int[] blockTab2ResIds = {
            Resource.Drawable.Moveblock_LUpN, Resource.Drawable.Moveblock_UpN, Resource.Drawable.Moveblock_RUpN,
            Resource.Drawable.Moveblock_LeftN, Resource.Drawable.Moveblock_Empty, Resource.Drawable.Moveblock_RightN,
            Resource.Drawable.Moveblock_LDownN, Resource.Drawable.Moveblock_DownN, Resource.Drawable.Moveblock_RDownN,
            Resource.Drawable.Actblock_Slow, Resource.Drawable.Actblock_Fast, Resource.Drawable.Actblock_Flash,
            Resource.Drawable.Actblock_RRotateN, Resource.Drawable.Actblock_LRotateN, Resource.Drawable.Actblock_RotateLoop,
            Resource.Drawable.Actblock_Wave, Resource.Drawable.Actblock_TWave, Resource.Drawable.Actblock_RandomMove,
            Resource.Drawable.Actblock_Zigzag, Resource.Drawable.Actblock_TZigzag, Resource.Drawable.Actblock_Bounce,
            Resource.Drawable.Actblock_Jump, Resource.Drawable.Actblock_RLJump, Resource.Drawable.Actblock_Animate
        };
        public static int[] blockTab3ResIds = {
            Resource.Drawable.Contblock_Time1, Resource.Drawable.Contblock_Time2, Resource.Drawable.Contblock_TimeN,
            Resource.Drawable.Contblock_LoopN, Resource.Drawable.Contblock_loop, Resource.Drawable.Contblock_Flag,
            Resource.Drawable.Contblock_FlipX, Resource.Drawable.Contblock_FlipY, Resource.Drawable.Contblock_NextSprite,
            Resource.Drawable.Contblock_Show, Resource.Drawable.Contblock_Hide, Resource.Drawable.Contblock_Sound,
            Resource.Drawable.Contblock_Speak, Resource.Drawable.Contblock_SpeakStop, Resource.Drawable.Contblock_AdditionBackground,
            Resource.Drawable.Contblock_SendSignal, Resource.Drawable.Contblock_SendSigWait, Resource.Drawable.Contblock_XY,
            Resource.Drawable.Contblock_ChangeVal, Resource.Drawable.Contblock_SetVal, Resource.Drawable.Contblock_Stop,
            Resource.Drawable.Contblock_Clone
        };
        public static int[] blockTab4ResIds = {
            Resource.Drawable.Edublock_Down, Resource.Drawable.Edublock_Jump, Resource.Drawable.Edublock_Up,
            Resource.Drawable.Edublock_Left, Resource.Drawable.Edublock_LoopN, Resource.Drawable.Edublock_Right
        };

        public static string[] blockTab0ResIdStrs = {
            "ControlStart", "ControlRecvSig", "EventblockInputKey", 
            "ControlTouch", "ControlClickSprite", "EventblockClone"
        };
        
        public static string[] blockTab1ResIdStrs = {
            "MoveDown1", "MoveDown5", "MoveDown10",
            "MoveLeft1", "MoveLeft5", "MoveLeft10",
            "MoveRight1", "MoveRight5", "MoveRight10",
            "MoveUp1", "MoveUp5", "MoveUp10",
            "ActionSlow", "ActionFast", "ActionFlash",
            "ActionRRotate", "ActionLRotate", "ActionRotateLoop",
            "ActionWave", "ActionTWave", "ActionRandomMove",
            "ActionZigzag", "ActionTZigzag", "ActionBounce",
            "ActionJump", "ActionRLJump", "ActionAnimate"
        };
        public static string[] blockTab2ResIdStrs = {
            "MoveLUpN", "MoveUpN", "MoveRUpN",
            "MoveLeftN", "MoveEmpty", "MoveRightN",
            "MoveLDownN", "MoveDownN", "MoveRDownN",
            "ActionSlow", "ActionFast", "ActionFlash",
            "ActionRRotateN", "ActionLRotateN", "ActionRotateLoop",
            "ActionWave", "ActionTWave", "ActionRandomMove",
            "ActionZigzag", "ActionTZigzag", "ActionBounce",
            "ActionJump", "ActionRLJump", "ActionAnimate"
        };
        public static string[] blockTab3ResIdStrs = {
            "ControlTime1", "ControlTime2", "ControlTimeN",
            "ControlLoopN", "ControlLoop", "ControlFlag",
            "ControlFlipX", "ControlFlipY", "ControlNextSprite",
            "ControlShow", "ControlHide", "ControlSound",
            "ControlSpeak", "ControlSpeakStop", "ControlAdditionBackground",
            "ControlSendSignal", "ControlSendSignalWait", "ControlXY",
            "ControlChangeVal", "ControlSetVal", "ControlStop",
            "ControlClone"
        };
        public static string[] blockTab4ResIdStrs = {
            "GameDown", "GameJump", "GameUp",
            "GameLeft", "GameLoopN", "GameRight"
        };

        public static List<Dictionary<string, string>> TextViewLocations(Block block) {
            List<string> oneList = new List<string> { "MoveLUpN", "MoveUpN", "MoveRUpN", "MoveLeftN", "MoveRightN", "MoveLDownN", "MoveDownN", "MoveRDownN",
            "ActionRRotateN", "ActionLRotateN", "ControlTimeN", "ControlLoopN"};
            if (oneList.Contains(block.name))
            {
                if (!string.IsNullOrEmpty(block.text) || !string.IsNullOrEmpty(block.varName))
                {
                    List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("x", 12 / 70.0 + "");
                    dic.Add("y", 46 / 70.0 + "");
                    dic.Add("w", 47 / 70.0 + "");
                    dic.Add("h", 15 / 70.0 + "");
                    dic.Add("text", string.IsNullOrEmpty(block.text) ? block.varName : block.text);
                    list.Add(dic);
                    return list;
                }
            }
            List<string> twoList = new List<string> { "ControlRecvSig", "ControlSound", "ControlAdditionBackground", "ControlSendSignal", "ControlSpeak" };
            if (twoList.Contains(block.name))
            {
                if (!string.IsNullOrEmpty(block.text))
                {
                    List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("x", 12 / 70.0 + "");
                    dic.Add("y", 46 / 70.0 + "");
                    dic.Add("w", 47 / 70.0 + "");
                    dic.Add("h", 14 / 70.0 + "");
                    dic.Add("text", block.text);
                    list.Add(dic);
                    return list;
                }
            }
            List<string> threeList = new List<string> { "ControlTouch"};
            if (threeList.Contains(block.name))
            {
                if (!string.IsNullOrEmpty(block.text))
                {
                    List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("x", 12 / 99.0 + "");
                    dic.Add("y", 53 / 99.0 + "");
                    dic.Add("w", 75 / 99.0 + "");
                    dic.Add("h", 20 / 99.0 + "");
                    dic.Add("text", block.text);
                    list.Add(dic);
                    return list;
                }
            }
            List<string> FourList = new List<string> { "ControlChangeVal", "ControlSetVal"};
            if (FourList.Contains(block.name))
            {
                if (!string.IsNullOrEmpty(block.varName) && !string.IsNullOrEmpty(block.varValue))
                {
                    List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("x", 27 / 100.0f + "");
                    dic.Add("y", 24 / 100.0f + "");
                    dic.Add("w", 63 / 100.0f + "");
                    dic.Add("h", 24 / 100.0f + "");
                    dic.Add("text", block.varName);
                    Dictionary<string, string> dic1 = new Dictionary<string, string>();
                    dic1.Add("x", 42 / 100.0f + "");
                    dic1.Add("y", 58 / 100.0f + "");
                    dic1.Add("w", 46 / 100.0f + "");
                    dic1.Add("h", 24 / 100.0f + "");
                    dic1.Add("text", block.varValue);
                    list.Add(dic);
                    list.Add(dic1);
                    return list;
                }
            }
            return null;
        }

        public static int GetClickType(Block block) {
            List<List<string>> list = new List<List<string>> {
                new List<string>{ "MoveLUpN", "MoveUpN", "MoveRUpN", "MoveLeftN", "MoveRightN", "MoveLDownN", "MoveDownN", "MoveRDownN", "ActionRRotateN", "ActionLRotateN", "ControlTimeN", "ControlLoopN"},
                new List<string>{ "ControlChangeVal"},
                new List<string>{ "ControlSetVal"},
                new List<string>{ "ControlRecvSig"},
                new List<string>{ "ControlSendSignal"},
                new List<string>{ "ControlTouch"},
                new List<string>{ "ControlSpeak" },
                new List<string>{ "ControlSound" },
                new List<string>{ "ControlAdditionBackground" },
                new List<string>{ "ControlStart" }
            };
            for (int i = 0; i < list.Count; i++)
            {
                List<string> itemList = list[i];
                if (itemList.Contains(block.name))
                {
                    return i;
                }
            }
            return -1;
        }

        public static List<List<Block>> blocks { get; set; } = new List<List<Block>>();

        public int category { get; set; }
        public int inputState { get; set; }
        public int idx { get; set; }
        public string remotePath { get; set; }

        public string name { get; set; }
        public int resourceId { get; set; }
        public int tabIndex { get; set; }
        public int row { get; set; }
        public int index { get; set; }

        public string text { get; set; }
        public string varName { get; set; }
        public string varValue { get; set; }
        public string activateSpriteId { get; set; }
        public int backgroudId { get; set; }

        public bool collionSignal { get; set; } = false;
        public int signalCount { get; set; } = 0;
        public int clickSignalCount { get; set; } = 0;

        public static Block Copy(Block block) {
            Block b = new Block();
            b.category = block.category;
            b.inputState = block.inputState;
            b.idx = block.idx;
            b.remotePath = block.remotePath;

            b.name = block.name;
            b.resourceId = block.resourceId;
            b.tabIndex = block.tabIndex;
            b.row = block.row;
            b.index = block.index;

            b.text = block.text;
            b.varName = block.varName;
            b.varValue = block.varValue;
            b.activateSpriteId = block.activateSpriteId;
            return b;
        }
    }
}
