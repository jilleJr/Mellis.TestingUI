using System;
using System.Linq;
using System.Text;
using Mellis.Core.Entities;
using Mellis.Lang.Python3;
using Mellis.Lang.Python3.Interfaces;
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
    public CanvasGroup opCodesCanvasGroup;
    [Space] public Button stopButton;
    public Button walkLineButton;
    public Button walkInstructionButton;
    public Button compileButton;

    private void OnEnable()
    {
        UpdateIntractability();
    }

    private void Compile(string code)
    {
        var compiler = new PyCompiler();
        _processor = (PyProcessor) compiler.Compile(code);
        _opCodes = compiler.ToArray();
        _coloredCodeLines = coloredInputText.text.Split('\n');

        UpdateIntractability();
    }

    public void UpdateLineHighlight(SourceReference source)
    {

        int fromLine = source.FromRow;
        int toLine = source.ToRow;
        
        var newLines = new string[_coloredCodeLines.Length];
        for (var i = 0; i < _coloredCodeLines.Length; i++)
        {
            string codeLine = _coloredCodeLines[i];
            if (i >= fromLine - 1 && i <= toLine - 1)
                newLines[i] = $"<mark=#{ColorUtility.ToHtmlStringRGBA(highlightedLineColor)}>{codeLine}</mark>";
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

            if (i == 0 && _processor.ProgramCounter == -1)
            {
                builder.AppendLine($"<color=#{ColorUtility.ToHtmlStringRGBA(nextOpCodeColor)}>{opCode}</color>");
            }
            else if (i == _processor.ProgramCounter)
            {
                builder.AppendLine($"<color=#{ColorUtility.ToHtmlStringRGBA(currentOpCodeColor)}>{opCode}</color>");
            }
            else
            {
                builder.AppendLine(opCode.ToString());
            }
        }

        opCodeText.text = builder.ToString();
    }

    public void UpdateIntractability()
    {
        bool running = _processor?.State == ProcessState.Running ||
                       _processor?.State == ProcessState.NotStarted;
        inputField.interactable = !running;
        compileButton.interactable = !running;
        stopButton.interactable = running;
        walkLineButton.interactable = running;
        walkInstructionButton.interactable = running;
        opCodesCanvasGroup.alpha = running ? 1f : 0.5f;

        if (running)
        {
            UpdateLineHighlight(_processor.CurrentSource);
            UpdateOpCodeText();
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
}