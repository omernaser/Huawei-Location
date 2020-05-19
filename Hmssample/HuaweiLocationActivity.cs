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

using Android;
using Android.App;
using Android.OS;
using Android.Content;
using Android.Content.PM;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Views;
using Android.Util;

using XLocationDemoProjectRef.Hmssample.Location.Fusedlocation;
using XLocationDemoProjectRef.Hmssample.ActivityUpdate;
using XLocationDemoProjectRef.Hmssample.GeofenceUpdate;

namespace XLocationDemoProjectRef.Hmssample
{
    [Activity(Label = "HuaweiXamarinLocationDemo", Theme = "@style/AppTheme", MainLauncher = true)]
    class HuaweiLocationActivity : AppCompatActivity, View.IOnClickListener
    {
        public static readonly string TAG = "HuaweiLocationActivity";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_huaweilocation);
            //Button click listeners
            FindViewById(Resource.Id.location_requestLocationUpdatesWithIntent).SetOnClickListener(this);
            FindViewById(Resource.Id.location_getLastLocation).SetOnClickListener(this);
            FindViewById(Resource.Id.location_getLocationAvailability).SetOnClickListener(this);
            FindViewById(Resource.Id.location_requestLocationUpdatesWithCallback).SetOnClickListener(this);
            FindViewById(Resource.Id.location_setMockLocation).SetOnClickListener(this);
            FindViewById(Resource.Id.location_setMockMode).SetOnClickListener(this);
            FindViewById(Resource.Id.location_activity_update).SetOnClickListener(this);
            FindViewById(Resource.Id.location_activity_transition_update).SetOnClickListener(this);
            FindViewById(Resource.Id.GeoFence).SetOnClickListener(this);

            if (Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this,
                    Manifest.Permission.AccessFineLocation) != Permission.Granted
                    && Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this,
                    Manifest.Permission.AccessCoarseLocation) != Permission.Granted)
            {
                string[] strings =
                        {Manifest.Permission.AccessFineLocation, Manifest.Permission.AccessCoarseLocation};
                ActivityCompat.RequestPermissions(this, strings, 1);
            }
        }

        void View.IOnClickListener.OnClick(View v)
        {
            try
            {
                switch (v.Id)
                {
                    case Resource.Id.location_requestLocationUpdatesWithIntent:
                        Intent updatesWithIntent = new Intent();
                        updatesWithIntent.SetClass(this, typeof(RequestLocationUpdatesWithIntentActivity));
                        updatesWithIntent.SetFlags(ActivityFlags.ClearTop);
                        StartActivity(updatesWithIntent);
                        break;
                    case Resource.Id.location_getLastLocation:                      
                        Intent lastlocationIntent = new Intent();
                        lastlocationIntent.SetClass(this, typeof(GetLastLocationActivity));
                        lastlocationIntent.SetFlags(ActivityFlags.ClearTop);
                        StartActivity(lastlocationIntent);
                        break;
                    case Resource.Id.location_getLocationAvailability:
                        Intent locationAvailabilityIntent = new Intent();
                        locationAvailabilityIntent.SetClass(this, typeof(GetLocationAvailabilityActivity));
                        locationAvailabilityIntent.SetFlags(ActivityFlags.ClearTop);
                        StartActivity(locationAvailabilityIntent);
                        break;
                    case Resource.Id.location_requestLocationUpdatesWithCallback:
                        Intent updatesWithCallback = new Intent();
                        updatesWithCallback.SetClass(this, typeof(RequestLocationUpdatesWithCallbackActivity));
                        updatesWithCallback.SetFlags(ActivityFlags.ClearTop);
                        StartActivity(updatesWithCallback);
                        break;
                    case Resource.Id.location_setMockLocation:
                        Intent mockLocationIntent = new Intent();
                        mockLocationIntent.SetClass(this, typeof(SetMockLocationActivity));
                        mockLocationIntent.SetFlags(ActivityFlags.ClearTop);
                        StartActivity(mockLocationIntent);
                        break;
                    case Resource.Id.location_setMockMode:
                        Intent mockModeIntent = new Intent();
                        mockModeIntent.SetClass(this, typeof(SetMockModeActivity));
                        mockModeIntent.SetFlags(ActivityFlags.ClearTop);
                        StartActivity(mockModeIntent);
                        break;
                    case Resource.Id.GeoFence:
                        Intent geoFenceIntent = new Intent();
                        geoFenceIntent.SetClass(this, typeof(OperateGeofenceActivity));
                        geoFenceIntent.SetFlags(ActivityFlags.ClearTop);
                        StartActivity(geoFenceIntent);
                        break;
                    case Resource.Id.location_activity_transition_update:
                        Intent locationIntent3 = new Intent();
                        locationIntent3.SetClass(this, typeof(ActivityConversionActivity));
                        locationIntent3.SetFlags(ActivityFlags.ClearTop);
                        StartActivity(locationIntent3);
                        break;
                    case Resource.Id.location_activity_update:
                        Intent locationIntent4 = new Intent();
                        locationIntent4.SetClass(this, typeof(ActivityIdentificationActivity));
                        locationIntent4.SetFlags(ActivityFlags.ClearTop);
                        StartActivity(locationIntent4);
                        break;
                    default:
                        break;
                }
            } catch (Exception e)
            {
                Log.Info(TAG, "HuaweiLocation Exception:" + e);
            }
        }
    }
}