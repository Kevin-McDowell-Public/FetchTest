using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.PageObjects;

public class FetchTestPage
{
    IWebDriver driver;
    WebDriverWait wait;

    private static int weightCount = 0;
    public void Intialize(WebDriver driver)
    {
        this.driver = driver;
        wait = new WebDriverWait(driver, new TimeSpan(0, 0, 5));
        driver.Navigate().GoToUrl("http://sdetchallenge.fetch.com/");
    }

    public IWebElement GetLeftCell(int index)
    {
        return driver.FindElement(By.Id($"left_{index}"));
    }

    public IWebElement GetRightCell(int index)
    {
        return driver.FindElement(By.Id($"right_{index}"));
    }

    public IWebElement GetCoin(int index)
    {
        return driver.FindElement(By.Id($"coin_{index}"));
    }

    public List<IWebElement> GetCoinCollection()
    {
        IWebElement coinBag = driver.FindElement(By.ClassName("coins"));
        return coinBag.FindElements(By.ClassName("square")).ToList();
    }

    public IWebElement GetResetButton()
    {
        var resetButtons = driver.FindElements(By.Id("reset"));
        foreach(IWebElement button in resetButtons)
        {
            // In the test page, there are 2 reset buttons and one is enabled, return the one that is
            if(button.Enabled)
            {
                return button;
            }
        }

        // This would be less dynamic, but easier, if you knew the 1st button was always going to be disabled.
        // return resetButtons[1];
        return null;
    }

    public void WaitForScaleReset()
    {
        wait.Until(driver => driver.FindElement(By.Id("left_0")).GetAttribute("value") == "");  
    }

    public IWebElement GetWeighButton()
    {
        return driver.FindElement(By.Id("weigh"));
    }

    public IWebElement WeighingsResults
    {
        get
        {
            return driver.FindElement(By.ClassName("game-info"));
        }
    }

    public string GetLastWeightResult()
    {
        wait.Until(driver => this.WeighingsResults.FindElements(By.TagName("li")).Count == weightCount + 1);
        weightCount++;

        var weightList = this.WeighingsResults.FindElements(By.TagName("li"));

        return weightList[weightList.Count - 1].Text;
    }
}