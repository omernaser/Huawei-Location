# Huawei Xamarin Location Demo

## Contents
- Introduction
- Installation Guide
- React-Native Example Method Definition
- Configuration Description
- Licensing and Terms

## 1. Introduction

The demo project is an example that aims to demonstrate how the HUAWEI Location Kit SDK for Xamarin can be used.

The Xamarin SDK wraps the Android SDK with Managed Callable Wrappers through the usage of Android Bindings Library projects. It provides the same APIs as the native SDK.

The Xamarin SDK libraries are described as follows:

- Library .DLL files: These are the files enable the usage of the native Android SDK interfaces. Once generated, these files can be referenced & used directly in a Xamarin.Android project.

## 2. Installation Guide
Before using the Xamarin SDK code, ensure that Visual Studio 2019 is installed with "Mobile development with .NET" support.

### 2.1 Huawei Xamarin Location Library
You can retrieve the library from [developer.huawei.com](https://developer.huawei.com)

### 2.2 Download native Android SDK packages
The location SDK and its dependencies must be downloaded from the Huawei repository.
Use the following URLs to download the packages.
- [location-4.0.2.300.aar](https://developer.huawei.com/repo/com/huawei/hms/location/4.0.2.300/location-4.0.2.300.aar)
- [tasks-1.3.3.300.aar](https://developer.huawei.com/repo/com/huawei/hmf/tasks/1.3.3.300/tasks-1.3.3.300.aar)
- [update-2.0.6.300.aar](https://developer.huawei.com/repo/com/huawei/hms/update/2.0.6.300/update-2.0.6.300.aar)
- [network-grs-4.0.2.300.aar](https://developer.huawei.com/repo/com/huawei/hms/network-grs/4.0.2.300/network-grs-4.0.2.300.aar)
- [network-common-4.0.2.300.aar](https://developer.huawei.com/repo/com/huawei/hms/network-common/4.0.2.300/network-common-4.0.2.300.aar)
- [base-4.0.2.300.aar](https://developer.huawei.com/repo/com/huawei/hms/base/4.0.2.300/base-4.0.2.300.aar)
- [agconnect-core-1.0.0.300.aar](https://developer.huawei.com/repo/com/huawei/agconnect/agconnect-core/1.0.0.300/agconnect-core-1.0.0.300.aar)

### 2.3 Open the library project
An Android Bindings Library project for Xamarin allows the usage of only one .aar file. For this reason the library repository comes with multiple library projects. 

Open up Visual Studio 2019. Then from the menu;
	
- Click "Open a project or a solution"
- Navigate to the directory where you cloned the repository and open "XLocation-4.0.2.300.csproj".

### 2.4 Import the downloaded packages
Once you open the library project for the location SDK, each package you downloaded in the first step must placed under its related library project.

Inside the "Solution Explorer", expand each project and repeat the steps below:
- Right click "Jars" -> "Add" -> "Existing Item" (Shift + Alt + A)
- Navigate to the folder where you downloaded the packages and select the related .aar or .jar file.	
    
         Example: For XTasks-1.3.3.300 project, import "tasks-1.3.3.300.aar"
- Click on the package file you just imported. 
		In the **properties** window, 
			
    - set the Build Action as "LibraryProjectZip" if the file type is .aar
	- set the Build Action as "EmbeddedJar" if the file type is .jar

### 2.5 Build the library.
From the Visual Studio's toolbar, click "Build" -> "Build Solution" (Ctrl + Shift + B).
Once the build process is complete, generated classes should be visible in the object browser and ready to use.

(View -> Object Browser) (Ctrl + Alt + J)

### 2.6 Copy the libary .dll files
Once you build the location SDK library project, the generated .dll files should be copied inside the demo project.
- Copy the .dll files from "...\XLocation-4.0.2.300\bin\Debug\" or "...\XLocation-4.0.2.300\bin\Release\" depending on your build type selection.
- To the "_LibDlls" folder of the demo project. 

### 2.7 Run the application
You can now run your application and it should automatically start up on your mobile device.

## 3. Xamarin Example method definition
No. Developer can flexibly develop projects based on the example code. 

## 4. Configure parameters.    
No.

## 5. Licensing and Terms  
Huawei Xamarin SDK uses the Apache 2.0 license.

