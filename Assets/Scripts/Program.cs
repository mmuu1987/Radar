/*
	Simple TUIO v1.1 / OSC v1.1 network listener.
	
	Listen for TUIO or OSC network traffic and output it to the console. Useful for quickly checking/debugging data sent from TUIO server apps.
	
	Defaults to listening for TUIO on port 3333. Output radians/degrees using rads/degs options. Invert X/Y axis values in TUIO data using the invertx/y/xy options.
	
	Usage:
		> mono TUIOListener [port] [tuio|osc] [rads|degs] [invertx|inverty|invertxy]
		> mono TUIOListener -help
	Libraries:
		https://github.com/valyard/TUIOsharp (v1.1 development branch)
		https://github.com/valyard/OSCsharp
	
	Author:
		Greg Harding greg@flightless.co.nz
		www.flightless.co.nz
	
	Copyright 2015 Flightless Ltd.
	
	The MIT License (MIT)
	Copyright (c) 2015 Flightless Ltd
	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:
	The above copyright notice and this permission notice shall be included in all
	copies or substantial portions of the Software.
	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
	SOFTWARE.
*/

using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;

using TUIOsharp;
using TUIOsharp.DataProcessors;
using TUIOsharp.Entities;

using OSCsharp;
using OSCsharp.Data;
using OSCsharp.Net;
using OSCsharp.Utils;
using UnityEngine;

namespace TUIOListener
{

    class Program : MonoBehaviour
    {

        //public enum MessageType
        //{
        //    TUIO,
        //    OSC
        //};

        public static int port = 3333;
        //public static MessageType messageType = MessageType.TUIO;
        public static bool degs = false;
        public static bool invertX = false;
        public static bool invertY = false;

        private static TuioServer tuioServer;
        private int screenWidth;
        private int screenHeight;

        private void connect()
        {
            if (!Application.isPlaying) return;
            if (tuioServer != null) disconnect();

            tuioServer = new TuioServer(port);
            Debug.Log("TUIO Port" + port);
            tuioServer.Connect();
            Debug.Log("TUIO Connect");
        }
        private void OnEnable()
        {
            Debug.Log(string.Format("TUIO listening on port {0}... (Press escape to quit)", port));

            screenWidth = Screen.width;
            screenHeight = Screen.height;
            // tuio

            CursorProcessor cursorProcessor = new CursorProcessor();
            
            cursorProcessor.CursorAdded += OnCursorAdded;
            cursorProcessor.CursorUpdated += OnCursorUpdated;
            cursorProcessor.CursorRemoved += OnCursorRemoved;

            BlobProcessor blobProcessor = new BlobProcessor();
            blobProcessor.BlobAdded += OnBlobAdded;
            blobProcessor.BlobUpdated += OnBlobUpdated;
            blobProcessor.BlobRemoved += OnBlobRemoved;

            ObjectProcessor objectProcessor = new ObjectProcessor();
            objectProcessor.ObjectAdded += OnObjectAdded;
            objectProcessor.ObjectUpdated += OnObjectUpdated;
            objectProcessor.ObjectRemoved += OnObjectRemoved;

            // listen...
            connect();
            tuioServer.AddDataProcessor(cursorProcessor);
            tuioServer.AddDataProcessor(blobProcessor);
            tuioServer.AddDataProcessor(objectProcessor);





            Debug.Log("connect");
        }

        protected void OnDisable()
        {
            disconnect();
        }


        private void OnCursorAdded(object sender, TuioCursorEventArgs e)
        {
            
            var entity = e.Cursor;
            lock (tuioServer)
            {
                //var x = invertX ? (1 - entity.X) : entity.X;
                //var y = invertY ? (1 - entity.Y) : entity.Y;
                var x = entity.X * screenWidth;
                var y = (1 - entity.Y) * screenHeight;
                Debug.Log(string.Format("{0} Cursor Added {1}:{2},{3}", ((CursorProcessor)sender).FrameNumber, entity.Id, x, y));
            }
            Debug.Log("OnCursorAdded");
        }

        private void OnCursorUpdated(object sender, TuioCursorEventArgs e)
        {
            var entity = e.Cursor;
            lock (tuioServer)
            {
                //var x = invertX ? (1 - entity.X) : entity.X;
                //var y = invertY ? (1 - entity.Y) : entity.Y;
                var x = Mathf.Round(entity.X * screenWidth);
                var y = (1 - entity.Y) * screenHeight;
                Debug.Log(string.Format("{0} Cursor Moved {1}:{2},{3}", ((CursorProcessor)sender).FrameNumber, entity.Id, x, y));
                //MyTest.Instance.LimitGetPos(remapCoordinates(new Vector2(x, y)));
                //项目中测试方法 
                //MyTest.Instance.LimitGetPos(new Vector2(x, y));
            }
            Debug.Log("OnCursorUpdated");
        }

        private void OnCursorRemoved(object sender, TuioCursorEventArgs e)
        {
            var entity = e.Cursor;
            lock (tuioServer)
            {
                Debug.Log(string.Format("{0} Cursor Removed {1}", ((CursorProcessor)sender).FrameNumber, entity.Id));
            }
        }

        private static void OnBlobAdded(object sender, TuioBlobEventArgs e)
        {
            var entity = e.Blob;
            lock (tuioServer)
            {
                var x = invertX ? (1 - entity.X) : entity.X;
                var y = invertY ? (1 - entity.Y) : entity.Y;
                var angle = degs ? (entity.Angle * (180f / Math.PI)) : entity.Angle;
                Debug.Log(string.Format("{0} Blob Added {1}:{2},{3} {4:F3}", ((BlobProcessor)sender).FrameNumber, entity.Id, x, y, angle));
            }
            Debug.Log("OnBlobAdded");
        }

        private static void OnBlobUpdated(object sender, TuioBlobEventArgs e)
        {
            var entity = e.Blob;
            lock (tuioServer)
            {
                var x = invertX ? (1 - entity.X) : entity.X;
                var y = invertY ? (1 - entity.Y) : entity.Y;
                var angle = degs ? (entity.Angle * (180f / Math.PI)) : entity.Angle;
                Debug.Log(string.Format("{0} Blob Moved {1}:{2},{3} {4:F3}", ((BlobProcessor)sender).FrameNumber, entity.Id, x, y, angle));
            }
            Debug.Log("OnBlobUpdated");
        }

        private static void OnBlobRemoved(object sender, TuioBlobEventArgs e)
        {
            var entity = e.Blob;
            lock (tuioServer)
            {
                Debug.Log(string.Format("{0} Blob Removed {1}", ((BlobProcessor)sender).FrameNumber, entity.Id));
            }
            Debug.Log("OnBlobRemoved");
        }

        private static void OnObjectAdded(object sender, TuioObjectEventArgs e)
        {
            var entity = e.Object;
            lock (tuioServer)
            {
                var x = invertX ? (1 - entity.X) : entity.X;
                var y = invertY ? (1 - entity.Y) : entity.Y;
                var angle = degs ? (entity.Angle * (180f / Math.PI)) : entity.Angle;
                Debug.Log(string.Format("{0} Object Added {1}/{2}:{3},{4} {5:F3}", ((ObjectProcessor)sender).FrameNumber, entity.ClassId, entity.Id, x, y, angle));
            }
            Debug.Log("OnObjectAdded");
        }

        private static void OnObjectUpdated(object sender, TuioObjectEventArgs e)
        {
            var entity = e.Object;
            lock (tuioServer)
            {
                var x = invertX ? (1 - entity.X) : entity.X;
                var y = invertY ? (1 - entity.Y) : entity.Y;
                var angle = degs ? (entity.Angle * (180f / Math.PI)) : entity.Angle;
                Debug.Log(string.Format("{0} Object Moved {1}/{2}:{3},{4} {5:F3}", ((ObjectProcessor)sender).FrameNumber, entity.ClassId, entity.Id, x, y, angle));
            }
        }

        private  void OnObjectRemoved(object sender, TuioObjectEventArgs e)
        {
            var entity = e.Object;
            lock (tuioServer)
            {
                Debug.Log(string.Format("{0} Object Removed {1}/{2}", ((ObjectProcessor)sender).FrameNumber, entity.ClassId, entity.Id));
            }
        }
        void disconnect()
        {
            if (tuioServer != null)
            {
                tuioServer.RemoveAllDataProcessors();
                tuioServer.Disconnect();
                tuioServer = null;
            }
        }
    }

}