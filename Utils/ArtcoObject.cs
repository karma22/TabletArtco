using System.Collections.Generic;
using System.IO;
using System.Text;
using Android.Graphics;
using Java.Lang;

namespace TabletArtco
{
    class ArtcoObject
    {
        public List<Block> blocks { get; set; } = new List<Block>();
        public List<Bitmap> images { get; set; } = new List<Bitmap>();
        public List<int> imgSizes { get; set; } = new List<int>();
        public string name { get; set; }
        public int x { get; set; }
        public int y { get; set; }

        public bool SaveObject(ActivatedSprite sprite, string name)
        {
            //string externalDir = Android.OS.Environment.ExternalStorageDirectory.Path; //"/storage/emulated/0"
            string dirPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + "/object";
            string filePath = dirPath + "/" + name + ".ArtcoObject";

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            if (File.Exists(filePath))
            {
                //using MsgBoxForm msgBox = new MsgBoxForm("该文件已存在, 是否覆盖?", true);
                //msgBox.ShowDialog();
                //if (msgBox.DialogResult == System.Windows.Forms.DialogResult.No)
                //    return false;
            }

            string header = sprite.sprite.name + "\n";
            header += sprite.curPoint.X.ToString() + ":" + sprite.curPoint.Y.ToString() + "\n";

            var codes = sprite.mBlocks;
            int codeCount = 0;
            for (int i = 0; i < codes.Count; i++)
                codeCount += codes[i].Count;

            header += codeCount.ToString() + "\n";

            for (int i = 0; i < codes.Count; i++)
            {
                for (int j = 0; j < codes[i].Count; j++)
                {
                    var code = codes[i][j];
                    //if (code.blockView.controls != null)
                    //{
                    //    string values = code.name;
                    //    for (int k = 0; k < code.blockView.controls.Count; k++)
                    //        values += ":" + code.blockView.controls[k].Text;

                    //    values += "\n";
                    //    header += values;
                    //}
                    //else
                    //{
                        header += code.name + "\n";
                    //}
                }
            }

            int spriteImgCnt = sprite.originBitmapList.Count;
            header += spriteImgCnt.ToString() + "\n";

            List<byte[]> bytes = new List<byte[]>();

            for (int i = 0; i < spriteImgCnt; i++)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    sprite.originBitmapList[i].Compress(Bitmap.CompressFormat.Png, 100, ms);
                    bytes.Add(ms.ToArray());
                }

                header += bytes[i].Length.ToString() + "\n";
            }

            int headerSize = Encoding.UTF8.GetBytes(header).Length;

            try
            {
                using (StreamWriter wr = new StreamWriter(filePath))
                {
                    wr.Write(header);
                    wr.Write(headerSize.ToString() + "\n");
                }

                for (int i = 0; i < spriteImgCnt; i++)
                {
                    using FileStream file = new FileStream(filePath, FileMode.Append, FileAccess.Write);
                    file.Write(bytes[i], 0, bytes[i].Length);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool LoadObject(string path)
        {
            //try
            //{
            //    using StreamReader rdr = new StreamReader(path);

            //    string name = rdr.ReadLine();
            //    string[] splits = rdr.ReadLine().Split(':');
            //    int x = int.Parse(splits[0]);
            //    int y = int.Parse(splits[1]);

            //    List<Block> codes = new List<Block>();
            //    List<string> values = new List<string>();

            //    int codeCnt = int.Parse(rdr.ReadLine());
            //    for (int i = 0; i < codeCnt; i++)
            //    {
            //        string codeName = rdr.ReadLine();
            //        var split = codeName.Split(':');

            //        Block code;
            //        if (split.Length > 1)
            //        {
            //            code = new Block(Block.GetBlockByName(split[0]));
            //            string value = split[1];
            //            for (int j = 2; j < split.Length; j++)
            //                value += ":" + split[j];

            //            values.Add(value);
            //        }
            //        else
            //        {
            //            code = new Block(Block.GetBlockByName(split[0]));
            //            values.Add(null);
            //        }

            //        codes.Add(code);
            //    }

            //    int spriteCnt = int.Parse(rdr.ReadLine());
            //    List<int> spriteSizes = new List<int>();
            //    for (int i = 0; i < spriteCnt; i++)
            //    {
            //        spriteSizes.Add(int.Parse(rdr.ReadLine()));
            //    }

            //    string header = rdr.ReadLine();
            //    int headerLength = header.Length + 1;
            //    int startPoint = int.Parse(header) + headerLength;

            //    List<Bitmap> bmpList = new List<Bitmap>();
            //    for (int i = 0; i < spriteCnt; i++)
            //    {
            //        using FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
            //        file.Seek(startPoint, SeekOrigin.Begin);
            //        byte[] bytes = new byte[spriteSizes[i]];
            //        int readSize = file.Read(bytes, 0, spriteSizes[i]);
            //        bmpList.Add(BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length));
            //        startPoint += readSize;
            //    }

            //    Sprite sprite = new Sprite(name, null, false, null);
            //    sprite.SetSaveBitmapList(bmpList);

            //    MainForm._selectSpriteHandler?.Invoke(sprite);

            //    ActivatedSprite._activatedSprites[ActivatedSprite._curSpriteNum].cx = x;
            //    ActivatedSprite._activatedSprites[ActivatedSprite._curSpriteNum].cy = y;

            //    for (int i = 0; i < codes.Count; i++)
            //    {
            //        ActivatedSprite._activatedSprites[ActivatedSprite._curSpriteNum].spriteEditor.AddCode(codes[i]);
            //        if (values[i] != null)
            //        {
            //            string[] value = values[i].Split(':');
            //            if (value.Length > 1)
            //            {
            //                for (int j = 0; j < codes[i].blockView.controls.Count; j++)
            //                    codes[i].blockView.controls[j].Text = value[j];

            //                UserVariableManager.AddVariable(value[0], double.Parse(value[1]));
            //            }
            //            else
            //            {
            //                codes[i].blockView.controls[0].Text = value[0];
            //            }
            //        }
            //    }
            //}
            //catch (Exception)
            //{
            //    return false;
            //}

            return true;
        }

    }
}