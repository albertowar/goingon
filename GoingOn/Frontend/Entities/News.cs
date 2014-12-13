namespace Frontend
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class News
    {
        public string Content { get; private set; }

        public News(string content)
        {
            Content = content;
        }
    }
}