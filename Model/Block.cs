using System.Collections.Generic;

namespace TabletArtco
{
    class Block
    {
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
            Resource.Drawable.Contblock_AdditionBackground, Resource.Drawable.Contblock_SendSignal, Resource.Drawable.Contblock_ReceiveSignal,
            Resource.Drawable.Contblock_Speak, Resource.Drawable.Contblock_Start
        };
        public static int[] blockTab4ResIds = {
            Resource.Drawable.Edublock_Down, Resource.Drawable.Edublock_Jump, Resource.Drawable.Edublock_Up,
            Resource.Drawable.Edublock_Left, Resource.Drawable.Edublock_LoopN, Resource.Drawable.Edublock_Right
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
            "ControlLoopN", "Controlloop", "ControlFlag",
            "ControlFlipX", "ControlFlipY", "ControlNextSprite",
            "ControlShow", "ControlHide", "ControlSound",
            "ControlAdditionBackground", "ControlSendSignal", "ControlReceiveSignal",
            "ControlSpeak", "ControlStart"
        };
        public static string[] blockTab4ResIdStrs = {
            "GameDown", "GameJump", "GameUp",
            "GameLeft", "GameLoopN", "GameRight"
        };

        public static List<List<Block>> blocks { get; set; } = new List<List<Block>>();

        public int category { get; set; }
        public int inputState { get; set; }
        public int idx { get; set; }
        public string remotePath { get; set; }

        public string name { get; set; }
        public string text { get; set; }
        public int resourceId { get; set; }
        public int tabIndex { get; set; }
        public int row { get; set; }
        public int index { get; set; }

    }
}
