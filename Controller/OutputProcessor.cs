namespace HashPhotoSlideshow.Controller
{
    using System;
    using System.IO;
    using HashPhotoSlideshow.Model;

    public class OutputProcessor
    {
        private string slideshowOutputFileName = $"slideshow_{DateTime.Now}";
        private Slideshow Slideshow { get; }

        public OutputProcessor(Slideshow slideshow)
        {
            Slideshow = slideshow;
        }

        public void ProcessSlideShow()
        {
            using (var fs = File.Create(slideshowOutputFileName))
            {
                using (var sr = new StreamWriter(fs))
                {
                    sr.WriteLine(Slideshow.Slides.Count.ToString());

                    foreach (var slide in Slideshow.Slides)
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
    }
}
