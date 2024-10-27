#Requires AutoHotkey v2.0
SetCapsLockState "AlwaysOff"
CapsLock & j::Left
CapsLock & k::Down
CapsLock & i::Up
CapsLock & l::Right
CapsLock & u::Home
CapsLock & o::End

CapsLock & h::{
    If GetKeyState("Shift", "P") {
        Send "+^{Left}"
    } else {
        Send "^{Left}"
    }
}

CapsLock & `;::{
    If GetKeyState("Shift", "P") {
        Send "+^{Right}"
    } else {
        Send "^{Right}"
    }
}