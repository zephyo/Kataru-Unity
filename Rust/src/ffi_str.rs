use std::ffi::CStr;
use std::fmt;
use std::os::raw::c_char;

#[repr(C)]
#[derive(Clone, Copy)]
pub struct FFIStr {
    strptr: *const u8,
    length: usize,
}

impl FFIStr {
    pub fn from(string: &str) -> Self {
        Self {
            strptr: string.as_ptr(),
            length: string.len(),
        }
    }

    pub fn to_str(c_chars: *const c_char) -> &'static str {
        unsafe { CStr::from_ptr(c_chars).to_str().unwrap() }
    }

    pub fn result<T, E: fmt::Debug>(result: Result<T, E>) -> Self {
        match result {
            Ok(_) => Self::from(""),
            Err(e) => Self::from(&format!("{:?}", e)),
        }
    }
}
