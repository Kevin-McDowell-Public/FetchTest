using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;

namespace selenium
{
    class Program
    {
        static void Main(string[] args)
        {
            EdgeOptions options = new EdgeOptions();
            options.AddArgument("--guest");
            EdgeDriver driver = new EdgeDriver(options);
            
            WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 5));

            FetchTestPage fetch = new FetchTestPage();
            fetch.Intialize(driver);

            bool foundLightWeightGroup = false;
            // The easy way, assuming everything is hard-coded:
            // int[] startList = [1,2,3,4,5,6,7,8]; 
            // int initialStartListLength = 8;

            // The correct way, if the list is "coin" list was going to be dynamic
            int[] startList = GetCoins(fetch);
            int initialStartListLength = startList.Length;

            string foundValue = "";

            while(foundLightWeightGroup == false)
            {
                // Alternate adding values to each side (skipping 0)
                for(int i = 1; i <= startList.Length/2; i++)
                {
                    fetch.GetLeftCell(i-1).SendKeys($"{startList[(i*2)-2]}");
                    fetch.GetRightCell(i-1).SendKeys($"{startList[(i*2)-1]}");
                }

                fetch.GetWeighButton().Click();

                string lastWeighIn = fetch.GetLastWeightResult();

                if(lastWeighIn.Contains('=') && 
                    startList.Length == initialStartListLength)
                {
                    // If this list was odd and we're on the first pass, 
                    // and the 2 sides are equal, then the low value is the one 
                    // we skipped
                    fetch.GetCoin(0);
                    foundValue = "0";
                    foundLightWeightGroup = true;
                }
                else
                {
                    string evaluatedList = "";
                    //Find the new start list
                    if(lastWeighIn.Contains('<'))
                    {
                        // "lightest" value is on left side of the scales
                        evaluatedList = lastWeighIn.Split('<')[0];
                    }
                    else
                    {
                        // "lightest" is on right side of the scales
                        evaluatedList = lastWeighIn.Split('>')[1];
                    }

                    // Trim whitespace, trim array brackets, split by comma
                    string[] newStartList = evaluatedList.Trim().Trim(['[',']']).Split(',');

                    if(newStartList.Length == 1)
                    {
                        // If we're down to a single value, then that's the lightest value:
                        foundValue = newStartList[0];
                        fetch.GetCoin(int.Parse(newStartList[0])).Click();
                        foundLightWeightGroup = true;
                    }
                    else
                    {
                        //Convert the text array of cell values into an int array 
                        // to process the group with the "lightest" value
                        startList = Array.ConvertAll(newStartList, n => int.Parse(n));
                    }
                }

                if(!foundLightWeightGroup)
                {
                    fetch.GetResetButton().Click();
                    fetch.WaitForScaleReset();
                }
            }

            try
            {
                if(driver.SwitchTo().Alert().Text == "Yay! You find it!")
                {
                    Console.WriteLine($"The code found it: {foundValue}");
                }
                else
                {
                    Console.WriteLine($"The code failed to find the low value.");
                }
            }
            catch
            {
                Console.WriteLine("This didn't work as expected");
            }

            driver.Quit();
        }

        private static int[] GetCoins(FetchTestPage fetch)
        {
            List<IWebElement> coinCollection = fetch.GetCoinCollection();
            int[] coinDrop = new int[coinCollection.Count - coinCollection.Count % 2];
            // If it's an even number with start with 0, else start with 1
            int startValue = coinCollection.Count % 2;
            for(int i = startValue; i < coinCollection.Count; i++)
            {
                coinDrop[i - startValue] = int.Parse(coinCollection[i].Text);
            }

            return coinDrop;
        }
    }
}