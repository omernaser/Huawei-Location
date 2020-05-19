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
using Android.Util;

using Com.Huawei.Hms.Location;
using Com.Huawei.Hmf.Tasks;

using XLocationDemoProjectRef.Logger;

namespace XLocationDemoProjectRef.Hmssample.Location.Fusedlocation
{
    [Activity(Label = "SetMockModeActivity")]
    public class SetMockModeActivity : LocationBaseActivity, View.IOnClickListener, RadioGroup.IOnCheckedChangeListener
    {
        public static readonly System.String TAG = "SetMockModeActivity";

        //the mockMode flag
        private bool mMockFlag;

        private RadioGroup mRadioGroupSetMockMode;

        private FusedLocationProviderClient mFusedLocationProviderClient;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_location_set_mock_mode);
            // Button click listeners
            FindViewById(Resource.Id.location_setMockMode).SetOnClickListener(this);

            mRadioGroupSetMockMode = (RadioGroup) FindViewById(Resource.Id.radioGroup_mockMode);
            mRadioGroupSetMockMode.SetOnCheckedChangeListener(this);

            AddLogFragment();

            mFusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(this);
        }

        /**
     * Setting the mock Mode
     */
        private void SetMockMode()
        {
            try
            {
                Log.Info(TAG, "setMockMode mock mode is " + mMockFlag);
                // Note: To enable the mock function, enable the android.permission.ACCESS_MOCK_LOCATION permission in the AndroidManifest.xml file,
                // and set the application to the mock location app in the device setting.
                Task voidTask = mFusedLocationProviderClient.SetMockMode(mMockFlag);
                voidTask
                    .AddOnSuccessListener(new MockModeOnSuccessListenerImpl())
                    .AddOnFailureListener(new MockModeOnFailureListenerImpl());
            } catch (Exception e) {
                LocationLog.Error(TAG, "setMockMode exception:" + e.Message);
            }
    }

        public void OnClick(View v)
        {
            try
            {
                switch (v.Id)
                {
                    case Resource.Id.location_setMockMode:
                        SetMockMode();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "setMockMode Exception:" + e);
            }
        }

        public void OnCheckedChanged(RadioGroup group, int checkedId)
        {
            //If you do not need to simulate a location, set mode to false. Otherwise, other applications cannot use the positioning function of Huawei location service.
            RadioButton radioButton = (RadioButton) group.FindViewById(checkedId);
            mMockFlag = bool.Parse(radioButton.Text);
        }
    }

    class MockModeOnSuccessListenerImpl : Java.Lang.Object, IOnSuccessListener
    {
        public static readonly string TAG = "SetMockModeActivity:SuccessListenerImpl";

        public void OnSuccess(Java.Lang.Object locObj)
        {
            LocationLog.Info(TAG, "setMockMode onSuccess");
        }
    }

    class MockModeOnFailureListenerImpl : Java.Lang.Object, IOnFailureListener
    {
        public static readonly string TAG = "SetMockModeActivity:FailureListenerImpl";

        public void OnFailure(Java.Lang.Exception e)
        {
            LocationLog.Error(TAG, "setMockMode onFailure:" + e.Message);
        }
    }
}