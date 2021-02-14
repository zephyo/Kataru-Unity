mod ffi_str;
use ffi_str::FFIStr;
use kataru::*;
use std::os::raw::c_char;

static mut STORY: Option<Story> = None;
static mut BOOKMARK: Option<Bookmark> = None;
static mut RUNNER: Option<Runner> = None;
static mut LINE: Option<Line> = None;

// Return a pointer to vec of
static mut CHOICES: Vec<FFIStr> = Vec::new();
static mut CMDS: Vec<FFIStr> = Vec::new();
static mut PARAMS: Vec<FFIStr> = Vec::new();
static mut VALUES: Vec<FFIStr> = Vec::new();

fn try_load_bookmark(path: &str) -> Result<(), ParseError> {
    unsafe {
        BOOKMARK = Some(Bookmark::load(path)?);
        Ok(())
    }
}
#[no_mangle]
pub extern "C" fn load_bookmark(path: *const c_char, length: usize) -> FFIStr {
    let path = FFIStr::to_str(path, length);
    FFIStr::result(try_load_bookmark(path))
}

fn try_load_story(path: &str) -> Result<(), ParseError> {
    unsafe {
        STORY = Some(Story::load(path)?);
        Ok(())
    }
}
#[no_mangle]
pub extern "C" fn load_story(path: *const c_char, length: usize) -> FFIStr {
    let path = FFIStr::to_str(path, length);
    FFIStr::result(try_load_story(path))
}

fn try_init_runner() -> Result<(), ParseError> {
    unsafe {
        if let Some(bookmark) = BOOKMARK.as_mut() {
            if let Some(story) = &STORY.as_ref() {
                RUNNER = Some(Runner::new(bookmark, story));
                return Ok(());
            } else {
                return Err(perror!("Story was None."));
            }
        } else {
            return Err(perror!("Bookmark was None."));
        }
    }
}
#[no_mangle]
pub extern "C" fn init_runner() -> FFIStr {
    FFIStr::result(try_init_runner())
}

#[no_mangle]
pub extern "C" fn next(input: *const c_char, length: usize) -> LineTag {
    let input = FFIStr::to_str(input, length);
    unsafe {
        LINE = Some(RUNNER.as_mut().unwrap().next(input));
        LineTag::tag(&LINE)
    }
}

#[no_mangle]
pub extern "C" fn get_text() -> FFIStr {
    unsafe {
        match &LINE {
            Some(Line::Text(text)) => FFIStr::from(&text),
            _ => FFIStr::from(""),
        }
    }
}

#[no_mangle]
pub extern "C" fn get_speaker() -> FFIStr {
    unsafe {
        match &LINE {
            Some(Line::Dialogue(dialogue)) => {
                for (speaker, _speech) in dialogue {
                    return FFIStr::from(speaker);
                }
                FFIStr::from("")
            }
            _ => FFIStr::from(""),
        }
    }
}

#[no_mangle]
pub extern "C" fn get_speech() -> FFIStr {
    unsafe {
        match &LINE {
            Some(Line::Dialogue(dialogue)) => {
                for (_speaker, speech) in dialogue {
                    return FFIStr::from(speech);
                }
                FFIStr::from("")
            }
            _ => FFIStr::from(""),
        }
    }
}

#[no_mangle]
pub extern "C" fn get_choices() -> usize {
    unsafe {
        match &LINE {
            Some(Line::Choices(choices)) => {
                CHOICES = Vec::new();
                CHOICES.reserve(choices.choices.len());
                for (choice, _target) in &choices.choices {
                    CHOICES.push(FFIStr::from(choice));
                }
                choices.choices.len()
            }
            _ => 0,
        }
    }
}

#[no_mangle]
pub extern "C" fn get_timeout() -> f64 {
    unsafe {
        match &LINE {
            Some(Line::Choices(choices)) => choices.timeout,
            _ => 0.0,
        }
    }
}

#[no_mangle]
pub extern "C" fn get_choice(i: usize) -> FFIStr {
    unsafe { CHOICES[i] }
}

#[no_mangle]
pub extern "C" fn get_commands() -> usize {
    unsafe {
        match &LINE {
            Some(Line::Cmds(cmds)) => {
                CMDS = Vec::new();
                CMDS.reserve(cmds.len());
                for cmd in cmds {
                    for (cmd_name, _params) in cmd {
                        CMDS.push(FFIStr::from(cmd_name));
                    }
                }
                cmds.len()
            }
            _ => 0,
        }
    }
}

#[no_mangle]
pub extern "C" fn get_command(i: usize) -> FFIStr {
    unsafe { CMDS[i] }
}

#[no_mangle]
pub extern "C" fn get_params(i: usize) -> usize {
    unsafe {
        match &LINE {
            Some(Line::Cmds(cmds)) => {
                let cmd = &cmds[i];
                for (_cmd_name, params) in cmd {
                    PARAMS = Vec::new();
                    PARAMS.reserve(params.len());
                    VALUES = Vec::new();
                    VALUES.reserve(params.len());
                    for (param, value) in params {
                        PARAMS.push(FFIStr::from(param));
                        VALUES.push(FFIStr::from(&value.to_string()));
                    }
                    return params.len();
                }
                0
            }
            _ => 0,
        }
    }
}

#[no_mangle]
pub extern "C" fn get_param(i: usize) -> FFIStr {
    unsafe { PARAMS[i] }
}

#[no_mangle]
pub extern "C" fn get_value(i: usize) -> FFIStr {
    unsafe { VALUES[i] }
}

#[no_mangle]
pub extern "C" fn get_passage() -> FFIStr {
    unsafe {
        match &BOOKMARK {
            Some(bookmark) => FFIStr::from(&bookmark.passage),
            _ => FFIStr::from(""),
        }
    }
}
