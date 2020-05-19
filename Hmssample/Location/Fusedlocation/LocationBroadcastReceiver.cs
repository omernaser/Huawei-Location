/*
*       Copyright 2020. Huawei Technologies Co., Ltd. All rights reserved.

        Licensed under the Apache License, Version 2.0 (the "License");
        you may not use this file except in compliance with the License.
        You may obtain a copy of the License at

        http://www.apache.org/licenses/LICENSE-2.0

        Unless required by applicable law or agreed to in writing, software
        distributed under the License is distributed on an "AS IS" BASIS,
        WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
        See the License for the specific language governing permissions and
        limitations under the License.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Com.Huawei.Hms.Location;

using Android.Util;

using XLocationDemoProjectRef.Logger;
using XLocationDemoProjectRef.Hmssample.ActivityUpdate;

/**
 * location broadcast receiver
 */
namespace XLocationDemoProjectRef.Hmssample.Location.Fusedlocation
{
    [BroadcastReceiver]
    public class LocationBroadcastReceiver : BroadcastReceiver
    {
        public static readonly string ACTION_PROCESS_LOCATION = "com.huawei.hms.location.ACTION_PROCESS_LOCATION";

        private static readonly string TAG = "HuaweiLocationReceiver";

        public static bool isListenActivityIdentification = false;

        public static bool isListenActivityConversion = false;

        public override void OnReceive(Context context, Intent intent)
        {
            if(intent != null)
            {
                string action = intent.Action;
                StringBuilder sb = new StringBuilder();
                if(ACTION_PROCESS_LOCATION.Equals(action))
                {
                    // Processing LocationResult information
                    Log.Info(TAG, "null != intent");
                    string messageBack = "";
                    ActivityConversionResponse activityTransitionResult = ActivityConversionResponse.GetDataFromIntent(intent);
                    if (activityTransitionResult != null && isListenActivityConversion == true)
                    {
                        IList<ActivityConversionData> list = activityTransitionResult.ActivityConversionDatas;
                        for (int i = 0; i < list.Count; i++)
                        {
                            Log.Info(TAG, "activityTransitionEvent[" + i + "]" + list.ElementAt(i));
                            messageBack += list.ElementAt(i) + "\n";
                        }
                        LocationLog.Debug("TAG", messageBack);
                    }

                    ActivityIdentificationResponse activityRecognitionResult = ActivityIdentificationResponse.GetDataFromIntent(intent);
                    if (activityRecognitionResult != null && isListenActivityIdentification == true)
                    {
                        LocationLog.Info(TAG, "activityRecognitionResult:" + activityRecognitionResult);
                        IList<ActivityIdentificationData> list = activityRecognitionResult.ActivityIdentificationDatas;
                        ActivityIdentificationActivity.SetData(list);
                    }

                    if (LocationResult.HasResult(intent))
                    {
                        LocationResult result = LocationResult.ExtractResult(intent);
                        if(result != null)
                        {
                            IList<Android.Locations.Location> locations = result.Locations;
                            if(locations.Count != 0)
                            {
                                sb.Append("requestLocationUpdatesWithIntent[Longitude,Latitude,Accuracy]:\r\n");
                                foreach (Android.Locations.Location location in locations)
                                {
                                    sb.Append(location.Longitude)
                                    .Append(",")
                                    .Append(location.Latitude)
                                    .Append(",")
                                    .Append(location.Accuracy)
                                    .Append("\r\n");
                                }
                            }
                        }
                    }

                    // Processing LocationAvailability information
                    if (LocationAvailability.HasLocationAvailability(intent))
                    {
                        LocationAvailability locationAvailability =
                            LocationAvailability.ExtractLocationAvailability(intent);
                        if (locationAvailability != null)
                        {
                            sb.Append("[locationAvailability]:" + locationAvailability.IsLocationAvailable + "\r\n");
                        }
                    }
                    if (sb.ToString().Length != 0)
                    {
                        LocationLog.Info(TAG, sb.ToString());
                    }
                }
            }
        }
        public static void AddConversionListener()
        {
            isListenActivityConversion = true;
        }
        public static void RemoveConversionListener()
        {
            isListenActivityConversion = false;
        }
        public static void AddIdentificationListener()
        {
            isListenActivityIdentification = true;
        }
        public static void RemoveIdentificationListener()
        {
            isListenActivityIdentification = false;
        }
    }
}