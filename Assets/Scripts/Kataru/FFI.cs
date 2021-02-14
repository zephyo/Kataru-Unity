using System.Text;
using System.Runtime.InteropServices;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace Kataru
{
    public enum LineTag
    {
        Choices,
        InvalidChoice,
        Dialogue,
        InputCommand,
        Commands,
        None,
    }

    public class FFI
    {
        [DllImport("kataru_ffi")]
        static extern FFIStr load_bookmark(byte[] path, UIntPtr length);
        public static void LoadBookmark(string path)
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
        public static LineTag Next(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            return next(bytes, (UIntPtr)bytes.Length);
        }

        public static Dialogue LoadDialogue() => new Dialogue() { name = GetSpeaker(), text = GetSpeech() };

        public static Choices LoadChoices()
        {
            var choices = new List<string>();
            int numChoices = GetChoices();
            for (int i = 0; i < numChoices; ++i) choices.Add(GetChoice(i));
            return new Choices() { choices = choices, timeout = GetTimeout() };
        }

        public static IEnumerable<Command> LoadCommands()
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
                yield return new Command() { name = GetCommand(i), parameters = parameters };
            }
        }

        public static InputCommand LoadInputCommand()
        {
            return new InputCommand() { prompt = "Not implemented" };
        }
    }
}