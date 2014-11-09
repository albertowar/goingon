namespace GoingOn.Frontend
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class News
    {
        public string Content { get; private set; }

        public News(string content)
        {
            Content = content;
        }
    }
}