﻿using System;
using Radical.Instant;
using Radical.Uniques;
using FluentValidation.Results;
using MediatR;
using Newtonsoft.Json;

namespace Radical.Servitizing.Entity
{
    [Serializable]
    public enum MathOperand
    {
        None,
        Equal,
        GreaterOrEqual,
        LessOrEqual,
        Greater,
        Less,
        Like,
        NotLike,
        Contains
    }

    [Serializable]
    public enum LogicOperand
    {
        And,
        Or
    }

    public static class EntityFilterOperand
    {
        public static MathOperand ParseMathOperand(string operandString)
        {
            MathOperand _operand = MathOperand.None;
            switch (operandString)
            {
                case "=":
                    _operand = MathOperand.Equal;
                    break;
                case ">=":
                    _operand = MathOperand.GreaterOrEqual;
                    break;
                case ">":
                    _operand = MathOperand.Greater;
                    break;
                case "<=":
                    _operand = MathOperand.LessOrEqual;
                    break;
                case "<":
                    _operand = MathOperand.Less;
                    break;
                case "%":
                    _operand = MathOperand.Like;
                    break;
                case "!%":
                    _operand = MathOperand.NotLike;
                    break;
                default:
                    _operand = MathOperand.None;
                    break;
            }
            return _operand;
        }

        public static string ConvertMathOperand(MathOperand operand)
        {
            string operandString = "";
            switch (operand)
            {
                case MathOperand.Equal:
                    operandString = "=";
                    break;
                case MathOperand.GreaterOrEqual:
                    operandString = ">=";
                    break;
                case MathOperand.Greater:
                    operandString = ">";
                    break;
                case MathOperand.LessOrEqual:
                    operandString = "<=";
                    break;
                case MathOperand.Less:
                    operandString = "<";
                    break;
                case MathOperand.Like:
                    operandString = "%";
                    break;
                case MathOperand.NotLike:
                    operandString = "!%";
                    break;
                default:
                    operandString = "=";
                    break;
            }
            return operandString;
        }
    }
}
