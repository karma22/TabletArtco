using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Android.Content;
using Android.Graphics;
using Java.Lang;

namespace TabletArtco
{
    class ArtcoProject
    {
        private Context context;

        public ArtcoProject(Context context)
        {
            this.context = context;
        }

        public bool SaveProject(string name, bool isCover = false)
        {
            if (!Android.OS.Environment.ExternalStorageState.Equals(Android.OS.Environment.MediaMounted))
            {
                ToastUtil.ShowToast(context, "没有存储权限，请在设置->应用管理->Artco打开权限");
                return false;
            }

            string dirPath = UserDirectoryPath.projectPath;
            string filePath = dirPath + "/" + name + ".ArtcoProject";

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            if (File.Exists(filePath) && !isCover)
            {
                MessageBoxDialog dialog = new MessageBoxDialog(context, "该文件已存在, 是否覆盖?", () =>
                {
                    SaveProject(name, true);
                });
                dialog.Show();
                return false;
            }

            int headerSize = 0;
            string backgroundName = Project.currentBack != null ? Project.currentBack.name + "\n" : "null\n" ;
            string bgMusic = SoundPlayer.bgmPath != null ? SoundPlayer.bgmPath + "\n" : "null\n";
            string variableCount = Variable.variableMap.Count.ToString() + "\n";
            string spriteCount = Project.mSprites.Count.ToString() + "\n";

            try
            {
                List<List<byte[]>> bytes = new List<List<byte[]>>();

                using (StreamWriter wr = new StreamWriter(filePath))
                {
                    wr.Write(backgroundName);
                    wr.Write(bgMusic);
                    wr.Write(variableCount);
                    wr.Write(spriteCount);
                }

                string header = string.Empty;
                foreach (var key in Variable.variableMap.Keys)
                {
                    string val = Variable.variableMap[key].ToString();
                    header += key + ":" + val + "\n";
                }

                for (int i = 0; i < Project.mSprites.Count; i++)
                {
                    ActivatedSprite sprite = Project.mSprites[i];
                    
                    header += sprite.sprite.name + "\n";
                    header += sprite.curPoint.X.ToString() + ":" + sprite.curPoint.Y.ToString() + "\n";

                    var codes = sprite.mBlocks;
                    int codeCount = 0;
                    for (int j = 0; j < codes.Count; j++)
                        codeCount += codes[j].Count;

                    header += codeCount.ToString() + "\n";

                    for (int j = 0; j < codes.Count; j++)
                    {
                        for (int k = 0; k < codes[j].Count; k++)
                        {
                            var code = codes[j][k];

                            header += code.name;
                            if (code.text != null && code.text.Length > 0)
                                header += ">>text>>" + code.text;
                            if (code.varName != null && code.varName.Length > 0)
                                header += ">>varName>>" + code.varName;
                            if (code.varValue != null && code.varValue.Length > 0)
                                header += ">>varValue>>" + code.varValue;
                            if (code.backgroundName != null)
                                header += ">>backgroundName>>" + code.backgroundName;
                            header += "\n";
                        }
                    }

                    int spriteImgCnt = sprite.originBitmapList.Count;
                    header += spriteImgCnt.ToString() + "\n";

                    bytes.Add(new List<byte[]>());
                    for (int j = 0; j < spriteImgCnt; j++)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            sprite.originBitmapList[j].Compress(Bitmap.CompressFormat.Png, 100, ms);
                            bytes[i].Add(ms.ToArray());
                        }

                        header += bytes[i][j].Length.ToString() + "\n";
                    }
                }

                headerSize += Encoding.UTF8.GetBytes(header).Length;
                headerSize += Encoding.UTF8.GetBytes(backgroundName).Length;
                headerSize += Encoding.UTF8.GetBytes(bgMusic).Length;
                headerSize += Encoding.UTF8.GetBytes(variableCount).Length;
                headerSize += Encoding.UTF8.GetBytes(spriteCount).Length;

                using (StreamWriter wr = new StreamWriter(filePath, true))
                {
                    wr.Write(header + headerSize.ToString() + "\n");
                }

                for (int i = 0; i < bytes.Count; i++)
                {
                    for (int j = 0; j < bytes[i].Count; j++)
                    {
                        using FileStream file = new FileStream(filePath, FileMode.Append, FileAccess.Write);
                        file.Write(bytes[i][j], 0, bytes[i][j].Length);
                    }
                }
            }
            catch (Java.Lang.Exception)
            {
                return false;
            }

            return true;
        }

        public bool LoadProject(string path)
        {
            try
            {
                using StreamReader rdr = new StreamReader(path);
                string headersize;
                List<ArtcoObject> objects = new List<ArtcoObject>();
                List<List<string>> values = new List<List<string>>();

                string backgroundname = rdr.ReadLine();
                Project.currentBack = Background.GetNameToBack(backgroundname);
                string bgMusic = rdr.ReadLine();
                SoundPlayer.bgmPath = bgMusic;
                int variablecount = int.Parse(rdr.ReadLine());
                int spritecount = int.Parse(rdr.ReadLine());

                for (int i = 0; i < variablecount; i++)
                {
                    string[] splits = rdr.ReadLine().Split(':');
                    Variable.AddVariable(splits[0], splits[1]);
                }

                for (int i = 0; i < spritecount; i++)
                {
                    ArtcoObject artcoobject = new ArtcoObject(context);
                    artcoobject.name = rdr.ReadLine();

                    string[] splits = rdr.ReadLine().Split(':');
                    artcoobject.x = int.Parse(splits[0]);
                    artcoobject.y = int.Parse(splits[1]);

                    values.Add(new List<string>());
                    int codecnt = int.Parse(rdr.ReadLine());

                    for (int j = 0; j < codecnt; j++)
                    {
                        string codeName = rdr.ReadLine();
                        string[] split = codeName.Split(">>");
                        Block code = Block.Copy(Block.GetBlockByName(split[0]));
                        for (int k = 1; k < split.Length; k += 2)
                        {
                            if (split[k].Equals("text"))
                            {
                                code.text = split[k + 1];
                            }
                            else if (split[k].Equals("varName"))
                            {
                                code.varName = split[k + 1];
                            }
                            else if (split[k].Equals("varValue"))
                            {
                                code.varValue = split[k + 1];
                            }
                            else if (split[k].Equals("backgroundName"))
                            {
                                code.backgroundName = split[k + 1];
                                if (!Project.backgroundsList.ContainsKey(code.backgroundName)) {
                                    Project.backgroundsList.Add(code.backgroundName, Background.GetNameToBack(code.backgroundName));
                                }
                            }
                        }
                        artcoobject.blocks.Add(code);
                    }
                    int spritecnt = int.Parse(rdr.ReadLine());
                    for (int j = 0; j < spritecnt; j++)
                    {
                        artcoobject.imgSizes.Add(int.Parse(rdr.ReadLine()));
                    }
                    objects.Add(artcoobject);
                }
                headersize = rdr.ReadLine();
                int headerlength = headersize.Length + 1;
                int startpoint = int.Parse(headersize) + headerlength;

                for (int i = 0; i < objects.Count; i++)
                {
                    for (int j = 0; j < objects[i].imgSizes.Count; j++)
                    {
                        int imgsize = objects[i].imgSizes[j];
                        using FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
                        file.Seek(startpoint, SeekOrigin.Begin);
                        byte[] bytes = new byte[imgsize];
                        int readSize = file.Read(bytes, 0, imgsize);
                        Bitmap bitmap = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
                        objects[i].images.Add(Bitmap.CreateScaledBitmap(bitmap, bitmap.Width - 1, bitmap.Height - 1, false));
                        startpoint += readSize;
                    }

                    Sprite sprite = new Sprite();
                    sprite.name = objects[i].name;
                    sprite.category = 100;
                    sprite.bitmap = Bitmap.CreateBitmap(objects[i].images[0]);
                    Project.AddSprite(sprite);
                    int lastSprite = Project.mSprites.Count - 1;
                    Project.mSprites[lastSprite].SetSrcBitmapList(objects[i].images);
                    Project.mSprites[lastSprite].originPoint.X = objects[i].x;
                    Project.mSprites[lastSprite].originPoint.Y = objects[i].y;
                    Project.mSprites[lastSprite].curPoint.X = objects[i].x;
                    Project.mSprites[lastSprite].curPoint.Y = objects[i].y;
                    for (int j = 0; j < objects[i].blocks.Count; j++)
                    {
                        Project.mSprites[lastSprite].AddBlock(objects[i].blocks[j]);
                    }
                }
            }
            catch (Java.Lang.Exception e)
            {
                e.PrintStackTrace();
                return false;
            }
            return true;
        }
    }
}