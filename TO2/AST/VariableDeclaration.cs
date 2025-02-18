﻿using System.Diagnostics.CodeAnalysis;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public interface IVariableContainer {
    IVariableContainer? ParentContainer { get; }

    TO2Type? FindVariableLocal(IBlockContext context, string name);
}

public static class VariableContainerExtensions {
    public static TO2Type? FindVariable(this IVariableContainer? current, IBlockContext context, string name) {
        while (current != null) {
            var variableType = current.FindVariableLocal(context, name);

            if (variableType != null) return variableType;
            current = current.ParentContainer;
        }

        return null;
    }
}

public class DeclarationParameter {
    public readonly string? source;
    public readonly string? target;
    public readonly TO2Type? type;

    public DeclarationParameter() {
        target = null;
        source = null;
        type = null;
    }

    public DeclarationParameter(string target, string source) {
        this.target = target;
        this.source = source;
        type = null;
    }

    public DeclarationParameter(string target, string source, TO2Type type) {
        this.target = target;
        this.source = source;
        this.type = type;
    }

    public bool IsPlaceholder => target == null;

    public bool IsInferred => type == null;
}

public class VariableDeclaration : Node, IBlockItem, IVariableRef {
    public readonly DeclarationParameter declaration;
    private readonly Expression expression;
    private readonly bool isConst;
    private bool lookingUp;

    public VariableDeclaration(DeclarationParameter declaration, bool isConst, Expression expression,
        Position start = new(), Position end = new()) : base(start, end) {
        this.declaration = declaration;
        this.isConst = isConst;
        this.expression = expression;
        this.expression.TypeHint = context => this.declaration.type?.UnderlyingType(context.ModuleContext);
    }

    public bool IsComment => false;

    public IVariableContainer VariableContainer {
        set => expression.VariableContainer = value;
    }

    public TypeHint? TypeHint {
        set { }
    }

    public TO2Type ResultType(IBlockContext context) {
        return BuiltinType.Unit;
    }

    public void Prepare(IBlockContext context) {
    }

    public void EmitCode(IBlockContext context, bool dropResult) {
        var valueType = expression.ResultType(context);
        var variableType = declaration.IsInferred ? valueType : declaration.type;

        if (context.HasErrors) return;

        if (context.FindVariable(declaration.target!) != null) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.DuplicateVariableName,
                $"Variable '{declaration.target}' already declared in this scope",
                Start,
                End
            ));
            return;
        }

        if (!variableType!.IsAssignableFrom(context.ModuleContext, valueType)) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.IncompatibleTypes,
                $"Variable '{declaration.target}' is of type {variableType} but is initialized with {valueType}",
                Start,
                End
            ));
            return;
        }

        var variable = context.DeclaredVariable(declaration.target!, isConst,
            variableType.UnderlyingType(context.ModuleContext));

        variable.Type.AssignFrom(context.ModuleContext, valueType).EmitAssign(context, variable, expression, true);
    }

    public override REPLValueFuture Eval(REPLContext context) {
        var expressionFuture = expression.Eval(context);
        var variableType = declaration.IsInferred ? expressionFuture.Type : declaration.type;

        if (context.FindVariable(declaration.target) != null)
            throw new REPLException(this, $"Variable '{declaration.target}' already declared in this scope");

        if (!variableType!.IsAssignableFrom(context.replModuleContext, expressionFuture.Type))
            throw new REPLException(this,
                $"Variable '{declaration.target}' is of type {variableType} but is initialized with {expressionFuture.Type}");

        var variable = context.DeclaredVariable(declaration.target!, isConst,
            variableType.UnderlyingType(context.replModuleContext));
        var assign = variable.declaredType.AssignFrom(context.replModuleContext, expressionFuture.Type);

        return expressionFuture.Then(variableType, value => {
            var converted = assign.EvalConvert(this, value);

            variable.value = converted;

            return converted;
        });
    }

    public string Name => declaration.target!;

    public TO2Type? VariableType(IBlockContext context) {
        if (lookingUp) return null;
        lookingUp = true; // Somewhat ugly workaround if there is a cycle in inferred variables that should produce a correct error message
        var type = declaration.IsInferred ? expression.ResultType(context) : declaration.type;
        lookingUp = false;
        return type;
    }
}
