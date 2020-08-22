using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace TabletArtco
{
    public static class BlockResources
    {
        public static Dictionary<string, int> _blockImages = new Dictionary<string, int>()
        {
            { "MoveDown1", Resource.Drawable.Moveblock_Down1 },
            { "MoveDown5", Resource.Drawable.Moveblock_Down5 },
            { "MoveDown10", Resource.Drawable.Moveblock_Down10 },
            { "MoveLeft1", Resource.Drawable.Moveblock_Left1 },
            { "MoveLeft5", Resource.Drawable.Moveblock_Left5 },
            { "MoveLeft10", Resource.Drawable.Moveblock_Left10 },
            { "MoveRight1", Resource.Drawable.Moveblock_Right1 },
            { "MoveRight5", Resource.Drawable.Moveblock_Right5 },
            { "MoveRight10", Resource.Drawable.Moveblock_Right10 },
            { "MoveUp1", Resource.Drawable.Moveblock_Up1 },
            { "MoveUp5", Resource.Drawable.Moveblock_Up5 },
            { "MoveUp10", Resource.Drawable.Moveblock_Up10 },
            { "ActionSlow", Resource.Drawable.Actblock_Slow },
            { "ActionFast", Resource.Drawable.Actblock_Fast },
            { "ActionFlash", Resource.Drawable.Actblock_Flash },
            { "ActionRRotate", Resource.Drawable.Actblock_RRotate },
            { "ActionLRotate", Resource.Drawable.Actblock_LRotate },
            { "ActionRotateLoop", Resource.Drawable.Actblock_RotateLoop },
            { "ActionWave", Resource.Drawable.Actblock_Wave },
            { "ActionTWave", Resource.Drawable.Actblock_TWave },
            { "ActionRandomMove", Resource.Drawable.Actblock_RandomMove },
            { "ActionZigzag", Resource.Drawable.Actblock_Zigzag },
            { "ActionTZigzag", Resource.Drawable.Actblock_TZigzag },
            { "ActionBounce", Resource.Drawable.Actblock_Bounce },
            { "ActionJump", Resource.Drawable.Actblock_Jump },
            { "ActionRLJump", Resource.Drawable.Actblock_RLJump },
            { "ActionAnimate", Resource.Drawable.Actblock_Animate},
            { "MoveLUpN", Resource.Drawable.Moveblock_LUpN },
            { "MoveUpN", Resource.Drawable.Moveblock_UpN },
            { "MoveRUpN", Resource.Drawable.Moveblock_RUpN },
            { "MoveLeftN", Resource.Drawable.Moveblock_LeftN },
            { "MoveEmpty", Resource.Drawable.Moveblock_Empty },
            { "MoveRightN", Resource.Drawable.Moveblock_RightN },
            { "MoveLDownN", Resource.Drawable.Moveblock_LDownN},
            { "MoveDownN", Resource.Drawable.Moveblock_DownN },
            { "MoveRDownN", Resource.Drawable.Moveblock_RDownN },
            { "ActionSlowN", Resource.Drawable.Actblock_SlowN },
            { "ActionFastN", Resource.Drawable.Actblock_FastN },
            { "ActionFlashN", Resource.Drawable.Actblock_FlashN },
            { "ActionRRotateN", Resource.Drawable.Actblock_RRotateN },
            { "ActionLRotateN", Resource.Drawable.Actblock_LRotateN },
            { "ActionRotateLoopN", Resource.Drawable.Actblock_RotateLoopN },
            { "ActionWaveN", Resource.Drawable.Actblock_WaveN },
            { "ActionTWaveN", Resource.Drawable.Actblock_TWaveN },
            { "ActionZigzagN", Resource.Drawable.Actblock_ZigzagN },
            { "ActionTZigzagN", Resource.Drawable.Actblock_TZigzagN },
            { "ActionJumpN", Resource.Drawable.Actblock_JumpN },
            { "ActionRLJumpN", Resource.Drawable.Actblock_RLJumpN},
            { "ActionAnimateN", Resource.Drawable.Actblock_AnimateN },
            { "ControlTime1", Resource.Drawable.Contblock_Time1 },
            { "ControlTime2", Resource.Drawable.Contblock_Time2},
            { "ControlTimeN", Resource.Drawable.Contblock_TimeN},
            { "ControlLoopN", Resource.Drawable.Contblock_LoopN },
            { "ControlLoop", Resource.Drawable.Contblock_loop },
            { "ControlFlag", Resource.Drawable.Contblock_Flag},
            { "ControlFlipX", Resource.Drawable.Contblock_FlipX },
            { "ControlFlipY", Resource.Drawable.Contblock_FlipY },
            { "ControlNextSprite", Resource.Drawable.Contblock_NextSprite },
            { "ControlShow", Resource.Drawable.Contblock_Show },
            { "ControlHide", Resource.Drawable.Contblock_Hide },
            { "ControlSound", Resource.Drawable.Contblock_Sound},
            { "ControlSpeak", Resource.Drawable.Contblock_Speak },
            { "ControlSpeakStop", Resource.Drawable.Contblock_SpeakStop },
            { "ControlChangeBack", Resource.Drawable.Contblock_AdditionBackground },
            { "ControlSendSig", Resource.Drawable.Contblock_SendSignal },
            { "ControlSendSigWait", Resource.Drawable.Contblock_SendSigWait },
            { "ControlMoveXY",Resource.Drawable.Contblock_XY},
            { "ControlChangeVal", Resource.Drawable.Contblock_ChangeVal },
            { "ControlSetVal", Resource.Drawable.Contblock_SetVal },
            { "ControlStop", Resource.Drawable.Contblock_Stop },
            { "ControlClone", Resource.Drawable.Contblock_Clone },
            { "ControlCondition", Resource.Drawable.Contblock_If },
            { "GameDown", Resource.Drawable.Edublock_Down },
            { "GameJump", Resource.Drawable.Edublock_Jump },
            { "GameUp", Resource.Drawable.Edublock_Up },
            { "GameLeft", Resource.Drawable.Edublock_Left },
            { "GameLoopN", Resource.Drawable.Edublock_LoopN },
            { "GameRight",  Resource.Drawable.Edublock_Right},
            { "GameFlag", Resource.Drawable.Gameblock_Flag},
            { "EventStart",Resource.Drawable.Eventblock_Start },
            { "EventRecvSig", Resource.Drawable.Eventblock_RecvSignal},
            { "EventTouch", Resource.Drawable.Eventblock_Touch },
            { "EventClickSprite",  Resource.Drawable.Eventblock_ClickSprite },
            { "EventClone", Resource.Drawable.Eventblock_Clone },
        };

        public static List<Dictionary<string, string>> TextViewLocations(Block block)
        {
            List<string> oneList = new List<string>
            {
                "MoveLUpN", "MoveUpN", "MoveRUpN",
                "MoveLeftN", "MoveRightN",
                "MoveLDownN", "MoveDownN", "MoveRDownN",
                "ActionSlowN", "ActionFastN", "ActionFlashN",
                "ActionRRotateN", "ActionLRotateN", "ActionRotateLoopN",
                "ActionWaveN", "ActionTWaveN",
                "ActionZigzagN", "ActionTZigzagN",
                "ActionJumpN", "ActionRLJumpN", "ActionAnimateN",
                "ControlTimeN", "ControlLoopN",
                "GameLoopN"
            };
            if (oneList.Contains(block.name))
            {
                if (!string.IsNullOrEmpty(block.text) || !string.IsNullOrEmpty(block.varName))
                {
                    List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("x", 12 / 70.0 + "");
                    dic.Add("y", 44 / 70.0 + "");
                    dic.Add("w", 47 / 70.0 + "");
                    dic.Add("h", 15 / 70.0 + "");
                    dic.Add("text", string.IsNullOrEmpty(block.text) ? block.varName : block.text);
                    list.Add(dic);
                    return list;
                }
            }
            List<string> twoList = new List<string>
            {
                "EventRecvSig", "ControlSound", "ControlChangeBack", "ControlSendSig", "ControlSpeak", "ControlSendSigWait"
            };
            if (twoList.Contains(block.name))
            {
                if (!string.IsNullOrEmpty(block.text))
                {
                    List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("x", 12 / 70.0 + "");
                    dic.Add("y", 44 / 70.0 + "");
                    dic.Add("w", 47 / 70.0 + "");
                    dic.Add("h", 14 / 70.0 + "");
                    dic.Add("text", block.text);
                    list.Add(dic);
                    return list;
                }
            }
            List<string> threeList = new List<string> { "EventTouch", "ControlCondition" };
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
            List<string> FourList = new List<string> { "ControlChangeVal", "ControlSetVal" };
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
            List<string> FiveList = new List<string> { "ControlMoveXY" };
            if (FiveList.Contains(block.name))
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
                    dic1.Add("x", 27 / 100.0f + "");
                    dic1.Add("y", 58 / 100.0f + "");
                    dic1.Add("w", 63 / 100.0f + "");
                    dic1.Add("h", 24 / 100.0f + "");
                    dic1.Add("text", block.varValue);
                    list.Add(dic);
                    list.Add(dic1);
                    return list;
                }
            }
            return null;
        }

        public static int GetClickType(Block block)
        {
            List<List<string>> list = new List<List<string>> {
                new List<string>
                {
                    "MoveLUpN", "MoveUpN", "MoveRUpN",
                    "MoveLeftN", "MoveRightN",
                    "MoveLDownN", "MoveDownN", "MoveRDownN",
                    "ActionSlowN", "ActionFastN", "ActionFlashN",
                    "ActionRRotateN", "ActionLRotateN", "ActionRotateLoopN",
                    "ActionWaveN", "ActionTWaveN",
                    "ActionZigzagN", "ActionTZigzagN",
                    "ActionJumpN", "ActionRLJumpN", "ActionAnimateN",
                    "ControlTimeN", "ControlLoopN"
                },
                new List<string>{ "ControlChangeVal"},
                new List<string>{ "ControlSetVal"},
                new List<string>{ "EventRecvSig"},
                new List<string>{ "ControlSendSig", "ControlSendSigWait"},
                new List<string>{ "EventTouch"},
                new List<string>{ "ControlSpeak" },
                new List<string>{ "ControlSound" },
                new List<string>{ "ControlChangeBack" },
                new List<string>{ "EventStart" },
                new List<string>{ "ControlMoveXY" }
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
    }
}