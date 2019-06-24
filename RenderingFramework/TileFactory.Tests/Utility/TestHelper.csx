using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;

var s = new StringBuilder();
var geo = new List<int[]>();

var assertXY = new Action<int, int[], StringBuilder>((index, items, text) =>
{
    text.AppendLine($"Assert.AreEqual({items[0]}, transformed.TransformedFeatures[{index}].X);");
    text.AppendLine($"Assert.AreEqual({items[1]}, transformed.TransformedFeatures[{index}].Y);");
});

// Given a string with this structure: "[[[997,-34],[634,-43],[997,-34]]]"
// parse it into a list of int[]
var processJSONtoList = new Func<string, string>((json) =>
{
    var result = json
    .Replace("[[[", "[")
    .Replace("]]]", "}}")
    .Replace(']', '}')
    .Replace('[', '{')
    .Replace("{", "new int[] {");

    result = $@"new List<int[]> {{{result}";
    return result;
});