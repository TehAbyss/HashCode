namespace HashPhotoSlideshow
{
    using System;
    using System.Globalization;
    using HashPhotoSlideshow.Controller;

    class Program
    {
        static void Main(string[] args)
        {
            var userInput = string.Empty;
            var isAll = false;
            var isQuit = false;

            var slideshowProcessor = new SlideshowProcessor();

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
                userInput = Console.ReadLine();

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
                            slideshowProcessor.RunAlgorithm(i);
                            Console.WriteLine();
                        }
                    }
                    else if (TryParseAlgorithmIndex(userInput, slideshowProcessor.AlgorithmNames.Count, out int i))
                    {
                        slideshowProcessor.RunAlgorithm(i);
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("Invalid selection. Please select another option.");
                        Console.WriteLine();
                    }

                    Console.Write("Press enter to continue...");
                    Console.ReadLine();
                }
            }
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
