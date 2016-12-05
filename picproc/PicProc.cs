using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Newtonsoft;
using Newtonsoft.Json;

namespace picproc
{
    public class PicProcOptions
    {
        public string DstFolder;
        public bool RenameBasedOnTime = true;
        public bool KeepOriginal = true;

        public string IndexFileName = "index.json";
        
        public string FixedHeightDst = "Height";
        public List<int> FixedHeightSizes = new List<int>() { 50, 200 };

        public string FixedWidthDst = "Width";
        public List<int> FixedWidthSizes = new List<int>() { 50, 200 };

        public string SquareCropDst = "Square";
        public List<int> SquareCropSizes = new List<int>() { 50, 200 };
    }

    public class PicInfo
    {
        public int H;
        public int W;
        public string Name;
    }


    public class PicProc
    {
        public PicProcOptions Options { get; set; }
        List<PicInfo> PicInfos = new List<PicInfo>();

        public PicProc(string dstFolder) : 
            this(new PicProcOptions
            {
                DstFolder = dstFolder
            })
        { }

        public PicProc(PicProcOptions options)
        {
           Options = options;
        }

        public void Process(List<string> inputFiles)
        {
            inputFiles.ForEach(inputFile =>
            {
                ProcessImage(inputFile);
            });

            SaveIndexFile();
        }

        public void ProcessImage(string imgPath)
        {
            Bitmap bmp = Image.FromFile(imgPath) as Bitmap;
            string bmpName = GetName(imgPath);

            Console.WriteLine(imgPath + " -> " + bmpName);

            AddToPicInfos(bmp.Width, bmp.Height, bmpName);
            SaveOriginal(bmp, bmpName);
            CreateFixedHeightImages(bmp, bmpName);
            CreateFixedWidthImages(bmp, bmpName);
            SquareCrop(bmp, bmpName);
        }

        private string GetName(string imgPath)
        {
            if (Options.RenameBasedOnTime)
            {
                return DateTime.Now.Ticks.ToString() + ".png";
            }
            else
            {
                return Path.GetFileName(imgPath);
            }
        }

        private void AddToPicInfos(int w, int h, string name)
        {
            PicInfos.Add(new PicInfo
            {
                W = w,
                H = h,
                Name = name
            });
        }

        private void SaveOriginal(Bitmap original, string name)
        {
            if (Options.KeepOriginal)
            {
                Save(original, "original", name);
            }
        }

        private void Save(Bitmap bmp, string dstSubFolder, string name)
        {
            var dstPath = Options.DstFolder + "/" + dstSubFolder + "/";

            if (!Directory.Exists(dstPath))
            {
                Directory.CreateDirectory(dstPath);
            }

            bmp.Save(dstPath + name);
        }

        private void CreateFixedHeightImages(Bitmap bmp, string name)
        {
            Options.FixedHeightSizes.ForEach(newHeight =>
            {
                var newWidth = GetWidthGivenNewHeight(newHeight, bmp.Width, bmp.Height);
                var fixedHeightImg = ImageUtils.ResizeImage(bmp, newWidth, newHeight);
                Save(fixedHeightImg, Options.FixedHeightDst + newHeight.ToString(), name);
            });
        }

        private void CreateFixedWidthImages(Bitmap bmp, string name)
        {
            Options.FixedWidthSizes.ForEach(newWidth =>
            {
                var newHeight = GetHeightGivenNewWidth(newWidth, bmp.Width, bmp.Height);
                var fixedWidthImg = ImageUtils.ResizeImage(bmp, newWidth, newHeight);
                Save(fixedWidthImg, Options.FixedWidthDst + newWidth.ToString(), name);
            });
        }

        private int GetWidthGivenNewHeight(int newHeight, int w, int h)
        {
            return newHeight * w / h;
        }

        private int GetHeightGivenNewWidth(int newWidth, int w, int h)
        {
            return newWidth * h / w;
        }

        private void SquareCrop(Bitmap bmp, string name)
        {
            Options.SquareCropSizes.ForEach(size =>
            {
                var squareImg = CropImageCenteredSquare(bmp);
                var resizedSquareImg = ImageUtils.ResizeImage(squareImg, size, size);
                Save(resizedSquareImg, Options.SquareCropDst + size.ToString(), name);
            });
            
        }

        private Bitmap CropImageCenteredSquare(Bitmap bmp)
        {
            var w = bmp.Width;
            var h = bmp.Height;
            var x = 0;
            var y = 0;
            var size = w;

            if (h > w)
            {
                y = (h - w) / 2;
            }
            else if (w > h)
            {
                x = (w - h) / 2;
                size = h;
            }

            return ImageUtils.CropImage(bmp, x, y, size, size);
        }

        private void SaveIndexFile()
        {
            var json = JsonConvert.SerializeObject(PicInfos);
            File.WriteAllText(Options.DstFolder + "/" + "index.json", json);
        }
    }
}
