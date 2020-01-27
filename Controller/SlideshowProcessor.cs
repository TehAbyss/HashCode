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
            var vSlide = new Slide();

            try {
                foreach(var tag in tagHeapSorting?.TagSort) {                   
                    foreach(var photo in tag?.Photos) {
                        if (!photoCache.Contains(photo.Id)) {
                            photoCache.Add(photo.Id);

                            if (photo.Orientation == Orientation.Horizontal) {
                                var hSlide = new Slide();
                                hSlide.Photos.Add(photo);
                                slideShow.Slides.Add(hSlide);
                            }
                            else {
                                vSlide.Photos.Add(photo);

                                if (vSlide.Photos.Count > 1) {
                                    slideShow.Slides.Add(vSlide);
                                    vSlide = new Slide();
                                }
                            }
                        }
                    }
                }


                return slideShow;
            }
            catch {
                Console.WriteLine("Error in GetSLideShow");
                throw;
            }
        }
    }
}
