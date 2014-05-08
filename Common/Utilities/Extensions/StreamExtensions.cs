// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.Research.DataOnboarding.Utilities.Extensions
{
    public static class StreamExtensions
    {
        /// <summary>
        /// copying the data stream to file stream
        /// </summary>
        /// <param name="source">data stream</param>
        /// <param name="target">file stream</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public static void CopyToStream(this Stream source, Stream target)
        {
            Check.IsNotNull<Stream>(source, "sourceStream");
            Check.IsNotNull<Stream>(target, "targetStream");

            byte[] copyBuf = new byte[0x1000];
            int bytesRead = 0;
            int bufSize = copyBuf.Length;

            //source.Position = 0;

            while ((bytesRead = source.Read(copyBuf, 0, bufSize)) > 0)
            {
                target.Write(copyBuf, 0, bytesRead);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public static byte[] GetBytes(this Stream source)
        {
            Check.IsNotNull(source, "source");
            if (source.Length == 0)
            {
                throw new ArgumentException("File is either empty or corrupted.");
            }

            if (source.CanSeek)
            {
                source.Position = 0;
            }

            byte[] output = new byte[source.Length];
            int streamReadOrWriteChunkSize = Constants.StreamReadOrWriteChunkSize, numBytesRead = 0, numBytesToRead = Math.Min(output.Length - numBytesRead, streamReadOrWriteChunkSize), n;

            do
            {
                n = source.Read(output, numBytesRead, numBytesToRead);
                numBytesRead += n;
                numBytesToRead = Math.Min(output.Length - numBytesRead, streamReadOrWriteChunkSize);
            } while (numBytesToRead > 0);

            return output;
        }

        public async static Task<byte[]> GetBytesAsync(this Stream source)
        {
            Check.IsNotNull(source, "source");
            if (source.Length == 0)
            {
                throw new ArgumentException("File is either empty or corrupted.");
            }

            if (source.CanSeek)
            {
                source.Position = 0;
            }

            byte[] output = new byte[source.Length];
            int streamReadOrWriteChunkSize = Constants.StreamReadOrWriteChunkSize, numBytesRead = 0, numBytesToRead = Math.Min(output.Length - numBytesRead, streamReadOrWriteChunkSize), n;

            do
            {
                n = await source.ReadAsync(output, numBytesRead, numBytesToRead);
                numBytesRead += n;
                numBytesToRead = Math.Min(output.Length - numBytesRead, streamReadOrWriteChunkSize);
            } while (numBytesToRead > 0);

            return output;
        }
    }
}
