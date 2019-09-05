using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Drawing;
using System.IO;

// From fork https://github.com/hguy/dds-reader

namespace DDSReader
{
    public class DDSImage
    {
        private readonly Pfim.IImage _image;

        public byte[] Data
        {
            get
            {
                if (_image != null)
                    return _image.Data;
                else
                    return new byte[0];
            }
        }

        private byte[] TightData()
        {
            // Since image sharp can't handle data with line padding in a stride
            // we create an stripped down array if any padding is detected
            var tightStride = _image.Width * _image.BitsPerPixel / 8;
            if (_image.Stride == tightStride)
            {
                return _image.Data;
            }

            byte[] newData = new byte[_image.Height * tightStride];
            for (int i = 0; i < _image.Height; i++)
            {
                Buffer.BlockCopy(_image.Data, i * _image.Stride, newData, i * tightStride, tightStride);
            }

            return newData;
        }

        public DDSImage(string file)
        {
            _image = Pfim.Pfim.FromFile(file);
            Process();
        }

        public DDSImage(Stream stream)
        {
            if (stream == null)
                throw new Exception("DDSImage ctor: Stream is null");

            _image = Pfim.Dds.Create(stream, new Pfim.PfimConfig());
            Process();
        }

        public DDSImage(byte[] data)
        {
            if (data == null || data.Length <= 0)
                throw new Exception("DDSImage ctor: no data");

            _image = Pfim.Dds.Create(data, new Pfim.PfimConfig());
            Process();
        }

        public void Save(string file)
        {
            if (_image.Format == Pfim.ImageFormat.Rgba32)
                Save<Bgra32>(file);
            else if (_image.Format == Pfim.ImageFormat.Rgb24)
                Save<Bgr24>(file);
            else
                throw new Exception("Unsupported pixel format (" + _image.Format + ")");
        }

        private void Process()
        {
            if (_image == null)
                throw new Exception("DDSImage image creation failed");

            if (_image.Compressed)
                _image.Decompress();
        }

        private void Save<T>(string file)
            where T : struct, IPixel<T>
        {
            Image<T> image = SixLabors.ImageSharp.Image.LoadPixelData<T>(
                TightData(), _image.Width, _image.Height);
            image.Save(file);
        }

        public static Bitmap ConvertDDSToBitmap(byte[] ddsFile)
        {
            var file = new DDSImage(ddsFile);
            var converted = new MemoryStream();
            file.SaveAsBmp(converted);
            return new Bitmap(converted);
        }

        #region SaveAs*

        public void SaveAsBmp(Stream file)
        {
            if (_image.Format == Pfim.ImageFormat.Rgba32)
                SaveAsBmp<Bgra32>(file);
            else if (_image.Format == Pfim.ImageFormat.Rgb24)
                SaveAsBmp<Bgr24>(file);
            else
                throw new Exception("Unsupported pixel format (" + _image.Format + ")");
        }

        public void SaveAsGif(Stream file)
        {
            if (_image.Format == Pfim.ImageFormat.Rgba32)
                SaveAsGif<Bgra32>(file);
            else if (_image.Format == Pfim.ImageFormat.Rgb24)
                SaveAsGif<Bgr24>(file);
            else
                throw new Exception("Unsupported pixel format (" + _image.Format + ")");
        }

        public void SaveAsJpeg(Stream file)
        {
            if (_image.Format == Pfim.ImageFormat.Rgba32)
                SaveAsJpeg<Bgra32>(file);
            else if (_image.Format == Pfim.ImageFormat.Rgb24)
                SaveAsJpeg<Bgr24>(file);
            else
                throw new Exception("Unsupported pixel format (" + _image.Format + ")");
        }

        public void SaveAsPng(Stream file)
        {
            if (_image.Format == Pfim.ImageFormat.Rgba32)
                SaveAsPng<Bgra32>(file);
            else if (_image.Format == Pfim.ImageFormat.Rgb24)
                SaveAsPng<Bgr24>(file);
            else
                throw new Exception("Unsupported pixel format (" + _image.Format + ")");
        }

        #endregion

        #region SaveAs*<T>

        private void SaveAsBmp<T>(Stream file)
            where T : struct, IPixel<T>
        {
            Image<T> image = SixLabors.ImageSharp.Image.LoadPixelData<T>(
                TightData(), _image.Width, _image.Height);
            image.SaveAsBmp<T>(file);
        }

        private void SaveAsGif<T>(Stream file)
            where T : struct, IPixel<T>
        {
            Image<T> image = SixLabors.ImageSharp.Image.LoadPixelData<T>(
                TightData(), _image.Width, _image.Height);
            image.SaveAsGif<T>(file);
        }
        private void SaveAsJpeg<T>(Stream file)
            where T : struct, IPixel<T>
        {
            Image<T> image = SixLabors.ImageSharp.Image.LoadPixelData<T>(
                TightData(), _image.Width, _image.Height);
            image.SaveAsJpeg<T>(file);
        }
        private void SaveAsPng<T>(Stream file)
            where T : struct, IPixel<T>
        {
            Image<T> image = SixLabors.ImageSharp.Image.LoadPixelData<T>(
                TightData(), _image.Width, _image.Height);
            image.SaveAsPng<T>(file);
        }

        #endregion
    }
}
