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

        public bool SaveProject(string name)
        {
            string dirPath = UserDirectoryPath.projectPath;
            string filePath = dirPath + "/" + name + ".ArtcoProject";

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

            int headerSize = 0;
            string backgroundName = Project.currentBack != null ? Project.currentBack.name + "\n" : "null\n" ;
            string variableCount = Variable.variableMap.Count.ToString() + "\n";
            string spriteCount = Project.mSprites.Count.ToString() + "\n";

            try
            {
                List<List<byte[]>> bytes = new List<List<byte[]>>();

                using (StreamWriter wr = new StreamWriter(filePath))
                {
                    wr.Write(backgroundName);
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
                    
                    header = sprite.sprite.name + "\n";
                    header += sprite.curPoint.X.ToString() + ":" + sprite.curPoint.Y.ToString() + "\n";

                    var codes = sprite.mBlocks;
                    int codeCount = 0;
                    for (int j = 0; j < codes.Count; j++)
                        codeCount += codes[i].Count;

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
                            if (code.backgroundId != -1)
                                header += ">>backgroundId>>" + code.backgroundId;

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
                            sprite.originBitmapList[i].Compress(Bitmap.CompressFormat.Png, 100, ms);
                            bytes[j].Add(ms.ToArray());
                        }

                        header += bytes[i][j].Length.ToString() + "\n";
                    }
                }

                headerSize += Encoding.UTF8.GetBytes(header).Length;
                headerSize += Encoding.UTF8.GetBytes(backgroundName).Length;
                headerSize += Encoding.UTF8.GetBytes(variableCount).Length;
                headerSize += Encoding.UTF8.GetBytes(spriteCount).Length;

                using (StreamWriter wr = new StreamWriter(filePath, false))
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


            return false;
        }
    }
}