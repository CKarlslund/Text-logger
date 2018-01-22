using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConsoleApp
{
    partial class Program
    {
        public class CKReader
        {
            private string _filePath;
            private int _rowsToRead;

            public CKReader(string filePath, int rows)
            {
                _rowsToRead = rows;
                _filePath = filePath;
            }

            /// <summary>
            /// Returnerar sista rows räknat från offset
            /// </summary>
            /// <param name="offset"></param>
            /// <returns></returns>
            public LogData ReadLines(int offset)
            {
                List<string> lines = new List<string>();
                int offsetToReturn = 0;

                if (File.Exists(_filePath))
                {
                    var startPosition = FindLineNumber(_rowsToRead, offset);

                    using (var fs = File.OpenRead(_filePath))
                    {
                        offsetToReturn = (int)fs.Seek(0, SeekOrigin.End);

                        fs.Seek(startPosition, SeekOrigin.Begin);

                        using (var sr = new StreamReader(fs, Encoding.UTF8))
                        {
                            string line;

                            while ((line = sr.ReadLine()) != null)
                            {
                                lines.Add(line);
                            }                     
                        }
                    }
                }
                return new LogData(){
                    Offset = offsetToReturn,
                    Rows = lines.ToArray()
                };
            }

            public int FindLineNumber(int line, int offset)
            {
                int linePosition;

                using (var fs = File.OpenRead(_filePath))
                {
                    var fileEnd = (int)fs.Seek(0, SeekOrigin.End);
                    linePosition = fileEnd;

                    int linesGoneThrough = 0;

                    while (linesGoneThrough < line && linePosition > offset)
                    {
                        var lineOffset = FindBeginningOfLine(linePosition, fs);

                        linePosition = lineOffset;

                        linesGoneThrough++;
                    }                                    
                }

                return linePosition;
            }

            private int FindBeginningOfLine(int startPosition, FileStream fs)
            {
                bool isAtNewLine = false;
                var newPosition = startPosition;

                while (!isAtNewLine)
                {
                    if (newPosition >= 0)
                    {
                        fs.Seek(newPosition, SeekOrigin.Begin);

                        using (var sr = new StreamReader(fs, Encoding.UTF8, false, 512, true))
                        {

                            string character = "";

                            fs.Seek(newPosition, SeekOrigin.Begin);

                            character = ((char)sr.Peek()).ToString();

                            if (character == "\n")
                            {
                                isAtNewLine = true;
                            }
                        }

                        newPosition--;
                    }
                    else
                    {
                        break;
                    }

                }

                return newPosition;
            }
        }
    }
}
