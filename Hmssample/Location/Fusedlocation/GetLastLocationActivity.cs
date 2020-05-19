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

using Android.App;
using Android.OS;
using Android.Views;
using Android.Util;

using Com.Huawei.Hms.Location;
using Com.Huawei.Hmf.Tasks;

using XLocationDemoProjectRef.Logger;

namespace XLocationDemoProjectRef.Hmssample.Location.Fusedlocation
{
    [Activity(Label = "GetLastLocationActivity")]
    public class GetLastLocationActivity : LocationBaseActivity, View.IOnClickListener
    {
        public static readonly string TAG = "GetLocationActivity";

        private FusedLocationProviderClient mFusedLocationProviderClient;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_location_get_last_location);
            // Create your application here
            FindViewById(Resource.Id.location_getLastLocation).SetOnClickListener(this);
            AddLogFragment();
            mFusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(this);
        }

        private void GetLastLocation()
        {
            Task lastLocation = mFusedLocationProviderClient.LastLocation;
            lastLocation
                .AddOnSuccessListener(new LastLocOnSuccessListenerImpl())
                .AddOnFailureListener(new LastLocOnFailureListenerImpl());
        }

        public void OnClick(View v)
        {
            try
            {
                switch (v.Id)
                {
                    case Resource.Id.location_getLastLocation:
                        GetLastLocation();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Info(TAG, "GetLastLocationActivity Exception:" + e);
            }
        }
    }

    class LastLocOnSuccessListenerImpl : Java.Lang.Object, IOnSuccessListener
    {
        public static readonly string TAG = "GetLocationActivity:SuccessListenerImpl";

        public void OnSuccess(Java.Lang.Object locObj)
        {
            if (locObj == null)
            {
                LocationLog.Info(TAG, "getLastLocation onSuccess location is null");
                return;
            }
            Android.Locations.Location location = (Android.Locations.Location) locObj;
            LocationLog.Info(TAG,
                "getLastLocation onSuccess location[Longitude,Latitude]:" + location.Longitude + ","
                + location.Latitude
                );
            return;
        }
    }

    class LastLocOnFailureListenerImpl : Java.Lang.Object, IOnFailureListener
    {
        public static readonly string TAG = "GetLocationActivity:FailureListenerImpl";

        public void OnFailure(Java.Lang.Exception e)
        {
            LocationLog.Error(TAG, "getLastLocation onFailure:" + e.Message);
        }
    }
}