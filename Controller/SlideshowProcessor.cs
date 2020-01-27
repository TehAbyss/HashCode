namespace HashPhotoSlideshow.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using HashPhotoSlideshow.Algorithm;
    using HashPhotoSlideshow.Model;

    public class SlideshowProcessor
    {
        private static readonly string PythonScriptPath = @"Python";
        private static readonly string PythonHeader = "Python:";
        public ReadOnlyCollection<string> AlgorithmNames { get => AlgorithmNamesInternal.AsReadOnly(); }
        private List<string> AlgorithmNamesInternal = new List<string>();
        private Dictionary<string, string> PythonAlgorithms = new Dictionary<string, string>();
        private Dictionary<string, Type> SlideshowAlgorithms = new Dictionary<string, Type>();

        public SlideshowProcessor()
        {
            LoadAlgorithms();
        }

        public void RunAlgorithm(int algorithmIndex)
        {
            // Parse input file OR use test data
            var photoCollection = GetTestData();

            // Generate slideshow
            Console.WriteLine($"Running {AlgorithmNames[algorithmIndex]}");
            var slideshow = InvokeAlgorithm(photoCollection, algorithmIndex);

            // Score slideshow
            var score = ScoringProcessor.Judge(slideshow);
            Console.WriteLine();
            Console.WriteLine($"[Score] {score}");
            Console.WriteLine($"[Number of slides] {slideshow.Slides.Count}");
            Console.WriteLine();

            // Output submission file
            string outputFileName = $"slideshow_{AlgorithmNames[algorithmIndex]}_{DateTime.Now.ToString("yyyyMMdd_hhmmss")}.txt";
            OutputProcessor.ProcessSlideShow(slideshow, outputFileName);
            Console.WriteLine($"Submission file saved to {Path.GetFullPath(outputFileName)}");
            Console.WriteLine();
        }

        private void LoadAlgorithms()
        {
            // Get all entities in this assembly that implements the ISlideshowAlgorithm interface
            SlideshowAlgorithms = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => !x.IsInterface && !x.IsAbstract)
                .Where(x => typeof(ISlideshowAlgorithm).IsAssignableFrom(x))
                .Where(x => !typeof(IInternalAlgorithm).IsAssignableFrom(x))
                .ToDictionary(x => x.Name);

            AlgorithmNamesInternal.AddRange(SlideshowAlgorithms.Keys);

            // Get all python scripts
            if (Directory.Exists(PythonScriptPath))
            {
                PythonAlgorithms = Directory.GetFiles(PythonScriptPath, "*.py")
                    .ToDictionary(x => $"{PythonHeader}{Path.GetFileName(x)}");

                AlgorithmNamesInternal.AddRange(PythonAlgorithms.Keys);
            }
        }

        private Slideshow InvokeAlgorithm(PhotoCollection photoCollection, int algorithmIndex)
        {
            var algorithmName = AlgorithmNames[algorithmIndex];
            ISlideshowAlgorithm slideshowAlgorithm = null;

            if (algorithmName.StartsWith(PythonHeader))
            {
                var scriptPath = PythonAlgorithms[algorithmName];
                slideshowAlgorithm = (ISlideshowAlgorithm)(new PythonRunner(scriptPath));
            }
            else
            {
                var algorithmType = SlideshowAlgorithms[algorithmName];
                slideshowAlgorithm = (ISlideshowAlgorithm)Activator.CreateInstance(algorithmType);
            }

            return slideshowAlgorithm.GenerateSlideshow(photoCollection);
        }

        private PhotoCollection GetTestData()
        {
            var photoCollection = new PhotoCollection();
            photoCollection.Add(new Photo()
            {
                Id = 0,
                Orientation = Orientation.Horizontal,
                Tags = { "cat", "beach", "sun" }
            });
            photoCollection.Add(new Photo()
            {
                Id = 1,
                Orientation = Orientation.Vertical,
                Tags = { "selfie", "smile" }
            });
            photoCollection.Add(new Photo()
            {
                Id = 2,
                Orientation = Orientation.Vertical,
                Tags = { "selfie", "garden" }
            });
            photoCollection.Add(new Photo()
            {
                Id = 3,
                Orientation = Orientation.Horizontal,
                Tags = { "cat", "garden" }
            });
            return photoCollection;
        }
    }
}
