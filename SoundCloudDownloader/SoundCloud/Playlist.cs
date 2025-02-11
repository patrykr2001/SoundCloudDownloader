using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundCloudDownloader.SoundCloud
{
    public class Playlist
    {
        public string Title { get; set; } = string.Empty;
        public Track[] Tracks { get; set; } = [];
    }
}
