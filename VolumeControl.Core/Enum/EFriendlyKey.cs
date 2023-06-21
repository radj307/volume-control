namespace VolumeControl.Core.Enum
{
    /// <Summary>
    /// Specifies the possible key values on a keyboard. 
    /// </Summary>
    public enum EFriendlyKey
    {
        /// <Summary>
        /// <b>VK_LBUTTON</b>
        /// Left mouse button
        /// </Summary>
        None = 0,
        /// <Summary>
        /// <b>VK_CANCEL</b>
        /// Control-break processing
        /// </Summary>
        /// <remarks>
        /// This corresponds to CTRL+PAUSE
        /// </remarks>
        Cancel = 1,
        /// <Summary>
        /// <b>VK_BACK</b>
        /// BACKSPACE key
        /// </Summary>
        Backspace = 2,
        /// <Summary>
        /// <b>VK_TAB</b>
        /// TAB key
        /// </Summary>
        Tab = 3,
        /// <Summary>
        /// <b>VK_CLEAR</b>
        /// CLEAR key
        /// </Summary>
        Clear = 5,
        /// <Summary>
        /// <b>VK_RETURN</b>
        /// ENTER key.
        /// Applies to both the Enter key and the numpad Enter key.
        /// </Summary>
        Enter = 6,
        /// <Summary>
        /// <b>VK_PAUSE</b>
        /// PAUSE key
        /// </Summary>
        /// <remarks>
        /// Pressing CTRL+PAUSE will cause the CANCEL key to be received instead.
        /// </remarks>
        PauseBreak = 7,
        /// <Summary>
        /// <b>VK_CAPITAL</b>
        /// CAPS LOCK key
        /// </Summary>
        CapsLock = 8,
        /// <Summary>
        /// <b>VK_ESCAPE</b>
        /// ESC key
        /// </Summary>
        Escape = 13,
        /// <Summary>
        /// <b>VK_KANA</b>
        /// IME Kana mode
        /// </Summary>
        ImeKanaMode = 9,
        /// <Summary>
        /// <b>VK_JUNJA</b>
        /// IME Junja mode
        /// </Summary>
        ImeJunjaMode = 10,
        /// <Summary>
        /// <b>VK_FINAL</b>
        /// IME final mode
        /// </Summary>
        ImeFinalMode = 11,
        /// <Summary>
        /// <b>VK_HANJA</b>
        /// IME Hanja mode
        /// </Summary>
        ImeHanjaMode = 12,
        /// <Summary>
        /// <b>VK_CONVERT</b>
        /// IME convert
        /// </Summary>
        ImeConvert = 14,
        /// <Summary>
        /// <b>VK_NONCONVERT</b>
        /// IME nonconvert
        /// </Summary>
        ImeNonConvert = 15,
        /// <Summary>
        /// <b>VK_ACCEPT</b>
        /// IME accept
        /// </Summary>
        ImeAccept = 16,
        /// <Summary>
        /// <b>VK_MODECHANGE</b>
        /// IME mode change request
        /// </Summary>
        ImeModeChange = 17,
        /// <Summary>
        /// <b>VK_SPACE</b>
        /// SPACEBAR
        /// </Summary>
        Space = 18,
        /// <Summary>
        /// <b>VK_PRIOR</b>
        /// PAGE UP key
        /// </Summary>
        PageUp = 19,
        /// <Summary>
        /// <b>VK_NEXT</b>
        /// PAGE DOWN key
        /// </Summary>
        PageDown = 20,
        /// <Summary>
        /// <b>VK_END</b>
        /// END key
        /// </Summary>
        End = 21,
        /// <Summary>
        /// <b>VK_HOME</b>
        /// HOME key
        /// </Summary>
        Home = 22,
        /// <Summary>
        /// <b>VK_LEFT</b>
        /// LEFT ARROW key
        /// </Summary>
        LeftArrow = 23,
        /// <Summary>
        /// <b>VK_UP</b>
        /// UP ARROW key
        /// </Summary>
        UpArrow = 24,
        /// <Summary>
        /// <b>VK_RIGHT</b>
        /// RIGHT ARROW key
        /// </Summary>
        RightArrow = 25,
        /// <Summary>
        /// <b>VK_DOWN</b>
        /// DOWN ARROW key
        /// </Summary>
        DownArrow = 26,
        /// <Summary>
        /// <b>VK_SELECT</b>
        /// SELECT key
        /// </Summary>
        Select = 27,
        /// <Summary>
        /// <b>VK_PRINT</b>
        /// PRINT key
        /// </Summary>
        Print = 28,
        /// <Summary>
        /// <b>VK_EXECUTE</b>
        /// EXECUTE key
        /// </Summary>
        Execute = 29,
        /// <Summary>
        /// <b>VK_SNAPSHOT</b>
        /// PRINT SCREEN key
        /// </Summary>
        PrintScreen = 30,
        /// <Summary>
        /// <b>VK_INSERT</b>
        /// INS key
        /// </Summary>
        Insert = 31,
        /// <Summary>
        /// <b>VK_DELETE</b>
        /// DEL key
        /// </Summary>
        Delete = 32,
        /// <Summary>
        /// <b>VK_HELP</b>
        /// HELP key
        /// </Summary>
        Help = 33,
        /// <Summary>
        /// <b></b>
        /// 0 key
        /// </Summary>
        D0 = 34,
        /// <Summary>
        /// <b></b>
        /// 1 key
        /// </Summary>
        D1 = 35,
        /// <Summary>
        /// <b></b>
        /// 2 key
        /// </Summary>
        D2 = 36,
        /// <Summary>
        /// <b></b>
        /// 3 key
        /// </Summary>
        D3 = 37,
        /// <Summary>
        /// <b></b>
        /// 4 key
        /// </Summary>
        D4 = 38,
        /// <Summary>
        /// <b></b>
        /// 5 key
        /// </Summary>
        D5 = 39,
        /// <Summary>
        /// <b></b>
        /// 6 key
        /// </Summary>
        D6 = 40,
        /// <Summary>
        /// <b></b>
        /// 7 key
        /// </Summary>
        D7 = 41,
        /// <Summary>
        /// <b></b>
        /// 8 key
        /// </Summary>
        D8 = 42,
        /// <Summary>
        /// <b></b>
        /// 9 key
        /// </Summary>
        D9 = 43,
        /// <Summary>
        /// <b></b>
        /// A key
        /// </Summary>
        A = 44,
        /// <Summary>
        /// <b></b>
        /// B key
        /// </Summary>
        B = 45,
        /// <Summary>
        /// <b></b>
        /// C key
        /// </Summary>
        C = 46,
        /// <Summary>
        /// <b></b>
        /// D key
        /// </Summary>
        D = 47,
        /// <Summary>
        /// <b></b>
        /// E key
        /// </Summary>
        E = 48,
        /// <Summary>
        /// <b></b>
        /// F key
        /// </Summary>
        F = 49,
        /// <Summary>
        /// <b></b>
        /// G key
        /// </Summary>
        G = 50,
        /// <Summary>
        /// <b></b>
        /// H key
        /// </Summary>
        H = 51,
        /// <Summary>
        /// <b></b>
        /// I key
        /// </Summary>
        I = 52,
        /// <Summary>
        /// <b></b>
        /// J key
        /// </Summary>
        J = 53,
        /// <Summary>
        /// <b></b>
        /// K key
        /// </Summary>
        K = 54,
        /// <Summary>
        /// <b></b>
        /// L key
        /// </Summary>
        L = 55,
        /// <Summary>
        /// <b></b>
        /// M key
        /// </Summary>
        M = 56,
        /// <Summary>
        /// <b></b>
        /// N key
        /// </Summary>
        N = 57,
        /// <Summary>
        /// <b></b>
        /// O key
        /// </Summary>
        O = 58,
        /// <Summary>
        /// <b></b>
        /// P key
        /// </Summary>
        P = 59,
        /// <Summary>
        /// <b></b>
        /// Q key
        /// </Summary>
        Q = 60,
        /// <Summary>
        /// <b></b>
        /// R key
        /// </Summary>
        R = 61,
        /// <Summary>
        /// <b></b>
        /// S key
        /// </Summary>
        S = 62,
        /// <Summary>
        /// <b></b>
        /// T key
        /// </Summary>
        T = 63,
        /// <Summary>
        /// <b></b>
        /// U key
        /// </Summary>
        U = 64,
        /// <Summary>
        /// <b></b>
        /// V key
        /// </Summary>
        V = 65,
        /// <Summary>
        /// <b></b>
        /// W key
        /// </Summary>
        W = 66,
        /// <Summary>
        /// <b></b>
        /// X key
        /// </Summary>
        X = 67,
        /// <Summary>
        /// <b></b>
        /// Y key
        /// </Summary>
        Y = 68,
        /// <Summary>
        /// <b></b>
        /// Z key
        /// </Summary>
        Z = 69,
        /// <Summary>
        /// <b>VK_APPS</b>
        /// Applications key (Natural keyboard)
        /// </Summary>
        Apps = 72,
        /// <Summary>
        /// <b>VK_SLEEP</b>
        /// Computer Sleep key
        /// </Summary>
        Sleep = 73,
        /// <Summary>
        /// <b>VK_NUMPAD0</b>
        /// Numeric keypad 0 key
        /// </Summary>
        NumPad0 = 74,
        /// <Summary>
        /// <b>VK_NUMPAD1</b>
        /// Numeric keypad 1 key
        /// </Summary>
        NumPad1 = 75,
        /// <Summary>
        /// <b>VK_NUMPAD2</b>
        /// Numeric keypad 2 key
        /// </Summary>
        NumPad2 = 76,
        /// <Summary>
        /// <b>VK_NUMPAD3</b>
        /// Numeric keypad 3 key
        /// </Summary>
        NumPad3 = 77,
        /// <Summary>
        /// <b>VK_NUMPAD4</b>
        /// Numeric keypad 4 key
        /// </Summary>
        NumPad4 = 78,
        /// <Summary>
        /// <b>VK_NUMPAD5</b>
        /// Numeric keypad 5 key
        /// </Summary>
        NumPad5 = 79,
        /// <Summary>
        /// <b>VK_NUMPAD6</b>
        /// Numeric keypad 6 key
        /// </Summary>
        NumPad6 = 80,
        /// <Summary>
        /// <b>VK_NUMPAD7</b>
        /// Numeric keypad 7 key
        /// </Summary>
        NumPad7 = 81,
        /// <Summary>
        /// <b>VK_NUMPAD8</b>
        /// Numeric keypad 8 key
        /// </Summary>
        NumPad8 = 82,
        /// <Summary>
        /// <b>VK_NUMPAD9</b>
        /// Numeric keypad 9 key
        /// </Summary>
        NumPad9 = 83,
        /// <Summary>
        /// <b>VK_MULTIPLY</b>
        /// Multiply key
        /// </Summary>
        NumPadMultiply = 84,
        /// <Summary>
        /// <b>VK_ADD</b>
        /// Add key
        /// </Summary>
        NumPadAdd = 85,
        /// <Summary>
        /// <b>VK_SEPARATOR</b>
        /// Separator key
        /// </Summary>
        Separator = 86,
        /// <Summary>
        /// <b>VK_SUBTRACT</b>
        /// Subtract key
        /// </Summary>
        NumPadSubtract = 87,
        /// <Summary>
        /// <b>VK_DECIMAL</b>
        /// Decimal key
        /// </Summary>
        NumPadDecimal = 88,
        /// <Summary>
        /// <b>VK_DIVIDE</b>
        /// Divide key
        /// </Summary>
        NumPadDivide = 89,
        /// <Summary>
        /// <b>VK_F1</b>
        /// F1 key
        /// </Summary>
        F1 = 90,
        /// <Summary>
        /// <b>VK_F2</b>
        /// F2 key
        /// </Summary>
        F2 = 91,
        /// <Summary>
        /// <b>VK_F3</b>
        /// F3 key
        /// </Summary>
        F3 = 92,
        /// <Summary>
        /// <b>VK_F4</b>
        /// F4 key
        /// </Summary>
        F4 = 93,
        /// <Summary>
        /// <b>VK_F5</b>
        /// F5 key
        /// </Summary>
        F5 = 94,
        /// <Summary>
        /// <b>VK_F6</b>
        /// F6 key
        /// </Summary>
        F6 = 95,
        /// <Summary>
        /// <b>VK_F7</b>
        /// F7 key
        /// </Summary>
        F7 = 96,
        /// <Summary>
        /// <b>VK_F8</b>
        /// F8 key
        /// </Summary>
        F8 = 97,
        /// <Summary>
        /// <b>VK_F9</b>
        /// F9 key
        /// </Summary>
        F9 = 98,
        /// <Summary>
        /// <b>VK_F10</b>
        /// F10 key
        /// </Summary>
        F10 = 99,
        /// <Summary>
        /// <b>VK_F11</b>
        /// F11 key
        /// </Summary>
        F11 = 100,
        /// <Summary>
        /// <b>VK_F12</b>
        /// F12 key
        /// </Summary>
        F12 = 101,
        /// <Summary>
        /// <b>VK_F13</b>
        /// F13 key
        /// </Summary>
        F13 = 102,
        /// <Summary>
        /// <b>VK_F14</b>
        /// F14 key
        /// </Summary>
        F14 = 103,
        /// <Summary>
        /// <b>VK_F15</b>
        /// F15 key
        /// </Summary>
        F15 = 104,
        /// <Summary>
        /// <b>VK_F16</b>
        /// F16 key
        /// </Summary>
        F16 = 105,
        /// <Summary>
        /// <b>VK_F17</b>
        /// F17 key
        /// </Summary>
        F17 = 106,
        /// <Summary>
        /// <b>VK_F18</b>
        /// F18 key
        /// </Summary>
        F18 = 107,
        /// <Summary>
        /// <b>VK_F19</b>
        /// F19 key
        /// </Summary>
        F19 = 108,
        /// <Summary>
        /// <b>VK_F20</b>
        /// F20 key
        /// </Summary>
        F20 = 109,
        /// <Summary>
        /// <b>VK_F21</b>
        /// F21 key
        /// </Summary>
        F21 = 110,
        /// <Summary>
        /// <b>VK_F22</b>
        /// F22 key
        /// </Summary>
        F22 = 111,
        /// <Summary>
        /// <b>VK_F23</b>
        /// F23 key
        /// </Summary>
        F23 = 112,
        /// <Summary>
        /// <b>VK_F24</b>
        /// F24 key
        /// </Summary>
        F24 = 113,
        /// <Summary>
        /// <b>VK_NUMLOCK</b>
        /// NUM LOCK key
        /// </Summary>
        NumLock = 114,
        /// <Summary>
        /// <b>VK_SCROLL</b>
        /// SCROLL LOCK key
        /// </Summary>
        ScrollLock = 115,
        /// <Summary>
        /// <b>VK_BROWSER_BACK</b>
        /// Browser Back key
        /// </Summary>
        BrowserBack = 122,
        /// <Summary>
        /// <b>VK_BROWSER_FORWARD</b>
        /// Browser Forward key
        /// </Summary>
        BrowserForward = 123,
        /// <Summary>
        /// <b>VK_BROWSER_REFRESH</b>
        /// Browser Refresh key
        /// </Summary>
        BrowserRefresh = 124,
        /// <Summary>
        /// <b>VK_BROWSER_STOP</b>
        /// Browser Stop key
        /// </Summary>
        BrowserStop = 125,
        /// <Summary>
        /// <b>VK_BROWSER_SEARCH</b>
        /// Browser Search key
        /// </Summary>
        BrowserSearch = 126,
        /// <Summary>
        /// <b>VK_BROWSER_FAVORITES</b>
        /// Browser Favorites key
        /// </Summary>
        BrowserFavorites = 127,
        /// <Summary>
        /// <b>VK_BROWSER_HOME</b>
        /// Browser Start and Home key
        /// </Summary>
        BrowserHome = 128,
        /// <Summary>
        /// <b>VK_VOLUME_MUTE</b>
        /// Volume Mute key
        /// </Summary>
        VolumeMute = 129,
        /// <Summary>
        /// <b>VK_VOLUME_DOWN</b>
        /// Volume Down key
        /// </Summary>
        VolumeDown = 130,
        /// <Summary>
        /// <b>VK_VOLUME_UP</b>
        /// Volume Up key
        /// </Summary>
        VolumeUp = 131,
        /// <Summary>
        /// <b>VK_MEDIA_NEXT_TRACK</b>
        /// Next Track key
        /// </Summary>
        MediaNextTrack = 132,
        /// <Summary>
        /// <b>VK_MEDIA_PREV_TRACK</b>
        /// Previous Track key
        /// </Summary>
        MediaPreviousTrack = 133,
        /// <Summary>
        /// <b>VK_MEDIA_STOP</b>
        /// Stop Media key
        /// </Summary>
        MediaStop = 134,
        /// <Summary>
        /// <b>VK_MEDIA_PLAY_PAUSE</b>
        /// Play/Pause Media key
        /// </Summary>
        MediaPlayPause = 135,
        /// <Summary>
        /// <b>VK_LAUNCH_MAIL</b>
        /// Start Mail key
        /// </Summary>
        LaunchMail = 136,
        /// <Summary>
        /// <b>VK_LAUNCH_MEDIA_SELECT</b>
        /// Select Media key
        /// </Summary>
        SelectMedia = 137,
        /// <Summary>
        /// <b>VK_LAUNCH_APP1</b>
        /// Start Application 1 key
        /// </Summary>
        LaunchApplication1 = 138,
        /// <Summary>
        /// <b>VK_LAUNCH_APP2</b>
        /// Start Application 2 key
        /// </Summary>
        LaunchApplication2 = 139,
        /// <Summary>
        /// <b>VK_OEM_1</b>
        /// Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the ';:' key
        /// </Summary>
        Semicolon = 140,
        /// <Summary>
        /// <b>VK_OEM_PLUS</b>
        /// For any country/region, the '=+' key
        /// </Summary>
        Equals = 141,
        /// <Summary>
        /// <b>VK_OEM_COMMA</b>
        /// For any country/region, the ',' key
        /// </Summary>
        Comma = 142,
        /// <Summary>
        /// <b>VK_OEM_MINUS</b>
        /// For any country/region, the '-' key
        /// </Summary>
        Dash = 143,
        /// <Summary>
        /// <b>VK_OEM_MINUS</b>
        /// For any country/region, the '-' key
        /// </Summary>
        Minus = 143,
        /// <Summary>
        /// <b>VK_OEM_PERIOD</b>
        /// For any country/region, the '.' key
        /// </Summary>
        Period = 144,
        /// <Summary>
        /// <b>VK_OEM_2</b>
        /// Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '/?' key
        /// </Summary>
        ForwardSlash = 145,
        /// <Summary>
        /// <b>VK_OEM_3</b>
        /// Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '`~' key
        /// </Summary>
        Grave = 146,
        /// <Summary>
        /// <b>VK_OEM_4</b>
        /// Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '[{' key
        /// </Summary>
        OpenBracket = 149,
        /// <Summary>
        /// <b>VK_OEM_5</b>
        /// Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '\|' key
        /// </Summary>
        BackSlash = 150,
        /// <Summary>
        /// <b>VK_OEM_6</b>
        /// Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the ']}' key
        /// </Summary>
        CloseBracket = 151,
        /// <Summary>
        /// <b>VK_OEM_7</b>
        /// Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the 'single-quote/double-quote' key
        /// </Summary>
        Quote = 152,
        /// <Summary>
        /// <b>VK_OEM_8</b>
        /// Used for miscellaneous characters; it can vary by keyboard.
        /// </Summary>
        Oem8 = 153,
        /// <Summary>
        /// <b>VK_PROCESSKEY</b>
        /// IME PROCESS key
        /// </Summary>
        ImeProcess = 155,
        /// <Summary>
        /// <b>VK_ATTN</b>
        /// Attn key
        /// </Summary>
        Attn = 163,
        /// <Summary>
        /// <b>VK_CRSEL</b>
        /// CrSel key
        /// </Summary>
        CrSel = 164,
        /// <Summary>
        /// <b>VK_EXSEL</b>
        /// ExSel key
        /// </Summary>
        ExSel = 165,
        /// <Summary>
        /// <b>VK_EREOF</b>
        /// Erase EOF key
        /// </Summary>
        EraseEof = 166,
        /// <Summary>
        /// <b>VK_PLAY</b>
        /// Play key
        /// </Summary>
        Play = 167,
        /// <Summary>
        /// <b>VK_ZOOM</b>
        /// Zoom key
        /// </Summary>
        Zoom = 168,
        /// <Summary>
        /// <b>VK_PA1</b>
        /// PA1 key
        /// </Summary>
        Pa1 = 170,
        /// <Summary>
        /// <b>VK_OEM_CLEAR</b>
        /// Clear key
        /// </Summary>
        OemClear = 171,
    }
}
