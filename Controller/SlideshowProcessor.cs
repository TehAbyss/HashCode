namespace HashPhotoSlideshow.Controller
{
    using System;
    using HashPhotoSlideshow.Model;
    using HashPhotoSlideshow.Algorithm;
    using System.Collections.Generic;
    public static class SlideshowProcessor
    {
        private static HashSet<int> photoCache;
        public static Slideshow GenerateSlideShow(ISlideshowAlgorithm algo) {
            if (algo is TagHeapSorting) {
                return GetSlideshow(algo as TagHeapSorting);
            }

            return new Slideshow();
        }

        private static Slideshow GetSlideshow(TagHeapSorting tagHeapSorting) {
            var slideShow = new Slideshow();
            photoCache = new HashSet<int>();

            try {
                foreach(var tag in tagHeapSorting?.TagSort) {
                    var slide = new Slide();
                    
                    foreach(var photo in tag?.Photos) {
                        if (!photoCache.Contains(photo.Id)) {
                            photoCache.Add(photo.Id);
                            
                            slide.Photos.Add(photo);
                            if (photo.Orientation == Orientation.Horizontal || slide.Photos.Count > 1) {
                                slideShow.Slides.Add(slide);
                                slide = new Slide();
                            }
                        }
                    }
                }
            }
            catch {
                Console.WriteLine("Error in GetSLideShow");
                throw;
            }

            return slideShow;
        }
    }
}
