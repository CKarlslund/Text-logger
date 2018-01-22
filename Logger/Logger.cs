using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Logger.Models;

namespace Logger
{
    ///<Summary>
    /// Class for Log
    ///</Summary>
    public class Logger
    {
        private string _filePath;
        private int _rowsToRead;

        public Logger(string filePath, int rows)
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
            var lines = new List<string>();
            var offsetToReturn = 0;

            if (File.Exists(_filePath))
            {
                var startPosition = FindLineNumber(_rowsToRead, offset);

                using (var fileStream = File.OpenRead(_filePath))
                {
                    offsetToReturn = (int)fileStream.Seek(0, SeekOrigin.End);

                    fileStream.Seek(startPosition, SeekOrigin.Begin);

                    using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                    {
                        string line;

                        while ((line = streamReader.ReadLine()) != null)
                        {
                            lines.Add(line);
                        }
                    }
                }
            }
            return new LogData()
            {
                Offset = offsetToReturn,
                Rows = lines.ToArray()
            };
        }

        public int FindLineNumber(int line, int offset)
        {
            int linePosition;

            using (var fileStream = File.OpenRead(_filePath))
            {
                var fileEnd = (int)fileStream.Seek(0, SeekOrigin.End);
                linePosition = fileEnd;

                var linesGoneThrough = 0;

                while (linesGoneThrough < line && linePosition > offset)
                {
                    var lineOffset = FindBeginningOfLine(linePosition, fileStream);

                    linePosition = lineOffset;

                    linesGoneThrough++;
                }
            }
            return linePosition;
        }

        private static int FindBeginningOfLine(int startPosition, FileStream fileStream)
        {
            var isAtNewLine = false;
            var newPosition = startPosition;

            while (!isAtNewLine)
            {
                if (newPosition >= 0)
                {
                    fileStream.Seek(newPosition, SeekOrigin.Begin);

                    using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, false, 512, true))
                    {
                        var character = "";

                        fileStream.Seek(newPosition, SeekOrigin.Begin);

                        character = ((char)streamReader.Peek()).ToString();

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

