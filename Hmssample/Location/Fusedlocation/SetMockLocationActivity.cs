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

using Android.Util;

using Com.Huawei.Hms.Location;
using Com.Huawei.Hmf.Tasks;

using XLocationDemoProjectRef.Logger;

namespace XLocationDemoProjectRef.Hmssample.Location.Fusedlocation
{
    [Activity(Label = "SetMockLocationActivity")]
    public class SetMockLocationActivity : LocationBaseActivity, View.IOnClickListener
    {
        public static readonly string TAG = "SetMockLocationActivity";

        private FusedLocationProviderClient mFusedLocationProviderClient;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_location_set_mock_location);
            // Button click listeners
            FindViewById(Resource.Id.location_setMockLocation).SetOnClickListener(this);
            AddLogFragment();
            mFusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(this);
        }

        /**
        * Set the specific mock location.
        */
        private void SetMockLocation()
        {
            try
            {
                //Fill in the information sources such as gps and network based on the application situation.
                Android.Locations.Location mockLocation = new Android.Locations.Location(Android.Locations.LocationManager.GpsProvider);
                mockLocation.Longitude = 118.76;
                mockLocation.Latitude = 31.98;
                // Note: To enable the mock function, enable the android.permission.ACCESS_MOCK_LOCATION permission in the AndroidManifest.xml file,
                // and set the application to the mock location app in the device setting.
                Task voidTask = mFusedLocationProviderClient.SetMockLocation(mockLocation);
                voidTask
                    .AddOnSuccessListener(new MockLocationOnSuccessListenerImpl(mockLocation))
                    .AddOnFailureListener(new MockLocationOnFailureListenerImpl());
            } catch (Exception e) {
                LocationLog.Error(TAG, "setMockLocation exception:" + e.Message);
            }
        }

        public void OnClick(View v)
        {
            try
            {
                switch (v.Id)
                {
                    case Resource.Id.location_setMockLocation:
                        SetMockLocation();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "setMockLocation Exception:" + e);
            }
        }
    }

    class MockLocationOnSuccessListenerImpl : Java.Lang.Object, IOnSuccessListener
    {
        public static readonly string TAG = "SetMockLocationActivity:SuccessListenerImpl";
        private Android.Locations.Location mockLocation;

        public MockLocationOnSuccessListenerImpl(Android.Locations.Location location)
        {
            mockLocation = location;
        }

        public void OnSuccess(Java.Lang.Object locObj)
        {
            LocationLog.Info(TAG, "setMockLocation onSuccess" + mockLocation.ToString());
        }
    }

    class MockLocationOnFailureListenerImpl : Java.Lang.Object, IOnFailureListener
    {
        public static readonly string TAG = "SetMockLocationActivity:FailureListenerImpl";

        public void OnFailure(Java.Lang.Exception e)
        {
            LocationLog.Error(TAG, "setMockLocation onFailure:" + e.Message);
        }
    }
}