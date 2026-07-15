using System.Collections.Generic;
using System.Text;

namespace BarcodeList.Tool
{
    /// <summary>
    /// AI要素のリストから、GS1-128としてエンコードすべき生データ文字列を組み立てる。
    /// 可変長AIが末尾以外に来る場合のみ、後ろにGS(区切り文字)を挿入する。
    /// </summary>
    public static class Gs1ValueBuilder
    {
        private const char GroupSeparator = (char)29;

        public static string Build(IReadOnlyList<Gs1Element> elements)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < elements.Count; i++)
            {
                var element = elements[i];
                sb.Append(element.Ai).Append(element.Value);

                var isLast = i == elements.Count - 1;
                if (!Gs1AiTable.IsFixedLength(element.Ai) && !isLast)
                {
                    sb.Append(GroupSeparator);
                }
            }

            return sb.ToString();
        }
    }
}
