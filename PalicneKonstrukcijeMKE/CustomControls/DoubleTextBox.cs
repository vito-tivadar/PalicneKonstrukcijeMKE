using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PalicneKonstrukcijeMKE.CustomControls;

public class DoubleTextBox : TextBox
{
    public static string DecimalSeparator { get; set; } = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
    private static string PositiveSign = System.Globalization.NumberFormatInfo.CurrentInfo.PositiveSign;
    private static string NegativeSign = System.Globalization.NumberFormatInfo.CurrentInfo.NegativeSign;

    private static List<string> _valuesThatReturnZero = new List<string>
    {
        String.Empty,
        DecimalSeparator,
        NegativeSign,
        NegativeSign + DecimalSeparator,
        PositiveSign,
        PositiveSign + DecimalSeparator,
        "0" + DecimalSeparator
    };

    #region Dependency Properties
    public double DoubleValue
    {
        get { return (double)GetValue(DoubleValueProperty); }
        set { SetValue(DoubleValueProperty, value); }
    }
    public static readonly DependencyProperty DoubleValueProperty =
        DependencyProperty.Register("DoubleValue", typeof(double), typeof(DoubleTextBox), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDoubleValueChanged, CoerceDoubleValue, isAnimationProhibited: true, UpdateSourceTrigger.PropertyChanged));

    public bool DoubleValueAsTooltip
    {
        get { return (bool)GetValue(DoubleValueAsTooltipProperty); }
        set { SetValue(DoubleValueAsTooltipProperty, value); }
    }

    public static readonly DependencyProperty DoubleValueAsTooltipProperty =
        DependencyProperty.Register("DoubleValueAsTooltip", typeof(bool), typeof(DoubleTextBox), new PropertyMetadata(false));

    #endregion //DependencyProperties

    public DoubleTextBox()
    {
        Text = DoubleValue.ToString();
    }

    static DoubleTextBox() { }

    protected override void OnTextChanged(TextChangedEventArgs e)
    {
        if (e.Changes != null)
        {
            string text = this.Text;

            // Set decimal value to 0 if text contains only single sign or is empty
            if (_valuesThatReturnZero.Contains(text))
            {
                DoubleValue = 0;
                base.OnTextChanged(e);
                return;
            }

            bool textEdited = false;
            int CaretPosition = this.CaretIndex;

            // Everything except numbers, decimal separator, negative and positive sign
            MatchCollection matches1 = new Regex($"[^\\d\\{DecimalSeparator}+-]+").Matches(text);
            if (matches1.Count > 0)
            {
                int calculatedCaretPosition = 0;
                foreach (Match m in matches1)
                {
                    text = text.Remove(m.Index - calculatedCaretPosition, m.Length);
                    calculatedCaretPosition += m.Length;
                }
                CaretPosition -= calculatedCaretPosition;
                textEdited = true;
            }

            // If there is negative or positive sign after digit or decimal separator
            MatchCollection matches2 = new Regex($"(\\d[{PositiveSign}\\{NegativeSign}]+)|(\\{DecimalSeparator}[{PositiveSign}\\{NegativeSign}]+)").Matches(text);
            if (matches2.Count > 0)
            {
                int calculatedCaretPosition = 0;
                foreach (Match m in matches2)
                {
                    text = text.Remove(m.Index + 1 - calculatedCaretPosition, m.Length - 1);
                    calculatedCaretPosition += m.Length - 1;
                }
                CaretPosition -= calculatedCaretPosition;
                textEdited = true;
            }

            // If there are multiple decimal separators or positive or negative signs remove all except first of each one
            MatchCollection matches3 = new Regex($"[\\{PositiveSign}\\{NegativeSign}\\{DecimalSeparator}]").Matches(text);
            if (matches3.Count > 1)
            {
                bool decimalSeparatorSkipped = false, positiveSignSkipped = false, negativeSignSkipped = false;
                int calculatedCaretPosition = 0;

                for (int m = 0; m < matches3.Count; m++)
                {
                    Match currentMatch = matches3[m];
                    if (decimalSeparatorSkipped == false && currentMatch.Value == DecimalSeparator)
                    {
                        decimalSeparatorSkipped = true;
                        continue;
                    }

                    if (positiveSignSkipped == false && currentMatch.Value == PositiveSign)
                    {
                        positiveSignSkipped = true;
                        continue;
                    }

                    if (negativeSignSkipped == false && currentMatch.Value == NegativeSign)
                    {
                        negativeSignSkipped = true;
                        continue;
                    }

                    text = text.Remove(currentMatch.Index - calculatedCaretPosition, currentMatch.Length);
                    calculatedCaretPosition += currentMatch.Length;
                    textEdited = true;
                }
                CaretPosition -= calculatedCaretPosition;
            }

            // Add 0 between positive or negative sign and decimal separator
            MatchCollection matches4 = new Regex($"(\\{NegativeSign}\\{DecimalSeparator})|(\\{PositiveSign}\\{DecimalSeparator})").Matches(text);
            if (matches4.Count > 0)
            {
                int calculatedCaretPosition = 0;
                foreach (Match m in matches4)
                {
                    text = text.Insert(m.Index + 1, "0");
                    calculatedCaretPosition += 1;
                }
                CaretPosition += calculatedCaretPosition;
                textEdited = true;
            }

            // remove leading 0 if it's not 0 and decimal separator
            if (text.StartsWith("0") && !text.StartsWith($"0{DecimalSeparator}") && text.Length > 1)
            {
                text = text.Remove(0, 1);
                CaretPosition += 1;
                textEdited = true;
            }

            // add 0 if text starts with decimal separator
            if (text.StartsWith(DecimalSeparator))
            {
                text = "0" + text;
                CaretPosition += 1;
                textEdited = true;
            }

            if (textEdited)
            {
                this.Text = text;
                this.CaretIndex = CaretPosition;
            }
            else
            {
                double newDoubleValue = Convert.ToDouble(this.Text);
                if (newDoubleValue != DoubleValue)
                {
                    DoubleValue = newDoubleValue;
                    if (DoubleValueAsTooltip) this.ToolTip = DoubleValue;
                    base.OnTextChanged(e);
                }
            }

        }
    }

    private static void OnDoubleValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        DoubleTextBox dtb = (DoubleTextBox)d;
        bool test = !_valuesThatReturnZero.Contains(dtb.Text);
        if (dtb.Text != e.NewValue.ToString())
            if (!_valuesThatReturnZero.Contains(dtb.Text))
                dtb.Text = e.NewValue.ToString();
    }

    private static object CoerceDoubleValue(DependencyObject d, object baseValue)
    {
        return baseValue;
    }
}
