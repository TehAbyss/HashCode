namespace HashPhotoSlideshow.Algorithm
{
    using System;
    using System.Collections.Generic;
    using HashPhotoSlideshow.Model;

    public class TagHeapSorting : ISlideshowAlgorithm
    {
        public SortedSet<Tag> TagSort = new SortedSet<Tag>(new ByPhotoCount());
        private Dictionary<string, Tag> TagCache = new Dictionary<string, Tag>();

        public Slideshow GenerateSlideshow(PhotoCollection photoCollection)
        {
            PopulateCache(photoCollection);
            SortByTag();
            return GetSlideshow();
        }

        private void PopulateCache(PhotoCollection photoCollection)
        {
            try
            {
                foreach (var photo in photoCollection)
                {
                    foreach (var tag in photo.Tags)
                    {
                        if (!TagCache.ContainsKey(tag))
                        {
                            var tagObj = new Tag()
                            {
                                Name = tag
                            };
                            TagCache.Add(tag, tagObj);
                        }
                        TagCache[tag].Photos.Add(photo);
                    }
                }
            }
            catch
            {
                Console.WriteLine("Error in Populate Cache");
                throw;
            }
        }

        private void SortByTag()
        {
            try
            {
                foreach (var kv in TagCache)
                {
                    if (!TagSort.Add(kv.Value))
                    {
                        Console.WriteLine($"Could not add {kv.Key}");
                    }
                }
            }
            catch
            {
                Console.WriteLine("Error in SortByTag");
                throw;
            }
        }

        private Slideshow GetSlideshow()
        {
            var slideShow = new Slideshow();
            var photoCache = new HashSet<int>();
            var vSlide = new Slide();

            try
            {
                foreach (var tag in TagSort)
                {
                    foreach (var photo in tag?.Photos)
                    {
                        if (!photoCache.Contains(photo.Id))
                        {
                            photoCache.Add(photo.Id);

                            if (photo.Orientation == Orientation.Horizontal)
                            {
                                var hSlide = new Slide();
                                hSlide.Photos.Add(photo);
                                slideShow.Slides.Add(hSlide);
                            }
                            else
                            {
                                vSlide.Photos.Add(photo);

                                if (vSlide.Photos.Count > 1)
                                {
                                    slideShow.Slides.Add(vSlide);
                                    vSlide = new Slide();
                                }
                            }
                        }
                    }
                }


                return slideShow;
            }
            catch
            {
                Console.WriteLine("Error in GetSLideShow");
                throw;
            }
        }
    }

    internal class ByPhotoCount : IComparer<Tag>
    {
        public int Compare(Tag a, Tag b)
        {
            if (a.Photos.Count.CompareTo(b.Photos.Count) >= 0)
            {
                return 1;
            }

            return -1;
        }
    }
}