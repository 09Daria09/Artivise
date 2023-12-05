using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;

namespace Artivise.Model
{
    public class ImageData
    {
        public string ImagePath { get; set; }
        public List<Rating> Ratings { get; set; }
        public string AuthorName { get; set; }
        public string Title { get; set; }

        public ImageData()
        {
            Ratings = new List<Rating>();
        }

        public class Rating
        {
            public string UserName { get; set; }
            public int Score { get; set; }
        }
    }
}


