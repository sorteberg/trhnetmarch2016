using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elastic.Test
{
    class Article
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string[] Places { get; set; }
        public string[] Topics { get; set; }
        public DateTime Date { get; set; }
        public int Id { get; set; }
    }
}
