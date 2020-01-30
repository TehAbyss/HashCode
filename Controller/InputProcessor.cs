using HashPhotoSlideshow.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HashPhotoSlideshow.Controller
{
    public class InputProcessor
    {
        public static List<Photo> ReadInputFile(string filePath)
        {
            List<Photo> photoList = new List<Photo>();

            if (File.Exists(filePath))
            {
                FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream))
                {
                    int photoCount = 0;
                    int photoId = 0;
                    string line;

                    while ((line = streamReader.ReadLine()) != null)
                    {
                        int wordsPos = 0;

                        if (line.Length == 1)
                        {
                            photoCount = Int32.Parse(line);
                        }
                        else
                        {
                            Photo photo = new Photo();

                            string[] words = line.Split(' ');

                            if (words[wordsPos++] == "H")
                                photo.Orientation = Orientation.Horizontal;
                            else
                                photo.Orientation = Orientation.Vertical;

                            int tagCount = Int32.Parse(words[wordsPos++]);

                            for (int i = 0; i < tagCount; i++)
                            {
                                photo.Tags.Add(words[wordsPos++]);
                            }

                            photo.Id = photoId++;
                            photoList.Add(photo);
                        }
                    }
                }
            }

            return photoList;
        }

    }
}
