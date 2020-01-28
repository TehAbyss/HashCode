using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HashPhotoSlideshow.Controller
{
    public class InputProcessor
    {
        List<string> _lines;
        public List<Photo> _photo;
        public int _totalPhoto;

        public InputProcessor()
        {
            _lines = new List<string>();
            _photo = new List<Photo>();
            _totalPhoto = 0;
        }

        private void ReadInputFile()
        {
            //string path = "..\\..\\..\\inputFile.txt";
            string path = "inputFile.txt";

            if (File.Exists(path))
            {
                List<string> list = new List<string>();
                FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        _lines.Add(line);
                    }
                }
            }
        }

        public void GetInputData()
        {
            ReadInputFile();

            _totalPhoto = Int32.Parse(_lines[0]);

            for (int i = 1; i < _lines.Count; i++)
            {
                string[] words = _lines[i].Split(' ');

                Photo photo = new Photo();

                int pos = 0;

                if (words[pos++] == "H")
                    photo.Orientation = Orientation.Horizontal;
                else
                    photo.Orientation = Orientation.Vertical;

                int count = Int32.Parse(words[pos++]);

                for(int j = 0; j < count; j++)
                {
                    photo.Tags.Add(words[pos++]);
                }

                _photo.Add(photo);
            }

        }
    }
}






                                                         