namespace HashPhotoSlideshow.Controller
{
    using System.Collections.Generic;
    using HashPhotoSlideshow.Model;

    public class ScoringProcessor
    {
        public static int Judge(Slideshow slideshow)
        {
            int interestScore = 0;

            if (slideshow?.Slides?.Count > 1)
            {
                for (int i = 0; i < slideshow.Slides.Count - 1; i++)
                {
                    interestScore += Judge(slideshow.Slides[i], slideshow.Slides[i + 1]);
                }
            }

            return interestScore;
        }

        public static int Judge(Slide slide1, Slide slide2)
        {
            // Get tags from slide 1
            var s1Tags = new HashSet<string>();
            foreach (var photo in slide1.Photos)
            {
                s1Tags.UnionWith(photo.Tags);
            }

            // Get tags fom slide 2
            var s2Tags = new HashSet<string>();
            foreach (var photo in slide2.Photos)
            {
                s2Tags.UnionWith(photo.Tags);
            }

            // Calculate interest scores
            var commonTags = new HashSet<string>(s1Tags);
            commonTags.IntersectWith(s2Tags);

            var s1OnlyTags = new HashSet<string>(s1Tags);
            s1OnlyTags.ExceptWith(s2Tags);

            var s2OnlyTags = new HashSet<string>(s2Tags);
            s2OnlyTags.ExceptWith(s1Tags);

            int minScore = commonTags.Count;
            minScore = s1OnlyTags.Count < minScore ? s1OnlyTags.Count : minScore;
            minScore = s2OnlyTags.Count < minScore ? s2OnlyTags.Count : minScore;

            return minScore;
        }
    }
}
