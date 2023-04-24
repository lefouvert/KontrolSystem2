import { Expression, Node } from ".";
import { BUILTIN_UNIT, TO2Type } from "./to2-type";
import { InputPosition } from "../../parser";

export class Call extends Expression {
  constructor(
    public readonly namePath: string[],
    public readonly args: Expression[],
    start: InputPosition,
    end: InputPosition
  ) {
    super(start, end);
  }

  public resultType(): TO2Type {
    return BUILTIN_UNIT;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.args.reduce(
      (prev, arg) => arg.reduceNode(combine, prev),
      combine(initialValue, this)
    );
  }
}
