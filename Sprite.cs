using System.Collections.Generic;
using Android.Graphics;
using Android.Util;
using System.Threading;

namespace TabletArtco
{
    static class SpriteManager
    {
        public delegate void invalidate_blocks();
        public static invalidate_blocks invalidateBlocks;

        public delegate void invalidate_editor();
        public static invalidate_editor invalidateEditor;

        public delegate void remove_spriteinfo(ActivatedSprite sprite);
        public static remove_spriteinfo removeSpriteInfo;

        public static bool isMove { get; set; }
        public static int curSpriteNum { get; set; } = -1; // 전체 스프라이트 구분 번호
        public static List<ActivatedSprite> sprites { get; set; } = new List<ActivatedSprite>();
        public static List<string> blocksBuffer { get; set; } = new List<string>();
        public static List<Thread> codeThreadList { get; set; } = new List<Thread>();

        public static void SpriteSizeChange(bool isFool)
        {
            for (int i = 0; i < sprites.Count; i++)
            {
                sprites[i].setFullMode(isFool);
            }
        }

        public static void AddSprite(Sprite sprite)
        {
            ActivatedSprite activatedSprite = new ActivatedSprite(sprite);
            sprites.Add(activatedSprite);
        }

        public static void RunCode()
        {
            for (int i = 0; i < SpriteManager.sprites.Count; i++)
            {
                ActivatedSprite sprite = SpriteManager.sprites[i];
                Thread codeRunThread = new Thread(sprite.CodeRunThread) { IsBackground = true };
                codeThreadList.Add(codeRunThread);
            }

            for (int i = 0; i < SpriteManager.sprites.Count; i++)
            {
                codeThreadList[i].Start(i);
            }
        }

        public static void StopCode()
        {
            for (int i = 0; i < codeThreadList.Count; i++)
            {
                codeThreadList[i].Abort();
            }

            codeThreadList.RemoveRange(0, codeThreadList.Count);

            for (int i = 0; i < SpriteManager.sprites.Count; i++)
            {
                var sprite = SpriteManager.sprites[i];
                sprite.reset();
            }
        }
    }

    class Sprite
    {
        // List by category
        public static List<List<Sprite>> _sprites { get; set; } = new List<List<Sprite>>();
        
        public string name { get; set; }
        public int category { get; set; }
        public string remotePath { get; set; }
        public bool isUser { get; set; }

        public override string ToString()
        {
            return name + "\n" + category + "\n" + remotePath + "\n" + isUser;
        }
        
        public static Sprite ToSprite(string spriteStr)
        {
            if (spriteStr != null)
            {
                string[] datas = spriteStr.Split('\n');
                if (datas.Length == 4)
                {
                    Sprite sprite = new Sprite()
                    {
                        name = datas[0],
                        category = int.Parse(datas[1]),
                        remotePath = datas[2],
                        isUser = (datas[3].Equals("true") || datas[3].Equals("TRUE")) ? true : false
                    };
                    return sprite;
                }
            }
            return null;
        }
    }

    enum MoveArrow { Default, RightUp, RightDown, LeftDown, LeftUp, Right, Down, Left, Up };
    enum Wall { Default, RightWall, DownWall, LeftWall, UpWall };

    class ActivatedSprite
    {
        public Sprite _sprite { get; set; }
        // Block list
        public List<List<Block>> mBlocks { get; set; } = new List<List<Block>>();
        public List<Bitmap> _originBitmapList { get; set; } = new List<Bitmap>();
        public Point _originPoint { get; set; } = new Point(0, 0);

        // animation arguments
        public List<Bitmap> curbitmapList { get; set; } = new List<Bitmap>();
        public Point curPoint { get; set; } = new Point(0, 0);
        public Size curSize { get; set; } = new Size(0, 0);
        public Size boundSize { get; set; } = new Size(0, 0);
        public int  curIndex { get; set; } = 0;

        public MoveArrow curMoveArrow { get; set; } = MoveArrow.Default;
        public Wall _wall { get; set; } = Wall.Default;
        public float _angle { get; set; } = 0;
        public float _scale { get; set; } = 1;
        public bool _isVisible { get; set; } = true;
        public int rowMaxCount = 10;

        public ActivatedSprite(Sprite sprite)
        {
            _sprite = sprite;
            //_spriteBit
            //_normalSpriteBit.Add(new Bitmap(new Bitmap(sprite._localPath)));
            //if (_sprite._isUser)
            //{
            //    SetTransparentBit(_normalSpriteBit[0]);
            //}

            //_bigSpriteBit.Add(new Bitmap(_normalSpriteBit[0], new Size((int)(_normalSpriteBit[0].Width * 1.92f), (int)(_normalSpriteBit[0].Height * 1.94f))));
            //_spriteBit.Add(_normalSpriteBit[0]);

            //_miniView = new SpriteMiniView()
            //{
            //    spriteImage = _spriteBit[0],
            //    spriteName = sprite._name,
            //    path = sprite._localPath,
            //    formBackImage = Properties.Resources.Sprite_Info2,
            //};

            //_miniView.miniViewLClick += MiniViewLClick;
            //_miniView.miniViewRClick += MiniViewRClick;
            //_miniView.miniViewDBClick += MiniViewDBClick;
        }

        public ActivatedSprite(string sprite) {
            
        }


        public void CodeRunThread(object arg)
        {
        START:
            int loopCnt = 0;
            int loopStartTab = 0;
            int loopStartIdx = 0;

            for (int tab = 0; tab < mBlocks.Count; tab++)
            {
                for (int idx = 0; idx < mBlocks[tab].Count; idx++)
                {
                    try
                    {
                    SAVE:
                        Block block = mBlocks[tab][idx];
                        string blockName = mBlocks[tab][idx].name;

                        if (blockName.Equals("ControlLoop")) goto START;
                        else if (blockName.Equals("ControlLoopN") || blockName.Equals("GameLoopN"))
                        {
                            if (!System.Int32.TryParse(block.text, out int n))
                            {
                                //new MsgBoxForm($"{i + 1}번째 그림, {tab + 1}페이지, {idx + 1}번째에 실행할 수 없는 값이 있습니다.").ShowDialog();
                            }
                            else if (n == 0)
                            {
                                continue;
                            }
                            else
                            {
                                if (++loopCnt != n)
                                {
                                    tab = loopStartTab;
                                    idx = loopStartIdx;

                                    goto SAVE;
                                }
                                else
                                {
                                    if (idx < mBlocks[tab].Count - 1)
                                    {
                                        loopStartIdx = idx = idx + 1;
                                        loopStartTab = tab;
                                    }
                                    else
                                    {
                                        if ((idx == rowMaxCount - 1) && (tab < mBlocks.Count - 1))
                                        {
                                            loopStartIdx = idx = 0;
                                            loopStartTab = tab = tab + 1;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }

                                    loopCnt = 0;
                                    goto SAVE;
                                }
                            }
                        }
                        else if (blockName.Equals("ControlFlag"))
                        {
                            if (idx < mBlocks[tab].Count - 1)
                            {
                                loopStartIdx = idx + 1;
                                loopStartTab = tab;
                            }
                            else
                            {
                                if ((idx == rowMaxCount - 1) && (tab < mBlocks.Count - 1))
                                {
                                    loopStartIdx = 0;
                                    loopStartTab = tab + 1;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            continue;
                        }
                        else if (blockName.Equals("MoveRDownN")) MoveVariable(MoveArrow.RightDown, block.text);
                        else if (blockName.Equals("MoveRUpN")) MoveVariable(MoveArrow.RightUp, block.text);
                        else if (blockName.Equals("MoveLDownN")) MoveVariable(MoveArrow.LeftDown, block.text);
                        else if (blockName.Equals("MoveLUpN")) MoveVariable(MoveArrow.LeftUp, block.text);
                        else if (blockName.Equals("MoveRight1")) MoveConstant(MoveArrow.Right, 1, true);
                        else if (blockName.Equals("MoveRight5")) MoveConstant(MoveArrow.Right, 5, true);
                        else if (blockName.Equals("MoveRight10")) MoveConstant(MoveArrow.Right, 10, true);
                        else if (blockName.Equals("MoveRightN")) MoveVariable(MoveArrow.Right, block.text);
                        else if (blockName.Equals("MoveDown1")) MoveConstant(MoveArrow.Down, 1, true);
                        else if (blockName.Equals("MoveDown5")) MoveConstant(MoveArrow.Down, 5, true);
                        else if (blockName.Equals("MoveDown10")) MoveConstant(MoveArrow.Down, 10, true);
                        else if (blockName.Equals("MoveDownN")) MoveVariable(MoveArrow.Down, block.text);
                        else if (blockName.Equals("MoveLeft1")) MoveConstant(MoveArrow.Left, 1, true);
                        else if (blockName.Equals("MoveLeft5")) MoveConstant(MoveArrow.Left, 5, true);
                        else if (blockName.Equals("MoveLeft10")) MoveConstant(MoveArrow.Left, 10, true);
                        else if (blockName.Equals("MoveLeftN")) MoveVariable(MoveArrow.Left, block.text);
                        else if (blockName.Equals("MoveUp1")) MoveConstant(MoveArrow.Up, 1, true);
                        else if (blockName.Equals("MoveUp5")) MoveConstant(MoveArrow.Up, 5, true);
                        else if (blockName.Equals("MoveUp10")) MoveConstant(MoveArrow.Up, 10, true);
                        else if (blockName.Equals("MoveUpN")) MoveVariable(MoveArrow.Up, block.text);
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
                        else if (blockName.Equals("ActionRRotateN")) RotateArrowValue(true, block.text);
                        else if (blockName.Equals("ActionLRotateN")) RotateArrowValue(false, block.text);
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
                        else if (blockName.Equals("ControlTimeN")) SleepVariable(block.text);
                        //else if (blockName.Equals("ControlSpeak")) ControlSpeak(block.text);
                        //else if (blockName.Equals("ControlSound")) EffectSoundPlay(sprite._codes[tab][idx]._inputTextbox.Text);
                        //else if (blockName.Equals("GameRight")) TurnAndMoveForward(5);
                        //else if (blockName.Equals("GameDown")) TurnAndMoveForward(6);
                        //else if (blockName.Equals("GameLeft")) TurnAndMoveForward(7);
                        //else if (blockName.Equals("GameUp")) TurnAndMoveForward(8);
                        //else if (blockName.Equals("GameJump")) ArrowJump();

                        Thread.Sleep(10);

                    }
                    catch (ThreadAbortException)
                    {

                        //ActivatedSprite.mciSendString("close wav", null, 0, IntPtr.Zero);
                        //lock (sprite._lockObj)
                        //{
                        //    sprite._spriteBit[sprite._curSpriteNum] = new Bitmap(sprite.GetSpriteBit());
                        //    sprite._angle = 0.0f;
                        //    sprite._speakText = null;
                        //    sprite._isVisible = true;
                        //}
                        reset();
                    }
                }
            }
        }

        public void reset() {

        }

        public void setFullMode(bool isFull) {

        }


        public delegate void SafeCallDrawStage();
        public void InvalidateStage()
        {
            //if (MainForm.stageForm.InvokeRequired)
            //{
            //    var d = new SafeCallDrawStage(InvalidateStage);
            //    MainForm.stageForm.Invoke(d);
            //}
            //else
            //{
            //    int x = curPoint.X;
            //    int y = curPoint.Y;

            //    int width, height;
            //    lock (_lockObj)
            //    {
            //        width = _spriteBit[_curSpriteNum].Width;
            //        height = _spriteBit[_curSpriteNum].Height;
            //    }

            //    MainForm.stageForm.Invalidate(new Rectangle(x, y, width, height));
            //}
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

        //public void SetSpriteBit()
        //{
        //    if (!StagePlayer.isFullMode)
        //    {
        //        lock (_lockObj)
        //        {
        //            _spriteBit[_curSpriteNum] = _normalSpriteBit[_curSpriteNum];
        //        }
        //    }
        //    else
        //    {
        //        lock (_lockObj)
        //        {
        //            _spriteBit[_curSpriteNum] = _bigSpriteBit[_curSpriteNum];
        //        }
        //    }
        //}

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

            //lock (_lockObj)
            //{
            curbitmapList[curIndex] = GetSpriteBit();
            //}

            InvalidateStage();
        }

        public void SetNextBit()
        {
            InvalidateStage();

            int maxNum = curbitmapList.Count - 1;
            if (curIndex < maxNum)
            {
                curIndex++;
            }
            else
            {
                curIndex = 0;
            }

            //lock (_lockObj)
            //{
                curbitmapList[curIndex] = GetSpriteBit();
            //}

            InvalidateStage();
        }

        //public void SpeakText(string text)
        //{
        //    _speakText = text;
        //    MainForm.stageForm.Invalidate();
        //}

        //[DllImport("winmm.dll")]
        //public static extern Int32 mciSendString(string command, StringBuilder buffer, int bufferSize, IntPtr hwndCallback);
        //public void EffectSoundPlay(string path)
        //{
        //    string openPath = "open " + path + " alias wav type waveaudio wait";
        //    mciSendString(openPath, null, 0, IntPtr.Zero);

        //    StringBuilder lengthBuf = new StringBuilder(32);
        //    mciSendString("status wav length", lengthBuf, lengthBuf.Capacity, IntPtr.Zero);

        //    int length = 0;
        //    int.TryParse(lengthBuf.ToString(), out length);

        //    mciSendString("play wav", null, 0, IntPtr.Zero);
        //    Thread.Sleep(length);

        //    mciSendString("close wav", null, 0, IntPtr.Zero);
        //}

        public Bitmap GetSpriteBit()
        {
            return curbitmapList[curIndex];
        }

        public void FlipXSprite()
        {
            Matrix matrix = new Matrix();
            matrix.SetScale(1, -1);
            for (int i = 0; i < curbitmapList.Count; i++) {
                Bitmap bitmap = curbitmapList[i];
                Bitmap newBM = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, false);
                curbitmapList[i] = newBM;
            }
            InvalidateStage();
        }

        public void FlipYSprite()
        {
            Matrix matrix = new Matrix();
            matrix.SetScale(-1, 1);
            Bitmap bitmap = curbitmapList[curIndex];
            Bitmap newBM = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, false);
            curbitmapList[curIndex] = newBM;
            InvalidateStage();
        }

        public void AnimateSpritesLoop()
        {
            while (true)
            {
                SetNextBit();
                Thread.Sleep(1000);
            }
        }

        private double DegreeToRadian(double angle)
        {
            return Java.Lang.Math.Pi * angle / 180.0;
        }

        // 움직이는 거리, 움직임 횟수, 무한 반복 여부
        public void MoveSprite(int value, int n, MoveArrow moveArrow, int delay = 1)
        {
            switch (moveArrow)
            {
                case MoveArrow.RightUp:
                    for (int i = 0; i < n; i++) {
                        RightUpMove(value);
                        Thread.Sleep(delay);
                    }
                    break;
                case MoveArrow.RightDown:
                    for (int i = 0; i < n; i++)
                    {
                        RightDownMove(value);
                        Thread.Sleep(delay);
                    }
                    break;
                case MoveArrow.LeftDown:
                    for (int i = 0; i < n; i++)
                    {
                        LeftDownMove(value);
                        Thread.Sleep(delay);
                    }
                    break;
                case MoveArrow.LeftUp:
                    for (int i = 0; i < n; i++)
                    {
                        LeftUpMove(value);
                        Thread.Sleep(delay);
                    }
                    break;
                case MoveArrow.Right:
                    for (int i = 0; i < n; i++)
                    {
                        RightMove(value);
                        Thread.Sleep(delay);
                    }
                    break;
                case MoveArrow.Down:
                    for (int i = 0; i < n; i++)
                    {
                        DownMove(value);
                        Thread.Sleep(delay);
                    }
                    break;
                case MoveArrow.Left:
                    for (int i = 0; i < n; i++)
                    {
                        LeftMove(value);
                        Thread.Sleep(delay);
                    }
                    break;
                case MoveArrow.Up:
                    for (int i = 0; i < n; i++)
                    {
                        UpMove(value);
                        Thread.Sleep(delay);
                    }
                    break;
                default:
                    break;
            }
        }

        public int ChangeArrow(MoveArrow _arrow, Wall _wall)
        {
            int arrow = (int)_arrow;
            int wall = (int)_wall;
            if (arrow == wall)
            {
                if (arrow > 1)
                {
                    return arrow - 1;
                }
                else
                {
                    return 4;
                }
            }

            int arrowTemp = arrow;
            if (arrowTemp > 1)
            {
                arrowTemp--;
            }
            else
            {
                arrowTemp = 4;
            }

            if (arrowTemp == wall)
            {
                if (arrow < 4)
                {
                    return arrow + 1;
                }
                else
                {
                    return 1;
                }
            }

            return -1;
        }

        public void AddCode(Block code)
        {
            int editorTab = 0;
            //EffectSound.blockLinkSound.Play();
            if (mBlocks.Count != 0)
            {
                editorTab = (GetListCount() / rowMaxCount);
                //_maxEditorPageNumber = editorTab;
            }
            if (mBlocks.Count < editorTab + 1)
            {
                mBlocks.Add(new List<Block>());
            }
            mBlocks.Add(new List<Block>());
            mBlocks[editorTab].Add(code);
            //_programCnt++;x 
            //Editor.SetEditorLocation(editorTab);
            //_editorPageNumber = editorTab;
            //SpriteManager.invalidateEditor?.Invoke();
            //SpriteManager.invalidateBlocks?.Invoke();
        }

        public int GetListCount()
        {
            int cnt = 0;
            for (int i = 0; i < mBlocks.Count; i++)
            {
                for (int j = 0; j < mBlocks[i].Count; j++)
                {
                    cnt++;
                }
            }
            return cnt;
        }

        public void ClearCode()
        {
            for (int i = 0; i < mBlocks.Count; i++)
            {
                mBlocks[i].RemoveRange(0, mBlocks[i].Count);
            }
        }




        public void MoveConstant(MoveArrow arrow, int n, bool isDelay)
        {
            curMoveArrow = arrow;
            MoveSprite(n * 10, 1, curMoveArrow);

            if (isDelay)
            {
                Thread.Sleep(1000);
            }
        }

        public void MoveVariable(MoveArrow arrow, string valueString)
        {
            if (!System.Int32.TryParse(valueString, out int value))
            {
                //new MsgBoxForm("실행할 수 없는 값이 있습니다.").ShowDialog();
            }
            else
            {
                curMoveArrow = arrow;
                MoveSprite(value * 10, 1, curMoveArrow);
            }
        }

        public void ActionBounce()
        {
            if (curMoveArrow != MoveArrow.Default)
            {
                while (true)
                {
                    MoveArrow moveArrow = (MoveArrow)BounceMove(curMoveArrow);
                    if (moveArrow != MoveArrow.Default)
                    {
                        curMoveArrow = moveArrow;
                    }

                    Thread.Sleep(50);
                }
            }
        }

        public void ActionWave()
        {
            if (curMoveArrow == MoveArrow.Right)
            {
                WaveMove(true);
            }
            else if (curMoveArrow == MoveArrow.Left)
            {
                WaveMove(false);
            }
        }

        public void ActionTWave()
        {
            if (curMoveArrow == MoveArrow.Down)
            {
                WaveMoveVert(true);
            }
            else if (curMoveArrow == MoveArrow.Up)
            {
                WaveMoveVert(false);
            }
        }

        public void ActionZigzag()
        {
            if (curMoveArrow == MoveArrow.Right)
            {
                TWaveMove(true);
            }
            else if (curMoveArrow == MoveArrow.Left)
            {
                TWaveMove(false);
            }
        }

        public void ActionTZigzag()
        {
            if (curMoveArrow == MoveArrow.Down)
            {
                TWaveMoveVert(true);
            }
            else if (curMoveArrow == MoveArrow.Up)
            {
                TWaveMoveVert(false);
            }
        }

        public void ActionFast()
        {
            if (curMoveArrow == MoveArrow.Right)
            {
                FastMove(true);
            }
            else if (curMoveArrow == MoveArrow.Left)
            {
                FastMove(false);
            }
        }

        public  void ActionSlow()
        {
            if (curMoveArrow == MoveArrow.Right)
            {
                SlowMove(true);
            }
            else if (curMoveArrow == MoveArrow.Left)
            {
                SlowMove(false);
            }
        }

        public void RotateArrowValue(bool cw, string valueString)
        {
            if (!float.TryParse(valueString, out float n))
            {
                //new MsgBoxForm("실행할 수 없는 값이 있습니다.").ShowDialog();
            }
            else
            {
                RotateLoop(n, cw, false);
            }
        }

        public static void SleepVariable(string valueString)
        {
            if (!float.TryParse(valueString, out float value))
            {
                //new MsgBoxForm("실행할 수 없는 값이 있습니다.").ShowDialog();
            }
            else
            {
                int wait = (int)(value * 100.0f);
                Thread.Sleep(wait);
            }
        }

        public static void ControlSpeak(ActivatedSprite sprite, string text)
        {
            //sprite.SpeakText(text);
            //Thread.Sleep(2000);
            //sprite._speakText = null;
            //MainForm.stageForm.Invalidate();
            //Thread.Sleep(500);
        }

        /*
        *    ||
        *   \\//
        */
        public void DownMove(int n)
        {
            if (curPoint.Y + n + curSize.Height < boundSize.Height)
            {
                curPoint.Y += n;
            }
            InvalidateStage();
        }

        /*
        *   ===》
        */
        public void RightMove(int n)
        {
            if (curPoint.X + n + curSize.Width < boundSize.Width)
            {
                curPoint.X += n;
            }
            InvalidateStage();
        }

        /*
        *  《===
        */
        public void LeftMove(int n)
        {
            if (curPoint.X - n > 0)
            {
                curPoint.X -= n;
            }
            InvalidateStage();
        }

        /*
         *  //\\
         *   ||
        */
        public void UpMove(int n)
        {            
            if (curPoint.Y - n > 0)
            {
                curPoint.Y -= n;
            }
            InvalidateStage();
        }

        /*
         *    /|
         *   //
        */
        public Wall RightUpMove(int n)
        {
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
         *   \\
         *    \|
        */
        public Wall RightDownMove(int n)
        {
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

        /*
         *    //
         *   |/
        */
        public Wall LeftDownMove(int n)
        {
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
         *   |\
         *    \\
        */
        public Wall LeftUpMove(int n)
        {
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


        public void SlowMove(bool isRight)
        {
            while (true)
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
                        FlipXSprite();
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
                        FlipXSprite();
                        isRight = true;
                    }

                }
                InvalidateStage();
                Thread.Sleep(300);
            }
        }

        public void FastMove(bool isRight)
        {
            while (true)
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
                        FlipXSprite();
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
                        FlipXSprite();
                        isRight = true;
                    }
                }
                InvalidateStage();
                Thread.Sleep(100);
            }
        }

        public void ShowSprite(bool flag)
        {
            _isVisible = flag;
            InvalidateStage();
        }

        public void ShowHideLoop()
        {
            bool flag = true;
            while (true)
            {
                ShowSprite(flag ^= true);
                InvalidateStage();
                Thread.Sleep(1000);
            }
        }

        /**
         * 选择变换
         *
         * @param origin 原图
         * @param alpha  旋转角度，可正可负
         * @return 旋转后的图片
         */
        private Bitmap rotateBitmap(Bitmap origin, float angle)
        {
            if (origin == null)
            {
                return null;
            }
            _angle += angle;
            if (_angle >= 360.0f)
            {
                _angle -= ((int)(_angle/360.0f))*360;
            }
            int width = origin.Width;
            int height = origin.Height;
            Matrix matrix = new Matrix();
            matrix.SetRotate(_angle);
            Point center = new Point(curPoint.X+width/2, curPoint.Y + height/2);
            double tempAngle = Java.Lang.Math.Atan(Java.Lang.Math.Tan(height * 1.0 / width));
            double line = height / 2 / Java.Lang.Math.Sin(tempAngle);
            Point one = new Point(center.X + (int)(line * Java.Lang.Math.Cos(tempAngle + _angle)), center.Y + (int)(line * Java.Lang.Math.Sin(tempAngle + _angle)));
            Point two = new Point(center.X + (int)(line * Java.Lang.Math.Cos(tempAngle - _angle)), center.Y + (int)(line * Java.Lang.Math.Sin(tempAngle - _angle)));
            Point three = new Point(2*center.X - one.X , 2*center.Y - one.Y);
            Point four = new Point(2 * center.X - two.X, 2 * center.Y - two.Y);
            int w = (center.X - Java.Lang.Math.Min(Java.Lang.Math.Min(one.X, two.X), Java.Lang.Math.Min(three.X, four.X))) * 2;
            int h = (center.Y - Java.Lang.Math.Min(Java.Lang.Math.Min(one.Y, two.Y), Java.Lang.Math.Min(three.Y, four.Y))) * 2;
            curPoint = new Point(center.X-w/2, center.Y-h/2);
            curSize = new Size(w, h);
            Bitmap newBM = Bitmap.CreateBitmap(origin, 0, 0, w, h, matrix, false);            
            if (newBM.Equals(origin))
            {
                return newBM;
            }
            return newBM;
        }

        public void RotateLoop(float angle, bool cw, bool isInfinite)
        {
            float cnt = angle;
            while (true)
            {
                Bitmap temp = _originBitmapList[curIndex];
                temp = rotateBitmap(temp, cw ? cnt : 360.0f-cnt);
                curbitmapList[curIndex] = temp;
                InvalidateStage();
                if (!isInfinite)
                    break;
                Thread.Sleep(250);
            }
        }

        public void WaveMove(bool isRight)
        {
            int zero = curPoint.Y;
            while (true)
            {
                // 오른쪽 물결 움직이기
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
                            FlipXSprite();
                            isRight = false;
                            break;
                        }

                        InvalidateStage();
                        Thread.Sleep(100);
                    }
                }
                // 왼쪽 물결 움직이기
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
                            FlipXSprite();
                            isRight = true;
                            break;
                        }
                        InvalidateStage();
                        Thread.Sleep(100);
                    }
                }
            }
        }

        public void WaveMoveVert(bool isDown)
        {
            int zero = curPoint.X;
            while (true)
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
                        Thread.Sleep(200);
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
                        Thread.Sleep(200);
                    }
                }
            }
        }

        public void RandomMove()
        {
            while (true)
            {
                int random = (int)(1 + Java.Lang.Math.Random() * (9 - 1 + 1)); //1~8 move arrow
                int n = (int)(1 + Java.Lang.Math.Random() * (6 - 1 + 1)); // 1~5 delay time
                //_arrow = new Random(Guid.NewGuid().GetHashCode()).Next(1, 9); // 1~8 난수 생성
                //int n = new Random(Guid.NewGuid().GetHashCode()).Next(1, 6);
                MoveSprite(10, n, (MoveArrow)random, 100);
            }
        }

        public void TWaveMove(bool isRight)
        {
            int zero = curPoint.Y;
            while (true)
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
                            FlipXSprite();
                            isRight = false;
                            break;
                        }

                        InvalidateStage();
                        Thread.Sleep(100);
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
                            FlipXSprite();
                            isRight = true;
                            break;
                        }

                        InvalidateStage();
                        Thread.Sleep(100);
                    }
                }
            }
        }

        public void TWaveMoveVert(bool isDown)
        {
            int zero = curPoint.X;
            while (true)
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
                        Thread.Sleep(200);
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
                        Thread.Sleep(200);
                    }
                }
            }
        }

        public int BounceMove(MoveArrow arrow)
        {
            // 1 : 오른쪽 위, 2 : 오른쪽 아래, 3 : 왼쪽 아래, 4 : 왼쪽 위             
            Wall wall;
            switch (arrow)
            {
                case MoveArrow.RightUp:
                    wall = RightUpMove(10);
                    if (wall != Wall.Default)
                        return ChangeArrow(arrow, wall);
                    break;
                case MoveArrow.RightDown:
                    wall = RightDownMove(10);
                    if (wall != Wall.Default)
                        return ChangeArrow(arrow, wall);
                    break;
                case MoveArrow.LeftDown:
                    wall = LeftDownMove(10);
                    if (wall != Wall.Default)
                        return ChangeArrow(arrow, wall);
                    break;
                case MoveArrow.LeftUp:
                    wall = LeftUpMove(10);
                    if (wall != Wall.Default)
                        return ChangeArrow(arrow, wall);
                    break;
                default:
                    break;
            }
            return -1;
        }

        public void JumpSprite()
        {
            for (int i = 5; i > 0; i--)
            {
                MoveSprite(i * 10, 1, MoveArrow.Up, 100);
            }
            for (int i = 1; i <= 5; i++)
            {
                MoveSprite(i * 10, 1, MoveArrow.Down, 100);
            }
        }

        public void RightLeftJumpLoop()
        {
            MoveArrow moveArrow = MoveArrow.Left;
            while (true)
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
                    Thread.Sleep(delay);
                    InvalidateStage();
                }
                curPoint.X += (6 - (int)moveArrow);
                Thread.Sleep(100);
            }
        }

        //public void ArrowJump()
        //{
        //    if (((int)curMoveArrow == 5 || (int)curMoveArrow == 7) && MoveGame.moveGame.CheckPath(_arrow, curPoint.X, curPoint.Y, 200))
        //    {
        //        EffectSound.moveSuccessedSound.Play();

        //        lock (_lockObj)
        //        {
        //            _spriteBit[_curSpriteNum] = new Bitmap(MoveGame.moveGame.jumpBitmapList[_curSpriteNum]);
        //        }

        //        int delay = 5;
        //        int zero = curPoint.Y;
        //        for (double i = 0; i <= 180; i++)
        //        {
        //            curPoint.Y = (int)(-Java.Lang.Math.Sin(DegreeToRadian(i)) * 100.0) + zero;

        //            if (_arrow == 5)
        //            {
        //                curPoint.X += 1;
        //            }
        //            else if (_arrow == 7)
        //            {
        //                curPoint.X -= 1;
        //            }

        //            if (i % 10 == 0 && i != 0)
        //            {
        //                if (_arrow == 5)
        //                {
        //                    curPoint.X += 1;
        //                }
        //                else if (_arrow == 7)
        //                {
        //                    curPoint.X -= 1;
        //                }
        //            }


        //            if (i % 10 == 0 && i != 0 && i < 91)
        //            {
        //                delay++;
        //            }
        //            else if (i % 10 == 0 && i != 0 && i > 90)
        //            {
        //                delay--;
        //            }

        //            Thread.Sleep(delay);
        //            MainForm.stageForm.Invalidate();
        //        }

        //        if (_arrow == 5)
        //        {
        //            curPoint.X += 1;
        //        }
        //        else if (_arrow == 7)
        //        {
        //            curPoint.X -= 1;
        //        }

        //        lock (_lockObj)
        //        {
        //            _spriteBit[_curSpriteNum] = new Bitmap(MoveGame.moveGame.bitmapList[_curSpriteNum]);
        //        }

        //        Thread.Sleep(500);
        //    }
        //    else
        //    {
        //        EffectSound.moveFailedSound.Play();
        //    }
        //}

        //public void TurnAndMoveForward(int target)
        //{
        //    int diff = _arrow - target;
        //    if (diff != 0)
        //    {
        //        int cnt = Java.Lang.Math.Abs(diff);
        //        for (int i = 0; i < cnt; i++)
        //        {
        //            if (diff > 0)
        //            {
        //                SetPrevBit();
        //            }
        //            else
        //            {
        //                SetNextBit();
        //            }
        //        }

        //        _arrow = target;
        //        Thread.Sleep(500);
        //    }

        //    MoveForward();
        //}

        //public void MoveForward()
        //{
        //    if (MoveGame.moveGame.CheckPath(_arrow, curPoint.X, curPoint.Y))
        //    {
        //        MoveSprite(100, 1);
        //        EffectSound.moveSuccessedSound.Play();
        //    }
        //    else
        //    {
        //        EffectSound.moveFailedSound.Play();
        //    }

        //    Thread.Sleep(1000);
        //}
    }


}