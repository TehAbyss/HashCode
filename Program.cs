using System;

namespace HashPhotoSlideshow
{
    using HashPhotoSlideshow.Algorithm;
    using HashPhotoSlideshow.Model;
    class Program
    {
        static void Main(string[] args)
        {
            var photoCollection = new PhotoCollection();
            photoCollection.Add(new Photo(){
                Id = 0,
                Orientation = Orientation.Horizontal,
                Tags = {"cat", "beach", "sun"}
            });
            photoCollection.Add(new Photo(){
                Id = 1,
                Orientation = Orientation.Vertical,
                Tags = {"selfie", "smile"}
            });
            photoCollection.Add(new Photo(){
                Id = 2,
                Orientation = Orientation.Vertical,
                Tags = {"selfie", "garden"}
            });
            photoCollection.Add(new Photo(){
                Id = 3,
                Orientation = Orientation.Horizontal,
                Tags = {"cat", "garden"}
            });

            try {
                var tagHeapSort = new TagHeapSorting().GenerateSlideshow(photoCollection);
                Console.WriteLine("#### Slideshow ####");
                Console.WriteLine($"Number of slides: {tagHeapSort?.Slides?.Count}");
                foreach(var slide in tagHeapSort?.Slides) {
                    foreach(var photo in slide?.Photos) {
                        Console.Write($"{photo.Id.ToString()} ");
                    }
                    Console.WriteLine();
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }
    }
}
