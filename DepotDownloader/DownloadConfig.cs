using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DepotDownloader
{

    public struct DownloadConfig : ICloneable
    {

        public static int CellID { get; set; }

        public bool DownloadAllPlatforms { get; set; }

        public bool DownloadAllLanguages { get; set; }

        public bool DownloadManifestOnly { get; set; }

        public string InstallDirectory { get; set; }

        public bool UsingFileList { get; set; }

        public ConcurrentQueue<string> FilesToDownload { get; set; }

        public ConcurrentQueue<Regex> FilesToDownloadRegex { get; set; }

        public bool UsingExclusionList { get; set; }

        public string BetaPassword { get; set; }

        public bool VerifyAll { get; set; }

        public int MaxServers { get; set; }

        public int MaxDownloads { get; set; }

        public string SuppliedPassword { get; set; }

        public bool RememberPassword { get; set; }

        // A Steam LoginID to allow multiple concurrent connections
        public uint? LoginID { get; set; }

        public DownloadConfig Clone()
        {
            var copy = this;
            return MakeSafeCopy(ref copy);
        }

        private static ref DownloadConfig MakeSafeCopy(ref DownloadConfig cfg)
        {
            cfg.FilesToDownload = cfg.FilesToDownload != null
                ? new ConcurrentQueue<string>(cfg.FilesToDownload)
                : null;
            cfg.FilesToDownloadRegex = cfg.FilesToDownloadRegex != null
                ? new ConcurrentQueue<Regex>(cfg.FilesToDownloadRegex)
                : null;
            return ref cfg;
        }

        object ICloneable.Clone()
            => Clone();

    }

}