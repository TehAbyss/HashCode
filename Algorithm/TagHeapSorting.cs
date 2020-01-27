namespace HashPhotoSlideshow.Algorithm
{
    using System;
    using System.Collections.Generic;
    using HashPhotoSlideshow.Model;
    using HashPhotoSlideshow.Controller;

    public class TagHeapSorting : ISlideshowAlgorithm
    {
        public SortedSet<Tag> TagSort = new SortedSet<Tag>(new ByPhotoCount());
        private Dictionary<string, Tag> TagCache = new Dictionary<string, Tag>();
        public Slideshow GenerateSlideshow(PhotoCollection photoCollection) {
            PopulateCache(photoCollection);
            SortByTag();
            return SlideshowProcessor.GenerateSlideShow(this);
        }

        private void PopulateCache(PhotoCollection photoCollection) {
            try {
                foreach(var photo in photoCollection) {
                    foreach(var tag in photo.Tags) {
                        if (!TagCache.ContainsKey(tag)) {
                            var tagObj = new Tag() {
                                Name = tag
                            };
                            TagCache.Add(tag, tagObj);
                        }
                        TagCache[tag].Photos.Add(photo);
                    }
                }
            }
            catch {
                Console.WriteLine("Error in Populate Cache");
                throw;
            }
        }

        private void SortByTag() {
            try {
                foreach(var kv in TagCache) {
                    if (!TagSort.Add(kv.Value)) {
                        Console.WriteLine($"Could not add {kv.Key}");
                    }
                }
            }
            catch {
                Console.WriteLine("Error in SortByTag");
                throw;
            }
        }
    }

    internal class ByPhotoCount : IComparer<Tag>
    {
        public int Compare(Tag a, Tag b) {
            if (a.Photos.Count.CompareTo(b.Photos.Count) >= 0)
            {
                return 1;
            }

            return -1;
        }
    }
}