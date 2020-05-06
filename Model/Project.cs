using System;
using System.Collections.Generic;

namespace TabletArtco
{

    public interface UpdateDelegate
    {
        void UpdateView();
        void UpdateBlockViewDelegate();
        void UpdateBackground(int backgroundId);

        void RowAnimateComplete();
    }

    class Project
    {
        // first background
        public static Background currentBack { get; set; } = null;
        // change backgrounds
        public static Dictionary<int, Background> backgroundsList { get; set; } = new Dictionary<int, Background>();
        // all sprites list
        public static List<ActivatedSprite> mSprites { get; set; } = new List<ActivatedSprite>();
        // Copy blocks list
        public static List<List<Block>> blocksList = new List<List<Block>>(); 
        // sprite animation thread
        public static List<Java.Lang.Thread> codeThreadList { get; set; } = new List<Java.Lang.Thread>();

        public static Dictionary<string, int> sendSignalWait { get; set; } = new Dictionary<string, int>();

        public static void ChangeMode(bool isFull)
        {
            ActivatedSprite.mIsFull = isFull;
            for (int i = 0; i < mSprites.Count; i++)
            {
                mSprites[i].ChangeMode();
            }
        }

        // Add Sprite
        public static void AddSprite(Sprite sprite, bool isClone = false)
        {
            ActivatedSprite activatedSprite = new ActivatedSprite(sprite, isClone);
            mSprites.Add(activatedSprite);
        }

        public static void ClearCollision(string collisionid) {
            for (int i = 0; i < mSprites.Count; i++)
            {
                mSprites[i].RemoveCollision(collisionid);
            }
        }

        // Copy Blocks
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

        // Paste Blocks
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

        // Run Sprite Animation
        public static void RunSprite()
        {
            Variable.InitCurVariable();
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

        // Stop Sprite Animation
        public static void StopSprite()
        {
            Android.Util.Log.Info("StopCode", "-----------------------");
            ActivatedSprite.isAnimationTag = false;
            Project.sendSignalWait.Clear();
            for (int i = 0; i < Project.mSprites.Count; i++)
            {
                var sprite = Project.mSprites[i];
                sprite.Reset();
            }
            codeThreadList.RemoveRange(0, codeThreadList.Count);
        }
    }

    class Variable {
        // variable init value
        public static Dictionary<string, string> variableMap = new Dictionary<string, string>();
        // variable current value
        public static Dictionary<string, string> curVariableMap = new Dictionary<string, string>();

        // add variable
        public static void AddVariable(string name, string value)
        {
            variableMap.Add(name, value);
        }

        // delete variable
        public static void RemoveVariable(string name)
        {
            variableMap.Remove(name);
            for (int i = 0; i < Project.mSprites.Count; i++)
            {
                ActivatedSprite activatedSprite = Project.mSprites[i];
                activatedSprite.RemoveVariable(name);
            }
        }

        // clear all variable
        public static void ClearVariable()
        {
            variableMap.Clear();
            for (int i = 0; i < Project.mSprites.Count; i++)
            {
                ActivatedSprite activatedSprite = Project.mSprites[i];
                activatedSprite.RemoveVariable();
            }
        }

        // init current variable
        public static void InitCurVariable()
        {
            curVariableMap.Clear();
            foreach (string name in variableMap.Keys)
            {
                curVariableMap.Add(name, variableMap[name]);
            }
        }
    }

    class Signal {
        
        public static void SendSignal(string signalName) {
            for (int i = 0; i < Project.mSprites.Count; i++)
            {
                ActivatedSprite activatedSprite = Project.mSprites[i];
                activatedSprite.ReceiveSignal(signalName);
            }
        }

    }

}
