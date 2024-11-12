using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Glitch9.IO.Network
{
    public static class NetworkUtils
    {
        public static async UniTask CheckNetworkAsync(int checkIntervalInMillis, int timeoutInMillis)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                GNLog.Warning("Internet is not reachable. Waiting for network connection...");
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutInMillis);

                try
                {
                    await UniTask.RunOnThreadPool(async () =>
                    {
                        await UniTask.Delay(checkIntervalInMillis);
                        while (Application.internetReachability == NetworkReachability.NotReachable)
                        {
                            await UniTask.Delay(checkIntervalInMillis); // Wait for half a second before checking again
                        }
                    }).Timeout(timeout);

                    GNLog.Info("Network connection re-established.");
                }
                catch (TimeoutException)
                {
                    GNLog.Error("Network check timed out.");
                    throw new TimeoutException("Network connection could not be re-established within the timeout period.");
                }
            }
        }
    }
}