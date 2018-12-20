using System.Collections.Generic;

namespace Vibrometer.WebClient.Model
{
    public class AppState
    {
        #region Constructors

        public AppState()
        {
            this.PageDescriptionSet = new List<PageDescription>()
            {
                new PageDescription("Page Test 1", "P1", "dashboard", "pagetest1", new List<PageDescription>() { new PageDescription("Page Test 1 Sub Page", "S1", "dashboard", "pagetest1/subpage") }),
                new PageDescription("Page Test 2", "P2", "dashboard", "pagetest2")
            };
        }

        #endregion

        #region Properties

        public List<PageDescription> PageDescriptionSet { get; }

        #endregion
    }
}
