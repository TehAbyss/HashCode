namespace HashPhotoSlideshow
{
    using System;
    using System.Globalization;
    using HashPhotoSlideshow.Controller;

    class Program
    {
        static SlideshowProcessor slideshowProcessor = new SlideshowProcessor();

        static void Main(string[] args)
        {
            var userInput = string.Empty;
            var isAll = false;
            var isQuit = false;
            var hasArgs = (args.Length > 0);

            while (!isQuit)
            {
                Console.WriteLine("Hash Code Photo Slideshow");
                Console.WriteLine();
                Console.WriteLine("Available algorithms:");

                for (int i = 0; i < slideshowProcessor.AlgorithmNames.Count; i++)
                {
                    Console.WriteLine($"    {i}: {slideshowProcessor.AlgorithmNames[i]}");
                }

                Console.WriteLine($"    a: All");
                Console.WriteLine($"    q: Quit");
                Console.WriteLine();

                Console.Write("Select algorithm: ");

                if (hasArgs)
                {
                    userInput = args[0];
                }
                else
                {
                    userInput = Console.ReadLine();
                }

                Console.WriteLine();

                isAll = (userInput.Equals("a") || userInput.Equals("A"));
                isQuit = (userInput.Equals("q") || userInput.Equals("Q"));

                if (!isQuit)
                {
                    if (isAll)
                    {
                        Console.WriteLine($"Running all algorithms");

                        for (int i = 0; i < slideshowProcessor.AlgorithmNames.Count; i++)
                        {
                            RunAlgorithm(i);
                        }
                    }
                    else if (TryParseAlgorithmIndex(userInput, slideshowProcessor.AlgorithmNames.Count, out int i))
                    {
                        RunAlgorithm(i);
                    }
                    else
                    {
                        Console.WriteLine("Invalid selection. Please select another option.");
                        Console.WriteLine();
                    }

                    if (hasArgs)
                    {
                        isQuit = true;
                    }
                    else
                    {
                        Console.Write("Press enter to continue...");
                        Console.ReadLine();
                    }
                }
            }
        }

        internal static void RunAlgorithm(int algorithmIndex)
        {
            var algorithmName = slideshowProcessor.AlgorithmNames[algorithmIndex];
            int totalScore = 0;
            var startTime = DateTime.Now;

            foreach (var inputFilePath in slideshowProcessor.InputFilePaths)
            {
                totalScore += slideshowProcessor.RunAlgorithm(algorithmIndex, inputFilePath);
                Console.WriteLine();
            }

            var endTime = DateTime.Now;

            Console.WriteLine($"{algorithmName}");
            Console.WriteLine($"[TOTAL DURATION] {endTime.Subtract(startTime).TotalMilliseconds} ms");
            Console.WriteLine($"[TOTAL SCORE] {totalScore}");
            Console.WriteLine();
        }

        internal static bool TryParseAlgorithmIndex(string userInput, int algorithmCount, out int algorithmIndex)
        {
            if (int.TryParse(userInput, NumberStyles.Number, CultureInfo.InvariantCulture, out algorithmIndex))
            {
                return (algorithmIndex < algorithmCount);
            }

            return false;
        }
    }
}
