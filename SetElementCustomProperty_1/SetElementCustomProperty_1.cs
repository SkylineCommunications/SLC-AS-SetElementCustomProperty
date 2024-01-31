/*
****************************************************************************
*  Copyright (c) 2023,  Skyline Communications NV  All Rights Reserved.    *
****************************************************************************

By using this script, you expressly agree with the usage terms and
conditions set out below.
This script and all related materials are protected by copyrights and
other intellectual property rights that exclusively belong
to Skyline Communications.

A user license granted for this script is strictly for personal use only.
This script may not be used in any way by anyone without the prior
written consent of Skyline Communications. Any sublicensing of this
script is forbidden.

Any modifications to this script by the user are only allowed for
personal use and within the intended purpose of the script,
and will remain the sole responsibility of the user.
Skyline Communications will not be responsible for any damages or
malfunctions whatsoever of the script resulting from a modification
or adaptation by the user.

The content of this script is confidential information.
The user hereby agrees to keep this confidential information strictly
secret and confidential and not to disclose or reveal it, in whole
or in part, directly or indirectly to any person, entity, organization
or administration without the prior written consent of
Skyline Communications.

Any inquiries can be addressed to:

	Skyline Communications NV
	Ambachtenstraat 33
	B-8870 Izegem
	Belgium
	Tel.	: +32 51 31 35 69
	Fax.	: +32 51 31 01 29
	E-mail	: info@skyline.be
	Web		: www.skyline.be
	Contact	: Ben Vandenberghe

****************************************************************************
Revision History:

DATE		VERSION		AUTHOR			COMMENTS

dd/mm/2023	1.0.0.1		XXX, Skyline	Initial version
****************************************************************************
*/

namespace SetElementCustomProperty_1
{
    using System;
    using System.Diagnostics;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Core.DataMinerSystem.Common;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    /// <summary>
    /// Represents a DataMiner Automation script.
    /// </summary>
    public class Script : IDisposable
    {
        private IDmsElement element;

        private InteractiveController app;

        private string propertyNameInputParameter;

        private PropertyDialog propertyDialog;

        public void Run(IEngine engine)
        {
            app = new InteractiveController(engine);

            var elementNameInputParameter = engine.GetScriptParam("Element Name").Value;
            propertyNameInputParameter = engine.GetScriptParam("Property Name").Value;
            var propertyActionInputParameter = (EditAction)Enum.Parse(typeof(EditAction), engine.GetScriptParam("Action").Value);

            if (!string.IsNullOrWhiteSpace(elementNameInputParameter)) element = IDmsElementExtensions.GetElement(elementNameInputParameter);

            var existingPropertyValue = GetExistingPropertyValue();
            switch (propertyActionInputParameter)
            {
                case EditAction.Delete:
                    SetPropertyValue(engine, string.Empty);
                    break;
                case EditAction.Edit:
                    propertyDialog = new PropertyDialog(engine, propertyNameInputParameter, existingPropertyValue);
                    propertyDialog.OkButton.Pressed += (s, e) => SetPropertyValue(engine, propertyDialog.MessageTextBox.Text);
                    propertyDialog.CancelButton.Pressed += (s, e) => engine.ExitSuccess("Element custom property set got canceled");
                    app.Run(propertyDialog);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown action: {propertyActionInputParameter}");
            }
        }

        private void EngineShowUiInComments()
        {
            //engine.ShowUI()
        }

        private void SetPropertyValue(IEngine engine, string propertyValue)
        {
            if (!string.IsNullOrWhiteSpace(propertyNameInputParameter))
            {
                if (element != null)
                {
                    element.SetPropertyValue(propertyNameInputParameter, propertyValue);
                    Retry(() => { return element.GetPropertyValue(propertyNameInputParameter) == propertyValue; }, TimeSpan.FromSeconds(5)); // Checking if property is eventually updated
                }
            }

            engine.ExitSuccess("Success");
        }

        private string GetExistingPropertyValue()
        {
            if (element != null && !string.IsNullOrWhiteSpace(propertyNameInputParameter))
            {
                return element.GetPropertyValue(propertyNameInputParameter);
            }

            return string.Empty;
        }

        /// <summary>
        /// Retry until success or until timeout. 
        /// </summary>
        /// <param name="func">Operation to retry.</param>
        /// <param name="timeout">Max TimeSpan during which the operation specified in <paramref name="func"/> can be retried.</param>
        private static void Retry(Func<bool> func, TimeSpan timeout)
        {
            bool success = false;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            do
            {
                success = func();
                if (!success)
                {
                    System.Threading.Thread.Sleep(100);
                }
            }
            while (!success && sw.Elapsed <= timeout);
        }

        #region IDisposable Support
        private bool isDisposed;
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~Script()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}