using System.Collections.Generic;
using System.IO;
using System.Text;
using Android.Content;
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

        private Context context;

        public ArtcoObject(Context context)
        {
            this.context = context;
        }

        public bool SaveObject(ActivatedSprite sprite, string name)
        {
            //string dirPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + "/object";
            string dirPath = UserDirectoryPath.objectPath;
            string filePath = dirPath + "/" + name + ".ArtcoObject";

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            //if (File.Exists(filePath))
            //{
            //    MessageBoxDialog dialog = new MessageBoxDialog(context, "该文件已存在, 是否覆盖?", () => {

            //    });
            //    dialog.Show();
            //}

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
                using (StreamWriter wr = new StreamWriter(filePath, false))
                {
                    wr.Write(header + headerSize.ToString() + "\n");
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
            try
            {
                using StreamReader rdr = new StreamReader(path);

                string name = rdr.ReadLine();
                string[] splits = rdr.ReadLine().Split(':');
                int x = int.Parse(splits[0]);
                int y = int.Parse(splits[1]);

                List<Block> codes = new List<Block>();
                List<string> values = new List<string>();

                int codeCnt = int.Parse(rdr.ReadLine());
                for (int i = 0; i < codeCnt; i++)
                {
                    string codeName = rdr.ReadLine();
                    var split = codeName.Split(':');

                    Block code = Block.GetBlockByName(split[0]);
                    if (split.Length > 1)
                    {
                        //code = new Block(Block.GetBlockByName(split[0]));
                        //string value = split[1];
                        //for (int j = 2; j < split.Length; j++)
                        //    value += ":" + split[j];

                        //values.Add(value);
                    }
                    else
                    {
                        //code = new Block(Block.GetBlockByName(split[0]));
                        //values.Add(null);
                    }

                    codes.Add(code);
                }

                int spriteCnt = int.Parse(rdr.ReadLine());
                List<int> spriteSizes = new List<int>();
                for (int i = 0; i < spriteCnt; i++)
                {
                    spriteSizes.Add(int.Parse(rdr.ReadLine()));
                }

                string header = rdr.ReadLine();
                int headerLength = header.Length + 1;
                int startPoint = int.Parse(header) + headerLength;

                List<Bitmap> bmpList = new List<Bitmap>();
                for (int i = 0; i < spriteCnt; i++)
                {
                    using FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
                    file.Seek(startPoint, SeekOrigin.Begin);
                    byte[] bytes = new byte[spriteSizes[i]];
                    int readSize = file.Read(bytes, 0, spriteSizes[i]);
                    bmpList.Add(BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length));
                    startPoint += readSize;
                }

                Sprite sprite = new Sprite();
                sprite.name = name;
                sprite.bitmap = bmpList[0];
                Project.AddSprite(sprite);
                int lastSprite = Project.mSprites.Count - 1;
                //Project.mSprites[lastSprite].originBitmapList = bmpList;
                //Project.mSprites[lastSprite].curbitmapList = bmpList;
                Project.mSprites[lastSprite].originPoint.X = x;
                Project.mSprites[lastSprite].originPoint.Y = y;
                Project.mSprites[lastSprite].curPoint.X = x;
                Project.mSprites[lastSprite].curPoint.Y = y;

                for (int i = 0; i < codes.Count; i++)
                {
                    Project.mSprites[lastSprite].AddBlock(codes[i]);
                    //if (values[i] != null)
                    //{
                    //    string[] value = values[i].Split(':');
                    //    if (value.Length > 1)
                    //    {
                    //        for (int j = 0; j < codes[i].blockView.controls.Count; j++)
                    //            codes[i].blockView.controls[j].Text = value[j];

                    //        UserVariableManager.AddVariable(value[0], double.Parse(value[1]));
                    //    }
                    //    else
                    //    {
                    //        codes[i].blockView.controls[0].Text = value[0];
                    //    }
                    //}
                }
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

    }
}