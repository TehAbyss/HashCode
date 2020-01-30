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
        private static readonly string InputFilePath = @"Input";
        private static readonly string PythonScriptPath = @"Python";
        private static readonly string PythonHeader = "Python:";
        public ReadOnlyCollection<string> InputFilePaths { get => InputFilePathsInternal.AsReadOnly(); }
        private List<string> InputFilePathsInternal = new List<string>();
        public ReadOnlyCollection<string> AlgorithmNames { get => AlgorithmNamesInternal.AsReadOnly(); }
        private List<string> AlgorithmNamesInternal = new List<string>();
        private Dictionary<string, string> PythonAlgorithms = new Dictionary<string, string>();
        private Dictionary<string, Type> SlideshowAlgorithms = new Dictionary<string, Type>();

        public SlideshowProcessor()
        {
            LoadInputFiles();
            LoadAlgorithms();
        }

        public int RunAlgorithm(int algorithmIndex, string inputFilePath)
        {
            // Parse input file OR use test data
            var photoCollection = new PhotoCollection(InputProcessor.ReadInputFile(inputFilePath));

            int numPhotos = photoCollection.Count;
            int numHorizontal = photoCollection.Where(p => p.Orientation == Orientation.Horizontal).Count();
            int numVertical = photoCollection.Where(p => p.Orientation == Orientation.Vertical).Count();

            // Generate slideshow
            Console.WriteLine($"Running {AlgorithmNames[algorithmIndex]}");
            var startAlgorithmTime = DateTime.Now;

            var slideshow = InvokeAlgorithm(photoCollection, algorithmIndex);

            var endAlgorithmTime = DateTime.Now;
            var algorithmDuration = endAlgorithmTime.Subtract(startAlgorithmTime).TotalMilliseconds;
            Console.WriteLine();
            Console.WriteLine($"[Duration] {algorithmDuration} ms");

            // Score slideshow
            var score = ScoringProcessor.Judge(slideshow);

            // Output submission file
            var inputFileName = Path.GetFileNameWithoutExtension(inputFilePath);
            var algorithmName = AlgorithmNames[algorithmIndex];
            var currentTime = DateTime.Now.ToString("yyyyMMdd_hhmmss");
            string outputFileName = $"slideshow_{inputFileName}_{algorithmName}_{currentTime}.txt";
            OutputProcessor.ProcessSlideShow(slideshow, outputFileName);
            Console.WriteLine($"Submission file saved to {Path.GetFullPath(outputFileName)}");

            // Result Information
            int numResultPhotos = 0;
            int numResultHorizontal = 0;
            int numResultVertical = 0;
            slideshow.Slides.ForEach((s) =>
            {
                numResultPhotos += s.Photos.Count;
                numResultHorizontal += s.Photos.Where(p => p.Orientation == Orientation.Horizontal).Count();
                numResultVertical += s.Photos.Where(p => p.Orientation == Orientation.Vertical).Count();
            });

            Console.WriteLine();
            Console.WriteLine($"[Input] {numPhotos} photos, {numHorizontal} horizontal, {numVertical} vertical");
            Console.WriteLine($"[Output] {numResultPhotos} photos, {numResultHorizontal} horizontal, {numResultVertical} vertical");
            Console.WriteLine($"[Slides] {slideshow.Slides.Count}");
            Console.WriteLine($"[Score] {score}");
            Console.WriteLine();

            return score;
        }

        private void LoadInputFiles()
        {
            // Get all input files
            if (Directory.Exists(InputFilePath))
            {
                InputFilePathsInternal = Directory.GetFiles(InputFilePath, "*.txt").ToList();
            }
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
