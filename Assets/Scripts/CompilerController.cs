using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Builtins;
using Mellis.Core.Entities;
using Mellis.Core.Exceptions;
using Mellis.Core.Interfaces;
using Mellis.Lang.Python3;
using Mellis.Lang.Python3.Interfaces;
using Mellis.Lang.Python3.VM;
using PM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompilerController : MonoBehaviour
{
    private PyProcessor _processor;
    private IOpCode[] _opCodes;
    private string[] _coloredCodeLines;

    public TMP_Text inputText;
    public TMP_Text coloredInputText;
    public TMP_InputField inputField;
    public TMP_Text opCodeText;
    public Color currentOpCodeColor = Color.red;
    public Color nextOpCodeColor = new Color(1, 0.5f, 0.2f);
    public Color highlightedLineColor = Color.green;
    public Color errorLineColor = Color.red;
    public CanvasGroup opCodesCanvasGroup;
    public RectTransform variableWindow;
    public TMP_Text variableTemplate;
    [Space] public Button stopButton;
    public Button walkLineButton;
    public Button walkInstructionButton;
    public Button compileButton;

    private readonly IClrFunction[] _myBuiltins =
    {
        new Print(),
        new Bind(),
        new Builtins.Time(),
        new Builtins.Input(),
    };

    private void OnEnable()
    {
        UpdateIntractability();
    }

    private void Compile(string code)
    {
        Stopwatch watch = Stopwatch.StartNew();
        _coloredCodeLines = coloredInputText.text.Split('\n');
        var compiler = new PyCompiler();
        _processor = (PyProcessor) compiler.Compile(code);
        _opCodes = compiler.ToArray();
        watch.Stop();

        ConsoleLogger.Info($"Compiled source in {watch.ElapsedMilliseconds} ms.");

        _processor.AddBuiltin(_myBuiltins);

        UpdateIntractability();
    }

    public void UpdateVariableWindow()
    {
        if (_processor != null)
        {
            foreach (Transform child in variableWindow)
            {
                if (child != variableTemplate.transform)
                    Destroy(child.gameObject);
            }

            var first = true;
            foreach (KeyValuePair<string, IScriptType> pair in _processor.CurrentScope.Variables)
            {
                TMP_Text tmpText = Instantiate(variableTemplate, variableWindow);
                tmpText.gameObject.SetActive(true);
                tmpText.text = $"{pair.Key}\n{IDEColorCoding.runColorCode(pair.Value.ToString())}";
                tmpText.GetComponentInChildren<Image>().enabled = !first;
                if (first)
                {
                    first = false;
                }
            }
        }
    }

    public void UpdateLineHighlight(SourceReference source, Color color)
    {
        int fromLine = source.FromRow;
        int toLine = source.ToRow;

        var newLines = new string[_coloredCodeLines.Length];
        for (var i = 0; i < _coloredCodeLines.Length; i++)
        {
            string codeLine = _coloredCodeLines[i];
            if (i >= fromLine - 1 && i <= toLine - 1)
                newLines[i] = $"<mark=#{ColorUtility.ToHtmlStringRGBA(color)}>{codeLine}</mark>";
            else
                newLines[i] = codeLine;
        }

        coloredInputText.text = string.Join("\n", newLines);
    }

    public void HideLineHighlight()
    {
        if (_coloredCodeLines != null)
            coloredInputText.text = string.Join("\n", _coloredCodeLines);
    }

    public void UpdateOpCodeText()
    {
        if (_opCodes == null)
        {
            opCodeText.text = string.Empty;
            return;
        }

        var builder = new StringBuilder();

        for (var i = 0; i < _opCodes.Length; i++)
        {
            IOpCode opCode = _opCodes[i];

            string opCodeTrueStr = opCode.ToString();
            string opCodeStr = Regex.Replace(opCodeTrueStr, @"^([a-zA-Z]+)(.*)$",
                $@"<size=70%>{i}</size><indent=20><b>$1</b><color=#999><i>$2</i></color></indent>");

            int open = opCodeStr.IndexOf('{');
            if (open != -1)
            {
                int close = opCodeStr.LastIndexOf('}');
                int len = close - open;
                opCodeStr = opCodeStr.Substring(0, open + 1) +
                            IDEColorCoding.runColorCode(opCodeStr.Substring(open + 1, len - 1)) +
                            opCodeStr.Substring(close);
            }

            if (i == 0 && _processor.ProgramCounter == -1)
            {
                builder.AppendLine($"<color=#{ColorUtility.ToHtmlStringRGBA(nextOpCodeColor)}>{opCodeStr}</color>");
            }
            else if (i == _processor.ProgramCounter)
            {
                builder.AppendLine($"<color=#{ColorUtility.ToHtmlStringRGBA(currentOpCodeColor)}>{opCodeStr}</color>");
            }
            else
            {
                builder.AppendLine(opCodeStr);
            }
        }

        opCodeText.text = builder.ToString();
    }

    public void UpdateIntractability()
    {
        bool running = _processor?.State == ProcessState.Running ||
                       _processor?.State == ProcessState.Yielded ||
                       _processor?.State == ProcessState.NotStarted;
        bool yielded = _processor?.State == ProcessState.Yielded;

        inputField.interactable = !running;
        compileButton.interactable = !running;
        stopButton.interactable = running;
        walkLineButton.interactable = running && !yielded;
        walkInstructionButton.interactable = running && !yielded;
        opCodesCanvasGroup.alpha = running ? 1f : 0.5f;
        UpdateOpCodeText();

        UpdateVariableWindow();
        if (running)
        {
            UpdateLineHighlight(_processor.CurrentSource, highlightedLineColor);
        }
        else
            HideLineHighlight();
    }

    public void CompileInput()
    {
        try
        {
            Compile(inputText.text);
        }
        catch (SyntaxException e)
        {
            UpdateLineHighlight(e.SourceReference, errorLineColor);
            ConsoleLogger.Error("Exception thrown when invoking WalkLine.");
            ConsoleLogger.Exception(e);
        }
        catch (Exception e)
        {
            ConsoleLogger.Error("Exception thrown when compiling source.");
            ConsoleLogger.Exception(e);
        }
    }

    public void WalkLine()
    {
        if (_processor == null) return;

        try
        {
            _processor.WalkLine();
            UpdateIntractability();
        }
        catch (Exception e)
        {
            ConsoleLogger.Error("Exception thrown when invoking WalkLine.");
            ConsoleLogger.Exception(e);
        }
    }

    public void WalkInstruction()
    {
        if (_processor == null) return;

        try
        {
            _processor.WalkInstruction();
            UpdateIntractability();
        }
        catch (Exception e)
        {
            ConsoleLogger.Error("Exception thrown when invoking WalkInstruction.");
            ConsoleLogger.Exception(e);
        }
    }

    public void StopProcess()
    {
        _processor = null;
        UpdateIntractability();
    }

    public static void ResolveYield(string value)
    {
        var controller = FindObjectOfType<CompilerController>();
        PyProcessor pyProcessor = controller._processor;
        pyProcessor.ResolveYield(pyProcessor.Factory.Create(value));
        controller.UpdateIntractability();
    }
}