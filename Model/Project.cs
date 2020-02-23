using System;
using System.Collections.Generic;

namespace TabletArtco
{

    public interface UpdateDelegate
    {
        void UpdateView();
    }

    class Project
    {
        public static List<Background> backgroundsList { get; set; } = new List<Background>();
        public static List<ActivatedSprite> mSprites { get; set; } = new List<ActivatedSprite>();
        public static List<List<Block>> blocksList = new List<List<Block>>();
        public static Dictionary<string, string> variableMap = new Dictionary<string, string>();
        public static Dictionary<string, string> curVariableMap = new Dictionary<string, string>();
        public static List<string> signalList = new List<string>();

        public static bool isMove { get; set; }
        public static int curSpriteNum { get; set; } = -1;
        
        public static List<string> blocksBuffer { get; set; } = new List<string>();
        public static List<Java.Lang.Thread> codeThreadList { get; set; } = new List<Java.Lang.Thread>();

       
        public static void SpriteSizeChange(bool isFool)
        {
            for (int i = 0; i < mSprites.Count; i++)
            {
                //mSprites[i].setFullMode(isFool);
            }
        }

        public static void AddSprite(Sprite sprite, bool isClone = false)
        {
            ActivatedSprite activatedSprite = new ActivatedSprite(sprite, isClone);
            mSprites.Add(activatedSprite);
        }

        public static void CopyBlocks(List<List<Block>> list) {
            blocksList.RemoveRange(0, blocksList.Count);
            for (int i = 0; i < list.Count; i++)
            {
                List<Block> temp = list[i];
                List<Block> arr = new List<Block>();
                for (int j = 0; j < temp.Count; j++)
                {
                    Block block = temp[j];
                    arr.Add(Block.Copy(block));
                }
                blocksList.Add(arr);
            }
        }

        public static List<List<Block>> PasteBlocks() {
            List<List<Block>> list = new List<List<Block>>();
            for (int i = 0; i < blocksList.Count; i++)
            {
                List<Block> temp = blocksList[i];
                List<Block> arr = new List<Block>();
                for (int j = 0; j < temp.Count; j++)
                {
                    Block block = temp[j];
                    arr.Add(Block.Copy(block));
                }
                list.Add(arr);
            }
            return list;
        }

        public static void RunSprite()
        {

            Android.Util.Log.Info("RunCode", "-----------------------");
            for (int i = 0; i < Project.mSprites.Count; i++)
            {
                ActivatedSprite sprite = Project.mSprites[i];
                Java.Lang.Thread thread = new Java.Lang.Thread(new Java.Lang.Runnable(() =>
                {
                    sprite.Start();
                }));
                codeThreadList.Add(thread);
            }

            ActivatedSprite.isAnimationTag = true;
            for (int i = 0; i < Project.mSprites.Count; i++)
            {
                codeThreadList[i].Start();
            }
        }

        public static void StopSprite()
        {
            Android.Util.Log.Info("StopCode", "-----------------------");
            ActivatedSprite.isAnimationTag = false;
            for (int i = 0; i < Project.mSprites.Count; i++)
            {
                var sprite = Project.mSprites[i];
                sprite.Reset();
            }
            codeThreadList.RemoveRange(0, codeThreadList.Count);
        }

    }
}
