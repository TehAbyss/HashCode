namespace HashPhotoSlideshow.Controller
{
    using System;
    using System.IO;
    using HashPhotoSlideshow.Model;

    public class OutputProcessor
    {
        public static void ProcessSlideShow(Slideshow slideshow, string outputFileName)
        {
            // sanitize file names if necessary
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                outputFileName = outputFileName.Replace(c, '_');
            }

            using (var fs = File.Create(outputFileName))
            {
                using (var sr = new StreamWriter(fs))
                {
                    sr.WriteLine(slideshow.Slides.Count.ToString());

                    foreach (var slide in slideshow.Slides)
                    {
                        foreach (var photo in slide.Photos)
                        {
                            sr.Write($"{photo.Id.ToString()} ");
                        }
                        sr.WriteLine();
                    }
                }
            }
        }

        public void ProcessSlideshowToConsole(Slideshow slideshow)
        {
            try
            {
                Console.WriteLine("#### Slideshow ####");
                Console.WriteLine($"Number of slides: {slideshow?.Slides?.Count}");
                foreach (var slide in slideshow?.Slides)
                {
                    foreach (var photo in slide?.Photos)
                    {
                        Console.Write($"{photo.Id.ToString()} ");
                    }
                    Console.WriteLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
