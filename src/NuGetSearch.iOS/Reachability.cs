/*
/// <summary>
/// This file was adapted from the example at https://raw.githubusercontent.com/xamarin/monotouch-samples/master/ReachabilitySample/reachability.cs
/// </summary>
using System;
using System.Net;
using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;
using MonoTouch.SystemConfiguration;
using MonoTouch.UIKit;

// TODO: Trim this down to just what is needed

namespace NuGetSearch.IOS
{
    public enum NetworkStatus 
    {
        NotReachable,
        ReachableViaCarrierDataNetwork,
        ReachableViaWiFiNetwork
    }

    public static class Reachability 
    {
        private static string hostName = "www.google.com";

        private static NetworkReachability adhocWiFiNetworkReachability;

        private static NetworkReachability defaultRouteReachability;

        private static NetworkReachability remoteHostReachability;

        // Raised every time there is an interesting reachable event, 
        // we do not even pass the info as to what changed, and 
        // we lump all three status we probe into one
        public static event EventHandler ReachabilityChanged;

        public static bool IsReachableWithoutRequiringConnection(NetworkReachabilityFlags flags)
        {
            // Is it reachable with the current network configuration?
            bool isReachable = (flags & NetworkReachabilityFlags.Reachable) != 0;

            // Do we need a connection to reach it?
            bool noConnectionRequired = (flags & NetworkReachabilityFlags.ConnectionRequired) == 0;

            // Since the network stack will automatically try to get the WAN up,
            // probe that
            if ((flags & NetworkReachabilityFlags.IsWWAN) != 0)
            {
                noConnectionRequired = true;
            }

            return isReachable && noConnectionRequired;
        }

        // Is the host reachable with the current network configuration
        public static bool IsHostReachable(string host)
        {
            if (host == null || host.Length == 0)
            {
                return false;
            }
             
            using (var r = new NetworkReachability(host))
            {
                NetworkReachabilityFlags flags;

                if (r.TryGetFlags(out flags))
                {
                    return IsReachableWithoutRequiringConnection(flags);
                }
            }

            return false;
        }

        public static bool IsAdHocWiFiNetworkAvailable(out NetworkReachabilityFlags flags)
        {
            if (adhocWiFiNetworkReachability == null)
            {
                adhocWiFiNetworkReachability = new NetworkReachability(new IPAddress(new byte[] { 169, 254, 0, 0 }));
                adhocWiFiNetworkReachability.SetCallback(OnChange);
                adhocWiFiNetworkReachability.Schedule(CFRunLoop.Current, CFRunLoop.ModeDefault);
            }

            if (!adhocWiFiNetworkReachability.TryGetFlags(out flags))
            {
                return false;
            }

            return IsReachableWithoutRequiringConnection(flags);
        }

        public static NetworkStatus RemoteHostStatus()
        {
            NetworkReachabilityFlags flags;
            bool reachable;

            if (remoteHostReachability == null)
            {
                remoteHostReachability = new NetworkReachability(hostName);

                // Need to probe before we queue, or we wont get any meaningful values
                // this only happens when you create NetworkReachability from a hostname
                reachable = remoteHostReachability.TryGetFlags(out flags);

                remoteHostReachability.SetCallback(OnChange);
                remoteHostReachability.Schedule(CFRunLoop.Current, CFRunLoop.ModeDefault);
            }
            else
            {
                reachable = remoteHostReachability.TryGetFlags(out flags);         
            }

            if (!reachable)
            {
                return NetworkStatus.NotReachable;
            }

            if (!IsReachableWithoutRequiringConnection(flags))
            {
                return NetworkStatus.NotReachable;
            }

            if ((flags & NetworkReachabilityFlags.IsWWAN) != 0)
            {
                return NetworkStatus.ReachableViaCarrierDataNetwork;
            }

            return NetworkStatus.ReachableViaWiFiNetwork;
        }

        public static NetworkStatus InternetConnectionStatus()
        {
            NetworkReachabilityFlags flags;
            bool defaultNetworkAvailable = IsNetworkAvailable(out flags);
            if (defaultNetworkAvailable && ((flags & NetworkReachabilityFlags.IsDirect) != 0))
            {
                return NetworkStatus.NotReachable;
            }
            else if ((flags & NetworkReachabilityFlags.IsWWAN) != 0)
            {
                return NetworkStatus.ReachableViaCarrierDataNetwork;
            }
            else if (flags == 0)
            {
                return NetworkStatus.NotReachable;
            }

            return NetworkStatus.ReachableViaWiFiNetwork;
        }

        public static NetworkStatus LocalWifiConnectionStatus()
        {
            NetworkReachabilityFlags flags;
            if (IsAdHocWiFiNetworkAvailable(out flags))
            {
                if ((flags & NetworkReachabilityFlags.IsDirect) != 0)
                {
                    return NetworkStatus.ReachableViaWiFiNetwork;
                }
            }

            return NetworkStatus.NotReachable;
        }

        private static void OnChange(NetworkReachabilityFlags flags)
        {
            var h = ReachabilityChanged;
            if (h != null)
            {
                h(null, EventArgs.Empty);
            }
        }

        private static bool IsNetworkAvailable(out NetworkReachabilityFlags flags)
        {
            if (defaultRouteReachability == null)
            {
                defaultRouteReachability = new NetworkReachability(new IPAddress(0));
                defaultRouteReachability.SetCallback(OnChange);
                defaultRouteReachability.Schedule(CFRunLoop.Current, CFRunLoop.ModeDefault);
            }

            if (!defaultRouteReachability.TryGetFlags(out flags))
            {
                return false;
            }

            return IsReachableWithoutRequiringConnection(flags);
        }   
    }
}
*/