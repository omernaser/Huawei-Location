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
    [Activity(Label = "GetLocationAvailabilityActivity")]
    public class GetLocationAvailabilityActivity : LocationBaseActivity, View.IOnClickListener
    {
        public static readonly string TAG = "GetLocationAvailabilityActivity";

        private FusedLocationProviderClient mFusedLocationProviderClient;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_location_get_location_availability);
            // Button click listeners
            FindViewById(Resource.Id.location_getLocationAvailability).SetOnClickListener(this);
            AddLogFragment();

            mFusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(this);
        }

        private void GetLocationAvailability()
        {
            try {
                Task locationAvailability = mFusedLocationProviderClient.LocationAvailability;
                locationAvailability
                    .AddOnSuccessListener(new LocAvailOnSuccessListenerImpl())
                    .AddOnFailureListener(new LocAvailOnFailureListenerImpl());
            }
            catch (Exception e)
            {
                LocationLog.Error(TAG, "getLocationAvailability exception:" + e.Message);
            }
        }

        public void OnClick(View v)
        {
            try
            {
                switch (v.Id)
                {
                    case Resource.Id.location_getLocationAvailability:
                        GetLocationAvailability();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Info(TAG, "GetLocationAvailability Exception:" + e);
            }
        }
    }

    class LocAvailOnSuccessListenerImpl : Java.Lang.Object, IOnSuccessListener
    {
        public static readonly string TAG = "GetLocationAvailability:SuccessListenerImpl";

        public void OnSuccess(Java.Lang.Object locObj)
        {
            if (locObj == null)
            {
                LocationLog.Info(TAG, "getLocationAvailability onSuccess location is null");
                return;
            }
            LocationAvailability locationAvailability = (LocationAvailability) locObj;
            LocationLog.Info(TAG, "getLocationAvailability onSuccess:" + locationAvailability.ToString());
        }
    }

    class LocAvailOnFailureListenerImpl : Java.Lang.Object, IOnFailureListener
    {
        public static readonly string TAG = "GetLocationAvailability:FailureListenerImpl";

        public void OnFailure(Java.Lang.Exception e)
        {
            LocationLog.Error(TAG, "getLocationAvailability onFailure:" + e.Message);
        }
    }
}