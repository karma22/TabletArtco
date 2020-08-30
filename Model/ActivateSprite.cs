
using System.Collections.Generic;
using Android.Graphics;
using Android.Util;
using System.Threading;
using System;
using Android.Media;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace TabletArtco
{
    public enum EventType { EventStart, EventRecvSig, EventCollision = 3, EventClick, EventClone };
    public enum MoveArrow { Default, RightUp, RightDown, LeftDown, LeftUp, Right, Down, Left, Up };
    public enum Wall { Default, RightWall, DownWall, LeftWall, UpWall };
    public class ActivatedSprite
    {
        private static string Tag = "ActivatedSprite";

        public static Size notFullSize;
        public static Size fullSize;
        public static bool mIsFull;
        public static bool isAnimationTag;

        public string activateSpriteId = Java.Lang.JavaSystem.CurrentTimeMillis() + "";
        public Sprite sprite;
        public List<List<Block>> mBlocks = new List<List<Block>>();
        public List<Bitmap> originBitmapList = new List<Bitmap>();
        public List<float> scaleList = new List<float>();
        public Point originPoint = new Point(0, 0);

        // animation arguments
        public bool stopThisSprite = false;
        public bool isVisible = true;
        public Point curPoint = new Point(0, 0);
        public Size curSize = new Size(0, 0);
        public string speakText = "";
        public Size boundSize
        {
            get {
                return mIsFull ? fullSize : notFullSize;
            }
        }

        public int curRow = 0;
        public int curIndex = 0;
        public List<Bitmap> curbitmapList = new List<Bitmap>();
        public float curAngle = 0;
        public MoveArrow curMoveArrow = MoveArrow.Default;
        public Wall curWall = Wall.Default;

        public int rowMaxCount = 10;
        List<Java.Lang.Thread> BlockThreadList = new List<Java.Lang.Thread>();

        public static UpdateDelegate mUpdateDelegate;
        public static Action<string> SoundAction;

        public int[] programCnt;
        public LoopStack loopStack = new LoopStack();

        public ActivatedSprite(Sprite s, bool isClone = false)
        {
            sprite = Sprite.ToSprite(s.ToString());
            if (s.category == 0 || s.category == -1)
                SetTransparentBit(s.bitmap, 100);

            sprite.bitmap = s.bitmap;
            if (isClone)
            {
                sprite.name = sprite.name + "1";
            }

            curbitmapList.Add(Bitmap.CreateBitmap(sprite.bitmap));
            originBitmapList.Add(Bitmap.CreateBitmap(sprite.bitmap));
            scaleList.Add(1);
            int width = sprite.bitmap.Width;
            int height = sprite.bitmap.Height;
            int x = (int)(1 + Java.Lang.Math.Random() * (notFullSize.Width - width + 1));
            int y = (int)(1 + Java.Lang.Math.Random() * (notFullSize.Height - height + 1));
            curPoint = new Point(Java.Lang.Math.Abs(x), Java.Lang.Math.Abs(y));
            originPoint.X = curPoint.X;
            originPoint.Y = curPoint.Y;
            curSize = new Size(width, height);
        }

        public void SetSrcBitmapList(List<Bitmap> list)
        {
            for (int i = originBitmapList.Count - 1; i >= 0; i--)
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

        public void AddBlocks(List<List<Block>> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                mBlocks.Add(list[i]);
            }
            ResetRowColumn();
        }

        //添加积木 add block
        public bool AddBlock(Block block)
        {
            if (block.name.Equals("EventStart") || block.name.Equals("EventRecvSig") || block.name.Equals("EventTouch") ||
                block.name.Equals("EventClickSprite") || block.name.Equals("EventClone"))
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
                    // 무한 반복 블럭
                    List<string> checkList = new List<string>
                    {
                        "ActionSlow", "ActionFast", "ActionFlash",
                        "ActionRRotate", "ActionLRotate", "ActionRotateLoop",
                        "ActionWave", "ActionTWave", "ActionRandomMove",
                        "ActionZigzag", "ActionTZigzag", "ActionBounce",
                        "ActionJump", "ActionRLJump", "ActionAnimate"
                    };

                    if (!checkList.Contains(mBlocks[curRow][mBlocks[curRow].Count - 1].name))
                    {
                        List<Block> list = mBlocks[curRow];
                        block.row = curRow;
                        list.Add(block);
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void DeleteBlock(int row, int column)
        {
            List<Block> list = mBlocks[row];
            if (column == 0)
            {
                mBlocks.Remove(list);
                if (curRow >= mBlocks.Count)
                {
                    curRow = mBlocks.Count - 1;
                }

            }
            else
            {
                list.Remove(list[column]);
            }
            ResetRowColumn();
        }

        public bool ExchangeBlock(int row, int column, int r, int c)
        {
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

        public void Location(int sx = -1, int sy = -1)
        {
            Bitmap bitmap = originBitmapList[0];
            int width = bitmap.Width;
            int height = bitmap.Height;
            int x = sx == -1 ? (int)(1 + Java.Lang.Math.Random() * (notFullSize.Width - width + 1)) : sx;
            int y = sy == -1 ? (int)(1 + Java.Lang.Math.Random() * (notFullSize.Height - height + 1)) : sy;
            curPoint.X = Java.Lang.Math.Abs(x);
            curPoint.Y = Java.Lang.Math.Abs(y);
            originPoint.X = curPoint.X;
            originPoint.Y = curPoint.Y;
            mUpdateDelegate?.UpdateView();
        }

        private void ResetRowColumn()
        {
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
            if (mIsFull)
            {
                float scaleX = fullSize.Width * 1.0f / notFullSize.Width;
                float scaleY = fullSize.Height * 1.0f / notFullSize.Height;

                originPoint.X = (int)(originPoint.X * scaleX);
                originPoint.Y = (int)(originPoint.Y * scaleY);
                curPoint.X = originPoint.X;
                curPoint.Y = originPoint.Y;                
            }
            else
            {
                float scaleX = notFullSize.Width * 1.0f / fullSize.Width;
                float scaleY = notFullSize.Height * 1.0f / fullSize.Height;

                originPoint.X = (int)(originPoint.X * scaleX);
                originPoint.Y = (int)(originPoint.Y * scaleY);
                curPoint.X = originPoint.X;
                curPoint.Y = originPoint.Y;
            }
        }

        //when delete variable to update view
        public void RemoveVariable(string name = null)
        {
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
                        else
                        {
                            block.text = null;
                            block.varName = null;
                            block.varValue = null;
                        }
                    }
                }
            }
            mUpdateDelegate?.UpdateBlockViewDelegate();
        }

        public void RemoveCollision(string collisionid)
        {
            for (int i = 0; i < mBlocks.Count; i++)
            {
                List<Block> list = mBlocks[i];
                for (int j = 0; j < 1; j++)
                {
                    Block block = list[j];
                    if (block.name.Equals("EventTouch") && block.activateSpriteId != null && block.activateSpriteId.Equals(collisionid))
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
                    if (block.name.Equals("EventClickSprite"))
                    {
                        block.clickSignalCount++;
                    }
                }
            }
        }

        // receive signal
        public void ReceiveSignal(string signal)
        {
            for (int i = 0; i < mBlocks.Count; i++)
            {
                List<Block> list = mBlocks[i];
                for (int j = 0; j < 1; j++)
                {
                    Block block = list[j];
                    if (block.name.Equals("EventRecvSig") && block.text != null && block.text.Equals(signal))
                    {
                        block.signalCount++;
                    }
                }
            }
        }

        public void ReceiveCollisionSignal(string signal)
        {
            for (int i = 0; i < mBlocks.Count; i++)
            {
                List<Block> list = mBlocks[i];
                for (int j = 0; j < 1; j++)
                {
                    Block block = list[j];
                    if (block.name.Equals("EventTouch") && block.activateSpriteId != null && block.activateSpriteId.Equals(signal))
                    {
                        block.collionSignal = true;
                    }
                }
            }
        }

        public int GetBlockCount()
        {
            int count = 0;
            for (int i = 0; i < mBlocks.Count; i++)
            {
                List<Block> list = mBlocks[i];
                count += list.Count;
            }
            return count;
        }

        public void AddToOriginPoint(int x, int y)
        {
            Bitmap bitmap = originBitmapList[0];
            int width = bitmap.Width;
            int height = bitmap.Height;
            int mW, mH;
            if (mIsFull)
            {
                mW = fullSize.Width - width + 1;
                mH = fullSize.Height - height + 1;
            }
            else
            {
                mW = notFullSize.Width - width + 1;
                mH = notFullSize.Height - height + 1;
            }

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

        private List<bool> isScaledList = new List<bool>() { false, false, false, false, false, false, false, false };
        public Bitmap GetSpriteBit()
        {
            float scale = 1;
            if (curIndex < scaleList.Count)            
                scale = scaleList[curIndex];

            Bitmap b = curbitmapList[curIndex];
            int width = (int)(b.Width * scale);
            int height = (int)(b.Height * scale);

            //int width = (int)(b.Width * ((!isScaledList[curIndex]) ? scale : 1));
            //int height = (int)(b.Height * ((!isScaledList[curIndex]) ? scale : 1));
            //if (scale != 1 && !isScaledList[curIndex])
            //    isScaledList[curIndex] = true;

            Bitmap bitmap = Bitmap.CreateScaledBitmap(b, width, height, false);
            curSize = new Size(bitmap.Width, bitmap.Height);
            return bitmap; 
        }

        public Bitmap GetNextSpriteBit()
        {
            float scale = 1;
            if (curIndex < scaleList.Count)
                scale = scaleList[curIndex];

            Bitmap b = curbitmapList[curIndex];



            int width = (int)(b.Width * ((!isScaledList[curIndex]) ? scale : 1));
            int height = (int)(b.Height * ((!isScaledList[curIndex]) ? scale : 1));
            if (scale != 1 && !isScaledList[curIndex])
                isScaledList[curIndex] = true;

            Bitmap bitmap = Bitmap.CreateScaledBitmap(b, width, height, false);
            curSize = new Size(bitmap.Width, bitmap.Height);
            return bitmap;
        }

        // bitmap transparent 
        public void SetTransparentBit(Bitmap spriteImage, int tolerance)
        {
            for (int i = 0; i < spriteImage.Width; i++)
            {
                for (int j = 0; j < spriteImage.Height; j++)
                {
                    int pixel = spriteImage.GetPixel(i, j);
                    int r = Color.GetRedComponent(pixel);
                    int g = Color.GetGreenComponent(pixel);
                    int b = Color.GetBlueComponent(pixel);

                    if (r >= tolerance && g >= tolerance && b >= tolerance)
                    {
                        int diff1 = Math.Abs(r - g);
                        int diff2 = Math.Abs(r - b);
                        int diff3 = Math.Abs(g - b);
                        if (diff1 <= 30 && diff2 <= 30 && diff3 <= 30)
                            spriteImage.SetPixel(i, j, Color.Transparent);
                    }
                }
            }
        }

        public void SetProgramCnt(int count)
        {
            programCnt = new int[count];
            loopStack.Init(count);
        }

        public void Start()
        {
            SetProgramCnt(mBlocks.Count);
            for (int i = 0; i < mBlocks.Count; i++)
            {
                Java.Lang.Thread thread = new Java.Lang.Thread(new Java.Lang.Runnable(() =>
                {
                    CodeRunThread();
                }));
                thread.Name = i + "";
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
            stopThisSprite = false;
            isVisible = true;
            speakText = null;
            curIndex = 0;
            curAngle = 0;
            curPoint.X = originPoint.X;
            curPoint.Y = originPoint.Y;

            mUpdateDelegate?.UpdateView();

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

            for(int i=0; i< isScaledList.Count; i++)            
                isScaledList[i] = false;
            
            LogUtil.CustomLog("Reset");
        }

        public void CodeRunThread()
        {
            string name = Java.Lang.Thread.CurrentThread().Name;
            if (!int.TryParse(name, out int codeLineIdx))
                return;

            List<Block> codes;

            do
            {
                programCnt[codeLineIdx] = 0;
                codes = mBlocks[codeLineIdx];
                ref var pc = ref programCnt[codeLineIdx];

                for (; pc < codes.Count; pc++)
                {
                    if (!isAnimationTag || stopThisSprite)
                        break;

                    var code = codes[pc];
                    Block._funcs[code.name].Invoke(code, new object[] { this, codeLineIdx });

                    Thread.Sleep(10);
                }

                // call practice mode result form
                mUpdateDelegate?.RowAnimateComplete();
                if (codes[0].eventType == (int)EventType.EventRecvSig && Project.sendSignalWait.ContainsKey(codes[0].text))
                    Project.sendSignalWait[codes[0].text]++;

                if (!isAnimationTag || stopThisSprite)
                    break;

            } while (codes[0].eventType != (int)EventType.EventStart);
        }
    }
}