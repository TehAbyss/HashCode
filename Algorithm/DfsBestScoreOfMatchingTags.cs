namespace HashPhotoSlideshow.Algorithm
{
    using System.Collections.Generic;
    using HashPhotoSlideshow.Controller;
    using HashPhotoSlideshow.Model;

    public class DfsBestScoreOfMatchingTags : ISlideshowAlgorithm
    {
        private Dictionary<string, PhotoCollection> TagCache = new Dictionary<string, PhotoCollection>();

        public Slideshow GenerateSlideshow(PhotoCollection photoCollection)
        {
            var slideshow = new Slideshow();

            ConsoleProgressBar progressBar = new ConsoleProgressBar(0, photoCollection.Count);

            // Make local copy of photo collection
            var photos = new PhotoCollection();
            photos.AddRange(photoCollection);

            // Cache tag and photos
            foreach (var photo in photos)
            {
                foreach (var tag in photo.Tags)
                {
                    if (!TagCache.ContainsKey(tag))
                    {
                        TagCache.Add(tag, new PhotoCollection());
                    }
                    TagCache[tag].Add(photo);
                }
            }

            // Proces photos
            while (photos.Count > 0)
            {
                Slide nextSlideToAdd = null;
                int maxScore = -1;

                if (slideshow.Slides.Count > 0)
                {
                    var slide1 = slideshow.Slides[slideshow.Slides.Count - 1];

                    // Get the set of photo that has the same tags as slide1
                    var tagSet = new HashSet<string>();
                    var photoSetWithMatchingTags = new HashSet<Photo>();
                    foreach (var photo in slide1.Photos)
                    {
                        tagSet.UnionWith(photo.Tags);

                        foreach (var tag in photo.Tags)
                        {
                            photoSetWithMatchingTags.UnionWith(TagCache[tag]);
                        }
                    }

                    // The best possible score we can ever get is half the number of tags
                    int maxPossibleScore = tagSet.Count / 2;

                    // Only compare the set of photos that has the same tags as slide1
                    var photosWithMatchingTags = new PhotoCollection();
                    photosWithMatchingTags.AddRange(photoSetWithMatchingTags);

                    for (int i = 0; i < photosWithMatchingTags.Count; i++)
                    {
                        var slide2 = new Slide(photosWithMatchingTags[i]);

                        if (photosWithMatchingTags[i].Orientation == Orientation.Horizontal)
                        {
                            // Cache best scoring slide
                            int score = ScoringProcessor.Judge(slide1, slide2);

                            if (score > maxScore)
                            {
                                maxScore = score;
                                nextSlideToAdd = slide2;
                            }
                        }
                        else
                        {
                            // If vertical photo, find a vertical photo pair
                            for (int j = 0; j < photosWithMatchingTags.Count; j++)
                            {
                                if (photosWithMatchingTags[j].Orientation == Orientation.Vertical && (i != j))
                                {
                                    slide2.Photos.Add(photosWithMatchingTags[j]);

                                    // Cache best scoring slide
                                    int score = ScoringProcessor.Judge(slide1, slide2);

                                    if (score > maxScore)
                                    {
                                        maxScore = score;
                                        nextSlideToAdd = slide2;
                                    }
                                    break;
                                }
                            }

                        }

                        // No need to process more slides because we already achieved best possible score
                        if (maxScore >= maxPossibleScore)
                        {
                            break;
                        }
                    }
                }

                // Add best scoring slide to slideshow
                if (nextSlideToAdd != null)
                {
                    slideshow.Slides.Add(nextSlideToAdd);
                }
                else
                {
                    // there were no matching tags or orphaned vertical photo
                    // try adding next possible slide without concern for score
                    nextSlideToAdd = new Slide(photos[0]);

                    if (photos[0].Orientation == Orientation.Horizontal)
                    {
                        slideshow.Slides.Add(nextSlideToAdd);
                    }
                    else
                    {
                        // find vertical photo pair
                        for (int j = 1; j < photos.Count; j++)
                        {
                            if (photos[j].Orientation == Orientation.Vertical)
                            {
                                nextSlideToAdd.Photos.Add(photos[j]);
                                slideshow.Slides.Add(nextSlideToAdd);
                                break;
                            }
                        }
                    }
                }

                // Remove used photos in slideshow from collection and tag cache
                if (nextSlideToAdd != null)
                {
                    foreach (var photo in nextSlideToAdd.Photos)
                    {
                        foreach (var tag in photo.Tags)
                        {
                            TagCache[tag].Remove(photo);
                        }

                        photos.Remove(photo);
                    }
                }

                progressBar.Progress(photoCollection.Count - photos.Count);
            }

            return slideshow;
        }
    }
}