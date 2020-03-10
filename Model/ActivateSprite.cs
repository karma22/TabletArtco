﻿
using System.Collections.Generic;
using Android.Graphics;
using Android.Util;
using System.Threading;

namespace TabletArtco
{
    enum MoveArrow { Default, RightUp, RightDown, LeftDown, LeftUp, Right, Down, Left, Up };
    enum Wall { Default, RightWall, DownWall, LeftWall, UpWall };
    class ActivatedSprite
    {
        private static string Tag = "ActivatedSprite";

        public static Size notFullSize { get; set; }
        public static Size fullSize { get; set; }
        public static bool mIsFull { get; set; }
        public static bool isAnimationTag {get; set;}

        public string activateSpriteId { get; set; } = Java.Lang.JavaSystem.CurrentTimeMillis() + "";
        public Sprite sprite { get; set; }
        public List<List<Block>> mBlocks { get; set; } = new List<List<Block>>();
        public List<Bitmap> originBitmapList { get; set; } = new List<Bitmap>();
        public Point originPoint { get; set; } = new Point(0, 0);

        // animation arguments
        public bool isVisible { get; set; } = true;
        public Point curPoint { get; set; } = new Point(0, 0);
        public Size curSize { get; set; } = new Size(0, 0);
        public string speakText { get; set; } = "";
        public Size boundSize
        {
            get
            {
                return mIsFull ? fullSize : notFullSize;
            }
        }

        public int curRow { get; set; } = 0;
        public int curIndex { get; set; } = 0;
        public List<Bitmap> curbitmapList { get; set; } = new List<Bitmap>();
        public float curAngle { get; set; } = 0;
        public MoveArrow curMoveArrow { get; set; } = MoveArrow.Default;
        public Wall curWall { get; set; } = Wall.Default;
        
        public int rowMaxCount = 10;
        List<Java.Lang.Thread> BlockThreadList { get; set; } = new List<Java.Lang.Thread>();

        public static UpdateDelegate mUpdateDelegate { get; set; }
        public static System.Action<string> SoundAction { get; set; }


        public ActivatedSprite(Sprite s, bool isClone = false)
        {
            sprite  = Sprite.ToSprite(s.ToString());
            sprite.bitmap = s.bitmap;
            if (isClone)
            {
                sprite.name = sprite.name+"1";
            }
            
            curbitmapList.Add(Bitmap.CreateBitmap(sprite.bitmap));
            originBitmapList.Add(Bitmap.CreateBitmap(sprite.bitmap));
            int width = sprite.bitmap.Width;
            int height = sprite.bitmap.Height;
            int x = (int)(1 + Java.Lang.Math.Random() * (notFullSize.Width - width + 1));
            int y = (int)(1 + Java.Lang.Math.Random() * (notFullSize.Height - height + 1));
            curPoint = new Point(Java.Lang.Math.Abs(x), Java.Lang.Math.Abs(y));
            originPoint.X = curPoint.X;
            originPoint.Y = curPoint.Y;
            curSize = new Size(width, height);
        }

        public ActivatedSprite(string sprite)
        {

        }

        public void SetSrcBitmapList(List<Bitmap> list) {
            for (int i = originBitmapList.Count-1; i>=0; i--)
            {
                Bitmap bitmap = originBitmapList[i];
                originBitmapList.Remove(bitmap);
                bitmap.Recycle();

                Bitmap b = curbitmapList[i];
                curbitmapList.Remove(b);
                b.Recycle();
            }
            for (int i = 0; i < list.Count; i++)
            {
                originBitmapList.Add(list[i]);
                curbitmapList.Add(Bitmap.CreateBitmap(list[i]));
            }
        }

        public void AddBlocks(List<List<Block>> list) {
            for (int i = 0; i < list.Count; i++)
            {
                mBlocks.Add(list[i]);
            }
            ResetRowColumn();
        }

        //添加积木 add block
        public void AddBlock(Block block)
        {
            if (block.name.Equals("ControlStart") || block.name.Equals("ControlRecvSig") || block.name.Equals("ControlTouch") || block.name.Equals("ControlClickSprite") || block.name.Equals("EventblockInputKey") || block.name.Equals("EventblockClone"))
            {
                List<Block> list = new List<Block>();
                list.Add(block);
                block.row = mBlocks.Count;
                mBlocks.Add(list);
                curRow = mBlocks.Count - 1;
            }
            else
            {
                if (mBlocks.Count > 0)
                {
                    List<Block> list = mBlocks[curRow];
                    block.row = curRow;
                    list.Add(block);
                }
            }
        }

        public void DeleteBlock(int row, int column)
        {
            List<Block> list = mBlocks[row];
            if (column == 0)
            {
                mBlocks.Remove(list);
                if (curRow >= mBlocks.Count) {
                    curRow = mBlocks.Count - 1;
                }

            }
            else {
                list.Remove(list[column]);
            }
            ResetRowColumn();
        }

        public bool ExchangeBlock(int row, int column, int r, int c) {
            // same row and column
            if (row == r && column == c)
            {
                return false;
            }

            if (c == 0)
            {
                return false;
            }

            if (row == r)
            {
                List<Block> list = mBlocks[row];
                if (column >= list.Count && c >= list.Count)
                {
                    return false;
                }
                if (c > list.Count - 1)
                {
                    Block block = list[column];
                    list.Remove(block);
                    list.Add(block);
                }
                else
                {
                    Block block = list[column];
                    list.Remove(block);
                    list.Insert(c, block);
                }
                ResetRowColumn();
                return true;
            }
            else
            {
                List<Block> list = mBlocks[row];
                Block block = list[column];
                list.Remove(block);
                List<Block> list1 = mBlocks[r];
                if (c >= list1.Count)
                {
                    list1.Add(block);
                }
                else
                {
                    list1.Insert(c, block);
                }
                ResetRowColumn();
                return true;
            }
        }

        //清除积木 clear block
        public void ClearCode()
        {
            mBlocks.RemoveRange(0, mBlocks.Count);
            ResetRowColumn();
        }

        public void Location() {
            Bitmap bitmap = originBitmapList[0];
            int width = bitmap.Width;
            int height = bitmap.Height;
            int x = (int)(1 + Java.Lang.Math.Random() * (notFullSize.Width - width + 1));
            int y = (int)(1 + Java.Lang.Math.Random() * (notFullSize.Height - height + 1));
            curPoint.X = Java.Lang.Math.Abs(x);
            curPoint.Y = Java.Lang.Math.Abs(y);
            originPoint.X = curPoint.X;
            originPoint.Y = curPoint.Y;
            mUpdateDelegate?.UpdateView();
        }

        private void ResetRowColumn() {
            for (int i = 0; i < mBlocks.Count; i++)
            {
                List<Block> list = mBlocks[i];
                for (int j = 0; j < list.Count; j++)
                {
                    Block block = list[j];
                    block.row = i;
                    block.index = j;
                }
            }
        }

        public void ChangeMode()
        {
            
        }

        //when delete variable to update view
        public void RemoveVariable(string name = null) {
            for (int i = 0; i < mBlocks.Count; i++)
            {
                List<Block> list = mBlocks[i];
                for (int j = 0; j < list.Count; j++)
                {
                    Block block = list[j];
                    if (block.varName != null)
                    {
                        if (name != null && name.Equals(block.varName))
                        {
                            block.text = null;
                            block.varName = null;
                            block.varValue = null;
                        }
                        else {
                            block.text = null;
                            block.varName = null;
                            block.varValue = null;
                        }
                    }
                }
            }
            mUpdateDelegate?.UpdateBlockViewDelegate();
        }

        public void RemoveCollision(string collisionid) {
            for (int i = 0; i < mBlocks.Count; i++)
            {
                List<Block> list = mBlocks[i];
                for (int j = 0; j < 1; j++)
                {
                    Block block = list[j];
                    if (block.name.Equals("ControlTouch") && block.activateSpriteId != null && block.activateSpriteId.Equals(collisionid))
                    {
                        block.text = "";
                        block.activateSpriteId = null;
                        mUpdateDelegate?.UpdateBlockViewDelegate();
                    }
                }
            }
        }

        // receive click signal
        public void ReceiveClickSignal()
        {
            for (int i = 0; i < mBlocks.Count; i++)
            {
                List<Block> list = mBlocks[i];
                for (int j = 0; j < 1; j++)
                {
                    Block block = list[j];
                    if (block.name.Equals("ControlClickSprite"))
                    {
                        block.clickSignalCount++;
                    }
                }
            }
        }

        // receive signal
        public void ReceiveSignal(string signal) {
            for (int i = 0; i < mBlocks.Count; i++)
            {
                List<Block> list = mBlocks[i];
                for (int j = 0; j < 1; j++)
                {
                    Block block = list[j];
                    if (block.name.Equals("ControlRecvSig") && block.text != null && block.text.Equals(signal))
                    {
                        block.signalCount++;
                    }
                }
            }
        }

        public void ReceiveCollisionSignal(string signal) {
            for (int i = 0; i < mBlocks.Count; i++)
            {
                List<Block> list = mBlocks[i];
                for (int j = 0; j < 1; j++)
                {
                    Block block = list[j];
                    if (block.name.Equals("ControlTouch") && block.activateSpriteId != null && block.activateSpriteId.Equals(signal))
                    {
                        block.collionSignal = true;
                    }
                }
            }
        }

        public int GetBlockCount() {
            int count = 0;
            for (int i = 0; i < mBlocks.Count; i++)
            {
                List<Block> list = mBlocks[i];
                count += list.Count;
            }
            return count;
        }

        public void AddToOriginPoint(int x, int y) {

            Bitmap bitmap = originBitmapList[0];
            int width = bitmap.Width;
            int height = bitmap.Height;
            int mW = notFullSize.Width - width + 1;
            int mH = notFullSize.Height - height + 1;

            int oX = originPoint.X + x;
            int oY = originPoint.Y + y;
            
            originPoint.X = oX < 0 ? 0 : oX > mW ? mW : oX;
            originPoint.Y = oY < 0 ? 0 : oY > mH ? mH : oY;
            curPoint.X = originPoint.X;
            curPoint.Y = originPoint.Y;
            mUpdateDelegate?.UpdateView();
        }

        public void InvalidateStage()
        {
            //LogUtil.CustomLog("InvalidateStage");
            if (mUpdateDelegate != null && isAnimationTag)
            {
                mUpdateDelegate.UpdateView();
            }
        }

        public Bitmap GetSpriteBit()
        {
            return curbitmapList[curIndex];
        }

        // bitmap transparent 
        public void SetTransparentBit(Bitmap spriteImage)
        {
            for (int i = 0; i < spriteImage.Width; i++)
            {
                for (int j = 0; j < spriteImage.Height; j++)
                {
                    int pixel = spriteImage.GetPixel(i, j);
                    int a = Color.GetAlphaComponent(pixel);
                    int r = Color.GetRedComponent(pixel);
                    int g = Color.GetGreenComponent(pixel);
                    int b = Color.GetBlueComponent(pixel);
                    if (r >= 180 && g >= 180 && b >= 180)
                    {
                        spriteImage.SetPixel(i, j, Color.Transparent);
                    }
                }
            }
        }

        public void Start()
        {
            for (int i = 0; i < mBlocks.Count; i++)
            {
                Java.Lang.Thread thread = new Java.Lang.Thread(new Java.Lang.Runnable(() =>
                {
                    CodeRunThread();
                }));
                thread.Name = i+"";   
                BlockThreadList.Add(thread);
            }

            for (int i = 0; i < mBlocks.Count; i++)
            {
                BlockThreadList[i].Start();
            }
        }

        public void Reset()
        {
            for (int i = 0; i < BlockThreadList.Count; i++)
            {
                Java.Lang.Thread thread = BlockThreadList[i];
                thread.GetState();
                if (thread.GetState() == Java.Lang.Thread.State.Blocked)
                {
                    thread.Notify();
                }

            }
            BlockThreadList.RemoveRange(0, BlockThreadList.Count);
            for (int i = 0; i < originBitmapList.Count; i++)
            { 
                Bitmap bitmap = curbitmapList[i];
                Bitmap originBm = originBitmapList[i];
                curbitmapList[i] = Bitmap.CreateBitmap(originBm);
                bitmap.Recycle();
            }
            curSize = new Size(curbitmapList[0].Width, curbitmapList[0].Height);
            isVisible = true;
            speakText = null;
            curPoint.X = originPoint.X;
            curPoint.Y = originPoint.Y;
            curIndex = 0;
            if (mUpdateDelegate != null)
            {
                mUpdateDelegate.UpdateView();
            }

            for (int i = 0; i < mBlocks.Count; i++)
            {
                List<Block> list = mBlocks[i];
                for (int j = 0; j < list.Count; j++)
                {
                    Block block = list[j];
                    block.signalCount = 0;
                    block.collionSignal = false;
                    block.clickSignalCount = 0;
                }
            }
            LogUtil.CustomLog("Reset");
        }

        public void CodeRunThread()
        {
            string name = Java.Lang.Thread.CurrentThread().Name;
            if (!System.Int32.TryParse(name, out int index))
            {
                return;
            }
            List<Block> list = mBlocks[index];
            int loopCnt = 0;
            int loopStartIdx = -1;
            int idx = 0;
            while (isAnimationTag) {
                try
                {
                    if (!isAnimationTag) break;
                    if (idx >= list.Count) {
                        Thread.Sleep(10);
                        if (list.Count>0 && isEvent(list[0].name))
                        {
                            idx = 0;
                            list[0].collionSignal = false;
                        } 
                        continue;
                    }
                    Block block = list[idx];
                    string blockName = list[idx].name;

                    if (blockName.Equals("ControlRecvSig"))
                    {
                        if (block.signalCount == 0)
                        {
                            Thread.Sleep(10);
                            continue;
                        }
                        else
                        {
                            block.signalCount--;
                        }
                    }
                    else if (blockName.Equals("ControlTouch"))
                    {
                        if (!block.collionSignal)
                        {
                            Thread.Sleep(10);
                            continue;
                        }
                        else
                        {
                            block.collionSignal = false;
                        }
                    }
                    else if (blockName.Equals("ControlClickSprite"))
                    {
                        if (block.clickSignalCount == 0)
                        {
                            Thread.Sleep(10);
                            continue;
                        }
                        else
                        {
                            block.clickSignalCount--;
                        }
                    }
                    else if (blockName.Equals("ControlLoop"))
                    {
                        loopCnt = 0;
                        loopStartIdx = -1;
                        idx = -1;
                    }
                    else if (blockName.Equals("ControlLoopN") || blockName.Equals("GameLoopN"))
                    {
                        if (!System.Int32.TryParse(block.text, out int n))
                        {
                            n = 1;
                        }
                        else if (n == 0) { }
                        else
                        {
                            if (++loopCnt != n)
                            {
                                idx = loopStartIdx;
                            }
                            else
                            {
                                loopStartIdx = idx;
                                loopCnt = 0;
                            }
                        }
                    }
                    else if (blockName.Equals("ControlFlag"))
                    {
                        loopStartIdx = idx;
                    }
                    else if (blockName.Equals("MoveRDownN")) MoveVariable(MoveArrow.RightDown, block);
                    else if (blockName.Equals("MoveRUpN")) MoveVariable(MoveArrow.RightUp, block);
                    else if (blockName.Equals("MoveLDownN")) MoveVariable(MoveArrow.LeftDown, block);
                    else if (blockName.Equals("MoveLUpN")) MoveVariable(MoveArrow.LeftUp, block);
                    else if (blockName.Equals("MoveRight1")) MoveConstant(MoveArrow.Right, 1, true);
                    else if (blockName.Equals("MoveRight5")) MoveConstant(MoveArrow.Right, 5, true);
                    else if (blockName.Equals("MoveRight10")) MoveConstant(MoveArrow.Right, 10, true);
                    else if (blockName.Equals("MoveRightN")) MoveVariable(MoveArrow.Right, block);
                    else if (blockName.Equals("MoveDown1")) MoveConstant(MoveArrow.Down, 1, true);
                    else if (blockName.Equals("MoveDown5")) MoveConstant(MoveArrow.Down, 5, true);
                    else if (blockName.Equals("MoveDown10")) MoveConstant(MoveArrow.Down, 10, true);
                    else if (blockName.Equals("MoveDownN")) MoveVariable(MoveArrow.Down, block);
                    else if (blockName.Equals("MoveLeft1")) MoveConstant(MoveArrow.Left, 1, true);
                    else if (blockName.Equals("MoveLeft5")) MoveConstant(MoveArrow.Left, 5, true);
                    else if (blockName.Equals("MoveLeft10")) MoveConstant(MoveArrow.Left, 10, true);
                    else if (blockName.Equals("MoveLeftN")) MoveVariable(MoveArrow.Left, block);
                    else if (blockName.Equals("MoveUp1")) MoveConstant(MoveArrow.Up, 1, true);
                    else if (blockName.Equals("MoveUp5")) MoveConstant(MoveArrow.Up, 5, true);
                    else if (blockName.Equals("MoveUp10")) MoveConstant(MoveArrow.Up, 10, true);
                    else if (blockName.Equals("MoveUpN")) MoveVariable(MoveArrow.Up, block);
                    else if (blockName.Equals("ActionRRotate")) RotateLoop(90, true, false);
                    else if (blockName.Equals("ActionLRotate")) RotateLoop(90, false, false);
                    else if (blockName.Equals("ActionBounce")) ActionBounce();
                    else if (blockName.Equals("ActionWave")) ActionWave();
                    else if (blockName.Equals("ActionTWave")) ActionTWave();
                    else if (blockName.Equals("ActionZigzag")) ActionZigzag();
                    else if (blockName.Equals("ActionTZigzag")) ActionTZigzag();
                    else if (blockName.Equals("ActionJump")) JumpSprite();
                    else if (blockName.Equals("ActionRandomMove")) RandomMove();
                    else if (blockName.Equals("ActionFast")) ActionFast();
                    else if (blockName.Equals("ActionSlow")) ActionSlow();
                    else if (blockName.Equals("ActionRotateLoop")) RotateLoop(10, true, true);
                    else if (blockName.Equals("ActionRRotateN")) RotateArrowValue(true, block);
                    else if (blockName.Equals("ActionLRotateN")) RotateArrowValue(false, block);
                    else if (blockName.Equals("ActionFlash")) ShowHideLoop();
                    else if (blockName.Equals("ActionRLJump")) RightLeftJumpLoop();
                    else if (blockName.Equals("ActionAnimate")) AnimateSpritesLoop();
                    else if (blockName.Equals("ControlFlipX")) FlipXSprite();
                    else if (blockName.Equals("ControlFlipY")) FlipYSprite();
                    else if (blockName.Equals("ControlShow")) ShowSprite(true);
                    else if (blockName.Equals("ControlHide")) ShowSprite(false);
                    else if (blockName.Equals("ControlNextSprite")) SetNextBit();
                    else if (blockName.Equals("ControlPrevSprite")) SetPrevBit();
                    else if (blockName.Equals("ControlTime1")) Thread.Sleep((int)(1 * 100.0f));
                    else if (blockName.Equals("ControlTime2")) Thread.Sleep((int)(5 * 100.0f));
                    else if (blockName.Equals("ControlTimeN")) SleepVariable(block);
                    else if (blockName.Equals("ControlSpeak")) ControlSpeak(block.text);
                    else if (blockName.Equals("ControlSpeakStop")) ControlSpeak(null);
                    else if (blockName.Equals("ControlSound")) SoundPlayer(block.varValue);
                    else if (blockName.Equals("ControlChangeVal"))
                    {
                        if (block.varName != null && Variable.curVariableMap.ContainsKey(block.varName))
                        {
                            Variable.curVariableMap[block.varName] = int.Parse(Variable.curVariableMap[block.name]) + int.Parse(block.varValue) + "";
                        }
                    }
                    else if (blockName.Equals("ControlSetVal"))
                    {
                        if (block.varName != null && Variable.curVariableMap.ContainsKey(block.varName))
                        {
                            Variable.curVariableMap[block.varName] = int.Parse(block.varValue) + "";
                        }
                    }
                    else if (blockName.Equals("ControlSendSignal"))
                    {
                        if (block.text != null)
                        {
                            Signal.SendSignal(block.text);
                        }
                    }

                    //else if (blockName.Equals("GameRight")) TurnAndMoveForward(5);
                    //else if (blockName.Equals("GameDown")) TurnAndMoveForward(6);
                    //else if (blockName.Equals("GameLeft")) TurnAndMoveForward(7);
                    //else if (blockName.Equals("GameUp")) TurnAndMoveForward(8);
                    //else if (blockName.Equals("GameJump")) ArrowJump();

                    Java.Lang.Thread.Sleep(10);
                }
                catch (Java.Lang.Exception e)
                {
                    Reset();
                }
                idx++;
            }
        }

        private bool isEvent(string name) {
            for (int i = 1; i < Block.blockTab0ResIdStrs.Length; i++)
            {
                if (name.Equals(Block.blockTab0ResIdStrs[i]))
                {
                    return true;
                }
            }
            return false;
        }


        /*============================================================================================*/
        /*
         * private function
         */
        /*============================================================================================*/
        // 度数转换
        private double DegreeToRadian(double angle)
        {
            return Java.Lang.Math.Pi * angle / 180.0;
        }

        /*
         * Bitmap旋转变换
         */
        private void rotateBitmap(float angle)
        {
            Bitmap origin = originBitmapList[curIndex];
            if (origin == null)
            {
                return;
            }
            int width = origin.Width;
            int height = origin.Height;
            Matrix matrix = new Matrix();
            matrix.SetRotate(angle);

            Bitmap curBitmap = curbitmapList[curIndex];
            Point center = new Point(curPoint.X + curBitmap.Width / 2, curPoint.Y + curBitmap.Height / 2);
            Bitmap newBM = Bitmap.CreateBitmap(origin, 0, 0, width, height, matrix, false);

            curSize = new Size(newBM.Width, newBM.Height);
            curPoint = new Point(center.X - newBM.Width / 2, center.Y - newBM.Height / 2);
            curbitmapList[curIndex] = newBM;
            curBitmap.Recycle();
        }


        /*============================================================================================*/
        /*
         * animate function
         */
        /*============================================================================================*/
        public void MoveConstant(MoveArrow arrow, int n, bool isDelay)
        {
            curMoveArrow = arrow;
            MoveSprite(n * 10, 1, curMoveArrow);
            if (isDelay)
            {
                Java.Lang.Thread.Sleep(1000);
                Java.Lang.Thread.Sleep(1000);
            }
        }

        public void MoveVariable(MoveArrow arrow, Block block)
        {
            if (block.varName != null)
            {
                if (Variable.curVariableMap.ContainsKey(block.varName))
                {
                    if (!System.Int32.TryParse(Variable.curVariableMap[block.varName], out int value))
                    {
                        return;
                    }
                    curMoveArrow = arrow;
                    MoveSprite(value * 10, 1, curMoveArrow);
                }
            }
            else
            {
                if (!System.Int32.TryParse(block.text, out int value))
                {
                    return;
                }
                curMoveArrow = arrow;
                MoveSprite(value * 10, 1, curMoveArrow);
            }
        }

        /*
         *  move sprite
         */
        public void MoveSprite(int value, int n, MoveArrow moveArrow, int delay = 1)
        {
            switch (moveArrow)
            {
                case MoveArrow.RightUp:
                    for (int i = 0; i < n; i++)
                    {
                        RightUpMove(value);
                        Java.Lang.Thread.Sleep(delay);
                    }
                    break;
                case MoveArrow.RightDown:
                    for (int i = 0; i < n; i++)
                    {
                        RightDownMove(value);
                        Java.Lang.Thread.Sleep(delay);
                    }
                    break;
                case MoveArrow.LeftDown:
                    for (int i = 0; i < n; i++)
                    {
                        LeftDownMove(value);
                        Java.Lang.Thread.Sleep(delay);
                    }
                    break;
                case MoveArrow.LeftUp:
                    for (int i = 0; i < n; i++)
                    {
                        LeftUpMove(value);
                        Java.Lang.Thread.Sleep(delay);
                    }
                    break;
                case MoveArrow.Right:
                    for (int i = 0; i < n; i++)
                    {
                        RightMove(value);
                        Java.Lang.Thread.Sleep(delay);
                    }
                    break;
                case MoveArrow.Down:
                    for (int i = 0; i < n; i++)
                    {
                        DownMove(value);
                        Java.Lang.Thread.Sleep(delay);
                    }
                    break;
                case MoveArrow.Left:
                    for (int i = 0; i < n; i++)
                    {
                        LeftMove(value);
                        Java.Lang.Thread.Sleep(delay);
                    }
                    break;
                case MoveArrow.Up:
                    for (int i = 0; i < n; i++)
                    {
                        UpMove(value);
                        Java.Lang.Thread.Sleep(delay);
                    }
                    break;
                default:
                    break;
            }
        }

        /*
        *    ||
        *   \\//
        *   向下移动
        */
        public void DownMove(int n)
        {
            LogUtil.CustomLog("DownMove " + n);
            if (curPoint.Y + n + curSize.Height < boundSize.Height)
            {
                curPoint.Y += n;
            }
            InvalidateStage();
        }

        /*
         *  《===
         *  向左运动
         */
        public void LeftMove(int n)
        {
            LogUtil.CustomLog("LeftMove " + n);
            if (curPoint.X - n > 0)
            {
                curPoint.X -= n;
            }
            InvalidateStage();
        }

        /*
        *   ===》
        *   向右移动
        */
        public void RightMove(int n)
        {
            LogUtil.CustomLog("RightMove " + n);
            if (curPoint.X + n + curSize.Width < boundSize.Width)
            {
                curPoint.X += n;
            }
            InvalidateStage();
        }

        /*
         *  //\\
         *   ||
         *   向上运动
        */
        public void UpMove(int n)
        {
            LogUtil.CustomLog("UpMove " + n);
            if (curPoint.Y - n > 0)
            {
                curPoint.Y -= n;
            }
            InvalidateStage();
        }

        /*
         *   |\
         *    \\
        */
        public Wall LeftUpMove(int n)
        {
            LogUtil.CustomLog("LeftUpMove");
            InvalidateStage();

            if (curPoint.X - n > 0)
            {
                if (curPoint.Y - n > 0)
                {
                    curPoint.X -= n;
                    curPoint.Y -= n;

                    InvalidateStage();
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

        /*
         *    /|
         *   //
        */
        public Wall RightUpMove(int n)
        {
            LogUtil.CustomLog("RightUpMove");
            if (curPoint.X + curSize.Width + n < boundSize.Width)
            {
                if (curPoint.Y - n > 0)
                {
                    curPoint.X += n;
                    curPoint.Y -= n;
                    InvalidateStage();
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

        /*
         *    //
         *   |/
        */
        public Wall LeftDownMove(int n)
        {
            LogUtil.CustomLog("LeftDownMove");
            if (curPoint.X - n > 0)
            {
                if (curPoint.Y + curSize.Height + n < boundSize.Height)
                {
                    curPoint.X -= n;
                    curPoint.Y += n;

                    InvalidateStage();
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

        /*
         *   \\
         *    \|
        */
        public Wall RightDownMove(int n)
        {
            LogUtil.CustomLog("RightDownMove");
            if (curPoint.X + curSize.Width + n < boundSize.Width)
            {
                if (curPoint.Y + curSize.Height + n < boundSize.Height)
                {
                    curPoint.X += n;
                    curPoint.Y += n;
                    InvalidateStage();
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

        public void ActionSlow()
        {
            LogUtil.CustomLog("SlowMove");
            bool isRight = false;
            if (curMoveArrow != MoveArrow.Left)
            {
                curMoveArrow = MoveArrow.Right;
                isRight = true;
            }
            while (isAnimationTag)
            {
                InvalidateStage();
                if (isRight)
                {
                    if (curPoint.X + curSize.Width + 10 < boundSize.Width)
                    {
                        curPoint.X += 10;
                    }
                    else
                    {
                        FlipYSprite();
                        isRight = false;
                    }
                }
                else
                {
                    if (curPoint.X - 10 > 0)
                    {
                        curPoint.X -= 10;
                    }
                    else
                    {
                        FlipYSprite();
                        isRight = true;
                    }

                }
                InvalidateStage();
                Java.Lang.Thread.Sleep(300);
            }
        }

        public void ActionFast()
        {
            LogUtil.CustomLog("FastMove");
            bool isRight = false;
            if (curMoveArrow != MoveArrow.Left)
            {
                curMoveArrow = MoveArrow.Right;
                isRight = true;
            }
            while (isAnimationTag)
            {
                InvalidateStage();
                if (isRight)
                {
                    if (curPoint.X + curSize.Width + 10 < boundSize.Width)
                    {
                        curPoint.X += 10;
                    }
                    else
                    {
                        FlipYSprite();
                        isRight = false;
                    }
                }
                else
                {
                    if (curPoint.X - 10 > 0)
                    {
                        curPoint.X -= 10;
                    }
                    else
                    {
                        FlipYSprite();
                        isRight = true;
                    }
                }
                InvalidateStage();
                Java.Lang.Thread.Sleep(100);
            }
        }

        // show hide Loop
        public void ShowHideLoop()
        {
            LogUtil.CustomLog("ShowHideLoop");
            bool flag = true;
            while (isAnimationTag)
            {
                ShowSprite(flag ^= true);
                InvalidateStage();
                Java.Lang.Thread.Sleep(1000);
            }
        }

        // 旋转
        public void RotateLoop(float angle, bool cw, bool isInfinite)
        {
            while (isAnimationTag)
            {
                angle = cw ? angle : 360.0f - angle;
                curAngle += angle;
                if (curAngle >= 360.0f)
                {
                    curAngle -= ((int)(curAngle / 360.0f)) * 360;
                }
                rotateBitmap(curAngle);
                InvalidateStage();
                Java.Lang.Thread.Sleep(250);
                if (!isInfinite)
                    break;
            }
        }

        // 按用户输入旋转
        public void RotateArrowValue(bool cw, Block block)
        {
            if (block.varName != null)
            {
                if (Variable.curVariableMap.ContainsKey(block.varName))
                {
                    if (!float.TryParse(Variable.curVariableMap[block.varName], out float n))
                    {
                        return;
                    }
                    RotateLoop(n, cw, false);
                }
            }
            else
            {
                if (!float.TryParse(block.text, out float n))
                {
                    return;
                }
                RotateLoop(n, cw, false);
            }
        }

        // Wave 波浪移动
        public void ActionWave()
        {
            LogUtil.CustomLog("WaveMove");
            int zero = curPoint.Y;
            bool isRight = false;
            if (curMoveArrow != MoveArrow.Left)
            {
                curMoveArrow = MoveArrow.Right;
                isRight = true;
            }
            while (isAnimationTag)
            {
                if (isRight)
                {
                    for (double i = 0; i <= 360; i++)
                    {
                        InvalidateStage();

                        if (curPoint.X + curSize.Width + 20 < boundSize.Width)
                        {
                            curPoint.Y = (int)(Java.Lang.Math.Sin(i / 4.0) * 50.0) + zero;
                            curPoint.X += 10;
                        }
                        else
                        {
                            FlipYSprite();
                            isRight = false;
                            break;
                        }

                        InvalidateStage();
                        Java.Lang.Thread.Sleep(100);
                    }
                }
                else
                {
                    for (double i = 0; i <= 360; i++)
                    {
                        InvalidateStage();
                        if (curPoint.X - 20 > 0)
                        {
                            curPoint.Y = (int)(Java.Lang.Math.Sin(i / 4.0) * 50.0) + zero;
                            curPoint.X -= 10;
                        }
                        else
                        {
                            FlipYSprite();
                            isRight = true;
                            break;
                        }
                        InvalidateStage();
                        Java.Lang.Thread.Sleep(100);
                    }
                }
            }
        }

        // vertical wave 
        public void ActionTWave()
        {
            LogUtil.CustomLog("WaveMoveVert");
            bool isDown = false;
            if (curMoveArrow != MoveArrow.Up)
            {
                curMoveArrow = MoveArrow.Down;
                isDown = true;
            }
            int zero = curPoint.X;
            while (isAnimationTag)
            {
                if (isDown)
                {
                    for (double i = 0; i <= 360; i++)
                    {
                        InvalidateStage();

                        if (curPoint.Y + curSize.Height + 20 < boundSize.Height)
                        {
                            curPoint.X = (int)(Java.Lang.Math.Sin(i / 3.0) * 50.0) + zero;
                            curPoint.Y += 20;
                        }
                        else
                        {
                            isDown = false;
                            break;
                        }

                        InvalidateStage();
                        Java.Lang.Thread.Sleep(200);
                    }
                }
                else
                {
                    for (double i = 0; i <= 360; i++)
                    {
                        InvalidateStage();

                        if (curPoint.Y - 20 > 0)
                        {
                            curPoint.X = (int)(Java.Lang.Math.Sin(i / 3.0) * 50.0) + zero;
                            curPoint.Y -= 20;
                        }
                        else
                        {
                            isDown = true;
                            break;
                        }
                        InvalidateStage();
                        Java.Lang.Thread.Sleep(200);
                    }
                }
            }
        }

        public void RandomMove()
        {
            LogUtil.CustomLog("RandomMove");
            while (isAnimationTag)
            {
                int random = (int)(1 + Java.Lang.Math.Random() * (9 - 1 + 1)); //1~8 move arrow
                int n = (int)(1 + Java.Lang.Math.Random() * (6 - 1 + 1)); // 1~5 delay time
                                                                          //_arrow = new Random(Guid.NewGuid().GetHashCode()).Next(1, 9); // 1~8 난수 생성
                                                                          //int n = new Random(Guid.NewGuid().GetHashCode()).Next(1, 6);
                MoveSprite(10, n, (MoveArrow)random, 100);
            }
        }

        //w wave 波浪运动
        public void ActionZigzag()
        {
            LogUtil.CustomLog("TWaveMove");
            bool isRight = false;
            if (curMoveArrow != MoveArrow.Left)
            {
                curMoveArrow = MoveArrow.Right;
                isRight = true;
            }
            int zero = curPoint.Y;
            while (isAnimationTag)
            {
                if (isRight)
                {
                    for (double i = 0; i <= 360; i++)
                    {
                        InvalidateStage();

                        if (curPoint.X + curSize.Width + 20 < boundSize.Width)
                        {
                            curPoint.Y = (int)(Java.Lang.Math.Sin(i) * 50.0) + zero;
                            curPoint.X += 10;
                        }
                        else
                        {
                            FlipYSprite();
                            isRight = false;
                            break;
                        }

                        InvalidateStage();
                        Java.Lang.Thread.Sleep(100);
                    }
                }
                else
                {
                    for (double i = 0; i <= 360; i++)
                    {
                        InvalidateStage();

                        if (curPoint.X - 20 > 0)
                        {
                            curPoint.Y = (int)(Java.Lang.Math.Sin(i) * 50.0) + zero;
                            curPoint.X -= 10;
                        }
                        else
                        {
                            FlipYSprite();
                            isRight = true;
                            break;
                        }

                        InvalidateStage();
                        Java.Lang.Thread.Sleep(100);
                    }
                }
            }
        }

        /*
         * vertical w wave 竖直W波浪运动
         */
        public void ActionTZigzag()
        {
            LogUtil.CustomLog("ActionTZigzag");
            bool isDown = false;
            if (curMoveArrow != MoveArrow.Up)
            {
                curMoveArrow = MoveArrow.Down;
                isDown = true;
            }
            int zero = curPoint.X;
            while (isAnimationTag)
            {
                if (isDown)
                {
                    for (double i = 0; i <= 360; i++)
                    {
                        InvalidateStage();

                        if (curPoint.Y + curSize.Height + 20 < boundSize.Height)
                        {
                            curPoint.X = (int)(Java.Lang.Math.Sin(i) * 50.0) + zero;
                            curPoint.Y += 20;
                        }
                        else
                        {
                            isDown = false;
                            break;
                        }
                        InvalidateStage();
                        Java.Lang.Thread.Sleep(200);
                    }
                }
                else
                {
                    for (double i = 0; i <= 360; i++)
                    {
                        InvalidateStage();

                        if (curPoint.Y - 20 > 0)
                        {
                            curPoint.X = (int)(Java.Lang.Math.Sin(i) * 50.0) + zero;
                            curPoint.Y -= 20;
                        }
                        else
                        {
                            isDown = true;
                            break;
                        }
                        InvalidateStage();
                        Java.Lang.Thread.Sleep(200);
                    }
                }
            }
        }

        //Bounce 斜向运动
        /*  move arrow
         *  4  8  1 
         *  7     5
         *  3  6  2
         *  wall
         *     4
         *  3     1
         *     2
         */
        public void ActionBounce()
        {
            LogUtil.CustomLog("BounceMove");
            Wall wall = Wall.Default;
            while (isAnimationTag)
            {
                if (curMoveArrow != MoveArrow.RightDown && curMoveArrow != MoveArrow.LeftDown && curMoveArrow != MoveArrow.LeftUp)
                {
                    curMoveArrow = MoveArrow.RightUp;
                }
                switch (curMoveArrow)
                {
                    case MoveArrow.RightUp:
                        wall = RightUpMove(10);
                        break;
                    case MoveArrow.RightDown:
                        wall = RightDownMove(10);
                        break;
                    case MoveArrow.LeftDown:
                        wall = LeftDownMove(10);
                        break;
                    case MoveArrow.LeftUp:
                        wall = LeftUpMove(10);
                        break;
                    default:
                        break;
                }
                int arrowNum = (int)curMoveArrow;
                int wallNum = (int)wall;
                if (wall != Wall.Default) {
                    arrowNum = wallNum % 2 == 0 ? 5 - arrowNum : (wallNum == arrowNum ? arrowNum - 1 : arrowNum + 1);
                    curMoveArrow = (MoveArrow)arrowNum;
                }
                Java.Lang.Thread.Sleep(50);
            }

        }

        //jump 上下跳
        public void JumpSprite()
        {
            LogUtil.CustomLog("JumpSprite");
            for (int i = 5; i > 0; i--)
            {
                MoveSprite(i * 10, 1, MoveArrow.Up, 100);
            }
            for (int i = 1; i <= 5; i++)
            {
                MoveSprite(i * 10, 1, MoveArrow.Down, 100);
            }
        }

        //Left right jump 左右跳
        public void RightLeftJumpLoop()
        {
            LogUtil.CustomLog("RightLeftJumpLoop");
            MoveArrow moveArrow = MoveArrow.Left;
            while (isAnimationTag)
            {
                moveArrow = moveArrow == MoveArrow.Left ? MoveArrow.Right : MoveArrow.Left;
                int delay = 1;
                int zero = curPoint.Y;
                for (int i = 0; i <= 180; i++)
                {
                    curPoint.Y = (int)(-Java.Lang.Math.Sin(DegreeToRadian(i)) * 100.0) + zero;
                    curPoint.X += (6 - (int)moveArrow);
                    if (i % 10 == 0 && i != 0)
                    {
                        curPoint.X += (6 - (int)moveArrow);
                    }
                    if (i % 20 == 0 && i != 0 && i < 91)
                    {
                        delay++;
                    }
                    else if (i % 20 == 0 && i != 0 && i > 90)
                    {
                        delay--;
                    }
                    Java.Lang.Thread.Sleep(delay);
                    InvalidateStage();
                }
                curPoint.X += (6 - (int)moveArrow);
                Java.Lang.Thread.Sleep(100);
            }
        }

        //Sprite bitmap loop
        public void AnimateSpritesLoop()
        {
            while (isAnimationTag)
            {
                SetNextBit();
                Java.Lang.Thread.Sleep(1000);
            }
        }

        // set wait time
        public static void SleepVariable(Block block)
        {

            if (block.varName != null)
            {
                if (Variable.curVariableMap.ContainsKey(block.varName))
                {
                    if (!float.TryParse(Variable.curVariableMap[block.varName], out float value))
                    {
                        return;
                    }
                    int wait = (int)(value * 100.0f);
                    Java.Lang.Thread.Sleep(wait);
                }
            }
            else
            {
                if (!float.TryParse(block.text, out float value))
                {
                    return;
                }
                int wait = (int)(value * 100.0f);
                Java.Lang.Thread.Sleep(wait);
            }
        }

        //Flip X 
        public void FlipXSprite()
        {
            Matrix matrix = new Matrix();
            matrix.SetScale(1, -1);
            for (int i = 0; i < curbitmapList.Count; i++)
            {
                Bitmap bitmap = curbitmapList[i];
                Bitmap newBM = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, false);
                curbitmapList[i] = newBM;
            }
            InvalidateStage();
        }

        //Flip Y 
        public void FlipYSprite()
        {
            Matrix matrix = new Matrix();
            matrix.SetScale(-1, 1);
            Bitmap bitmap = curbitmapList[curIndex];
            Bitmap newBM = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, false);
            curbitmapList[curIndex] = newBM;
            InvalidateStage();
        }

        //next sprite bitmap
        public void SetNextBit()
        {
            int maxNum = curbitmapList.Count - 1;
            if (curIndex < maxNum)
            {
                curIndex++;
            }
            else
            {
                curIndex = 0;
            }
            curbitmapList[curIndex] = GetSpriteBit();
            InvalidateStage();
        }

        //previous sprite bitmap
        public void SetPrevBit()
        {
            InvalidateStage();

            int maxNum = curbitmapList.Count - 1;
            if (curIndex > 0)
            {
                curIndex--;
            }
            else
            {
                curIndex = maxNum;
            }

            curbitmapList[curIndex] = GetSpriteBit();
            InvalidateStage();
        }

        // Show hide sprite 
        public void ShowSprite(bool flag)
        {
            LogUtil.CustomLog("ShowSprite");
            isVisible = flag;
            InvalidateStage();
        }

        // Speak text
        public void ControlSpeak(string text)
        {
            speakText = text;
            InvalidateStage();
            Thread.Sleep(2000);
            //speakText = null;
            //InvalidateStage();
            //Thread.Sleep(500);
        }

        public void SoundPlayer(string text)
        {
            SoundAction?.Invoke(text);
            Thread.Sleep(2000);
            //soundText = text;
            //LogUtil.CustomLog("1111");

            //InvalidateStage();
            //LogUtil.CustomLog("3333");
            //Thread.Sleep(2000);
            //soundText = null;
        }
    }
}