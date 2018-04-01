!include FileFunc.nsh

;--------------------------------
; Function Macros

!macro _TrimQuotes Input Output
  Push `${Input}`
  Call TrimQuotes
  Pop ${Output}
!macroend
!define TrimQuotes `!insertmacro _TrimQuotes`

;--------------------------------
; Functions

; TrimQuotes
; Usage:
;   StrCpy $0 `"blah"`
;   ${TrimQuotes} $0 $0
Function TrimQuotes
  Exch $R0
  Push $R1
 
  StrCpy $R1 $R0 1
  StrCmp $R1 `"` 0 +2
    StrCpy $R0 $R0 `` 1
  StrCpy $R1 $R0 1 -1
  StrCmp $R1 `"` 0 +2
    StrCpy $R0 $R0 -1
 
  Pop $R1
  Exch $R0
FunctionEnd

;--------------------------------

Function UninstallPreviousVersions
  Push $R0
  Push $R1

  ReadRegStr $R0 HKLM "${RegUninstallKey}" "UninstallString"
  StrCmp $R0 "" done

  ${TrimQuotes} $R0 $R0
  ${GetParent} $R0 $R1

  ClearErrors
  ExecWait '$R0 _?=$R1'

  Pop $R1
  Pop $R0
done:
FunctionEnd
