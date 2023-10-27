using Godot;
using System.Collections.Generic;

public class Portraits : Resource
{
    [Export]
    Dictionary<string, Dictionary<string, Texture>> portraits;
}
