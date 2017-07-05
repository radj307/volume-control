using System;
using System.Runtime.InteropServices;

namespace AutoHotkey.Interop
{
    /// <summary>
    /// These functions serve as a flat wrapper for AutoHotkey.dll.
    /// They assume AutoHotkey.dll is in the same directory as your
    /// executable.
    /// </summary>
    internal class AutoHotkeyAPI
    {
        private const string DLLPATH = "AutoHotkey.dll";

        #region Create Thread

        /// <summary>
        /// Start new thread from ahk file.
        /// </summary>
        /// <param name="path">This parameter must be a path to existing ahk file.</param>
        /// <param name="options">Additional parameter passed to AutoHotkey.dll (not available in Version 2 alpha).</param>
        /// <param name="parameters">Parameters passed to dll.</param>
        /// <returns>ahkdll returns a thread handle.</returns>
        /// <remarks>AhkTextDll is available in AutoHotkey[Mini].dll only, not in AutoHotkey.exe.</remarks>
        [DllImport(DLLPATH, EntryPoint = "ahkdll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint AhkDll(
            [MarshalAs(UnmanagedType.LPWStr)] string path,
            [MarshalAs(UnmanagedType.LPWStr)] string options,
            [MarshalAs(UnmanagedType.LPWStr)] string parameters);

        /// <summary>
        /// AhkTextDll is used to launch a script in a separate thread from text/variable.
        /// </summary>
        /// <param name="code">This parameter must be a string with ahk script.</param>
        /// <param name="options">Additional parameter passed to AutoHotkey.dll (not available in Version 2 alpha).</param>
        /// <param name="parameters">Parameters passed to dll.</param>
        /// <returns>ahkdll returns a thread handle.</returns>
        /// <remarks>AhkTextDll is available in AutoHotkey[Mini].dll only, not in AutoHotkey.exe.</remarks>
        [DllImport(DLLPATH, EntryPoint = "ahktextdll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint AhkTextDll(
            [MarshalAs(UnmanagedType.LPWStr)] string code,
            [MarshalAs(UnmanagedType.LPWStr)] string options,
            [MarshalAs(UnmanagedType.LPWStr)] string parameters);

        #endregion Create Thread

        #region Determine Thread's State

        /// <summary>
        /// AhkReady is used to check if a dll script is running or not.
        /// </summary>
        /// <returns>1 if a thread is running or 0 otherwise.</returns>
        /// <remarks>Available in AutoHotkey[Mini].dll only, not in AutoHotkey.exe.</remarks>
        [DllImport(DLLPATH, EntryPoint = "ahkReady", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool AhkReady();

        #endregion Determine Thread's State

        #region Control Thread

        /// <summary>
        /// AhkTerminate is used to stop and exit a running script.
        /// </summary>
        /// <param name="timeout">Time in milliseconds to wait until thread exits.</param>
        /// <returns>Returns always 0.</returns>
        /// <remarks>Available in AutoHotkey[Mini].dll only, not in AutoHotkey.exe.</remarks>
        [DllImport(DLLPATH, EntryPoint = "ahkTerminate", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern void AhkTerminate(uint timeout);

        /// <summary>
        /// AhkReload is used to terminate and start a running script again.
        /// </summary>
        [DllImport(DLLPATH, EntryPoint = "ahkReload", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern void AhkReload();

        /// <summary>
        /// AhkPause will pause/un-pause a thread and run traditional AutoHotkey Sleep internally.
        /// </summary>
        /// <param name="strState">Should be "On" or "Off" as a string</param>
        [DllImport(DLLPATH, EntryPoint = "ahkPause", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern void AhkPause(
            [MarshalAs(UnmanagedType.LPWStr)] string strState);

        #endregion Control Thread

        #region Add New Code

        /// <summary>
        /// addFile includes additional script from a file to the running script.
        /// </summary>
        /// <param name="filePath">Path to a file that will be added to a running script.</param>
        /// <param name="allowDuplicateInclude">Allow duplicate includes.</param>
        /// <param name="ignoreLoadFailure">Ignore if loading a file failed.</param>
        /// <returns>addFile returns a pointer to the first line of new created code.</returns>
        /// <remarks>pointerLine can be used in ahkExecuteLine to execute one line only or until a return is encountered.</remarks>
        [DllImport(DLLPATH, EntryPoint = "addFile", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint AddFile(
            [MarshalAs(UnmanagedType.LPWStr)] string filePath,
            byte allowDuplicateInclude,
            byte ignoreLoadFailure);

        // Constant values for the execute parameter of addScript
        public struct Execute
        {
            public const byte Add = 0, Run = 1, RunWait = 2;
        }

        /// <summary>
        /// addScript includes additional script from a string to the running script.
        /// </summary>
        /// <param name="code">cript that will be added to a running script.</param>
        /// <param name="execute">Determines whether the added script should be executed.</param>
        /// <returns>addScript returns a pointer to the first line of new created code.</returns>
        /// <remarks>pointerLine can be used in ahkExecuteLine to execute one line only or until a return is encountered.</remarks>
        [DllImport(DLLPATH, EntryPoint = "addScript", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint AddScript(
            [MarshalAs(UnmanagedType.LPWStr)] string code,
            byte execute);

        #endregion Add New Code

        #region Execute Some Code

        /// <summary>
        /// Execute a script from a string that contains ahk script.
        /// </summary>
        /// <param name="code">Script as string/text or variable containing script that will be executed.</param>
        /// <returns>Returns true if script was executed and false if there was an error.</returns>
        /// <remarks>ahkExec will execute the code and delete it before it returns.</remarks>
        [DllImport(DLLPATH, EntryPoint = "ahkExec", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool AhkExec(
            [MarshalAs(UnmanagedType.LPWStr)] string code);

        //TODO: ahkExecuteLine

        /// <summary>
        /// ahkLabel is used to launch a Goto/GoSub routine in script.
        /// </summary>
        /// <param name="labelName">Name of label to execute.</param>
        /// <param name="noWait">Do not to wait until execution finished. </param>
        /// <returns>	1 if label exists 0 otherwise.</returns>
        /// <remarks>Default is 0 = wait for code to finish execution.</remarks>
        [DllImport(DLLPATH, EntryPoint = "ahkLabel", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool AhkLabel(
            [MarshalAs(UnmanagedType.LPWStr)] string labelName,
            bool noWait);


        /// <summary>
        /// ahkFunction is used to launch a function in script.
        /// </summary>
        /// <param name="functionName">Name of function to call.</param>
        /// <param name="parameter1">The 1st parameter, or null</param>
        /// <param name="parameter2">The 2nd parameter, or null</param>
        /// <param name="parameter3">The 3rd parameter, or null</param>
        /// <param name="parameter4">The 4th parameter, or null</param>
        /// <param name="parameter5">The 5th parameter, or null</param>
        /// <param name="parameter6">The 6th parameter, or null</param>
        /// <param name="parameter7">The 7th parameter, or null</param>
        /// <param name="parameter8">The 8th parameter, or null</param>
        /// <param name="parameter9">The 9th parameter, or null</param>
        /// <param name="parameter10">The 10th parameter, or null</param>
        /// <returns>	Return value is always a string/text, add 0 to make sure it resolves to digit if necessary.</returns>
        [DllImport(DLLPATH, EntryPoint = "ahkFunction", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr AhkFunction(
            [MarshalAs(UnmanagedType.LPWStr)] string functionName,
            [MarshalAs(UnmanagedType.LPWStr)] string parameter1,
            [MarshalAs(UnmanagedType.LPWStr)] string parameter2,
            [MarshalAs(UnmanagedType.LPWStr)] string parameter3,
            [MarshalAs(UnmanagedType.LPWStr)] string parameter4,
            [MarshalAs(UnmanagedType.LPWStr)] string parameter5,
            [MarshalAs(UnmanagedType.LPWStr)] string parameter6,
            [MarshalAs(UnmanagedType.LPWStr)] string parameter7,
            [MarshalAs(UnmanagedType.LPWStr)] string parameter8,
            [MarshalAs(UnmanagedType.LPWStr)] string parameter9,
            [MarshalAs(UnmanagedType.LPWStr)] string parameter10);


        /// <summary>
        /// ahkFunction is used to launch a function in script.
        /// </summary>
        /// <param name="functionName">Name of function to call.</param>
        /// <param name="parameters">Parameters to pass to function.</param>
        /// <returns>0 if function exists else -1.</returns>
        [DllImport(DLLPATH, EntryPoint = "ahkPostFunction", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool AhkPostFunction(
            [MarshalAs(UnmanagedType.LPWStr)] string functionName,
            [MarshalAs(UnmanagedType.LPWStr)] string parameters);

        #endregion Execute Some Code

        #region Working With Variables

        /// <summary>
        /// AhkAssign is used to assign a string to a variable in script.
        /// </summary>
        /// <param name="variableName">Name of a variable.</param>
        /// <param name="newValue">Value to assign to variable.</param>
        /// <returns>Returns value is 0 on success and -1 on failure.</returns>
        /// <remarks>AhkAssign will create the variable if it does not exist.</remarks>
        [DllImport(DLLPATH, EntryPoint = "ahkassign", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool AhkAssign(
            [MarshalAs(UnmanagedType.LPWStr)] string variableName,
            [MarshalAs(UnmanagedType.LPWStr)] string newValue);

        /// <summary>
        /// AhkGetVar is used to get a value from a variable in script.
        /// </summary>
        /// <param name="variableName">Name of variable to get value from.</param>
        /// <param name="getPointer">Get value or pointer.</param>
        /// <returns>Returned value is always a string, add 0 to convert to integer if necessary, especially when using getPointer.</returns>
        /// <remarks>AhkGetVar returns empty string if variable does not exist or is empty.</remarks>
        [DllImport(DLLPATH, EntryPoint = "ahkgetvar", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr AhkGetVar(
            [MarshalAs(UnmanagedType.LPWStr)] string variableName,
            [MarshalAs(UnmanagedType.I4)] int getPointer);

        #endregion Working With Variables

        #region Advanced

        /// <summary>
        /// ahkFundFunc is used to get function its pointer
        /// </summary>
        /// <param name="funcName">Name of function to call.</param>
        /// <returns>Function pointer.</returns>
        [DllImport(DLLPATH, EntryPoint = "ahkFindFunc", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr AhkFindFunc(
            [MarshalAs(UnmanagedType.LPWStr)] string funcName);

        /// <summary>
        /// ahkFindLabel is used to get a pointer to the label.
        /// </summary>
        /// <param name="labelName">Name of label.</param>
        /// <returns>ahkFindLabel returns a pointer to a line where label points to.</returns>
        [DllImport(DLLPATH, EntryPoint = "ahkFindLabel", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr AhkFindLabel(
            [MarshalAs(UnmanagedType.LPWStr)] string labelName);

        /// <summary>
        /// Build in function to get a pointer to the structure of a user-defined variable.
        /// </summary>
        /// <param name="variable">the name of the variable</param>
        /// <returns>The pointer to the variable.</returns>
        [DllImport(DLLPATH, EntryPoint = "getVar", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetVar(
            [MarshalAs(UnmanagedType.LPWStr)] string variable);

        #endregion Advanced
    }
}