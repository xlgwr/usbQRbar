using Microsoft.PointOfService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace usbQRBar
{
    // The following code implements a thread helper class.
    // This class may be used by other Point Of Service
    // samples which may require a separate thread for monitoring
    // hardware.
    public abstract class ServiceObjectThreadHelper : IDisposable
    {
        // The thread object which will wait for data from the POS
        // device.
        private Thread ReadThread;

        // These events signal that the thread is starting or stopping.
        private AutoResetEvent ThreadTerminating;
        private AutoResetEvent ThreadStarted;

        // Keeps track of whether or not a thread should
        // be running.
        bool ThreadWasStarted;

        public ServiceObjectThreadHelper()
        {
            // Create events to signal the reader thread.
            ThreadTerminating = new AutoResetEvent(false);
            ThreadStarted = new AutoResetEvent(false);

            ThreadWasStarted = false;

            // You need to handle the ApplicationExit event so
            // that you can properly clean up the thread.
            System.Windows.Forms.Application.ApplicationExit +=
                        new EventHandler(Application_ApplicationExit);
        }

        ~ServiceObjectThreadHelper()
        {
            Dispose(true);
        }

        public virtual void ServiceObjectThreadOpen()
        {
            return;
        }

        public virtual void ServiceObjectThreadClose()
        {
            return;
        }

        // This is called when the thread starts successfully and
        // will be run on the new thread.
        public abstract void ServiceObjectThreadProcedure(
                AutoResetEvent ThreadStopEvent);

        private bool IsDisposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                try
                {
                    if (disposing == true)
                    {
                        CloseThread();
                    }
                }
                finally
                {
                    IsDisposed = true;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);

            // This object has been disposed of, so no need for
            // the GC to call the finalization code again.
            GC.SuppressFinalize(this);
        }

        public void OpenThread()
        {
            try
            {
                // Check to see if this object is still valid.
                if (IsDisposed)
                {
                    // Throw system exception to indicate that
                    // the object has already been disposed.
                    throw new ObjectDisposedException(
                            "ServiceObjectSampleThread");
                }

                // In case the application has called OpenThread
                // before calling CloseThread, stop any previously
                // started thread.
                SignalThreadClose();

                ServiceObjectThreadOpen();

                // Reset event used to signal the thread to quit.
                ThreadTerminating.Reset();

                // Reset the event that used by the thread to signal
                // that it has started.
                ThreadStarted.Reset();

                // Create the thread object and give it a name. The 
                // method used here, ThreadMethod, is a wrapper around
                // the actual thread procedure, which will be run in 
                // the threading object provided by the Service 
                // Object.
                ReadThread = new Thread(
                        new ThreadStart(ThreadMethod));

                // Set the thread background mode.
                ReadThread.IsBackground = false;

                // Finally, attempt to start the thread.
                ReadThread.Start();

                // Wait for the thread to start, or until the time-out
                // is reached.
                if (!ThreadStarted.WaitOne(3000, false))
                {
                    // If the time-out was reached before the event
                    // was set, then throw an exception.
                    throw new PosControlException(
                            "Unable to open the device for reading",
                            ErrorCode.Failure);
                }

                // The thread has started succesfully.
                ThreadWasStarted = true;
            }
            catch (Exception e)
            {
                // If an error occurred, be sure the new thread is
                // stopped.
                CloseThread();

                // Re-throw to let the application handle the 
                // failure.
                throw;
            }
        }

        private void SignalThreadClose()
        {
            if (ThreadTerminating != null && ThreadWasStarted)
            {
                // Tell the thread to terminate.
                ThreadTerminating.Set();

                // Give the thread a few seconds to end.
                ThreadStarted.WaitOne(10000, false);

                // Mark the thread as being terminated.
                ThreadWasStarted = false;
            }
        }

        public void CloseThread()
        {
            // Signal the thread that it should stop.
            SignalThreadClose();

            // Call back into the SO for any cleanup.
            ServiceObjectThreadClose();
        }

        private void Application_ApplicationExit(
                            object sender,
                            EventArgs e)
        {
            SignalThreadClose();
        }

        // This is the method run on the new thread. First it signals
        // the caller indicating that the thread has started 
        // correctly. Next, it calls the service object's thread
        // method which will loop waiting for data or a signal
        // to close.
        private void ThreadMethod()
        {
            try
            {
                // Set the event to indicate that the thread has
                // started successfully.
                ThreadStarted.Set();

                // Call into the thread procedure defined by the
                // Service Object.
                ServiceObjectThreadProcedure(ThreadTerminating);

                // Signal that the thread procedure is exiting.
                ThreadStarted.Set();
            }
            catch (Exception e)
            {
                Logger.Info("ServiceObjectThreadHelper",
                        "ThreadMethod Exception = " + e.ToString());
                throw;
            }
        }
    }

}
