using System.Collections.Generic;

namespace Vibrometer.WebClient.Model
{
    public class PageDescription
    {
        #region Constructors

        public PageDescription(string displayName, string shortName, string iconName, string route) : this(displayName, shortName, iconName, route, new List<PageDescription>())
        {
            //
        }

        public PageDescription(string displayName, string shortName, string iconName, string route, List<PageDescription> pageDescriptionSet)
        {
            this.DisplayName = displayName;
            this.ShortName = shortName;
            this.IconName = iconName;
            this.Route = route;

            this.PageDescriptionSet = pageDescriptionSet;
        }

        #endregion

        #region Properties

        public string DisplayName { get; }
        public string ShortName { get; }
        public string IconName { get; }
        public string Route { get; }

        public List<PageDescription> PageDescriptionSet { get; }

        #endregion
    }
}
