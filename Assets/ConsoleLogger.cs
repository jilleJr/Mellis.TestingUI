using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mellis.Core.Exceptions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleLogger : MonoBehaviour
{
    private static ConsoleLogger _instance;

    public Color infoColor = Color.yellow;
    public Color warningColor = new Color(1, 0.5f, 0.2f);
    public Color errorColor = Color.red;

    [Space]
    public Toggle autoScrollToggle;
    public TMP_Text logText;
    public ScrollRect logScroll;

    private readonly StringBuilder _builder = new StringBuilder();

    private void OnEnable()
    {
        _instance = this;
    }

    private void UpdateLogText()
    {
        logText.text = _builder.ToString();

        if (autoScrollToggle.isOn &&
            (logScroll.verticalNormalizedPosition < 0.01f ||
            !logScroll.verticalScrollbar.gameObject.activeSelf))
        {
            Canvas.ForceUpdateCanvases();
            logScroll.verticalNormalizedPosition = 0;
        }
    }

    public void ClearConsole()
    {
        Clear();
    }

    public static void Clear()
    {
        _instance._builder.Clear();
        _instance.UpdateLogText();
    }

    public static void Info(string text)
    {
        LogInternal(_instance.infoColor, "INFO", text);
    }

    public static void Warning(string text)
    {
        LogInternal(_instance.warningColor, "WARNING", text);
    }

    public static void Error(string text)
    {
        LogInternal(_instance.errorColor, "ERROR", text);
    }

    public static void Exception(Exception ex)
    {
        string[] lines;

        switch (ex)
        {
            case SyntaxException syntax:
                lines = new[] {
                    $"Message: <b>{ex.Message}</b>",
                    $"Localized key: <b>{syntax.LocalizeKey}</b>",
                    $"Source reference: <b>{syntax.SourceReference}</b>",
                };
                break;
            case InterpreterLocalizedException local:
                lines = new[] {
                    $"Message: <b>{ex.Message}</b>",
                    $"Localized key: <b>{local.LocalizeKey}</b>",
                };
                break;
            default:
                lines = new[] {
                    $"Message: <b>{ex.Message}</b>"
                };
                break;
        }

        if (ex is InterpreterLocalizedException local2)
        {
            lines = lines.Append(
                $"Format arguments: ({local2.FormatArgs.Length})"
            ).Concat(local2.FormatArgs
                .Select((o, i) => $"- [{i}]: <b>{o}</b>"))
                .ToArray();
        }

        LogInternal(_instance.errorColor, "EXCEPTION",
            string.Join("\n", lines.Concat(new[]
            {
                $"Type: <b>{ex.GetType().FullName}</b>",
                "<b>===(( Stacktrace ))===</b>",
                ex.StackTrace
            })));
    }

    private static void LogInternal(Color color, string type, string text)
    {
        _instance._builder.AppendLine(string.Format(
            "<color=#{0}>[{1}]</color> {2}",
            ColorUtility.ToHtmlStringRGB(color),
            type,
            text
        ));
        _instance.UpdateLogText();
    }
}
