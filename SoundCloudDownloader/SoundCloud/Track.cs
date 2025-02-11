using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundCloudDownloader.SoundCloud
{
    public class Track
    {
        public string Title { get; set; } = string.Empty;
        public string PermalinkUrl { get; set; } = string.Empty;
        public string StreamUrl { get; set; } = string.Empty;
    }
}
