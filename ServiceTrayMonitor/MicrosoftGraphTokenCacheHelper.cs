using System;
using System.IO;
using Microsoft.Identity.Client;

namespace Joseph.ServiceTrayMonitor
{
    public static class MicrosoftGraphTokenCacheHelper
    {
        public static readonly string CacheFilePath = 
            AppDomain.CurrentDomain.SetupInformation.ApplicationBase + Properties.Resources.PathMSALTokenCache;
        private static readonly object FileLock = new object();

        public static void EnableSerialization(ITokenCache tokenCache)
        {
            tokenCache.SetBeforeAccess(BeforeAccessNotification);
            tokenCache.SetAfterAccess(AfterAccessNotification);
        }

        private static void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            lock (FileLock)
            {
                args.TokenCache.DeserializeMsalV3(File.Exists(CacheFilePath) ? File.ReadAllBytes(CacheFilePath) : null);
            }
        }

        private static void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            if (!args.HasStateChanged) return;
            lock (FileLock)
            {
                File.WriteAllBytes(CacheFilePath, args.TokenCache.SerializeMsalV3());
            }
        }
    }
}