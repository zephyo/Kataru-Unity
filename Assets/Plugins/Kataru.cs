using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Text;

public class Kataru : MonoBehaviour
{
    enum LineTag
    {
        Choices,
        InvalidChoice,
        Dialogue,
        Text,
        InputCmd,
        Cmds,
        None,
    }

    struct FFIStr
    {
        public IntPtr strptr;
        public UIntPtr length;

        public override string ToString()
        {
            var buffer = new byte[(int)length];
            Marshal.Copy(strptr, buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer);
        }

        public void ThrowIfError()
        {
            if (length != UIntPtr.Zero)
            {
                string errorMsg = ToString();
                throw new Exception(errorMsg);
            }
        }
    }

    [DllImport("kataru_ffi")]
    static extern FFIStr load_bookmark(byte[] path, UIntPtr length);
    public void LoadBookmark(string path)
    {
        var bytes = Encoding.UTF8.GetBytes(path);
        load_bookmark(bytes, (UIntPtr)bytes.Length).ThrowIfError();
    }

    [DllImport("kataru_ffi")]
    static extern FFIStr load_story(byte[] path, UIntPtr length);
    public static void LoadStory(string path)
    {
        var bytes = Encoding.UTF8.GetBytes(path);
        load_story(bytes, (UIntPtr)bytes.Length).ThrowIfError();
    }


    [DllImport("kataru_ffi")]
    static extern void init_runner();
    public static void InitRunner() =>
        init_runner();

    [DllImport("kataru_ffi")]
    static extern FFIStr get_passage();
    public static string GetPassage() => get_passage().ToString();

    [DllImport("kataru_ffi")]
    static extern FFIStr get_text();
    static string GetText() => get_text().ToString();

    [DllImport("kataru_ffi")]
    static extern FFIStr get_speaker();
    static string GetSpeaker() => get_speaker().ToString();

    [DllImport("kataru_ffi")]
    static extern FFIStr get_speech();
    static string GetSpeech() => get_speech().ToString();

    [DllImport("kataru_ffi")]
    static extern UIntPtr get_choices();
    static int GetChoices() => (int)get_choices();

    [DllImport("kataru_ffi")]
    static extern FFIStr get_choice(UIntPtr i);
    static string GetChoice(int i) => get_choice((UIntPtr)i).ToString();

    [DllImport("kataru_ffi")]
    static extern double get_timeout();
    static double GetTimeout() => get_timeout();

    [DllImport("kataru_ffi")]
    static extern UIntPtr get_commands();
    static int GetCommands() => (int)get_commands();

    [DllImport("kataru_ffi")]
    static extern FFIStr get_command(UIntPtr i);
    static string GetCommand(int i) => get_command((UIntPtr)i).ToString();

    [DllImport("kataru_ffi")]
    static extern int get_params(UIntPtr i);
    static int GetParams(int i) => (int)get_params((UIntPtr)i);

    [DllImport("kataru_ffi")]
    static extern FFIStr get_param(UIntPtr i);
    static string GetParam(int i) => get_param((UIntPtr)i).ToString();

    [DllImport("kataru_ffi")]
    static extern int get_value(UIntPtr i);
    static string GetValue(int i) => get_value((UIntPtr)i).ToString();

    [DllImport("kataru_ffi")]
    static extern LineTag next(byte[] input, UIntPtr length);
    public void Next(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        LineTag tag = next(bytes, (UIntPtr)bytes.Length);
        Debug.Log(String.Format("Tag: {0}", tag));
        switch (tag)
        {
            case LineTag.Text:
                HandleText();
                break;
            case LineTag.Dialogue:
                HandleDialogue();
                break;
            case LineTag.Cmds:
                HandleCommands();
                break;
            case LineTag.Choices:
                HandleChoices();
                break;
            case LineTag.InputCmd:
                // HandleChoices();
                break;
            default:
                break;
        }
    }

    void HandleText() => OnText(GetText());

    void HandleDialogue() => OnDialogue(GetSpeaker(), GetSpeech());

    void HandleChoices()
    {
        var choices = new HashSet<string>();
        int numChoices = GetChoices();
        for (int i = 0; i < numChoices; ++i) choices.Add(GetChoice(i));
        double timeout = GetTimeout();
        OnChoices(choices, timeout);
    }

    void HandleCommands()
    {
        var commands = new List<string>();
        int numCommands = GetCommands();
        for (int i = 0; i < numCommands; ++i)
        {
            var parameters = new Dictionary<string, object>();
            int numParameters = GetParams(i);
            for (int j = 0; j < numParameters; ++j)
            {
                parameters[GetParam(j)] = GetValue(j);
            }
            OnCommand(GetCommand(i), parameters);
        }
    }

    void HandleInputCommand()
    {
        string input = OnInputCommand();
        Next(input);
    }

    // Interface to be overriden.
    protected virtual void OnText(string text) { }
    protected virtual void OnDialogue(string speaker, string speech) { }
    protected virtual void OnCommand(string command, IDictionary<string, object> parameters) { }
    protected virtual void OnChoices(ISet<string> choices, double timeout) { }
    protected virtual string OnInputCommand() => "Input command unimplemented";
}
