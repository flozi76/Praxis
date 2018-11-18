using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Tests.Duftfinder.Web
{
    /// <summary>
    /// Class is used to mock a HttpPostedFileBase for tests.
    /// </summary>
    /// <seealso> href="https://stackoverflow.com/questions/15622153/mocking-httppostedfilebase-and-inputstream-for-unit-test/">stackoverflow</seealso> 
    class MockPostedFileBase : HttpPostedFileBase
    {
        readonly Stream _stream;
        readonly string _contentType;
        readonly string _fileName;

        public MockPostedFileBase(Stream stream, string contentType, string fileName)
        {
            this._stream = stream;
            this._contentType = contentType;
            this._fileName = fileName;
        }

        public override int ContentLength
        {
            get { return (int)_stream.Length; }
        }

        public override string ContentType
        {
            get { return _contentType; }
        }

        public override string FileName
        {
            get { return _fileName; }
        }

        public override Stream InputStream
        {
            get { return _stream; }
        }

        public override void SaveAs(string filename)
        {
            throw new NotImplementedException();
        }
    }
}
