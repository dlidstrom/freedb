namespace Test
{
    using System.Collections.Generic;
    using System.IO;

    public class FS
    {
        public static IEnumerable<string> FindFiles(string dir, string match)
        {
            var stack = new Stack<DirectoryInfo>();
            stack.Push(new DirectoryInfo(dir));

            while (stack.Count > 0)
            {
                var curDir = stack.Pop();

                foreach (var fi in curDir.GetFiles(match))
                {
                    yield return fi.FullName;
                }

                foreach (var di in curDir.GetDirectories())
                {
                    stack.Push(di);
                }
            }
        }
    }
}
