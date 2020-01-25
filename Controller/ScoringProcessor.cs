namespace HashPhotoSlideshow.Controller
{
    using System;
    using System.Collections.Generic;
    using HashPhotoSlideshow.Model;

    public class ScoringProcessor
    {
        public static int Judge(Slideshow slideshow)
        {
            int interestScore = 0;

            if (slideshow == null)
            {
                return 0;
            }

            if (slideshow.Slides == null || slideshow.Slides.Count < 2) 
            {
                return 0;
            }

            for( int i = 0; i < slideshow.Slides.Count - 1; i++)
            {
                interestScore += CalculateInterestScore(slideshow.Slides[i], slideshow.Slides[i + 1]);
            }

            return interestScore;
        }

        public static int CalculateInterestScore(Slide slide1, Slide slide2)
        {
            int numberOfTagsInCommon = 0;
            int numberOfTagsInS1NotS2 = 0;
            int numberOfTagsInS2NotS1 = 0;

            List<string> trackTags = new List<string>();

            // Process slide1
            foreach(var photo in slide1.Photos)
            {
                foreach (var tag in photo.Tags)
                {
                    if (!trackTags.Contains(tag))
                    {
                        trackTags.Add(tag);
                        numberOfTagsInS1NotS2++;
                    }
                }
            }

            // Process slide2
            foreach(var photo in slide2.Photos)
            {
                foreach (var tag in photo.Tags)
                {
                    if (!trackTags.Contains(tag))
                    {
                        trackTags.Add(tag);
                        numberOfTagsInS2NotS1++;
                    }
                    else
                    {
                        numberOfTagsInCommon++;
                        numberOfTagsInS1NotS2--;
                    }
                }
            }

            int minScore = int.MaxValue;
            minScore = numberOfTagsInCommon < minScore ? numberOfTagsInCommon : minScore;
            minScore = numberOfTagsInS1NotS2 < minScore ? numberOfTagsInS1NotS2 : minScore;
            minScore = numberOfTagsInS2NotS1 < minScore ? numberOfTagsInS2NotS1 : minScore;

            return minScore;
        }
    }
}