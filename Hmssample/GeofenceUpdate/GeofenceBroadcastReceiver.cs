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
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;

using Com.Huawei.Hms.Location;
using Com.Huawei.Hmf.Tasks;

using XLocationDemoProjectRef.Logger;

namespace XLocationDemoProjectRef.Hmssample.GeofenceUpdate
{
    [BroadcastReceiver]
    public class GeofenceBroadcastReceiver : BroadcastReceiver
    {
        public static readonly string ACTION_PROCESS_LOCATION = "com.huawei.hmssample.geofence.GeoFenceBroadcastReceiver.ACTION_PROCESS_LOCATION";
        private static readonly string TAG = "GeoFenceReceiver";

        public override void OnReceive(Context context, Intent intent)
        {
            if(intent != null)
            {
                string action = intent.Action;
                StringBuilder sb = new StringBuilder();
                string next = "\n";
                if (ACTION_PROCESS_LOCATION.Equals(action))
                {
                    GeofenceData geofenceData = GeofenceData.GetDataFromIntent(intent);
                    if (geofenceData != null)
                    {
                        int errorCode = geofenceData.ErrorCode;
                        int conversion = geofenceData.Conversion;
                        List<IGeofence> list = (List<IGeofence>)geofenceData.ConvertingGeofenceList;
                        Android.Locations.Location myLocation = geofenceData.ConvertingLocation;
                        bool status = geofenceData.IsSuccess;
                        sb.Append("errorcode: " + errorCode + next);
                        sb.Append("conversion: " + conversion + next);
                        for (int i = 0; i < list.Count; i++)
                        {
                            sb.Append("geoFence id :" + list.ElementAt(i).UniqueId + next);
                        }
                        sb.Append("location is :" + myLocation.Longitude + " " + myLocation.Latitude + next);
                        sb.Append("is successful :" + status);
                        LocationLog.Info(TAG, sb.ToString());
                    }
                }
            }
        }
    }
}