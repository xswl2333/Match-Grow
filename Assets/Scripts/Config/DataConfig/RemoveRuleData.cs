using MessagePack;
using System.Collections.Generic;

[MessagePackObject]
public class RemoveRuleData
{
    [Key(0)]
    public Dictionary<RemoveBlockRuleEnum, BlockType> RuleBlockTypeMap { get; set; }
}