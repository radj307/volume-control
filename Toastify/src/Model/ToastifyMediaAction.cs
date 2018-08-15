using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Toastify.Core;
using ToastifyAPI.Events;
using ToastifyAPI.Native;
using ToastifyAPI.Native.Enums;
using Spotify = ToastifyAPI.Spotify;

namespace Toastify.Model
{
    public abstract class ToastifyMediaAction : ToastifyAction
    {
        #region Public properties

        /// <summary>
        ///     <i>cmd</i> value of a <see cref="WindowsMessagesFlags.WM_APPCOMMAND" />.
        /// </summary>
        /// <remarks> See: https://msdn.microsoft.com/en-us/library/windows/desktop/ms646275.aspx </remarks>
        public virtual long AppCommandCode { get; }

        /// <summary>
        ///     The virtual key code associated with this media action, if any.
        /// </summary>
        public virtual VirtualKeyCode VirtualKeyCode { get; }

        /// <summary>
        ///     The media action type, i.e. how this media action will be performed.
        ///     <para />
        ///     The default value is <see cref="MediaActionType.AppCommandMessage" />.
        /// </summary>
        public MediaActionType ActionType { get; set; } = MediaActionType.AppCommandMessage;

        #endregion

        protected ToastifyMediaAction()
        {
        }

        protected ToastifyMediaAction(string name, ToastifyActionEnum actionEnum, long appCommandCode) : this(name, actionEnum, appCommandCode, VirtualKeyCode.None)
        {
        }

        protected ToastifyMediaAction(string name, ToastifyActionEnum actionEnum, long appCommandCode, VirtualKeyCode virtualKeyCode) : base(name, actionEnum)
        {
            this.AppCommandCode = appCommandCode;
            this.VirtualKeyCode = virtualKeyCode;
        }

        protected void PerformMediaAction()
        {
            switch (this.ActionType)
            {
                case MediaActionType.AppCommandMessage:
                    this.PerformActionAsAppCommandMessage();
                    break;

                case MediaActionType.MediaKey:
                    this.PerformActionAsMediaKey();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void PerformActionAsAppCommandMessage()
        {
            // We need Spotify's main window handle
            Process process = Spotify.FindSpotifyProcess();
            if (process == null)
                this.RaiseActionFailed(this, new ActionFailedEventArgs("Couldn't find Spotify's process."));
            else
            {
                IntPtr hWnd = Spotify.GetMainWindowHandle(unchecked((uint)process.Id));
                if (hWnd == IntPtr.Zero)
                    this.RaiseActionFailed(this, new ActionFailedEventArgs("Couldn't find Spotify's main window handle."));
                else
                {
                    if (Windows.SendAppCommandMessage(hWnd, (IntPtr)this.AppCommandCode, true))
                        this.RaiseActionPerformed(this);
                    else
                        this.RaiseActionFailed(this, new ActionFailedEventArgs($"PostMessage failed with error code {Marshal.GetLastWin32Error()}"));
                }
            }
        }

        private void PerformActionAsMediaKey()
        {
            if (this.VirtualKeyCode == VirtualKeyCode.None)
                this.RaiseActionFailed(this, new ActionFailedEventArgs("This action doesn't have a virtual key code."));
            else if (Enum.IsDefined(typeof(VirtualKeyCode), this.VirtualKeyCode))
                User32.KeyboardEvent(this.VirtualKeyCode, 0, 1, IntPtr.Zero);
            else
                this.RaiseActionFailed(this, new ActionFailedEventArgs("Invalid virtual key code."));
        }

        #region Equals / GetHashCode

        protected bool Equals(ToastifyMediaAction other)
        {
            return base.Equals(other) &&
                   this.AppCommandCode == other.AppCommandCode &&
                   this.VirtualKeyCode == other.VirtualKeyCode;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj.GetType() == this.GetType() && this.Equals((ToastifyMediaAction)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)this.AppCommandCode;
                hashCode = (hashCode * 397) ^ this.VirtualKeyCode.GetHashCode();
                return hashCode;
            }
        }

        #endregion Equals / GetHashCode
    }
}