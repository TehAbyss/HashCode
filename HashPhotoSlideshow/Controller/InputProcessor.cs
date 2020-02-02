namespace HashPhotoSlideshow.Controller
{
    using HashPhotoSlideshow.Model;
    using System;
    using System.IO;

    public class InputProcessor
    {
        public static PhotoCollection ReadInputFile(string filePath)
        {
            PhotoCollection photoCollection = new PhotoCollection();

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

                        if (photoCount == 0)
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
                            photoCollection.Add(photo);
                        }
                    }
                }
            }

            return photoCollection;
        }

    }
}
