using System;

namespace SafeBlock.io.Models.API
{
    public class InitializeModel
    {
        public Guid EntryPoint { get; set; }
        public string PublicKey { get; set; }
        public string ReleaseAddress { get; set; }
    }
}