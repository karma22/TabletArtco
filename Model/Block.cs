using Android.Graphics;
using Android.Util;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace TabletArtco
{
    public partial class Block
    {
        public static List<List<Block>> _blocks = new List<List<Block>>();

        public int category;
        public int blockType;
        public int eventType;

        public string name;
        public int resourceId;
        public int tabIndex;
        public int row;
        public int index;

        public string text;
        public string varName;
        public string varValue;
        public string activateSpriteId;
        public string backgroundName;

        public bool collionSignal = false;
        public int signalCount = 0;
        public int clickSignalCount = 0;

        public Block() { }
        public Block(Block block)
        {
            category = block.category;
            blockType = block.blockType;
            eventType = block.eventType;

            name = block.name;
            resourceId = block.resourceId;
            tabIndex = block.tabIndex;            
            index = block.index;     
        }

        public static Block Copy(Block block)
        {
            Block b = new Block();
            b.category = block.category;
            b.blockType = block.blockType;
            b.eventType = block.eventType;

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

        public static Dictionary<string, MethodInfo> _funcs = new Dictionary<string, MethodInfo>();

        public static Block GetBlockByName(string name)
        {
            foreach (var tab in _blocks)
            {
                var ret = tab.Find(item => item.name.Equals(name));
                if (ret != null)
                    return ret;
            }

            return null;
        }

        public static void SetBlockFunctions()
        {
            for (int i = 0; i < _blocks.Count; i++)
            {
                for (int j = 0; j < _blocks[i].Count; j++)
                {
                    var block = _blocks[i][j];
                    if (_funcs.ContainsKey(block.name))
                        continue;

                    _funcs.Add(block.name, block.GetType().GetMethod(block.name));
                }
            }
        }

        public static double GetConstantOrVariable(ActivatedSprite s, int lineNumber)
        {
            var block = s.mBlocks[lineNumber][s.programCnt[lineNumber]];
            if (block.varName != null)
            {
                if (Variable.curVariableMap.ContainsKey(block.varName))
                {
                    if (double.TryParse(Variable.curVariableMap[block.varName], out double value))
                    {
                        return value;
                    }
                }
            }
            else
            {
                if (double.TryParse(block.text, out double value))
                {
                    return value;
                }
            }

            return 0;
        }

        public static string GetVarValueByBlock(ActivatedSprite s, int lineNumber) => s.mBlocks[lineNumber][s.programCnt[lineNumber]].varValue;
        public static string GetTextByBlock(ActivatedSprite s, int lineNumber) => s.mBlocks[lineNumber][s.programCnt[lineNumber]].text;

        public void EventStart(ActivatedSprite s, int lineNumber) { }
        public void EventRecvSig(ActivatedSprite s, int lineNumber) => WaitForSignal(s, lineNumber);
        public void EventTouch(ActivatedSprite s, int lineNumber) => WaitForCollision(s, lineNumber);
        public void EventClickSprite(ActivatedSprite s, int lineNumber) => WaitForTouch(s, lineNumber);

        // Move
        public void MoveRight1(ActivatedSprite s, int lineNumbers) => MoveConstant(s, MoveArrow.Right, 1, true);
        public void MoveRight5(ActivatedSprite s, int lineNumber) => MoveConstant(s, MoveArrow.Right, 5, true);
        public void MoveRight10(ActivatedSprite s, int lineNumber) => MoveConstant(s, MoveArrow.Right, 10, true);
        public void MoveDown1(ActivatedSprite s, int lineNumber) => MoveConstant(s, MoveArrow.Down, 1, true);
        public void MoveDown5(ActivatedSprite s, int lineNumber) => MoveConstant(s, MoveArrow.Down, 5, true);
        public void MoveDown10(ActivatedSprite s, int lineNumber) => MoveConstant(s, MoveArrow.Down, 10, true);
        public void MoveLeft1(ActivatedSprite s, int lineNumber) => MoveConstant(s, MoveArrow.Left, 1, true);
        public void MoveLeft5(ActivatedSprite s, int lineNumber) => MoveConstant(s, MoveArrow.Left, 5, true);
        public void MoveLeft10(ActivatedSprite s, int lineNumber) => MoveConstant(s, MoveArrow.Left, 10, true);
        public void MoveUp1(ActivatedSprite s, int lineNumber) => MoveConstant(s, MoveArrow.Up, 1, true);
        public void MoveUp5(ActivatedSprite s, int lineNumber) => MoveConstant(s, MoveArrow.Up, 5, true);
        public void MoveUp10(ActivatedSprite s, int lineNumber) => MoveConstant(s, MoveArrow.Up, 10, true);
        public void MoveRDownN(ActivatedSprite s, int lineNumber) => MoveConstant(s, MoveArrow.RightDown, (int)GetConstantOrVariable(s, lineNumber), false);
        public void MoveRUpN(ActivatedSprite s, int lineNumber) => MoveConstant(s, MoveArrow.RightUp, (int)GetConstantOrVariable(s, lineNumber), false);
        public void MoveLDownN(ActivatedSprite s, int lineNumber) => MoveConstant(s, MoveArrow.LeftDown, (int)GetConstantOrVariable(s, lineNumber), false);
        public void MoveLUpN(ActivatedSprite s, int lineNumber) => MoveConstant(s, MoveArrow.LeftUp, (int)GetConstantOrVariable(s, lineNumber), false);
        public void MoveRightN(ActivatedSprite s, int lineNumber) => MoveConstant(s, MoveArrow.Right, (int)GetConstantOrVariable(s, lineNumber), false);
        public void MoveDownN(ActivatedSprite s, int lineNumber) => MoveConstant(s, MoveArrow.Down, (int)GetConstantOrVariable(s, lineNumber), false);
        public void MoveLeftN(ActivatedSprite s, int lineNumber) => MoveConstant(s, MoveArrow.Left, (int)GetConstantOrVariable(s, lineNumber), false);
        public void MoveUpN(ActivatedSprite s, int lineNumber) => MoveConstant(s, MoveArrow.Up, (int)GetConstantOrVariable(s, lineNumber), false);
        public void MoveEmpty(ActivatedSprite s, int lineNumber) { }

        // Action
        public void ActionSlow(ActivatedSprite s, int lineNumber) => LeftRightMove(s, 300, 0);
        public void ActionSlowN(ActivatedSprite s, int lineNumber) => LeftRightMove(s, 300, (int)GetConstantOrVariable(s, lineNumber));
        public void ActionFast(ActivatedSprite s, int lineNumber) => LeftRightMove(s, 100, 0);
        public void ActionFastN(ActivatedSprite s, int lineNumber) => LeftRightMove(s, 100, (int)GetConstantOrVariable(s, lineNumber));
        public void ActionFlash(ActivatedSprite s, int lineNumber) => FlashSprite(s, 0);
        public void ActionFlashN(ActivatedSprite s, int lineNumber) => FlashSprite(s, (int)GetConstantOrVariable(s, lineNumber));
        public void ActionRRotate(ActivatedSprite s, int lineNumber) => RotateLoopN(s, 90, 0, true, 1);
        public void ActionRRotateN(ActivatedSprite s, int lineNumber) => RotateLoopN(s, (int)GetConstantOrVariable(s, lineNumber), 0, true, 1);
        public void ActionLRotate(ActivatedSprite s, int lineNumber) => RotateLoopN(s, 90, 0, false, 1);
        public void ActionLRotateN(ActivatedSprite s, int lineNumber) => RotateLoopN(s, (int)GetConstantOrVariable(s, lineNumber), 0, false, 1);
        public void ActionRotateLoop(ActivatedSprite s, int lineNumber) => RotateLoopN(s, 10, 0, true, 0);
        public void ActionRotateLoopN(ActivatedSprite s, int lineNumber) => RotateLoopN(s, 10, (int)GetConstantOrVariable(s, lineNumber), true, 0);
        public void ActionWave(ActivatedSprite s, int lineNumber) => LeftRightWaveMove(s, 0, 4);
        public void ActionWaveN(ActivatedSprite s, int lineNumber) => LeftRightWaveMove(s, (int)GetConstantOrVariable(s, lineNumber), 4);
        public void ActionTWave(ActivatedSprite s, int lineNumber) => UpDownWaveMove(s, 0, 3);
        public void ActionTWaveN(ActivatedSprite s, int lineNumber) => UpDownWaveMove(s, (int)GetConstantOrVariable(s, lineNumber), 3);
        public void ActionRandomMove(ActivatedSprite s, int lineNumber) => RandomMove(s);
        public void ActionZigzag(ActivatedSprite s, int lineNumber) => LeftRightWaveMove(s, 0, 1);
        public void ActionZigzagN(ActivatedSprite s, int lineNumber) => LeftRightWaveMove(s, (int)GetConstantOrVariable(s, lineNumber), 1);
        public void ActionTZigzag(ActivatedSprite s, int lineNumber) => UpDownWaveMove(s, 0, 1);
        public void ActionTZigzagN(ActivatedSprite s, int lineNumber) => UpDownWaveMove(s, (int)GetConstantOrVariable(s, lineNumber), 1);
        public void ActionBounce(ActivatedSprite s, int lineNumber) => BounceSprite(s);
        public void ActionJump(ActivatedSprite s, int lineNumber) => JumpSprite(s, 0);
        public void ActionJumpN(ActivatedSprite s, int lineNumber) => JumpSprite(s, (int)GetConstantOrVariable(s, lineNumber));
        public void ActionRLJump(ActivatedSprite s, int lineNumber) => RightLeftJumpLoop(s, 0);
        public void ActionRLJumpN(ActivatedSprite s, int lineNumber) => RightLeftJumpLoop(s, (int)GetConstantOrVariable(s, lineNumber));
        public void ActionAnimate(ActivatedSprite s, int lineNumber) => AnimateSpritesLoop(s, 0);
        public void ActionAnimateN(ActivatedSprite s, int lineNumber) => AnimateSpritesLoop(s, (int)GetConstantOrVariable(s, lineNumber));

        // Control
        public void ControlTime1(ActivatedSprite s, int lineNumber) => SleepVariable(1);
        public void ControlTime2(ActivatedSprite s, int lineNumber) => SleepVariable(2);
        public void ControlTimeN(ActivatedSprite s, int lineNumber) => SleepVariable((int)GetConstantOrVariable(s, lineNumber));
        public void ControlLoopN(ActivatedSprite s, int lineNumber) => PushLoopStack(s, lineNumber);
        public void ControlLoop(ActivatedSprite s, int lineNumber) => InitLoopVariables(s, lineNumber);
        public void ControlFlag(ActivatedSprite s, int lineNumber) => PopLoopStack(s, lineNumber);
        public void ControlFlipX(ActivatedSprite s, int lineNumber) => FlipXYSprite(s, true);
        public void ControlFlipY(ActivatedSprite s, int lineNumber) => FlipXYSprite(s, false);
        public void ControlNextSprite(ActivatedSprite s, int lineNumber) => SetNextBit(s);
        public void ControlShow(ActivatedSprite s, int lineNumber) => ShowSprite(s, true);
        public void ControlHide(ActivatedSprite s, int lineNumber) => ShowSprite(s, false);
        public void ControlSound(ActivatedSprite s, int lineNumber) => SoundPlayer(s, GetVarValueByBlock(s, lineNumber));
        public void ControlSpeak(ActivatedSprite s, int lineNumber) => SpeakText(s, GetTextByBlock(s, lineNumber));
        public void ControlSpeakStop(ActivatedSprite s, int lineNumber) => SpeakText(s, null);
        public void ControlChangeBack(ActivatedSprite s, int lineNumber) => ChangeBackground(s, lineNumber);
        public void ControlSendSig(ActivatedSprite s, int lineNumber) => BroadCastWithKey(s, GetTextByBlock(s, lineNumber));
        public void ControlSendSigWait(ActivatedSprite s, int lineNumber) => BroadCastWithKeyWait(s, lineNumber);
        public void ControlMoveXY(ActivatedSprite s, int lineNumber) => MoveXY(s, lineNumber);
        public void ControlSetVal(ActivatedSprite s, int lineNumber) => SetValue(s, lineNumber);
        public void ControlChangeVal(ActivatedSprite s, int lineNumber) => ChangeValue(s, lineNumber);
        public void ControlStop(ActivatedSprite s, int lineNumber) => s.stopThisSprite = true;

        // Practice        
        public void GameLoopN(ActivatedSprite s, int lineNumber) => ControlLoopN(s, lineNumber);
        public void GameFlag(ActivatedSprite s, int lineNumber) => ControlFlag(s, lineNumber);
        public void GameRight(ActivatedSprite s, int lineNumber) => TurnAndMoveForward(s, MoveArrow.Right);
        public void GameDown(ActivatedSprite s, int lineNumber) => TurnAndMoveForward(s, MoveArrow.Down);
        public void GameLeft(ActivatedSprite s, int lineNumber) => TurnAndMoveForward(s, MoveArrow.Left);
        public void GameUp(ActivatedSprite s, int lineNumber) => TurnAndMoveForward(s, MoveArrow.Up);
        public void GameJump(ActivatedSprite s, int lineNumber) => ArrowJump(s);

        // Implementation                
        public void WaitForSignal(ActivatedSprite s, int lineNumber)
        {
            var block = s.mBlocks[lineNumber][s.programCnt[lineNumber]];
            if (block.signalCount == 0)            
                s.programCnt[lineNumber] = -1;            
            else            
                block.signalCount--;            
        }

        public void WaitForCollision(ActivatedSprite s, int lineNumber)
        {
            var block = s.mBlocks[lineNumber][s.programCnt[lineNumber]];
            if (!block.collionSignal)            
                s.programCnt[lineNumber] = -1;            
            else            
                block.collionSignal = false;            
        }

        public void WaitForTouch(ActivatedSprite s, int lineNumber)
        {
            var block = s.mBlocks[lineNumber][s.programCnt[lineNumber]];
            if (block.clickSignalCount == 0)            
                s.programCnt[lineNumber] = -1;            
            else            
                block.clickSignalCount--;            
        }

        public void BroadCastWithKey(ActivatedSprite s, string text)
        {
            if (text == null)
                return;

            Signal.SendSignal(text);
        }

        public void BroadCastWithKeyWait(ActivatedSprite s, int lineNumber)
        {
            var block = s.mBlocks[lineNumber][s.programCnt[lineNumber]];
            if (block.text == null || Project.sendSignalWait.ContainsKey(block.text))
                return;

            Project.sendSignalWait.Add(block.text, 0);
            Signal.SendSignal(block.text);

            int num = 0;
            for (int i = 0; i < Project.mSprites.Count; i++)
            {
                for (int j = 0; j < Project.mSprites[i].mBlocks.Count; j++)
                {
                    Block temp = Project.mSprites[i].mBlocks[j][0];
                    if (temp.name.Equals("EventRecvSig") && temp.text != null)
                    {
                        if (temp.text.Equals(block.text))
                        {
                            num++;
                        }
                    }
                }
            }

            while (ActivatedSprite.isAnimationTag && !s.stopThisSprite)
            {
                Thread.Sleep(10);
                if (Project.sendSignalWait.ContainsKey(block.text))
                {
                    if (Project.sendSignalWait[block.text] == num)
                        break;
                }
            }
        }

        public void SetValue(ActivatedSprite s, int lineNumber)
        {
            var block = s.mBlocks[lineNumber][s.programCnt[lineNumber]];
            if (block.varName != null && Variable.curVariableMap.ContainsKey(block.varName))
            {
                Variable.curVariableMap[block.varName] = int.Parse(block.varValue) + "";
            }
        }

        public void ChangeValue(ActivatedSprite s, int lineNumber)
        {
            var block = s.mBlocks[lineNumber][s.programCnt[lineNumber]];
            if (block.varName != null && Variable.curVariableMap.ContainsKey(block.varName))
            {
                Variable.curVariableMap[block.varName] = int.Parse(Variable.curVariableMap[block.varName]) + int.Parse(block.varValue) + "";
            }
        }

        public void RotateLoopN(ActivatedSprite s, float angle, int repeat, bool cw, int n)
        {
            int i = 0;
            while (ActivatedSprite.isAnimationTag && !s.stopThisSprite)
            {
                if (repeat > 0 && i == repeat * (int)(360 / angle))
                    break;

                angle = cw ? angle : 360.0f - angle;
                s.curAngle += angle;
                if (s.curAngle >= 360.0f)
                    s.curAngle -= ((int)(s.curAngle / 360.0f)) * 360;

                rotateBitmap(s, s.curAngle);
                s.InvalidateStage();

                i = (i > 1000) ? 1 : i + 1;

                if (i == n)
                    break;

                Java.Lang.Thread.Sleep(250);
            }
        }

        private void rotateBitmap(ActivatedSprite s, float angle)
        {
            for (int i = 0; i < s.curbitmapList.Count; i++)
            {
                Bitmap origin = s.originBitmapList[i];
                if (origin == null)
                {
                    return;
                }
                int width = origin.Width;
                int height = origin.Height;
                Matrix matrix = new Matrix();
                matrix.SetRotate(angle);

                Bitmap curBitmap = s.curbitmapList[i];
                Point center = new Point(s.curPoint.X + curBitmap.Width / 2, s.curPoint.Y + curBitmap.Height / 2);
                Bitmap newBM = Bitmap.CreateBitmap(origin, 0, 0, width, height, matrix, false);
                
                if(s.curIndex == i)
                {
                    s.curSize = new Size(newBM.Width, newBM.Height);
                    s.curPoint = new Point(center.X - newBM.Width / 2, center.Y - newBM.Height / 2);
                }

                s.curbitmapList[i] = newBM;
                curBitmap.Recycle();
            }
        }

        public void SleepVariable(int value)
        {
            if (value < 0)
                return;

            Thread.Sleep(value * 1000);
        }

        public void PushLoopStack(ActivatedSprite s, int lineNumber)
        {
            s.loopStack.Push(lineNumber, s.programCnt[lineNumber], (int)GetConstantOrVariable(s, lineNumber));
        }

        public void PopLoopStack(ActivatedSprite s, int lineNumber)
        {
            int nextPC = s.loopStack.Pop(lineNumber);
            if (nextPC != 0)
                s.programCnt[lineNumber] = nextPC;
        }

        public void InitLoopVariables(ActivatedSprite s, int lineNumber)
        {
            s.loopStack.Push(lineNumber, s.programCnt[lineNumber], 0);
        }

        public void FlashSprite(ActivatedSprite s, int repeat)
        {
            bool flag = true;

            for (int i = (repeat > 0) ? 0 : -1; i < (repeat * 2) && ActivatedSprite.isAnimationTag && !s.stopThisSprite; i = (repeat > 0) ? i + 1 : i)
            {
                flag ^= true;
                ShowSprite(s, flag);
                Thread.Sleep(500);
            }
        }

        public void LeftRightMove(ActivatedSprite s, int delay, int repeat)
        {
            if (s.curMoveArrow != MoveArrow.Left && s.curMoveArrow != MoveArrow.Right)
                s.curMoveArrow = MoveArrow.Right;


            for (int i = (repeat > 0) ? 0 : -1; i < repeat && ActivatedSprite.isAnimationTag && !s.stopThisSprite;)
            {
                if ((s.curMoveArrow == MoveArrow.Right && s.curPoint.X + s.curSize.Width < s.boundSize.Width) ||
                    (s.curMoveArrow == MoveArrow.Left && s.curPoint.X > 0))
                {
                    s.curPoint.X += (s.curMoveArrow == MoveArrow.Right) ? 10 : -10;
                }
                else
                {
                    FlipXYSprite(s, false);
                    s.curMoveArrow = (s.curMoveArrow == MoveArrow.Right) ? MoveArrow.Left : MoveArrow.Right;

                    i = (repeat > 0) ? i + 1 : i;
                }

                s.InvalidateStage();
                Thread.Sleep(delay);
            }
        }

        public void LeftRightWaveMove(ActivatedSprite s, int repeat, int divider)
        {
            if (s.curMoveArrow != MoveArrow.Left && s.curMoveArrow != MoveArrow.Right)
                s.curMoveArrow = MoveArrow.Right;

            int zeroPoint = s.curPoint.Y;
            for (int i = (repeat > 0) ? 0 : -1; i < repeat && ActivatedSprite.isAnimationTag && !s.stopThisSprite;)
            {
                for (double angle = 0; angle < 360 && ActivatedSprite.isAnimationTag && !s.stopThisSprite; angle++)
                {
                    if ((s.curMoveArrow == MoveArrow.Right && s.curPoint.X + s.curSize.Width < s.boundSize.Width) ||
                        (s.curMoveArrow == MoveArrow.Left && s.curPoint.X > 0))
                    {
                        s.curPoint.Y = (int)(Java.Lang.Math.Sin(angle / divider) * 50.0) + zeroPoint;
                        s.curPoint.X += (s.curMoveArrow == MoveArrow.Right) ? 10 : -10;
                    }
                    else
                    {
                        FlipXYSprite(s, false);
                        s.curMoveArrow = (s.curMoveArrow == MoveArrow.Right) ? MoveArrow.Left : MoveArrow.Right;
                        i = (repeat > 0) ? i + 1 : i;
                        break;
                    }

                    s.InvalidateStage();
                    Thread.Sleep(100);
                }
            }
        }

        public void UpDownWaveMove(ActivatedSprite s, int repeat, int divider)
        {
            if (s.curMoveArrow != MoveArrow.Up && s.curMoveArrow != MoveArrow.Down)
                s.curMoveArrow = MoveArrow.Down;

            int zeroPoint = s.curPoint.X;
            for (int i = (repeat > 0) ? 0 : -1; i < repeat && ActivatedSprite.isAnimationTag && !s.stopThisSprite;)
            {
                for (double angle = 0; angle < 360 && ActivatedSprite.isAnimationTag && !s.stopThisSprite; angle++)
                {
                    if ((s.curMoveArrow == MoveArrow.Down && s.curPoint.Y + s.curSize.Height < s.boundSize.Height) ||
                        (s.curMoveArrow == MoveArrow.Up && s.curPoint.Y > 0))
                    {
                        s.curPoint.X = (int)(Java.Lang.Math.Sin(angle / divider) * 50.0) + zeroPoint;
                        s.curPoint.Y += (s.curMoveArrow == MoveArrow.Down) ? 10 : -10;
                    }
                    else
                    {
                        s.curMoveArrow = (s.curMoveArrow == MoveArrow.Down) ? MoveArrow.Up : MoveArrow.Down;
                        i = (repeat > 0) ? i + 1 : i;
                        break;
                    }

                    s.InvalidateStage();
                    Thread.Sleep(100);
                }
            }
        }

        public void RandomMove(ActivatedSprite s)
        {
            while (ActivatedSprite.isAnimationTag && !s.stopThisSprite)
            {
                int random = new Random(Guid.NewGuid().GetHashCode()).Next(1, 9); //1~8 move arrow
                int n = new Random(Guid.NewGuid().GetHashCode()).Next(1, 6); // 1~5 delay time
                MoveSprite(s, 10, n, (MoveArrow)random, 100);
            }
        }

        public void BounceSprite(ActivatedSprite s)
        {
            Wall wall = Wall.Default;
            while (ActivatedSprite.isAnimationTag && !s.stopThisSprite)
            {
                if (s.curMoveArrow != MoveArrow.RightDown && s.curMoveArrow != MoveArrow.LeftDown && s.curMoveArrow != MoveArrow.LeftUp)
                {
                    s.curMoveArrow = MoveArrow.RightUp;
                }
                switch (s.curMoveArrow)
                {
                    case MoveArrow.RightUp:
                        wall = RightUpMove(s, 10);
                        break;
                    case MoveArrow.RightDown:
                        wall = RightDownMove(s, 10);
                        break;
                    case MoveArrow.LeftDown:
                        wall = LeftDownMove(s, 10);
                        break;
                    case MoveArrow.LeftUp:
                        wall = LeftUpMove(s, 10);
                        break;
                    default:
                        break;
                }
                int arrowNum = (int)s.curMoveArrow;
                int wallNum = (int)wall;
                if (wall != Wall.Default)
                {
                    arrowNum = wallNum % 2 == 1 ? 5 - arrowNum : (wallNum == arrowNum ? arrowNum - 1 : arrowNum + 1);
                    s.curMoveArrow = (MoveArrow)arrowNum;
                }

                Thread.Sleep(50);
            }
        }

        public void JumpSprite(ActivatedSprite s, int repeat)
        {
            int iteration = 0;
            while (ActivatedSprite.isAnimationTag && !s.stopThisSprite)
            {
                if (repeat != 0 && iteration == repeat)
                    break;

                for (int i = 5; i > 0 && ActivatedSprite.isAnimationTag && !s.stopThisSprite; i--)
                {
                    MoveSprite(s, i * 10, 1, MoveArrow.Up, 100);
                }
                for (int i = 1; i <= 5 && ActivatedSprite.isAnimationTag && !s.stopThisSprite; i++)
                {
                    MoveSprite(s, i * 10, 1, MoveArrow.Down, 100);
                }

                iteration = (iteration < 1000) ? iteration + 1 : 0;
            }
        }

        public void RightLeftJumpLoop(ActivatedSprite s, int repeat)
        {
            if (s.curMoveArrow != MoveArrow.Left && s.curMoveArrow != MoveArrow.Right)
                s.curMoveArrow = MoveArrow.Right;

            int zeroPoint = s.curPoint.Y;
            for (int i = (repeat > 0) ? 0 : -1; i < repeat && ActivatedSprite.isAnimationTag && !s.stopThisSprite; i = (repeat > 0) ? i + 1 : i)
            {
                for (double angle = 0; angle < 180; angle += 10)
                {
                    double radian = Math.PI * angle / 180.0;
                    s.curPoint.Y = (int)(-Math.Sin(radian) * 50.0) + zeroPoint;
                    s.curPoint.X += (s.curMoveArrow == MoveArrow.Right) ? 5 : -5;

                    s.InvalidateStage();
                    Thread.Sleep(50);
                }
            }
        }

        public void MoveXY(ActivatedSprite s, int lineNumber)
        {
            int x = 0, y = 0;
            var block = s.mBlocks[lineNumber][s.programCnt[lineNumber]];

            if (block != null)
            {
                if ((block.varName != null && block.varName.Length > 0) && (block.varValue != null && block.varValue.Length > 0))
                {
                    if (int.TryParse(block.varName, out int valueX))
                    {
                        x = valueX;
                    }
                    else
                    {
                        if (Variable.curVariableMap.ContainsKey(block.varName))
                            x = int.Parse(Variable.curVariableMap[block.varName]);
                        else
                            return;
                    }

                    if (int.TryParse(block.varValue, out int valueY))
                    {
                        y = valueY;
                    }
                    else
                    {
                        if (Variable.curVariableMap.ContainsKey(block.varValue))
                            y = int.Parse(Variable.curVariableMap[block.varValue]);
                        else
                            return;
                    }
                }
            }
            s.curPoint.X = x;
            s.curPoint.Y = y;
            s.InvalidateStage();
        }

        public void AnimateSpritesLoop(ActivatedSprite s, int repeat)
        {
            int i = 0;
            while (ActivatedSprite.isAnimationTag && !s.stopThisSprite)
            {
                if (repeat != 0 && i == repeat)
                    break;

                SetNextBit(s);
                Thread.Sleep(1000);

                i++;
            }
        }

        public void SetNextBit(ActivatedSprite s)
        {
            int maxNum = s.curbitmapList.Count - 1;
            if (s.curIndex < maxNum)
            {
                s.curIndex++;
            }
            else
            {
                s.curIndex = 0;
            }
            s.curbitmapList[s.curIndex] = s.GetNextSpriteBit();
            s.InvalidateStage();
        }

        public void ShowSprite(ActivatedSprite s, bool isVisible)
        {
            s.isVisible = isVisible;
            s.InvalidateStage();
        }

        public void SoundPlayer(ActivatedSprite s, string text)
        {
            ActivatedSprite.SoundAction?.Invoke(text);
            Thread.Sleep(2000);
        }

        public void SpeakText(ActivatedSprite s, string text)
        {
            s.speakText = text;
            s.InvalidateStage();
        }

        public void ChangeBackground(ActivatedSprite s, int lineNumber)
        {
            string backName = s.mBlocks[lineNumber][s.programCnt[lineNumber]].backgroundName;
            if (backName == null)
                return;

            ActivatedSprite.mUpdateDelegate?.UpdateBackground(backName);
        }

        public void FlipXYSprite(ActivatedSprite s, bool isFlipX)
        {
            Matrix matrix = new Matrix();

            int sx = (isFlipX) ? 1 : -1;
            int sy = (isFlipX) ? -1 : 1;

            matrix.SetScale(sx, sy);
            for (int i = 0; i < s.curbitmapList.Count; i++)
            {
                Bitmap bitmap = s.curbitmapList[i];
                Bitmap newBM = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, false);
                s.curbitmapList[i] = newBM;
            }
            s.InvalidateStage();
        }

        public void MoveConstant(ActivatedSprite s, MoveArrow arrow, int n, bool isDelay)
        {
            s.curMoveArrow = arrow;
            MoveSprite(s, n * 10, 1, s.curMoveArrow);
            if (isDelay)
                Thread.Sleep(1000);
        }

        public void MoveSprite(ActivatedSprite s, int value, int n, MoveArrow moveArrow, int delay = 1)
        {
            var actions = new List<Func<ActivatedSprite, int, Wall>>()
            {
                RightUpMove, RightDownMove, LeftDownMove, LeftUpMove,
                RightMove, DownMove, LeftMove, UpMove
            };

            for (int i = 0; i < n; i++)
            {
                actions[(int)moveArrow - 1].Invoke(s, value);
                Thread.Sleep(delay);
            }
        }

        public void ArrowJump(ActivatedSprite s)
        {
            s.curIndex = 5;
            int delay = 5;
            int zero = s.curPoint.Y;
            int hdis = (int)(s.boundSize.Width * 99.0 / 1000 * 2);
            int originX = s.curPoint.X;
            for (double i = 0; i <= 180; i++)
            {
                double radian = Math.PI * i / 180.0;
                s.curPoint.Y = (int)(-Java.Lang.Math.Sin(radian) * 100.0) + zero;
                s.curPoint.X = originX + (int)(i / 180 * hdis);
                if (i % 10 == 0 && i != 0 && i < 91)
                {
                    delay++;
                }
                else if (i % 10 == 0 && i != 0 && i > 90)
                {
                    delay--;
                }
                Thread.Sleep(delay);
                s.InvalidateStage();
            }
            Thread.Sleep(500);
        }

        public void TurnAndMoveForward(ActivatedSprite s, MoveArrow arrow)
        {            
            switch (arrow)
            {
                case MoveArrow.Right:
                    {
                        s.curIndex = 0;
                        break;
                    }
                case MoveArrow.Left:
                    {
                        s.curIndex = 1;
                        break;
                    }
                case MoveArrow.Down:
                    {
                        s.curIndex = 2;
                        break;
                    }
                case MoveArrow.Up:
                    {
                        s.curIndex = 3;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            int hdis = (int)(s.boundSize.Width * 99.0 / 1000);
            int vdis = (int)(s.boundSize.Height * 100.0 / 548);
            s.InvalidateStage();
            Thread.Sleep(500);
            switch (arrow)
            {
                case MoveArrow.Right:
                    {
                        RightMove(s, hdis);
                        break;
                    }
                case MoveArrow.Left:
                    {
                        LeftMove(s, hdis);
                        break;
                    }
                case MoveArrow.Down:
                    {
                        DownMove(s, vdis);
                        break;
                    }
                case MoveArrow.Up:
                    {
                        UpMove(s, vdis);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            s.InvalidateStage();
            Thread.Sleep(500);
        }

        public Wall DownMove(ActivatedSprite s, int n)
        {
            if (s.curPoint.Y + n + s.curSize.Height < s.boundSize.Height)
                s.curPoint.Y += n;

            s.InvalidateStage();
            return Wall.Default;
        }

        public Wall LeftMove(ActivatedSprite s, int n)
        {
            if (s.curPoint.X - n > 0)
                s.curPoint.X -= n;

            s.InvalidateStage();
            return Wall.Default;
        }

        public Wall RightMove(ActivatedSprite s, int n)
        {
            if (s.curPoint.X + n + s.curSize.Width < s.boundSize.Width)
                s.curPoint.X += n;

            s.InvalidateStage();
            return Wall.Default;
        }

        public Wall UpMove(ActivatedSprite s, int n)
        {
            if (s.curPoint.Y - n > 0)
                s.curPoint.Y -= n;

            s.InvalidateStage();
            return Wall.Default;
        }

        public Wall LeftUpMove(ActivatedSprite s, int n)
        {
            s.InvalidateStage();

            if (s.curPoint.X - n > 0)
            {
                if (s.curPoint.Y - n > 0)
                {
                    s.curPoint.X -= n;
                    s.curPoint.Y -= n;

                    s.InvalidateStage();
                    return Wall.Default;
                }
                else
                {
                    return Wall.UpWall;
                }
            }
            else
            {
                return Wall.LeftWall;
            }
        }

        public Wall RightUpMove(ActivatedSprite s, int n)
        {
            if (s.curPoint.X + s.curSize.Width + n < s.boundSize.Width)
            {
                if (s.curPoint.Y - n > 0)
                {
                    s.curPoint.X += n;
                    s.curPoint.Y -= n;
                    s.InvalidateStage();
                    return Wall.Default;
                }
                else
                {
                    return Wall.UpWall;
                }
            }
            else
            {
                return Wall.RightWall;
            }
        }

        public Wall LeftDownMove(ActivatedSprite s, int n)
        {
            if (s.curPoint.X - n > 0)
            {
                if (s.curPoint.Y + s.curSize.Height + n < s.boundSize.Height)
                {
                    s.curPoint.X -= n;
                    s.curPoint.Y += n;

                    s.InvalidateStage();
                    return Wall.Default;
                }
                else
                {
                    return Wall.DownWall;
                }
            }
            else
            {
                return Wall.LeftWall;
            }
        }

        public Wall RightDownMove(ActivatedSprite s, int n)
        {
            if (s.curPoint.X + s.curSize.Width + n < s.boundSize.Width)
            {
                if (s.curPoint.Y + s.curSize.Height + n < s.boundSize.Height)
                {
                    s.curPoint.X += n;
                    s.curPoint.Y += n;
                    s.InvalidateStage();
                    return Wall.Default;
                }
                else
                {
                    return Wall.DownWall;
                }
            }
            else
            {
                return Wall.RightWall;
            }
        }
    }
}
